#include "ray_functions.gdshaderinc"

const int SUN_DATA_SIZE = 2;

group_uniforms Environment;

uniform bool enable_environment = false;

group_uniforms Environment.Sky;

uniform vec3 horizon_color: source_color;
uniform vec3 zenith_color: source_color;
uniform vec3 ground_color: source_color;

group_uniforms Environment.Sun;

uniform sampler2D sun_buffer;
uniform ivec2 sun_buffer_size;

struct Sun
{
	vec3 direction;
	float focus;
	vec3 color;
	float energy;
};

Sun get_sun(in int index)
{
	index *= SUN_DATA_SIZE;
	
	vec4 data0 = get_buffer_texel(sun_buffer, index, sun_buffer_size);
	vec4 data1 = get_buffer_texel(sun_buffer, index + 1, sun_buffer_size);
	return Sun(
		data0.xyz, // direction
		data0.w, // focus
		data1.rgb, // color
		data1.a // energy
	);
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