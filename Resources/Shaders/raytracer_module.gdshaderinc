#include "ray_functions.gdshaderinc"

const float SHAPE_DATA_SIZE = 0.75;
const int SPHERE_DATA_SIZE = 1;
const int MESH_DATA_SIZE = 6;
const int VERTEX_DATA_SIZE = 2;
const float TRIANGLE_DATA_SIZE = 0.75;
const int MATERIAL_DATA_SIZE = 4;

const int SHAPE_TYPE_SPHERE = 0;
const int SHAPE_TYPE_BOX = 1;
const int SHAPE_TYPE_MESH = 2;

const int MATERIAL_TYPE_DIFFUSE = 0;

group_uniforms Shapes;

uniform bool draw_shapes;
uniform isampler2D shape_buffer;

group_uniforms Shapes.Buffers;

uniform sampler2D sphere_buffer;

uniform sampler2D mesh_buffer;
uniform sampler2D vertex_buffer;
uniform isampler2D triangle_buffer;

uniform sampler2D material_buffer;
uniform sampler2DArray texture_buffer;

struct Shape
{
	int type;
	int data_index;
	int material_index;
};

struct Mesh
{
	int tri_start;
	int tri_count;
	vec3 bound_min;
	vec3 bound_max;
	mat4 transform;
};

struct Vertex
{
	vec3 position;
	vec3 normal;
	vec2 uv;
};

struct Triangle
{
	int v0;
	int v1;
	int v2;
};

struct Material
{
	int type;
	vec4 color;
	vec3 emissive;
	float emissive_intensity;
	float smoothness;
	vec3 specular_color;
	float specular_probability;
	int texture_index;
};

struct Sun
{
	vec3 direction;
	float focus;
	vec3 color;
	float energy;
};

struct HitResult
{
	HitInfo info;
	Shape shape;
};

Shape get_shape(in int index)
{
	float shape_size_f = SHAPE_DATA_SIZE * 4.0;
	int shape_size_i = int(floor(shape_size_f));
	
	// Texel index: i * 3 / 4
	// Coordinate index: i * 3 % 4
	return Shape(
		texelFetch(shape_buffer, ivec2(int(floor(float(index) * shape_size_f * .25)), 0), 0)[(index * shape_size_i) % 4] - 1, // type
		texelFetch(shape_buffer, ivec2(int(floor((float(index) * shape_size_f + 1.0) * .25)), 0), 0)[(index * shape_size_i + 1) % 4], // data_index
		texelFetch(shape_buffer, ivec2(int(floor((float(index) * shape_size_f + 2.0) * .25)), 0), 0)[(index * shape_size_i + 2) % 4] // material_index
	);
}

SpherePrimitive get_sphere(in int index)
{
	vec4 raw_data = texelFetch(sphere_buffer, ivec2(index * SPHERE_DATA_SIZE, 0), 0);
	return SpherePrimitive(
		raw_data.xyz, // center
		raw_data.w // radius
	);
}

Mesh get_mesh(in int index)
{
	vec4 raw_data0 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE, 0), 0);
	vec4 raw_data1 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE + 1, 0), 0);
	vec4 raw_data2 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE + 2, 0), 0);
	vec4 raw_data3 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE + 3, 0), 0);
	vec4 raw_data4 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE + 4, 0), 0);
	vec4 raw_data5 = texelFetch(mesh_buffer, ivec2(index * MESH_DATA_SIZE + 5, 0), 0);
	
	return Mesh(
		int(raw_data0.x), // tri_start
		int(raw_data0.y), // tri_count
		vec3(raw_data0.zw, raw_data1.x), // bound_min
		raw_data1.yzw, // bound_max
		mat4(raw_data2, raw_data3, raw_data4, raw_data5) // transform
	);
}

Vertex get_vertex(in int index)
{
	vec4 raw_data0 = texelFetch(vertex_buffer, ivec2(index * VERTEX_DATA_SIZE, 0), 0);
	vec4 raw_data1 = texelFetch(vertex_buffer, ivec2(index * VERTEX_DATA_SIZE + 1, 0), 0);
	
	return Vertex(
		raw_data0.xyz, // position
		vec3(raw_data0.w, raw_data1.xy), // normal
		raw_data1.zw // uv
	);
}

