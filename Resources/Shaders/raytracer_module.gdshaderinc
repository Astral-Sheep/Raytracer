#include "ray_functions.gdshaderinc"

const int SPHERE_DATA_SIZE = 5;
const int MESH_DATA_SIZE = 10;
const int TRIANGLE_DATA_SIZE = 6;

const float MAX_DISTANCE = 1e24;

struct Material
{
	vec4 color;
	vec3 emissive;
	float emissive_intensity;
	float smoothness;
	vec3 specular_color;
	float specular_probability;
	int texture_index;
};

struct Mesh
{
	int triangle_start_index;
	int triangle_count;
	vec3 bound_min;
	vec3 bound_max;
	mat4 transform;
	
	Material material;
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
	Material material;
};

void trace_spheres(in Ray ray, in sampler2D sphere_buffer, inout HitResult hit_result)
{
	int sphere_count = textureSize(sphere_buffer, 0).x / SPHERE_DATA_SIZE;
	
	for (int i = 0; i < sphere_count; i++)
	{
		int sphere_index = i * SPHERE_DATA_SIZE;
		vec4 data0 = texelFetch(sphere_buffer, ivec2(sphere_index, 0), 0);
		
		Sphere sphere = Sphere(data0.xyz, data0.w);
		HitInfo sphere_hit = ray_sphere(ray, sphere);
		
		if (!sphere_hit.hit || sphere_hit.dist >= hit_result.info.dist)
			continue;
		
		hit_result.info = sphere_hit;
		
		vec4 data1 = texelFetch(sphere_buffer, ivec2(sphere_index + 1, 0), 0);
		vec4 data2 = texelFetch(sphere_buffer, ivec2(sphere_index + 2, 0), 0);
		vec4 data3 = texelFetch(sphere_buffer, ivec2(sphere_index + 3, 0), 0);
		vec4 data4 = texelFetch(sphere_buffer, ivec2(sphere_index + 4, 0), 0);
		
		hit_result.material = Material(
			data1, // color
			data2.rgb, // emissive
			data2.w, // emissive_intensity
			data3.x, // smoothness
			data3.yzw, // specular_color
			data4.x, // specular_probability
			int(data4.y) // texture_index
		);
	}
}

void compute_mesh_collision(in Ray ray, in sampler2D triangle_buffer, in Mesh mesh, inout HitResult hit_result)
{
	if (!ray_box(ray, mesh.bound_min, mesh.bound_max, hit_result.info.dist))
		return;
	
	for (int i = mesh.triangle_start_index; i < mesh.triangle_start_index + mesh.triangle_count; i++)
	{
		int tri_index = i * TRIANGLE_DATA_SIZE;
		vec4 tdata0 = texelFetch(triangle_buffer, ivec2(tri_index, 0), 0);
		vec4 tdata1 = texelFetch(triangle_buffer, ivec2(tri_index + 1, 0), 0);
		vec4 tdata2 = texelFetch(triangle_buffer, ivec2(tri_index + 2, 0), 0);
		vec4 tdata3 = texelFetch(triangle_buffer, ivec2(tri_index + 3, 0), 0);
		vec4 tdata4 = texelFetch(triangle_buffer, ivec2(tri_index + 4, 0), 0);
		vec4 tdata5 = texelFetch(triangle_buffer, ivec2(tri_index + 5, 0), 0);
		
		Triangle triangle = Triangle(
			(mesh.transform * vec4(tdata0.xyz, 1.0)).xyz,          // Point A
			(mesh.transform * vec4(tdata0.w, tdata1.xy, 1.0)).xyz, // Point B
			(mesh.transform * vec4(tdata1.zw, tdata2.x, 1.0)).xyz, // Point C
			(mesh.transform * vec4(tdata2.yzw, 0.0)).xyz,          // Normal A
			(mesh.transform * vec4(tdata3.xyz, 0.0)).xyz,          // Normal B
			(mesh.transform * vec4(tdata3.w, tdata4.xy, 0.0)).xyz, // Normal C
			tdata4.zw,
			tdata5.xy,
			tdata5.zw
		);
		HitInfo triangle_hit = ray_triangle(ray, triangle);
		
		if (!triangle_hit.hit || triangle_hit.dist > hit_result.info.dist)
			continue;
		
		hit_result = HitResult(triangle_hit, mesh.material);
	}
}

void trace_meshes(in Ray ray, in sampler2D mesh_buffer, in sampler2D triangle_buffer, inout HitResult hit_result)
{
	int mesh_count = textureSize(mesh_buffer, 0).x / MESH_DATA_SIZE;
	
	for (int i = 0; i < mesh_count; i++)
	{
		int mesh_index = i * MESH_DATA_SIZE;
		vec4 data0 = texelFetch(mesh_buffer, ivec2(mesh_index, 0), 0);
		vec4 data1 = texelFetch(mesh_buffer, ivec2(mesh_index + 1, 0), 0);
		vec4 data2 = texelFetch(mesh_buffer, ivec2(mesh_index + 2, 0), 0);
		vec4 data3 = texelFetch(mesh_buffer, ivec2(mesh_index + 3, 0), 0);
		vec4 data4 = texelFetch(mesh_buffer, ivec2(mesh_index + 4, 0), 0);
		vec4 data5 = texelFetch(mesh_buffer, ivec2(mesh_index + 5, 0), 0);
		vec4 data6 = texelFetch(mesh_buffer, ivec2(mesh_index + 6, 0), 0);
		vec4 data7 = texelFetch(mesh_buffer, ivec2(mesh_index + 7, 0), 0);
		vec4 data8 = texelFetch(mesh_buffer, ivec2(mesh_index + 8, 0), 0);
		vec4 data9 = texelFetch(mesh_buffer, ivec2(mesh_index + 9, 0), 0);
		
		Mesh mesh = Mesh(
			int(data0.x), // triangle_start_index
			int(data0.y), // triangle_count
			vec3(data0.zw, data1.x), // bound_min
			data1.yzw, // bound_max
			mat4(data2, data3, data4, data5), // transform
			Material(
				data6, // color
				vec3(data7.xyz), // emissive
				data7.w, // emissive_intensity
				data8.x, // smoothness
				data8.yzw, // specular_color
				data9.x, // specular_probability
				int(data9.y) // texture_index
			)
		);
		
		compute_mesh_collision(ray, triangle_buffer, mesh, hit_result);
	}
}

HitResult trace_ray_collision(in Ray ray, in bool draw_spheres, in sampler2D sphere_buffer, in bool draw_meshes, in sampler2D mesh_buffer, in sampler2D triangle_buffer)
{
	HitResult result = HitResult(
		HitInfo(
			false, // hit
			MAX_DISTANCE * 2.0, // dist
			vec3(0.0), // point
			vec3(0.0), // normal
			vec2(0.0), // uv
			false // front_face
		),
		Material(
			vec4(0.0), // color
			vec3(0.0), // emissive
			0.0, // emissive_intensity
			0.0, // smoothness
			vec3(0.0), // specular_color
			0.0, // specular_probability
			0 // texture_index
		)
	);
	
	if (draw_spheres)
	{
		trace_spheres(ray, sphere_buffer, result);
	}
	
	if (draw_meshes)
	{
		trace_meshes(ray, mesh_buffer, triangle_buffer, result);
	}
	
	return result;
}