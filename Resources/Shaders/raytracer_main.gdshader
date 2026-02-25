shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled, shadows_disabled, ambient_light_disabled;

#include "raytracer_module.gdshaderinc"

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
uniform bool debug_focus_plane = false;
uniform float max_depth = 10.0;

group_uniforms DepthOfField;

uniform float diverge_strength = 0.0;
uniform float defocus_strength = 0.0;
uniform float focus_plane_distance_offset = 0.0;

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
uniform sampler2DArray mesh_textures;

group_uniforms _Internal.Plane;

uniform vec2 plane_size;
uniform float near_clip_plane;
uniform mat4 local_to_world_matrix;

group_uniforms _Internal.Accumulation;

uniform bool enable_accumulation;
uniform sampler2D last_frame_texture: source_color;
uniform uint frame_count;

group_uniforms;

Ray get_ray(in vec3 origin, in vec3 view_point, in float screen_width, in bool depth_of_field, inout uint rng_state)
{
	float inv_screen_width = 1.0 / screen_width;
	vec2 defocus_jitter = rand_disk(rng_state) * (float(depth_of_field) * defocus_strength  * inv_screen_width);
	vec2 diverge_jitter = rand_disk(rng_state) * (float(depth_of_field) * diverge_strength * inv_screen_width);
	
	Ray ray;
	ray.origin = origin + (local_to_world_matrix * vec4(defocus_jitter, 0.0, 0.0)).xyz;
	ray.dir = normalize((view_point + (local_to_world_matrix * vec4(diverge_jitter, 0.0, 0.0)).xyz) - ray.origin);
	ray.invdir = 1.0 / ray.dir;
	
	return ray;
}

vec3 get_environment_light(in Ray ray)
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

vec3 trace(in Ray ray, inout uint rng_state, inout int bounces)
{
	vec3 ray_color = vec3(1.0);
	vec3 light = vec3(0.0);
	
	for (int i = 0; i <= int(bounce_count); i++)
	{
		HitResult hit_result = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer);
		
		if (!hit_result.info.hit)
		{
			light += get_environment_light(ray) * ray_color;
			break;
		}
		
		ray.origin = hit_result.info.point;
		vec3 diffuse_dir = normalize(hit_result.info.normal + random_dir(rng_state));
		vec3 specular_dir = reflect(ray.dir, hit_result.info.normal);
		bool is_specular = hit_result.material.specular_probability >= randf(rng_state);
		
		ray.dir = normalize(mix(diffuse_dir, specular_dir, hit_result.material.smoothness * float(is_specular)));
		ray.invdir = 1.0 / ray.dir;
		
		light += hit_result.material.emissive * hit_result.material.emissive_intensity * ray_color;
		
		vec4 texture_color = hit_result.material.texture_index >= 0 ? texture(mesh_textures, vec3(hit_result.info.uv, float(hit_result.material.texture_index))) : vec4(1.0);
		ray_color *= mix(hit_result.material.color.rgb * texture_color.rgb, hit_result.material.specular_color, float(is_specular));
		++bounces;
	}
	
	return light;
}

float get_focus_plane_dist(in Ray ray)
{
	vec3 camera_forward = (local_to_world_matrix * vec4(0.0, 0.0, -1.0, 0.0)).xyz;
	HitInfo hit_info = ray_plane(ray, ray.origin + camera_forward * (near_clip_plane + focus_plane_distance_offset), (local_to_world_matrix * vec4(0.0, 0.0, -1.0, 0.0)).xyz);
	return mix(MAX_DISTANCE, hit_info.dist, float(hit_info.hit));
}

void display(inout vec3 albedo, in vec2 uv, in vec3 ray_origin, in vec3 view_point, in float screen_size, inout uint rng_state)
{
	vec3 light = vec3(0.0);
	int bounces = 0;
	
	for (int i = 0; i < int(ray_count); i++)
	{
		light += trace(get_ray(ray_origin, view_point, screen_size, true, rng_state), rng_state, bounces);
	}
	
	light /= float(ray_count);
	albedo += light;
	
	if (enable_accumulation && frame_count > 0u)
	{
		vec3 last_color = texture(last_frame_texture, uv).rgb;
		float weight = 1.0 / (float(frame_count) + 1.0);
		albedo.rgb = last_color * (1.0 - weight) + albedo.rgb * weight;
	}
	else if (debug_focus_plane)
	{
		Ray ray = get_ray(ray_origin, view_point, screen_size, false, rng_state);
		HitResult hit_result = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer);
		albedo.rgb *= mix(vec3(1.0), vec3(0.125, 1.0, 0.25), float(!hit_result.info.hit || get_focus_plane_dist(ray) <= hit_result.info.dist));
	}
}

