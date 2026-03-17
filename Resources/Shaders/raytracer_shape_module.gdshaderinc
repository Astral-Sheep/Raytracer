#include "ray_functions.gdshaderinc"

#define DEBUG 1

const int GLOBAL_BVH_DEPTH = 32;
const int MESH_BVH_DEPTH = 32;

// ===== SHAPE TYPES =====
const int BOUNDING_VOLUME = 1;
const int MESH = 2;
const int LEAF_MESH = 3;
const int MESH_VOLUME = 4;
const int SUBMESH = 5;
const int SPHERE = 6;

// ===== DATA SIZES =====

// Shapes
const int SHAPE_DATA_SIZE = 2;
const float BV_DATA_SIZE = 0.5;
const float SPHERE_DATA_SIZE = 2.0;
const float MESH_DATA_SIZE = 4.75;
const float SUBMESH_DATA_SIZE = 0.75;

// Mesh
const int VERTEX_DATA_SIZE = 2;
const float TRIANGLE_DATA_SIZE = 0.75;

const int MATERIAL_DATA_SIZE = 1;

// ===== MATERIAL TYPES =====
const int MAT_OPAQUE = 0;

// ===== BUFFERS =====
group_uniforms Shapes;

group_uniforms Shapes.Data;

uniform sampler2D shape_buffer;
uniform ivec2 shape_buffer_size;

uniform sampler2D data_buffer;
uniform ivec2 data_buffer_size;

uniform sampler2D vertex_buffer;
uniform ivec2 vertex_buffer_size;
uniform isampler2D triangle_buffer;
uniform ivec2 triangle_buffer_size;

group_uniforms Shapes.Materials;

uniform isampler2D material_buffer;
uniform ivec2 material_buffer_size;
uniform sampler2DArray texture_buffer;

group_uniforms;

struct Shape
{
	int type;
	int data_texel_index;
	vec3 bound_min;
	vec3 bound_max;
};

struct BoundingVolume
{
	int start_index;
	int count;
};

struct Sphere
{
	SpherePrimitive primitive;
	vec3 scale;
	int material_index;
};

struct Mesh
{
	int start_index;
	int count;
	mat4 transform;
	int material_index;
};

struct Submesh
{
	int start_index;
	int count;
	int material_index;
};

// Vertex data in the mesh local space
struct MeshVertex
{
	vec3 position;
	vec3 normal;
	vec2 uv;
};

// Triangle data pointing to the vertex buffer
struct MeshTriangle
{
	int v0;
	int v1;
	int v2;
};

struct Material
{
	int type;
	int texture_index;
	vec3 color;
	vec3 emissive;
	float emissive_intensity;
	float smoothness;
	vec3 specular_color;
	float specular_probability;
};

struct HitResult
{
	HitInfo info;
	int material_index;
};

Shape get_shape(in int index)
{
	index *= SHAPE_DATA_SIZE;
	
	vec4 raw_data0 = get_buffer_texel(shape_buffer, index, shape_buffer_size);
	vec4 raw_data1 = get_buffer_texel(shape_buffer, index + 1, shape_buffer_size);
	
	return Shape(
		floatBitsToInt(raw_data0.w), // type
		floatBitsToInt(raw_data1.w), // data_texel_index
		raw_data0.xyz, // bound_min
		raw_data1.xyz // bound_max
	);
}

Material get_material(in int index)
{
	index *= MATERIAL_DATA_SIZE;
	ivec4 raw_bytes = get_buffer_texel(material_buffer, index, material_buffer_size);
	
	return Material(
		raw_bytes.x & 0xFF, // type
		(raw_bytes.x & 0xFFFF00) >> 8 - (MIN_SHORT + 1), // texture_index
		vec3(float((raw_bytes.x & 0xFF000000) >> 24) * INV_255, float(raw_bytes.y & 0xFF) * INV_255, float((raw_bytes.y & 0xFF00) >> 8) * INV_255), // color
		vec3(float((raw_bytes.y & 0xFF0000) >> 16) * INV_255, float((raw_bytes.y & 0xFF000000) >> 24) * INV_255, float(raw_bytes.z & 0xFF) * INV_255), // emissive
		halfBitsToFloat((raw_bytes.z & 0xFFFF00) >> 8), // emissive_intensity
		float((raw_bytes.z & 0xFF000000) >> 24) * INV_255, // smoothness
		vec3(float(raw_bytes.w & 0xFF) * INV_255, float((raw_bytes.w & 0xFF00) >> 8) * INV_255, float((raw_bytes.w & 0xFF0000) >> 16) * INV_255), // specular_color
		float((raw_bytes.w & 0xFF000000) >> 24) * INV_255 // specular_probability
	);
}

