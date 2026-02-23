float randf(inout uint state)
{
	state = state * 747796405u + 2891336453u;
	uint result = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
	result = (result >> 22u) ^ result;
	return float(result) / 4294967295.0;
	//return fract(sin(dot(st + vec2(TIME, 0.0), vec2(12.9898, 78.233))) * 43758.5453123);
}

bool randb(inout uint state)
{
	return randf(state) < 0.5;
}

bvec2 randbvec2(inout uint state)
{
	return bvec2(
		randb(state),
		randb(state)
	);
}

bvec3 randbvec3(inout uint state)
{
	return bvec3(
		randb(state),
		randb(state),
		randb(state)
	);
}

bvec4 randbvec4(inout uint state)
{
	return bvec4(
		randb(state),
		randb(state),
		randb(state),
		randb(state)
	);
}

vec2 randvec2(inout uint state)
{
	return vec2(
		randf(state),
		randf(state)
	);
}

vec3 randvec3(inout uint state)
{
	return vec3(
		randf(state),
		randf(state),
		randf(state)
	);
}

vec4 randvec4(inout uint state)
{
	return vec4(
		randf(state),
		randf(state),
		randf(state),
		randf(state)
	);
}

float randf_range(inout uint state, in float p_min, in float p_max)
{
	return p_min + randf(state) * (p_max - p_min);
}

int randi_range(inout uint state, in int p_min, in int p_max)
{
	return int(floor(randf_range(state, float(p_min), float(p_max))));
}

ivec2 randivec2_range(inout uint state, in ivec2 p_min, in ivec2 p_max)
{
	return ivec2(
		randi_range(state, p_min.x, p_max.x),
		randi_range(state, p_min.y, p_max.y)
	);
}

ivec3 randivec3_range(inout uint state, in ivec3 p_min, in ivec3 p_max)
{
	return ivec3(
		randi_range(state, p_min.x, p_max.x),
		randi_range(state, p_min.y, p_max.y),
		randi_range(state, p_min.z, p_max.z)
	);
}

ivec4 randivec4_range(inout uint state, in ivec4 p_min, in ivec4 p_max)
{
	return ivec4(
		randi_range(state, p_min.x, p_max.x),
		randi_range(state, p_min.y, p_max.y),
		randi_range(state, p_min.z, p_max.z),
		randi_range(state, p_min.w, p_max.w)
	);
}

uint randu_range(inout uint state, in uint p_min, in uint p_max)
{
	return uint(randf_range(state, float(p_min), float(p_max)));
}

uvec2 randuvec2_range(inout uint state, in uvec2 p_min, in uvec2 p_max)
{
	return uvec2(
		randu_range(state, p_min.x, p_max.x),
		randu_range(state, p_min.y, p_max.y)
	);
}

uvec3 randuvec3_range(inout uint state, in uvec3 p_min, in uvec3 p_max)
{
	return uvec3(
		randu_range(state, p_min.x, p_max.x),
		randu_range(state, p_min.y, p_max.y),
		randu_range(state, p_min.z, p_max.z)
	);
}

uvec4 randuvec4_range(inout uint state, in uvec4 p_min, in uvec4 p_max)
{
	return uvec4(
		randu_range(state, p_min.x, p_max.x),
		randu_range(state, p_min.y, p_max.y),
		randu_range(state, p_min.z, p_max.z),
		randu_range(state, p_min.w, p_max.w)
	);
}

vec2 randvec2_range(inout uint state, in vec2 p_min, in vec2 p_max)
{
	return vec2(
		randf_range(state, p_min.x, p_max.x),
		randf_range(state, p_min.y, p_max.y)
	);
}

vec3 randvec3_range(inout uint state, in vec3 p_min, in vec3 p_max)
{
	return vec3(
		randf_range(state, p_min.x, p_max.x),
		randf_range(state, p_min.y, p_max.y),
		randf_range(state, p_min.z, p_max.z)
	);
}

vec4 randvec4_range(inout uint state, in vec4 p_min, in vec4 p_max)
{
	return vec4(
		randf_range(state, p_min.x, p_max.x),
		randf_range(state, p_min.y, p_max.y),
		randf_range(state, p_min.z, p_max.z),
		randf_range(state, p_min.w, p_max.w)
	);
}

float normal_randf(inout uint state)
{
	float theta = 2.0 * PI * randf(state);
	float rho = sqrt(-2.0 * log(randf(state)));
	return rho * cos(theta);
}