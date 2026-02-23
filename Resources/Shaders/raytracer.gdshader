shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled, shadows_disabled, ambient_light_disabled;

#include "ray_functions.gdshaderinc"
#include "shader_utilities.gdshaderinc"

const float MAX_DISTANCE = 1e24;
const int DISPLAY_DEFAULT = 0;
const int DISPLAY_NORMAL = 1;
const int DISPLAY_UNSHADED = 2;
const int DISPLAY_DEPTH = 3;
const int DISPLAY_RAY_UV = 4;

uniform uint ray_count = 1;
uniform uint bounce_count = 1;

group_uniforms Debug;

uniform int display_type: hint_enum("Default,Normal,Unshaded,Depth,Ray UV") = 0;
uniform float max_depth = 10.0;

group_uniforms Environment;

uniform bool enable_environment = false;

group_uniforms Environment.Sky;

uniform vec3 horizon_color: source_color;
uniform vec3 zenith_color: source_color;
uniform vec3 ground_color: source_color;

group_uniforms Environment.Sun;

uniform vec3 sun_light_direction;
uniform float sun_focus;
uniform float sun_intensity;

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
};

struct Mesh
{
	int triangle_start_index;
	int triangle_count;
	vec3 bound_min;
	vec3 bound_max;
	vec3 position;
	vec3 rotation;
	vec3 scale;
	
	Material material;
};

struct HitObject
{
	HitInfo hit;
	Material material;
};

vec3 GetEnvironmentLight(Ray ray)
{
	if (!enable_environment)
	{
		return vec3(0.0);
	}
	
	float sky_ratio = pow(smoothstep(0, 0.04, ray.dir.y), 0.35);
	vec3 sky_gradient = mix(horizon_color, zenith_color, sky_ratio);
	float sun = pow(max(0, dot(ray.dir, -normalize(sun_light_direction))), sun_focus) * sun_intensity;
	
	float ground_ratio = smoothstep(-0.01, 0.01, ray.dir.y);
	float sun_mask = float(ground_ratio >= 1.0);
	
	return mix(ground_color, sky_gradient, ground_ratio) + sun * sun_mask;
}

void ComputeSpheresCollision(inout Ray ray, inout HitInfo hit_info, inout Material material)
{
	if (!draw_spheres)
		return;
	
	int sphere_count = textureSize(sphere_buffer, 0).x / 3;
	
	for (int i = 0; i < sphere_count; i++)
	{
		vec4 data0 = texelFetch(sphere_buffer, ivec2(i * 3, 0), 0);
		
		Sphere sphere = Sphere(data0.xyz, data0.w);
		HitInfo sphere_hit = ray_sphere(ray, sphere);
		
		if (!sphere_hit.hit || sphere_hit.dist >= hit_info.dist)
			continue;
		
		hit_info = sphere_hit;
		
		vec4 data1 = texelFetch(sphere_buffer, ivec2(i * 3 + 1, 0), 0);
		vec4 data2 = texelFetch(sphere_buffer, ivec2(i * 3 + 2, 0), 0);
		material = Material(data1, data2.rgb, data2.w);
	}
}

void ComputeMeshCollision(inout Ray ray, inout HitInfo hit_info, inout Material material, in Mesh mesh)
{
	for (int j = 0; j < mesh.triangle_count; j++)
	{
		vec4 tdata0 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * 5, 0), 0);
		vec4 tdata1 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * 5 + 1, 0), 0);
		vec4 tdata2 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * 5 + 2, 0), 0);
		vec4 tdata3 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * 5 + 3, 0), 0);
		vec4 tdata4 = texelFetch(triangle_buffer, ivec2((mesh.triangle_start_index + j) * 5 + 4, 0), 0);
		
		Triangle triangle = Triangle(
			tdata0.xyz + mesh.position,                // Point A
			vec3(tdata0.w, tdata1.xy) + mesh.position, // Point B
			vec3(tdata1.zw, tdata2.x) + mesh.position, // Point C
			tdata2.yzw,                                // Normal A
			tdata3.xyz,                                // Normal B
			vec3(tdata3.w, tdata4.xy)                  // Normal C
		);
		HitInfo triangle_hit = ray_triangle(ray, triangle);
		
		if (!triangle_hit.hit || triangle_hit.dist > hit_info.dist)
			continue;
		
		hit_info = triangle_hit;
		material = mesh.material;
	}
}