Material get_empty_material()
{
	return Material(
		0,
		-1,
		vec3(0.0),
		vec3(0.0),
		0.0,
		0.0,
		vec3(0.0),
		0.0
	);
}

BoundingVolume get_bounding_volume(in int texel_index)
{
	// ===== Code optimized for when BV_DATA_SIZE is 0.5 =====
	//int offset = 2 * int(texel_index % 2 != 0);
	//texel_index /= 2;
	vec4 texel = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	
	return BoundingVolume(
		floatBitsToInt(texel.x), // start_index
		floatBitsToInt(texel.y) // tri_count
	);
}

Sphere get_sphere(in int texel_index)
{
	vec4 raw_data0 = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	vec4 raw_data1 = get_buffer_texel(data_buffer, texel_index + 1, data_buffer_size);
	
	return Sphere(
		SpherePrimitive(
			raw_data0.xyz, // center
			raw_data0.w // radius
		),
		raw_data1.xyz, // scale
		floatBitsToInt(raw_data1.w) // material_index
	);
}

Mesh get_mesh(in int texel_index)
{
	vec4 raw_data0 = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	vec4 raw_data1 = get_buffer_texel(data_buffer, texel_index + 1, data_buffer_size);
	vec4 raw_data2 = get_buffer_texel(data_buffer, texel_index + 2, data_buffer_size);
	vec4 raw_data3 = get_buffer_texel(data_buffer, texel_index + 3, data_buffer_size);
	vec4 raw_data4 = get_buffer_texel(data_buffer, texel_index + 4, data_buffer_size);
	
	return Mesh(
		floatBitsToInt(raw_data0.x), // start_index
		floatBitsToInt(raw_data0.y), // tri_count
		mat4(
			vec4(raw_data0.zw, raw_data1.xy),
			vec4(raw_data1.zw, raw_data2.xy),
			vec4(raw_data2.zw, raw_data3.xy),
			vec4(raw_data3.zw, raw_data4.xy)
		), // transform
		floatBitsToInt(raw_data4.z) // material_index
	);
}

Submesh get_submesh(in int texel_index)
{
	vec4 raw_data = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	return Submesh(
		floatBitsToInt(raw_data.x), // start_index
		floatBitsToInt(raw_data.y), // tri_count
		floatBitsToInt(raw_data.z) // material_index
	);
}

MeshVertex get_vertex(in int index)
{
	index *= VERTEX_DATA_SIZE;
	vec4 raw_data0 = get_buffer_texel(vertex_buffer, index, vertex_buffer_size);
	vec4 raw_data1 = get_buffer_texel(vertex_buffer, index + 1, vertex_buffer_size);
	
	return MeshVertex(
		raw_data0.xyz, // position
		vec3(raw_data0.w, raw_data1.xy), // normal
		raw_data1.zw // uv
	);
}

MeshTriangle get_triangle(in int index)
{
	float triangle_size_f = TRIANGLE_DATA_SIZE * 4.0;
	int triangle_size_i = int(floor(triangle_size_f));
	
	return MeshTriangle(
		get_buffer_texel(triangle_buffer, int(floor(float(index) * triangle_size_f * .25)), triangle_buffer_size)[(index * triangle_size_i) % 4], // v0
		get_buffer_texel(triangle_buffer, int(floor((float(index) * triangle_size_f + 1.0) * .25)), triangle_buffer_size)[(index * triangle_size_i + 1) % 4], // v1
		get_buffer_texel(triangle_buffer, int(floor((float(index) * triangle_size_f + 2.0) * .25)), triangle_buffer_size)[(index * triangle_size_i + 2) % 4] // v2
	);
}

TrianglePrimitive get_triangle_primitive(in MeshTriangle triangle)
{
	MeshVertex v0 = get_vertex(triangle.v0);
	MeshVertex v1 = get_vertex(triangle.v1);
	MeshVertex v2 = get_vertex(triangle.v2);
	
	return TrianglePrimitive(
		v0.position,
		v1.position,
		v2.position,
		v0.normal,
		v1.normal,
		v2.normal,
		v0.uv,
		v1.uv,
		v2.uv
	);
}

