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
const int BOX = 7;
const int CYLINDER = 8;
const int TORUS = 9;

// ===== DATA SIZES =====

// Shapes
const int SHAPE_DATA_SIZE = 2;
const float BV_DATA_SIZE = 0.5;
const float SPHERE_DATA_SIZE = 2.0;
const float MESH_DATA_SIZE = 4.75;
const float SUBMESH_DATA_SIZE = 0.75;
const float CYLINDER_DATA_SIZE = 4.75;
const float TORUS_DATA_SIZE = 4.75;

// Mesh
const int VERTEX_DATA_SIZE = 2;
const float TRIANGLE_DATA_SIZE = 0.75;

const int MATERIAL_DATA_SIZE = 4;

// ===== MATERIAL TYPES =====
const int MAT_OPAQUE = 0;

#if DEBUG
group_uniforms Debug;
uniform uint max_bvh_depth = 0;
#endif //DEBUG

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

uniform sampler2D material_buffer;
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
	float radius;
	vec3 center;
	vec3 scale;
	int material_index;
};

struct Cylinder
{
	CylinderPrimitive primitive;
	mat4 transform;
	int material_index;
};

struct Torus
{
	TorusPrimitive primitive;
	mat4 transform;
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

// TODO: add support for transparency and flat shading
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
	bool flat_shading;
};

struct HitResult
{
	HitInfo info;
	int material_index;
	
#if DEBUG
	int volume_test_count;
	int shape_test_count;
#endif //DEBUG
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
	//ivec4 raw_bytes = floatBitsToInt(get_buffer_texel(material_buffer, index, material_buffer_size));
	vec4 raw_data0 = get_buffer_texel(material_buffer, index, material_buffer_size);
	vec4 raw_data1 = get_buffer_texel(material_buffer, index + 1, material_buffer_size);
	vec4 raw_data2 = get_buffer_texel(material_buffer, index + 2, material_buffer_size);
	vec4 raw_data3 = get_buffer_texel(material_buffer, index + 3, material_buffer_size);
	
	Material mat;
	mat.type = floatBitsToInt(raw_data0.x);
	mat.texture_index = floatBitsToInt(raw_data0.y);
	
	mat.color.r = raw_data0.z;
	mat.color.g = raw_data0.w;
	mat.color.b = raw_data1.x;
	
	mat.emissive.r = raw_data1.y;
	mat.emissive.g = raw_data1.z;
	mat.emissive.b = raw_data1.w;
	mat.emissive_intensity = raw_data2.x;
	
	mat.smoothness = raw_data2.y;
	mat.specular_color.r = raw_data2.z;
	mat.specular_color.g = raw_data2.w;
	mat.specular_color.b = raw_data3.x;
	mat.specular_probability = raw_data3.y;
	
	//mat.flat_shading = bool(floatBitsToInt(raw_data3.z) != 0);
	mat.flat_shading = false;
	
	return mat;
}

Material get_empty_material()
{
	return Material(
		0, // type
		-1, // texture_index
		vec3(0.0), // color
		vec3(0.0), // emissive
		0.0, // emissive_intensity
		0.0, // smoothness
		vec3(0.0), // specular_color
		0.0, // specular_probability
		false // flat_shading
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
		floatBitsToInt(texel.y) // count
	);
}

Sphere get_sphere(in int texel_index)
{
	vec4 raw_data0 = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	vec4 raw_data1 = get_buffer_texel(data_buffer, texel_index + 1, data_buffer_size);
	
	return Sphere(
		raw_data0.w, // radius
		raw_data0.xyz, // center
		raw_data1.xyz, // scale
		floatBitsToInt(raw_data1.w) // material_index
	);
}

Cylinder get_cylinder(in int texel_index)
{
	vec4 raw_data0 = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	vec4 raw_data1 = get_buffer_texel(data_buffer, texel_index + 1, data_buffer_size);
	vec4 raw_data2 = get_buffer_texel(data_buffer, texel_index + 2, data_buffer_size);
	vec4 raw_data3 = get_buffer_texel(data_buffer, texel_index + 3, data_buffer_size);
	vec4 raw_data4 = get_buffer_texel(data_buffer, texel_index + 4, data_buffer_size);
	
	return Cylinder(
		CylinderPrimitive(
			raw_data4.x,
			raw_data4.y
		),
		mat4(raw_data0, raw_data1, raw_data2, raw_data3),
		floatBitsToInt(raw_data4.z)
	);
}

