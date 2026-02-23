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
uniform vec3 clear_color: source_color;

group_uniforms Debug;

uniform int display_type: hint_enum("Default,Normal,Unshaded,Depth,Ray UV") = 0;
uniform float max_depth = 10.0;

group_uniforms Shapes;

group_uniforms Shapes.Spheres;

uniform vec3 sphere_centers[16];
uniform float sphere_radiuses[16];
uniform vec3 sphere_colors[16]: source_color;
uniform vec3 sphere_emissives[16]: source_color;
uniform float sphere_emissive_intensities[16];
uniform int sphere_count = 0;

group_uniforms Shapes.Boxes;

uniform vec3 box_centers[16];
uniform vec3 box_extents[16];
uniform vec3 box_colors[16]: source_color;
uniform vec3 box_emissives[16]: source_color;
uniform float box_emissive_intensities[16];
uniform int box_count = 0;

//group_uniforms Shapes.Meshes
//
// uniform vec3 triangles[16 * 3];
// uniform vec3 triangles_colors[16];
// uniform vec3 triangles_emissives[16];
// uniform int triangleCount;

group_uniforms _Internal;

uniform vec2 plane_size;
uniform float near_clip_plane;
uniform mat4 local_to_world_matrix;
uniform sampler2D last_frame_texture;

group_uniforms;

struct Material
{
	vec3 color;
	vec3 emissive;
	float emissive_intensity;
};

struct HitObject
{
	HitInfo hit;
	Material material;
};

HitObject ComputeRayCollision(Ray ray)
{
	HitInfo hit = HitInfo(false, MAX_DISTANCE * 2.0, vec3(0.0), vec3(0.0), false);
	Material material = Material(vec3(0.0), vec3(0.0), 0.0);
	
	for (int i = 0; i < sphere_count; i++)
	{
		Sphere sphere = Sphere(sphere_centers[i], sphere_radiuses[i]);
		HitInfo sphere_hit = ray_sphere(ray, sphere);
		
		if (!sphere_hit.hit || sphere_hit.dist > hit.dist)
			continue;
		
		hit = sphere_hit;
		material = Material(sphere_colors[i], sphere_emissives[i], sphere_emissive_intensities[i]);
	}
	
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
			//color = clear_color;
			break;
		}
		
		light += object.material.emissive * object.material.emissive_intensity * ray_color;
		ray_color *= object.material.color;
		
		vec3 random_dir = random_dir(rngState);
		random_dir *= sign(dot(object.hit.normal, random_dir));
		ray.dir = normalize(random_dir);
	}
	
	return light;
}

void fragment()
{
	uint rngState = uint(FRAGCOORD.x * VIEWPORT_SIZE.y + FRAGCOORD.y);
	
	ALBEDO.rgb = vec3(0.0);
	vec2 axis_uv = vec2(SCREEN_UV.x, 1.0 - SCREEN_UV.y);
	
	vec3 local_view_point = vec3(axis_uv - vec2(0.5), -1.0) * vec3(plane_size * vec2(0.5), near_clip_plane);
	vec3 view_point = (local_to_world_matrix * vec4(local_view_point, 1.0)).xyz;
	
	Ray ray;
	ray.origin = (local_to_world_matrix * vec4(0.0, 0.0, 0.0, 1.0)).xyz;
	ray.dir = normalize(view_point - ray.origin);
	ray.invdir = 1.0 / ray.dir;
	
	switch (display_type)
	{
		case DISPLAY_DEFAULT:
			for (int i = 0; i < int(ray_count); i++)
			{
				ALBEDO.rgb += Trace(ray, rngState);
			}
			
			ALBEDO.rgb /= float(ray_count);
			break;
		case DISPLAY_NORMAL:
			HitInfo hit_info = ComputeRayCollision(ray).hit;
			
			if (hit_info.hit)
			{
				ALBEDO.rgb = (hit_info.normal + vec3(1.0)) * 0.5;
			}
			break;
		case DISPLAY_UNSHADED:
			ALBEDO.rgb = ComputeRayCollision(ray).material.color;
			break;
		case DISPLAY_RAY_UV:
			ALBEDO.rgb = ray.dir;
			break;
		case DISPLAY_DEPTH:
			ALBEDO.rgb = vec3(ComputeRayCollision(ray).hit.dist / max_depth);
			break;
	}
}