HitResult ray_submesh(
	in Ray ray,
	in Submesh submesh,
	in Mesh mesh,
	in float max_dist
#if DEBUG
	, inout int tri_test_count
#endif //DEBUG
)
{
	HitResult result = HitResult(get_empty_hit(), -1);
	result.info.dist = max_dist * max_dist;
	
	if (submesh.count <= 0)
	{
		const int MAX_LOOPS = (MESH_BVH_DEPTH + 1) * (MESH_BVH_DEPTH + 1);
		int loop_count = 0;

		int bvh_stack[MESH_BVH_DEPTH - 1];
		int stack_index = -1;
		
		bvh_stack[stack_index + 1] = mesh.start_index;
		
		while (stack_index >= 0 && loop_count < MAX_LOOPS)
		{
			Shape shape = get_shape(bvh_stack[stack_index]);
			
			if (!ray_box(ray, shape.bound_min, shape.bound_max, result.info.dist))
			{
				--stack_index;
				continue;
			}
			
			// Only MESH_VOLUME type is handled. No switch is needed
			BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
			
			if (volume.count <= 0)
			{
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + volume.count)
				{
					bvh_stack[stack_index + 1] = volume.start_index;
					++stack_index;
				}
				// All children were pushed
				else if (last_child_index == volume.start_index + volume.count - 1)
				{
					bvh_stack[stack_index + 1] = 0;
					--stack_index;
				}
				else
				{
					++bvh_stack[stack_index + 1];
					++stack_index;
				}
				
				break;
			}
			
			if (stack_index < 0 && bvh_stack[0] < submesh.start_index + submesh.count)
			{
				++bvh_stack[0];
				stack_index = 0;
			}
		}
	}
	else
	{
		for (int i = submesh.start_index; i < submesh.start_index + submesh.count; i++)
		{
			TrianglePrimitive triangle = get_triangle_primitive(get_triangle(i));
			HitInfo triangle_hit = ray_triangle(ray, triangle);
			
#if DEBUG
			++tri_test_count;
#endif //DEBUG
			
			if (!triangle_hit.hit || triangle_hit.dist >= result.info.dist)
				continue;
			
			triangle_hit.point = (mesh.transform * vec4(triangle_hit.point, 1.0)).xyz;
			triangle_hit.dist = length_squared(triangle_hit.point - ray.origin);
			
			if (triangle_hit.dist >= result.info.dist)
				continue;
			
			result = HitResult(triangle_hit, mesh.material_index);
		}
	}
	
	return result;
}

HitResult ray_mesh(
	in Ray ray,
	in Mesh mesh,
	in float max_dist
#if DEBUG
	, inout int tri_test_count
#endif //DEBUG
)
{
	HitResult result = HitResult(get_empty_hit(), -1);
	result.info.dist = max_dist * max_dist;
	
	mat4 world_to_local = inverse(mesh.transform);
	Ray local_ray;
	local_ray.origin = (world_to_local * vec4(ray.origin, 1.0)).xyz;
	local_ray.dir = normalize((world_to_local * vec4(ray.dir, 0.0)).xyz);
	local_ray.invdir = 1.0 / local_ray.dir;
	
	// -2 or less means that the mesh has more than one surface
	if (mesh.material_index > -2)
	{
		const int MAX_LOOPS = (MESH_BVH_DEPTH + 1) * (MESH_BVH_DEPTH + 1);
		int loop_count = 0;

		int bvh_stack[MESH_BVH_DEPTH];
		int stack_index = -1;
		
		bvh_stack[stack_index + 1] = mesh.start_index;
		
		while (stack_index >= 0 && loop_count < MAX_LOOPS)
		{
			Shape shape = get_shape(bvh_stack[stack_index]);
			
			if (!ray_box(local_ray, shape.bound_min, shape.bound_max, result.info.dist))
			{
				--stack_index;
				continue;
			}
			
			// Only MESH_VOLUME type is handled. No switch is needed
			BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
			
			if (volume.count <= 0)
			{
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + volume.count)
				{
					bvh_stack[stack_index + 1] = volume.start_index;
					++stack_index;
				}
				// All children were pushed
				else if (last_child_index == volume.start_index + volume.count - 1)
				{
					bvh_stack[stack_index + 1] = 0;
					--stack_index;
				}
				else
				{
					++bvh_stack[stack_index + 1];
					++stack_index;
				}
				
				break;
			}
			
			if (stack_index < 0 && bvh_stack[0] < mesh.start_index + mesh.count)
			{
				++bvh_stack[0];
				stack_index = 0;
			}
		}
	}
	else
	{
		for (int i = mesh.start_index; i < mesh.start_index + mesh.count; i++)
		{
			Shape submesh = get_shape(i);
			HitResult submesh_result = ray_submesh(
				local_ray,
				get_submesh(submesh.data_texel_index),
				mesh,
				result.info.dist
#if DEBUG
				, tri_test_count
#endif //DEBUG
			);
			
			// ray_submesh already converts point and dist to global space
			if (!submesh_result.info.hit || submesh_result.info.dist >= result.info.dist)
				continue;
			
			result = submesh_result;
		}
	}
	
	if (result.info.hit)
	{
		result.info.dist = sqrt(result.info.dist);
		result.info.normal = (mesh.transform * vec4(result.info.normal, 0.0)).xyz;
	}
	
	return result;
}