void ComputeMeshesCollision(inout Ray ray, inout HitInfo hit_info, inout Material material)
{
	if (!draw_meshes)
		return;
	
	int mesh_count = textureSize(mesh_buffer, 0).x / 7;
	
	for (int i = 0; i < mesh_count; i++)
	{
		vec4 data0 = texelFetch(mesh_buffer, ivec2(i * 7, 0), 0);
		vec4 data1 = texelFetch(mesh_buffer, ivec2(i * 7 + 1, 0), 0);
		vec4 data2 = texelFetch(mesh_buffer, ivec2(i * 7 + 2, 0), 0);
		vec4 data3 = texelFetch(mesh_buffer, ivec2(i * 7 + 3, 0), 0);
		vec4 data4 = texelFetch(mesh_buffer, ivec2(i * 7 + 4, 0), 0);
		vec4 data5 = texelFetch(mesh_buffer, ivec2(i * 7 + 5, 0), 0);
		vec4 data6 = texelFetch(mesh_buffer, ivec2(i * 7 + 6, 0), 0);
		
		Mesh mesh = Mesh(
			int(data0.x),            // Start index
			int(data0.y),            // Num triangles
			vec3(data0.zw, data1.x), // Min bound
			data1.yzw,               // Max bound
			data2.xyz,               // Position
			vec3(data2.w, data3.xy), // Rotation
			vec3(data3.zw, data4.x), // Scale
			Material(
				vec4(data4.yzw, data5.x), // Color
				vec3(data5.yzw),          // Emissive
				data6.x                   // Emissive intensity
			)
		);
		
		ComputeMeshCollision(ray, hit_info, material, mesh);
	}
}

HitObject ComputeRayCollision(Ray ray)
{
	HitInfo hit = HitInfo(false, MAX_DISTANCE * 2.0, vec3(0.0), vec3(0.0), false);
	Material material = Material(vec4(0.0), vec3(0.0), 0.0);
	
	ComputeSpheresCollision(ray, hit, material);
	ComputeMeshesCollision(ray, hit, material);
	
	//for (int i = 0; i < box_count; i++)
	//{
		//Box box = Box(box_centers[i], box_extents[i]);
		//HitInfo box_hit = ray_box(ray, box);
		//
		//if (!box_hit.hit || box_hit.dist > hit.dist)
			//continue;
		//
		//hit = box_hit;
		//material = Material(box_colors[i], box_emissives[i]);
	//}
	
	return HitObject(hit, material);
}

vec3 Trace(Ray ray, inout uint rngState)
{
	vec3 ray_color = vec3(1.0);
	vec3 light = vec3(0.0);
	
	for (int i = 0; i < int(bounce_count) + 1; i++)
	{
		HitObject object = ComputeRayCollision(ray);
		
		if (!object.hit.hit)
		{
			light += GetEnvironmentLight(ray) * ray_color;
			break;
		}
		
		ray.origin = object.hit.point;
		ray.dir = normalize(object.hit.normal + random_dir(rngState));
		
		light += object.material.emissive * object.material.emissive_intensity * ray_color;
		ray_color *= object.material.color.xyz;
	}
	
	return light;
}

void display(inout vec3 albedo, in Ray ray, in uint rngState, in vec2 uv)
{
	for (int i = 0; i < int(ray_count); i++)
	{
		albedo.rgb += Trace(ray, rngState);
	}
	
	albedo.rgb /= float(ray_count);
	
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

void debug_ray_uv(inout vec3 albedo, in Ray ray)
{
	albedo.rgb = ray.dir;
}

void fragment()
{
	uint rngState = uint(FRAGCOORD.x * VIEWPORT_SIZE.y + FRAGCOORD.y) + frame_count * 719393u;
	
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
			display(ALBEDO, ray, rngState, SCREEN_UV);
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
		case DISPLAY_RAY_UV:
			debug_ray_uv(ALBEDO, ray);
			break;
	}
}