void debug_normal(inout vec3 albedo, in Ray ray)
{
	HitInfo hit_info = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer).info;
	
	if (hit_info.hit)
	{
		albedo.rgb = (hit_info.normal + vec3(1.0)) * 0.5;
	}
	
	if (debug_focus_plane)
	{
		albedo.rgb *= mix(vec3(1.0), vec3(0.125, 1.0, 0.25), float(!hit_info.hit || get_focus_plane_dist(ray) <= hit_info.dist));
	}
}

void debug_unshaded(inout vec3 albedo, in Ray ray)
{
	HitResult hit_result = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer);
	albedo.rgb = hit_result.material.color.xyz;
	
	if (debug_focus_plane)
	{
		albedo.rgb *= mix(vec3(1.0), vec3(0.125, 1.0, 0.25), float(!hit_result.info.hit || get_focus_plane_dist(ray) <= hit_result.info.dist));
	}
}

void debug_depth(inout vec3 albedo, in Ray ray)
{
	HitResult hit_result = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer);
	albedo.rgb = vec3(hit_result.info.dist / max_depth);
	
	if (debug_focus_plane)
	{
		albedo.rgb *= mix(vec3(1.0), vec3(0.125, 1.0, 0.25), float(!hit_result.info.hit || get_focus_plane_dist(ray) <= hit_result.info.dist));
	}
}

void debug_bounce_count(inout vec3 albedo, in Ray ray, inout uint rng_state)
{
	int bounces = 0;
	trace(ray, rng_state, bounces);
	albedo.rgb = mix(vec3(0.0), vec3(1.0), float(bounces) / float(bounce_count + 1u));
	
	if (debug_focus_plane)
	{
		HitResult hit_result = trace_ray_collision(ray, draw_spheres, sphere_buffer, draw_meshes, mesh_buffer, triangle_buffer);
		albedo.rgb *= mix(vec3(1.0), vec3(0.125, 1.0, 0.25), float(!hit_result.info.hit || get_focus_plane_dist(ray) <= hit_result.info.dist));
	}
}

void debug_ray_uv(inout vec3 albedo, in Ray ray)
{
	albedo.rgb = ray.dir;
}

void fragment()
{
	ALBEDO.rgb = vec3(0.0);
	
	uint rng_state = uint(FRAGCOORD.x * VIEWPORT_SIZE.y + FRAGCOORD.y) + frame_count * 719393u;
	vec3 origin = (local_to_world_matrix * vec4(0.0, 0.0, 0.0, 1.0)).xyz;
	
	float focus_plane = near_clip_plane + focus_plane_distance_offset;
	vec3 local_view_point = vec3(SCREEN_UV.x - 0.5, 1.0 - SCREEN_UV.y - 0.5, -1.0) * vec3(plane_size * vec2(0.5) * (focus_plane / near_clip_plane), focus_plane);
	vec3 view_point = (local_to_world_matrix * vec4(local_view_point, 1.0)).xyz;
	
	switch (display_type)
	{
		case DISPLAY_DEFAULT:
			display(ALBEDO, SCREEN_UV, origin, view_point, VIEWPORT_SIZE.x, rng_state);
			break;
		case DISPLAY_NORMAL:
			debug_normal(ALBEDO, get_ray(origin, view_point, VIEWPORT_SIZE.x, false, rng_state));
			break;
		case DISPLAY_UNSHADED:
			debug_unshaded(ALBEDO, get_ray(origin, view_point, VIEWPORT_SIZE.x, false, rng_state));
			break;
		case DISPLAY_DEPTH:
			debug_depth(ALBEDO, get_ray(origin, view_point, VIEWPORT_SIZE.x, false, rng_state));
			break;
		case DISPLAY_BOUNCE_COUNT:
			debug_bounce_count(ALBEDO, get_ray(origin, view_point, VIEWPORT_SIZE.x, false, rng_state), rng_state);
			break;
		case DISPLAY_RAY_UV:
			debug_ray_uv(ALBEDO, get_ray(origin, view_point, VIEWPORT_SIZE.x, false, rng_state));
			break;
	}
}