Torus get_torus(in int texel_index)
{
	vec4 raw_data0 = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	vec4 raw_data1 = get_buffer_texel(data_buffer, texel_index + 1, data_buffer_size);
	vec4 raw_data2 = get_buffer_texel(data_buffer, texel_index + 2, data_buffer_size);
	vec4 raw_data3 = get_buffer_texel(data_buffer, texel_index + 3, data_buffer_size);
	vec4 raw_data4 = get_buffer_texel(data_buffer, texel_index + 4, data_buffer_size);
	
	return Torus(
		TorusPrimitive(
			raw_data4.x,
			raw_data4.y
		),
		mat4(raw_data0, raw_data1, raw_data2, raw_data3),
		floatBitsToInt(raw_data4.z)
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
		floatBitsToInt(raw_data4.x), // start_index
		floatBitsToInt(raw_data4.y), // count
		mat4(raw_data0, raw_data1, raw_data2, raw_data3), // transform
		floatBitsToInt(raw_data4.z) // material_index
	);
}

Submesh get_submesh(in int texel_index)
{
	vec4 raw_data = get_buffer_texel(data_buffer, texel_index, data_buffer_size);
	return Submesh(
		floatBitsToInt(raw_data.x), // start_index
		floatBitsToInt(raw_data.y), // count
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

HitResult get_default_result()
{
	return HitResult(
		get_empty_hit(),
		-1
#if DEBUG
		, 0,
		0
#endif //DEBUG
	);
}

bool ray_local_volume(in Ray local_ray, in Shape shape, in mat4 transform, inout HitResult result)
{
#if DEBUG
	++result.volume_test_count;
#endif //DEBUG
	
	float max_dist = mix(MAX_DISTANCE, length((transform * vec4(result.info.point, 1.0)).xyz), float(result.info.hit));
	return ray_box(local_ray, shape.bound_min, shape.bound_max, max_dist);
}

bool ray_volume(in Ray ray, in Shape shape, inout HitResult result)
{
#if DEBUG
	++result.volume_test_count;
#endif //DEBUG
	
	return ray_box(ray, shape.bound_min, shape.bound_max, result.info.dist);
}

bool ray_global_triangles(in Ray ray, int start, int count, inout HitResult result)
{
	bool hit = false;
	
	for (int i = start; i < start + count; i++)
	{
		HitInfo triangle_hit = ray_triangle(ray, get_triangle_primitive(get_triangle(i)));
		
#if DEBUG
		++result.shape_test_count;
#endif //DEBUG
		
		if (!triangle_hit.hit || triangle_hit.dist >= result.info.dist)
			continue;
		
		hit = true;
		result.info = triangle_hit;
	}
	
	return hit;
}

bool ray_local_triangles(in Ray local_ray, int start, int count, in mat4 local_to_world, inout HitResult result)
{
	bool hit = false;
	result.info.dist *= result.info.dist;
	
	for (int i = start; i < start + count; i++)
	{
		HitInfo triangle_hit = ray_triangle(local_ray, get_triangle_primitive(get_triangle(i)));
		
#if DEBUG
		++result.shape_test_count;
#endif //DEBUG
		
		if (!triangle_hit.hit)
			continue;
		
		triangle_hit.dist = length_squared((local_to_world * vec4(triangle_hit.point - local_ray.origin, 0.0)).xyz);
		
		if (triangle_hit.dist >= result.info.dist)
			continue;
		
		hit = true;
		result.info = triangle_hit;
	}
	
	result.info.dist = sqrt(result.info.dist);
	return hit;
}

bool ray_submesh(in Ray local_ray, in Ray global_ray, in Submesh submesh, in mat4 local_to_world, inout HitResult result)
{
	bool hit = false;
	
	if (submesh.count <= 0)
	{
		const int MAX_LOOPS = (MESH_BVH_DEPTH + 1) * (MESH_BVH_DEPTH + 1);
		int loop_count = 0;

		int bvh_stack[MESH_BVH_DEPTH - 1];
		int stack_index = -1;
		
		bvh_stack[stack_index + 1] = submesh.start_index;
		
		mat4 world_to_local = inverse(local_to_world);
		
		while (stack_index >= 0 && loop_count < MAX_LOOPS)
		{
			++loop_count;
			Shape shape = get_shape(bvh_stack[stack_index]);
			// Submesh children can only be volumes
			BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
			
			if (volume.count <= 0)
			{
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + volume.count)
				{
					if (ray_local_volume(local_ray, shape, world_to_local, result))
					{
						bvh_stack[stack_index + 1] = volume.start_index;
						++stack_index;
					}
					else
					{
						--stack_index;
						continue;
					}
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
			else
			{
				--stack_index;
				
				if (ray_local_volume(local_ray, shape, world_to_local, result))
				{
					hit = ray_local_triangles(local_ray, volume.start_index, volume.count, local_to_world, result);
				}
			}
			
			if (stack_index < 0 && bvh_stack[0] < submesh.start_index + submesh.count - 1)
			{
				++bvh_stack[0];
				stack_index = 0;
			}
		}
	}
	else
	{
		hit = ray_local_triangles(local_ray, submesh.start_index, submesh.count, local_to_world, result);
	}
	
	result.material_index = hit ? submesh.material_index : result.material_index;
	return hit;
}

bool ray_mesh(in Ray ray, in Mesh mesh, inout HitResult result)
{
	bool hit = false;
	mat4 world_to_local = inverse(mesh.transform);
	Ray local_ray = transform_ray(ray, world_to_local);
	
	// -2 or less means that the mesh has more than one surface
	if (mesh.material_index > -2)
	{
		const int MAX_LOOPS = (MESH_BVH_DEPTH + 1) * (MESH_BVH_DEPTH + 1);
		int loop_count = 0;

		int bvh_stack[MESH_BVH_DEPTH];
		int stack_index = -1;
		
		bvh_stack[stack_index + 1] = mesh.start_index;
		++stack_index;
		
		while (stack_index >= 0)
		{
			++loop_count;
			
			Shape shape = get_shape(bvh_stack[stack_index]);
			// Mesh children are all BoundingVolumes (Submesh is a type of BoundingVolume)
			BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
			
			if (volume.count <= 0)
			{
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + 2)
				{
					if (ray_local_volume(local_ray, shape, world_to_local, result))
					{
						bvh_stack[stack_index + 1] = volume.start_index;
						++stack_index;
					}
					else
					{
						--stack_index;
					}
				}
				// All children were pushed
				else if (last_child_index == volume.start_index + 1)
				{
					bvh_stack[stack_index + 1] = 0;
					--stack_index;
				}
				else
				{
					++bvh_stack[stack_index + 1];
					++stack_index;
				}
			}
			else
			{
				--stack_index;
				
				if (
					ray_local_volume(local_ray, shape, world_to_local, result)
					&& ray_local_triangles(local_ray, volume.start_index, volume.count, mesh.transform, result)
				)
				{
					result.material_index = mesh.material_index;
					hit = true;
				}
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
			hit = hit || ray_submesh(
				local_ray,
				ray,
				get_submesh(get_shape(i).data_texel_index),
				mesh.transform,
				result
			);
		}
	}
	
	if (hit)
	{
		result.info.point = (mesh.transform * vec4(result.info.point, 1.0)).xyz;
		result.info.normal = (mesh.transform * vec4(result.info.normal, 0.0)).xyz;
		result.info.flat_normal = (mesh.transform * vec4(result.info.flat_normal, 0.0)).xyz;
	}
	
	return hit;
}

bool ray_leaf_mesh(in Ray ray, in Mesh mesh, inout HitResult result)
{
	bool hit = false;
	
	if (ray_local_triangles(transform_ray(ray, inverse(mesh.transform)), mesh.start_index, mesh.count, mesh.transform, result))
	{
		result.info.point = (mesh.transform * vec4(result.info.point, 1.0)).xyz;
		result.info.normal = (mesh.transform * vec4(result.info.normal, 0.0)).xyz;
		result.info.flat_normal = (mesh.transform * vec4(result.info.flat_normal, 0.0)).xyz;
		result.material_index = mesh.material_index;
		hit = true;
	}
	
	return hit;
}

bool ray_sphere_shape(in Ray ray, in Sphere sphere, inout HitResult result)
{
	Ray local_ray;
	local_ray.origin = (ray.origin - sphere.center) / sphere.scale;
	local_ray.dir = normalize(ray.dir / sphere.scale);
	
	HitInfo sphere_hit = ray_sphere(local_ray, sphere.radius);
	
#if DEBUG
	++result.shape_test_count;
#endif //DEBUG
	
	if (!sphere_hit.hit)
		return false;
	
	sphere_hit.point = sphere_hit.point * sphere.scale + sphere.center;
	sphere_hit.dist = length_squared(sphere_hit.point - ray.origin);
	
	if (sphere_hit.dist >= result.info.dist * result.info.dist)
		return false;
	
	sphere_hit.dist = sqrt(sphere_hit.dist);
	sphere_hit.normal = normalize(sphere_hit.normal * sphere.scale);
	sphere_hit.flat_normal = sphere_hit.normal;
	result.info = sphere_hit;
	result.material_index = sphere.material_index;
	return true;
}

bool ray_cylinder_shape(in Ray ray, in Cylinder cylinder, inout HitResult result)
{
	HitInfo cylinder_hit = ray_cylinder(
		transform_ray(ray, inverse(cylinder.transform)),
		cylinder.primitive
	);
	
#if DEBUG
	++result.shape_test_count;
#endif //DEBUG
	
	if (!cylinder_hit.hit)
		return false;
	
	cylinder_hit.point = (cylinder.transform * vec4(cylinder_hit.point, 1.0)).xyz;
	cylinder_hit.dist = length_squared(cylinder_hit.point - ray.origin);
	
	if (cylinder_hit.dist >= result.info.dist * result.info.dist)
		return false;
	
	cylinder_hit.dist = sqrt(cylinder_hit.dist);
	cylinder_hit.normal = normalize((cylinder.transform * vec4(cylinder_hit.normal, 0.0)).xyz);
	cylinder_hit.flat_normal = cylinder_hit.normal;
	result.info = cylinder_hit;
	result.material_index = cylinder.material_index;
	return true;
}

bool ray_torus_shape(in Ray ray, in Torus torus, inout HitResult result)
{
	Ray local_ray = transform_ray(ray, inverse(torus.transform));
	HitInfo torus_hit = ray_torus(local_ray, torus.primitive);
	
#if DEBUG
	++result.shape_test_count;
#endif //DEBUG
	
	if (!torus_hit.hit)
		return false;
	
	torus_hit.dist = length_squared((torus.transform * vec4(torus_hit.point - local_ray.origin, 0.0)).xyz);
	
	if (torus_hit.dist >= result.info.dist * result.info.dist)
		return false;
	
	torus_hit.dist = sqrt(torus_hit.dist);
	torus_hit.point = (torus.transform * vec4(torus_hit.point, 1.0)).xyz;
	torus_hit.normal = (torus.transform * vec4(torus_hit.normal, 0.0)).xyz;
	torus_hit.flat_normal = torus_hit.normal;
	result.info = torus_hit;
	result.material_index = torus.material_index;
	return true;
}

HitResult trace_ray_collision(in Ray ray)
{
	HitResult result = get_default_result();
	
	if (shape_buffer_size.x * shape_buffer_size.y < SHAPE_DATA_SIZE)
	{
		return result;
	}
	
	const int MAX_LOOPS = (GLOBAL_BVH_DEPTH + 1) * (GLOBAL_BVH_DEPTH + 1);
	int loop_count = 0;
	
	int bvh_stack[GLOBAL_BVH_DEPTH + 1];
	int stack_index = -1;
	
#if DEBUG
	int depth = max_bvh_depth == 0u ? GLOBAL_BVH_DEPTH + 1 : clamp(int(max_bvh_depth), 1, GLOBAL_BVH_DEPTH + 1);
#endif //DEBUG
	
	bvh_stack[stack_index + 1] = 0; // Add root to stack
	++stack_index;
	
#if DEBUG
	while (stack_index >= 0 && stack_index < depth && loop_count < MAX_LOOPS)
#else
	while (stack_index >= 0 && loop_count < MAX_LOOPS)
#endif //DEBUG
	{
		++loop_count;
		Shape shape = get_shape(bvh_stack[stack_index]);
		
		if (
			shape.type < 1
			|| shape.data_texel_index < 0
			// Bounding volumes are checked several times, but we don't need to test the bounding box each time
			|| (shape.type != BOUNDING_VOLUME && !ray_volume(ray, shape, result))
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
				// TODO: sort children depending on distance and type
				BoundingVolume volume = get_bounding_volume(shape.data_texel_index);
				int last_child_index = bvh_stack[stack_index + 1];
				
				// First child to push
				if (last_child_index < volume.start_index || last_child_index >= volume.start_index + volume.count)
				{
					if (ray_volume(ray, shape, result))
					{
						bvh_stack[stack_index + 1] = volume.start_index;
						++stack_index;
					}
					else
					{
						--stack_index;
					}
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
				ray_mesh(ray, get_mesh(shape.data_texel_index), result);
				break;
			}
			// Unordered mesh
			case LEAF_MESH:
			{
				--stack_index;
				ray_leaf_mesh(ray, get_mesh(shape.data_texel_index), result);
				break;
			}
			// Sphere primitive
			case SPHERE:
			{
				--stack_index;
				ray_sphere_shape(ray, get_sphere(shape.data_texel_index), result);
				break;
			}
			case CYLINDER:
			{
				--stack_index;
				ray_cylinder_shape(ray, get_cylinder(shape.data_texel_index), result);
				break;
			}
			case TORUS:
			{
				--stack_index;
				ray_torus_shape(ray, get_torus(shape.data_texel_index), result);
				break;
			}
			default:
				--stack_index;
				break;
		}
	}
	
	return result;
}
