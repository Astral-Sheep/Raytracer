shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled, shadows_disabled, ambient_light_disabled;

#include "ray_functions.gdshaderinc"
#include "shader_utilities.gdshaderinc"

const float MAX_DISTANCE = 1e24;
const int SPHERE_DATA_SIZE = 5;
const int MESH_DATA_SIZE = 10;
const int TRIANGLE_DATA_SIZE = 5;

const int DISPLAY_DEFAULT = 0;
const int DISPLAY_NORMAL = 1;
const int DISPLAY_UNSHADED = 2;
const int DISPLAY_DEPTH = 3;
const int DISPLAY_BOUNCE_COUNT = 4;
const int DISPLAY_RAY_UV = 5;

uniform uint ray_count = 1;
uniform uint bounce_count = 1;

group_uniforms Debug;

uniform int display_type: hint_enum("Default,Normal,Unshaded,Depth,Bounces,Ray UV") = 0;
uniform float max_depth = 10.0;

group_uniforms Environment;

uniform bool enable_environment = false;

group_uniforms Environment.Sky;

uniform vec3 horizon_color: source_color;
uniform vec3 zenith_color: source_color;
uniform vec3 ground_color: source_color;

group_uniforms Environment.Sun;

uniform sampler2D sun_buffer;

group_uniforms Shapes;

uniform bool draw_spheres = false;
uniform sampler2D sphere_buffer;

uniform bool draw_boxes = false;
uniform sampler2D box_buffer;

uniform bool draw_meshes = false;
uniform sampler2D mesh_buffer;
uniform sampler2D triangle_buffer;

group_uniforms _Internal.Plane;

uniform vec2 plane_size;
uniform float near_clip_plane;
uniform mat4 local_to_world_matrix;

group_uniforms _Internal.Accumulation;

uniform bool enable_accumulation;
uniform sampler2D last_frame_texture: source_color;
uniform uint frame_count;

group_uniforms;

struct Material
{
	vec4 color;
	vec3 emissive;
	float emissive_intensity;
	float smoothness;
	vec3 specular_color;
	float specular_probability;
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

struct HitObject
{
	HitInfo hit;
	Material material;
};

struct Sun
{
	vec3 direction;
	float focus;
	vec3 color;
	float energy;
};

vec3 GetEnvironmentLight(in Ray ray)
{
	if (!enable_environment)
	{
		return vec3(0.0);
	}
	
	float sky_ratio = pow(smoothstep(0, 0.04, ray.dir.y), 0.35);
	vec3 sky_gradient = mix(horizon_color, zenith_color, sky_ratio);
	
	vec3 sun_color = vec3(0.0);
	
	for (int i = 0; i < textureSize(sun_buffer, 0).x / 2; i++)
	{
		vec4 data0 = texelFetch(sun_buffer, ivec2(i * 2, 0), 0);
		vec4 data1 = texelFetch(sun_buffer, ivec2(i * 2 + 1, 0), 0);
		Sun sun = Sun(
			data0.xyz,
			data0.w,
			data1.rgb,
			data1.a
		);
		
		sun_color += pow(max(0, dot(ray.dir, -normalize(sun.direction))), sun.focus) * sun.energy * sun.color;
	}
	
	float ground_ratio = smoothstep(-0.01, 0.01, ray.dir.y);
	float sun_mask = float(ground_ratio >= 1.0);
	
	return mix(ground_color, sky_gradient, ground_ratio) + sun_color * sun_mask;
}

void ComputeSpheresCollision(in Ray ray, inout HitInfo hit_info, inout Material material)
{
	if (!draw_spheres)
		return;
	
	int sphere_count = textureSize(sphere_buffer, 0).x / SPHERE_DATA_SIZE;
	
	for (int i = 0; i < sphere_count; i++)
	{
		vec4 data0 = texelFetch(sphere_buffer, ivec2(i * SPHERE_DATA_SIZE, 0), 0);
		
		Sphere sphere = Sphere(data0.xyz, data0.w);
		HitInfo sphere_hit = ray_sphere(ray, sphere);
		
		if (!sphere_hit.hit || sphere_hit.dist >= hit_info.dist)
			continue;
		
		hit_info = sphere_hit;
		
		vec4 data1 = texelFetch(sphere_buffer, ivec2(i * SPHERE_DATA_SIZE + 1, 0), 0);
		vec4 data2 = texelFetch(sphere_buffer, ivec2(i * SPHERE_DATA_SIZE + 2, 0), 0);
		vec4 data3 = texelFetch(sphere_buffer, ivec2(i * SPHERE_DATA_SIZE + 3, 0), 0);
		vec4 data4 = texelFetch(sphere_buffer, ivec2(i * SPHERE_DATA_SIZE + 4, 0), 0);
		
		material = Material(data1, data2.rgb, data2.w, data3.x, data3.yzw, data4.x);
	}
}

void ComputeMeshCollision(in Ray ray, in Mesh mesh, inout HitInfo hit_info, inout Material material)
{
	if (!ray_box(ray, mesh.bound_min, mesh.bound_max, hit_info.dist))
		return;
	
	for (int j = 0; j < mesh.triangle_count; j++)
	{
		vec4 tdata0 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * TRIANGLE_DATA_SIZE, 0), 0);
		vec4 tdata1 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * TRIANGLE_DATA_SIZE + 1, 0), 0);
		vec4 tdata2 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * TRIANGLE_DATA_SIZE + 2, 0), 0);
		vec4 tdata3 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * TRIANGLE_DATA_SIZE + 3, 0), 0);
		vec4 tdata4 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * TRIANGLE_DATA_SIZE + 4, 0), 0);
		
		Triangle triangle = Triangle(
			(mesh.transform * vec4(tdata0.xyz, 1.0)).xyz,          // Point A
			(mesh.transform * vec4(tdata0.w, tdata1.xy, 1.0)).xyz, // Point B
			(mesh.transform * vec4(tdata1.zw, tdata2.x, 1.0)).xyz, // Point C
			(mesh.transform * vec4(tdata2.yzw, 0.0)).xyz,          // Normal A
			(mesh.transform * vec4(tdata3.xyz, 0.0)).xyz,          // Normal B
			(mesh.transform * vec4(tdata3.w, tdata4.xy, 0.0)).xyz  // Normal C
		);
		HitInfo triangle_hit = ray_triangle(ray, triangle);
		
		if (!triangle_hit.hit || triangle_hit.dist > hit_info.dist)
			continue;
		
		hit_info = triangle_hit;
		material = mesh.material;
	}
}