Triangle get_triangle(in int index)
{
	float triangle_size_f = TRIANGLE_DATA_SIZE * 4.0;
	int triangle_size_i = int(floor(triangle_size_f));
	
	return Triangle(
		texelFetch(triangle_buffer, ivec2(int(floor(float(index) * triangle_size_f * .25)), 0), 0)[(index * triangle_size_i) % 4], // v0
		texelFetch(triangle_buffer, ivec2(int(floor((float(index) * triangle_size_f + 1.0) * .25)), 0), 0)[(index * triangle_size_i + 1) % 4], // v1
		texelFetch(triangle_buffer, ivec2(int(floor((float(index) * triangle_size_f + 2.0) * .25)), 0), 0)[(index * triangle_size_i + 2) % 4] // v2
	);
}

Material get_material(in int index)
{
	vec4 raw_data0 = texelFetch(material_buffer, ivec2(index * MATERIAL_DATA_SIZE, 0), 0);
	vec4 raw_data1 = texelFetch(material_buffer, ivec2(index * MATERIAL_DATA_SIZE + 1, 0), 0);
	vec4 raw_data2 = texelFetch(material_buffer, ivec2(index * MATERIAL_DATA_SIZE + 2, 0), 0);
	vec4 raw_data3 = texelFetch(material_buffer, ivec2(index * MATERIAL_DATA_SIZE + 3, 0), 0);
	
	return Material(
		int(raw_data0.x), // type
		vec4(raw_data0.yzw, raw_data1.x), // color
		raw_data1.yzw, // emissive
		raw_data2.x, // emissive_intensity
		raw_data2.y, // smoothness
		vec3(raw_data2.zw, raw_data3.x), // specular_color
		raw_data3.y, // specular_probability
		int(raw_data3.z) // texture_index
	);
}

TrianglePrimitive get_triangle_primitive(in Triangle triangle, in mat4 transform)
{
	Vertex v0 = get_vertex(triangle.v0);
	Vertex v1 = get_vertex(triangle.v1);
	Vertex v2 = get_vertex(triangle.v2);
	return TrianglePrimitive(
		(transform * vec4(v0.position, 1.0)).xyz,
		(transform * vec4(v1.position, 1.0)).xyz,
		(transform * vec4(v2.position, 1.0)).xyz,
		(transform * vec4(v0.normal, 0.0)).xyz,
		(transform * vec4(v1.normal, 0.0)).xyz,
		(transform * vec4(v2.normal, 0.0)).xyz,
		v0.uv,
		v1.uv,
		v2.uv
	);
}

HitInfo compute_mesh_collision(in Ray ray, in Mesh mesh, in float max_dist)
{
	HitInfo info = get_empty_hit();
	
	if (!ray_box(ray, mesh.bound_min, mesh.bound_max, max_dist))
	{
		return info;
	}
	
	for (int i = mesh.tri_start; i < mesh.tri_start + mesh.tri_count; i++)
	{
		TrianglePrimitive primitive = get_triangle_primitive(get_triangle(i), mesh.transform);
		HitInfo triangle_hit = ray_triangle(ray, primitive);
		
		if (!triangle_hit.hit || triangle_hit.dist >= info.dist)
			continue;
		
		info = triangle_hit;
	}
	
	return info;
}

HitResult trace_ray_collision(in Ray ray)
{
	HitResult result = HitResult(
		get_empty_hit(),
		Shape(
			-1, // type
			-1, // data_index
			-1  // material_index
		)
	);
	
	if (!draw_shapes)
	{
		return result;
	}
	
	int shape_count = int(floor(float(textureSize(shape_buffer, 0).x) / SHAPE_DATA_SIZE));
	
	for (int i = 0; i < shape_count; i++)
	{
		Shape shape = get_shape(i);
		HitInfo shape_hit = get_empty_hit();
		
		switch (shape.type)
		{
			case SHAPE_TYPE_SPHERE:
				shape_hit = ray_sphere(ray, get_sphere(shape.data_index));
				break;
			case SHAPE_TYPE_MESH:
				shape_hit = compute_mesh_collision(ray, get_mesh(shape.data_index), result.info.dist);
				break;
			default:
				break;
		}
		
		if (!shape_hit.hit || shape_hit.dist >= result.info.dist)
			continue;
		
		result.info = shape_hit;
		result.shape = shape;
	}
	
	return result;
}