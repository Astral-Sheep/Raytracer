#include "ray_functions.gdshaderinc"

#define DEBUG 1

const int MAX_BVH_DEPTH = 50;

// ===== SHAPE TYPES =====
const int BOUNDING_VOLUME = 0;
const int SPHERE = 1;
const int MESH = 2;

// ===== DATA SIZES =====

// Shapes
const int SHAPE_DATA_SIZE = 2;
const float BV_DATA_SIZE = 0.5;
const float SPHERE_DATA_SIZE = 1.25;
const float MESH_DATA_SIZE = 4.75;

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
	int child0;
	int child1;
};

struct Sphere
{
	SpherePrimitive primitive;
	int material_index;
};

struct Mesh
{
	int tri_start;
	int tri_count;
	mat4 transform;
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
		floatBitsToInt(raw_data0.x), // type
		floatBitsToInt(raw_data0.y), // data_texel_index
		vec3(raw_data0.zw, raw_data1.x), // bound_min
		vec3(raw_data1.yzw) // bound_max
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
		intBitsToFloat((raw_bytes.z & 0xFFFF00) >> 8), // emissive_intensity
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
	vec4 texel = get_buffer_texel(data_buffer, texel_index, shape_buffer_size);
	
	return BoundingVolume(
		floatBitsToInt(texel.x), // child0
		floatBitsToInt(texel.y) // child1
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
		floatBitsToInt(raw_data1.x) // material_index
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
		floatBitsToInt(raw_data0.x), // tri_start
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

HitInfo ray_mesh(
	in Ray ray,
	in Mesh mesh,
	in float max_dist
#if DEBUG
	, inout int tri_test_count
#endif // DEBUG
)
{
	HitInfo info = get_empty_hit();
	
	//if (!ray_box(ray, mesh.bound_min, mesh.bound_max, max_dist))
	//{
		//return info;
	//}
	
	mat4 world_to_local = inverse(mesh.transform);
	Ray local_ray;
	local_ray.origin = (world_to_local * vec4(ray.origin, 1.0)).xyz;
	local_ray.dir = normalize((world_to_local * vec4(ray.dir, 0.0)).xyz);
	local_ray.invdir = 1.0 / local_ray.dir;
	
	for (int i = mesh.tri_start; i < mesh.tri_start + mesh.tri_count; i++)
	{
		TrianglePrimitive primitive = get_triangle_primitive(get_triangle(i));
		HitInfo triangle_hit = ray_triangle(local_ray, primitive);
		
#if DEBUG
		++tri_test_count;
#endif //DEBUG
		
		// WARN: mesh scale can interfer with distance test, dist needs to be unscaled before test
		if (!triangle_hit.hit || triangle_hit.dist >= info.dist)
			continue;
		
		// OPT: transform data only on final hit if there is one
		// Needs to be profiled to ensure that a condition is faster than 2 matrix multiplications
		triangle_hit.point = (mesh.transform * vec4(triangle_hit.point, 1.0)).xyz;
		triangle_hit.normal = normalize((mesh.transform * vec4(triangle_hit.normal, 0.0)).xyz);
		info = triangle_hit;
	}
	
	return info;
}

HitResult trace_ray_collision(
	in Ray ray
#if DEBUG
	, inout int tri_test_count
#endif //DEBUG
)
{
	HitResult result = HitResult(get_empty_hit(), -1);
	
	if (shape_buffer_size.x * shape_buffer_size.y <= SHAPE_DATA_SIZE * 4)
	{
		return result;
	}
	
	Shape bvh_stack[MAX_BVH_DEPTH];
	int stack_index = -1;
	
	bvh_stack[stack_index + 1] = get_shape(0); // Add root to stack
	++stack_index;
	
	const int MAX_LOOPS = 1000;
	int loop_count = 0;
	
	while (stack_index >= 0 && loop_count < MAX_LOOPS)
	{
		Shape shape = bvh_stack[stack_index];
		
		if (shape.type < 0 || shape.data_texel_index < 0)
			continue;
		
		if (!ray_box(ray, shape.bound_min, shape.bound_max, result.info.dist))
			continue;
		
		switch (shape.type)
		{
			case BOUNDING_VOLUME:
			{
				// Shape should be leaf but isn't
				if (stack_index >= MAX_BVH_DEPTH - 1)
					break;
				
				BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
				Shape child0 = get_shape(volume.child0);
				Shape child1 = get_shape(volume.child1);
				
				float dist0 = length_squared((child0.bound_max + child0.bound_min) * 0.5 - ray.origin);
				float dist1 = length_squared((child1.bound_max + child1.bound_min) * 0.5 - ray.origin);
				
				if (dist0 > dist1)
				{
					Shape tmp = child1;
					child1 = child0;
					child0 = tmp;
				}
				
				Shape stacked_child = bvh_stack[stack_index + 1];
				
				// Child 0 has been evaluated: child 1 should be stacked
				if (
					stacked_child.type == child0.type
					&& stacked_child.data_texel_index == child0.data_texel_index
				)
				{
					bvh_stack[stack_index + 1] = child1;
					++stack_index;
				}
				// Child 1 has been evaluated: shape should be unstacked
				else if (
					stacked_child.type == child1.type
					&& stacked_child.data_texel_index == child1.data_texel_index
				)
				{
					bvh_stack[stack_index + 1] = Shape(-1, -1, vec3(0.0), vec3(0.0));
					--stack_index;
				}
				// No child has been evaluated: child 0 should be stacked
				else
				{
					bvh_stack[stack_index + 1] = child0;
					++stack_index;
				}
				
				break;
			}
			case SPHERE:
			{
				--stack_index;
				Sphere sphere = get_sphere(shape.data_texel_index);
				HitInfo hit_info = ray_sphere(ray, sphere.primitive);
				
#if DEBUG
				++tri_test_count;
#endif //DEBUG
				
				if (!hit_info.hit || hit_info.dist >= result.info.dist)
					break;
				
				result.info = hit_info;
				result.material_index = sphere.material_index;
				break;
			}
			case MESH:
			{
				--stack_index;
				Mesh mesh = get_mesh(shape.data_texel_index);
				HitInfo hit_info = ray_mesh(
					ray,
					mesh,
					result.info.dist
#if DEBUG
					, tri_test_count
#endif //DEBUG
				);
				
				if (!hit_info.hit || hit_info.dist >= result.info.dist)
					break;
				
				result.info = hit_info;
				result.material_index = mesh.material_index;
				break;
			}
			default:
				--stack_index;
				break;
		}
		
		++loop_count;
	}
	
	return result;
}