void ComputeMeshesCollision(in Ray ray, inout HitInfo hit_info, inout Material material)
{
	if (!draw_meshes)
		return;
	
	int mesh_count = textureSize(mesh_buffer, 0).x / 7;
	
	for (int i = 0; i < mesh_count; i++)
	{
		vec4 data0 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE, 0), 0);
		vec4 data1 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 1, 0), 0);
		vec4 data2 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 2, 0), 0);
		vec4 data3 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 3, 0), 0);
		vec4 data4 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 4, 0), 0);
		vec4 data5 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 5, 0), 0);
		vec4 data6 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 6, 0), 0);
		vec4 data7 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 7, 0), 0);
		vec4 data8 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 8, 0), 0);
		vec4 data9 = texelFetch(mesh_buffer, ivec2(i * MESH_DATA_SIZE + 9, 0), 0);
		
		Mesh mesh = Mesh(
			int(data0.x),                     // Start index
			int(data0.y),                     // Num triangles
			vec3(data0.zw, data1.x),          // Min bound
			data1.yzw,                        // Max bound
			mat4(data2, data3, data4, data5), // Transform
			Material(
				vec4(data6.xyz, data6.w), // Color
				vec3(data7.xyz),          // Emissive
				data7.w,                  // Emissive intensity
				data8.x,                  // Smoothness
				data8.yzw,                // Specular color
				data9.x                   // Specular probability
			)
		);
		
		ComputeMeshCollision(ray, mesh, hit_info, material);
	}
}

HitObject ComputeRayCollision(in Ray ray)
{
	HitInfo hit = HitInfo(false, MAX_DISTANCE * 2.0, vec3(0.0), vec3(0.0), false);
	Material material = Material(vec4(0.0), vec3(0.0), 0.0, 0.0, vec3(0.0), 0.0);
	
	ComputeSpheresCollision(ray, hit, material);
	ComputeMeshesCollision(ray, hit, material);
	
	return HitObject(hit, material);
}