HitResult ray_leaf_mesh(
	in Ray ray,
	in Mesh mesh,
	in float max_dist
#if DEBUG
	, inout int tri_test_count
#endif //DEBUG
)
{
	HitResult result = HitResult(get_empty_hit(), -1);
	result.info.dist = max_dist * max_dist;
	
	mat4 world_to_local = inverse(mesh.transform);
	Ray local_ray;
	local_ray.origin = (world_to_local * vec4(ray.origin, 1.0)).xyz;
	local_ray.dir = normalize((world_to_local * vec4(ray.dir, 0.0)).xyz);
	local_ray.invdir = 1.0 / local_ray.dir;
	
	for (int i = mesh.start_index; i < mesh.start_index + mesh.count; i++)
	{
		TrianglePrimitive triangle = get_triangle_primitive(get_triangle(i));
		HitInfo triangle_hit = ray_triangle(local_ray, triangle);
		
#if DEBUG
		++tri_test_count;
#endif //DEBUG
		
		if (!triangle_hit.hit || triangle_hit.dist >= result.info.dist)
			continue;
		
		triangle_hit.point = (mesh.transform * vec4(triangle_hit.point, 1.0)).xyz;
		triangle_hit.dist = length_squared(triangle_hit.point - ray.origin);
		
		if (triangle_hit.dist >= result.info.dist)
			continue;
		
		result = HitResult(triangle_hit, mesh.material_index);
	}
	
	if (result.info.hit)
	{
		result.info.dist = sqrt(result.info.dist);
		result.info.normal = (mesh.transform * vec4(result.info.normal, 0.0)).xyz;
	}
	
	return result;
}

HitResult trace_ray_collision(
	in Ray ray
#if DEBUG
	, inout int tri_test_count
#endif //DEBUG
)
{
	HitResult result = HitResult(get_empty_hit(), -1);
	
	if (shape_buffer_size.x * shape_buffer_size.y < SHAPE_DATA_SIZE)
	{
		return result;
	}
	
	const int MAX_LOOPS = (GLOBAL_BVH_DEPTH + 1) * (GLOBAL_BVH_DEPTH + 1);
	int loop_count = 0;
	
	int bvh_stack[GLOBAL_BVH_DEPTH + 1];
	int stack_index = -1;
	
	bvh_stack[stack_index + 1] = 0; // Add root to stack
	++stack_index;
	
	while (stack_index >= 0 && loop_count < MAX_LOOPS)
	{
		++loop_count;
		Shape shape = get_shape(bvh_stack[stack_index]);
		
		if (
			shape.type < 1
			|| shape.data_texel_index < 0
			|| !ray_box(ray, shape.bound_min, shape.bound_max, result.info.dist)
		)
		{
			--stack_index;
			continue;
		}
		
		switch (shape.type)
		{
			// Global bounding volume
			case BOUNDING_VOLUME:
			{
				BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + volume.count)
				{
					bvh_stack[stack_index + 1] = volume.start_index;
					++stack_index;
				}
				// All children were pushed
				else if (last_child_index == volume.start_index + volume.count - 1)
				{
					bvh_stack[stack_index + 1] = 0;
					--stack_index;
				}
				else
				{
					++bvh_stack[stack_index + 1];
					++stack_index;
				}
				
				break;
			}
			// Mesh bounding volume
			case MESH:
			{
				--stack_index;
				Mesh mesh = get_mesh(shape.data_texel_index);
				HitResult mesh_result = ray_mesh(
					ray,
					mesh,
					result.info.dist
#if DEBUG
					, tri_test_count
#endif //DEBUG
				);
				
				if (!mesh_result.info.hit || mesh_result.info.dist >= result.info.dist)
					break;
				
				result = mesh_result;
				break;
			}
			// Unordered mesh
			case LEAF_MESH:
			{
				--stack_index;
				
				Mesh mesh = get_mesh(shape.data_texel_index);
				HitResult mesh_result = ray_leaf_mesh(
					ray,
					mesh,
					result.info.dist
#if DEBUG
					, tri_test_count
#endif //DEBUG
				);
				
				if (!mesh_result.info.hit || mesh_result.info.dist >= result.info.dist)
					break;
				
				result = mesh_result;
				break;
			}
			// Sphere primitive
			case SPHERE:
			{
				--stack_index;
				
				Sphere sphere = get_sphere(shape.data_texel_index);
				
				Ray local_ray;
				local_ray.origin = ray.origin / sphere.scale;
				local_ray.dir = normalize(ray.dir / sphere.scale);
				HitInfo hit_info = ray_sphere(local_ray, sphere.primitive);
				
#if DEBUG
				++tri_test_count;
#endif //DEBUG
				
				if (!hit_info.hit || hit_info.dist >= result.info.dist)
					break;
				
				result.info = hit_info;
				result.material_index = sphere.material_index;
				break;
			}
			default:
				--stack_index;
				break;
		}
	}
	
	return result;
}
