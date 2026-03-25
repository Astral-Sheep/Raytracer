const float EPSILON = 1e-6;
const float INV_255 = 1.0 / 255.0;

const lowp int MIN_BYTE = -128;
const lowp int MAX_BYTE = 127;
const mediump int MIN_SHORT = -32768;
const mediump int MAX_SHORT = 32767;
const highp int MIN_INT = -2147483648;
const highp int MAX_INT = 2147483647;

float randf(inout uint state)
{
	state = state * 747796405u + 2891336453u;
	uint result = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
	result = (result >> 22u) ^ result;
	return float(result) / 4294967295.0;
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

vec2 rand_disk(inout uint state)
{
	float angle = randf(state) * 2.0 * PI;
	vec2 disk_point = vec2(cos(angle), sin(angle));
	return disk_point * sqrt(randf(state));
}

float length_squared(vec2 x)
{
	return x.x * x.x + x.y * x.y;
}

float length_squared(vec3 x)
{
	return x.x * x.x + x.y * x.y + x.z * x.z;
}

float length_squared(vec4 x)
{
	return x.x * x.x + x.y * x.y + x.z * x.z + x.w * x.w;
}

vec4 get_buffer_texel(sampler2D buffer, int texel_index, ivec2 buffer_size)
{
	return texelFetch(buffer, ivec2(texel_index % buffer_size.x, texel_index / buffer_size.x), 0);
}

ivec4 get_buffer_texel(isampler2D buffer, int texel_index, ivec2 buffer_size)
{
	return texelFetch(buffer, ivec2(texel_index % buffer_size.x, texel_index / buffer_size.x), 0);
}

float halfBitsToFloat(int x)
{
	int e = (x & 0x7C00) >> 10; // exponent
    int m = (x & 0x03FF) << 13; // mantissa
    int v = int(float(m)) >> 23; // evil log2 bit hack to count leading zeros in denormalized format
    return intBitsToFloat(
		(x & 0x8000) << 16
		| int(e != 0) * ((e + 112) << 23 | m)
		| (int(e == 0) & int(m != 0)) * ((v - 37) << 23 | ((m << (150 - v)) & 0x007FE000))
	); // sign : normalized : denormalized
}

int quartic_max_pos_roots(float a, float b, float c, float d, float e)
{
	int s = int(sign(a));
	int count = 0;
	
	count += int(int(sign(b)) != s);
	s = int(sign(b));
	
	count += int(int(sign(c)) != s);
	s = int(sign(c));
	
	count += int(int(sign(d)) != s);
	s = int(sign(d));
	
	count += int(int(sign(e)) != s);
	s = int(sign(e));
	
	return count;
}

float cbrt(float a)
{
	return sign(a) * exp(log(abs(a)) / 3.0);
}

//vec2 cbrti(vec2 v)
//{
	//float a = length(v);
	//a = exp(log(a) / 3.0);
	//float th = atan(v.y / v.x) / 3.0;
	//return a * vec2(cos(th), sin(th));
//}
//
//float cubic(float b, float c, float d)
//{
	//float p = -b / 3.0;
	//float q = p * p * p + b * c / 6.0 - 0.5 * d;
	//float r = c / 3.0;
	//
	//float s = r - p * p;
	//float u = q * q + s * s * s;
	//
	//if (u < 0.0)
	//{
		//vec2 t = vec2(0.0, sqrt(-u));
		//vec2 Q = vec2(q, 0.0);
		//return (cbrti(Q + t) + cbrti(Q - t)).x + p;
	//}
	//else
	//{
		//float t = sqrt(u);
		//return cbrt(q + t) + cbrt(q - t) + p;
	//}
//}
//
//float root_sort(float a, float b, bool i)
//{
	//return EPSILON < b && b < abs(a) ? b * (i ? -1.0 : 1.0) : a;
//}
//
//float quartic(float b, float c, float d, float e)
//{
	//float sqrb = b * b;
	//float p = c - 3.0 * sqrb / 8.0;
	//float q = b * sqrb / 8.0 - 0.5 * b * c + d;
	//float r = (-3.0 * sqrb * sqrb + 256.0 * e - 64.0 * b * d + 16.0 * sqrb * c) / 256.0;
	//
	//float m = cubic(p, 0.25 * p * p - r, q * q / 8.0);
	//
	//float A = -0.25 * b;
	//float B = 0.5 * sqrt(2.0 * m);
	//float C = -2.0 * p - 2.0 * m;
	//float D = -sqrt(2.0) * q / sqrt(m);
	//float x = 10.0;
	//float i = 1.0;
	//float sqrtcpd = sqrt(C + D);
	//float sqrtcmd = sqrt(C - D);
	//
	//x = root_sort(x, A + B + 0.5 * sqrtcpd, true);
	//x = root_sort(x, A + B - 0.5 * sqrtcpd, false);
	//x = root_sort(x, A - B + 0.5 * sqrtcmd, true);
	//x = root_sort(x, A - B - 0.5 * sqrtcmd, false);
	//return x;
//}