vec3 Trace(in Ray ray, inout uint rng_state, inout int bounces)
{
	vec3 ray_color = vec3(1.0);
	vec3 light = vec3(0.0);
	
	for (int i = 0; i <= int(bounce_count); i++)
	{
		HitObject object = ComputeRayCollision(ray);
		
		if (!object.hit.hit)
		{
			light += GetEnvironmentLight(ray) * ray_color;
			break;
		}
		
		ray.origin = object.hit.point;
		vec3 diffuse_dir = normalize(object.hit.normal + random_dir(rng_state));
		vec3 specular_dir = reflect(ray.dir, object.hit.normal);
		bool is_specular = object.material.specular_probability >= randf(rng_state);
		
		ray.dir = normalize(mix(diffuse_dir, specular_dir, object.material.smoothness * float(is_specular)));
		ray.invdir = 1.0 / ray.dir;
		
		light += object.material.emissive * object.material.emissive_intensity * ray_color;
		ray_color *= mix(object.material.color.xyz, object.material.specular_color, float(is_specular));
		++bounces;
	}
	
	return light;
}

void display(inout vec3 albedo, in Ray ray, in uint rng_state, in vec2 uv)
{
	vec3 light = vec3(0.0);
	int bounces = 0;
	
	for (int i = 0; i < int(ray_count); i++)
	{
		light += Trace(ray, rng_state, bounces);
	}
	
	light /= float(ray_count);
	albedo += light;
	
	if (enable_accumulation && frame_count > 0u)
	{
		vec3 last_color = texture(last_frame_texture, uv).rgb;
		float weight = 1.0 / (float(frame_count) + 1.0);
		albedo.rgb = last_color * (1.0 - weight) + albedo.rgb * weight;
	}
}

void debug_normal(inout vec3 albedo, in Ray ray)
{
	HitInfo hit_info = ComputeRayCollision(ray).hit;
	
	if (hit_info.hit)
	{
		albedo.rgb = (hit_info.normal + vec3(1.0)) * 0.5;
	}
}

void debug_unshaded(inout vec3 albedo, in Ray ray)
{
	albedo.rgb = ComputeRayCollision(ray).material.color.xyz;
}

void debug_depth(inout vec3 albedo, in Ray ray)
{
	albedo.rgb = vec3(ComputeRayCollision(ray).hit.dist / max_depth);
}

void debug_bounce_count(inout vec3 albedo, in Ray ray, inout uint rng_state)
{
	int bounces = 0;
	Trace(ray, rng_state, bounces);
	albedo.rgb = mix(vec3(0.0), vec3(1.0), float(bounces) / float(bounce_count + 1u));
}

void debug_ray_uv(inout vec3 albedo, in Ray ray)
{
	albedo.rgb = ray.dir;
}

void fragment()
{
	uint rng_sate = uint(FRAGCOORD.x * VIEWPORT_SIZE.y + FRAGCOORD.y) + frame_count * 719393u;
	
	ALBEDO.rgb = vec3(0.0);
	
	vec3 local_view_point = vec3(SCREEN_UV.x - 0.5, 1.0 - SCREEN_UV.y - 0.5, -1.0) * vec3(plane_size * vec2(0.5), near_clip_plane);
	vec3 view_point = (local_to_world_matrix * vec4(local_view_point, 1.0)).xyz;
	
	Ray ray;
	ray.origin = (local_to_world_matrix * vec4(0.0, 0.0, 0.0, 1.0)).xyz;
	ray.dir = normalize(view_point - ray.origin);
	ray.invdir = 1.0 / ray.dir;
	
	switch (display_type)
	{
		case DISPLAY_DEFAULT:
			display(ALBEDO, ray, rng_sate, SCREEN_UV);
			break;
		case DISPLAY_NORMAL:
			debug_normal(ALBEDO, ray);
			break;
		case DISPLAY_UNSHADED:
			debug_unshaded(ALBEDO, ray);
			break;
		case DISPLAY_DEPTH:
			debug_depth(ALBEDO, ray);
			break;
		case DISPLAY_BOUNCE_COUNT:
			debug_bounce_count(ALBEDO, ray, rng_sate);
			break;
		case DISPLAY_RAY_UV:
			debug_ray_uv(ALBEDO, ray);
			break;
	}
}