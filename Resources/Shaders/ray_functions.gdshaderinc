#include "shader_utilities.gdshaderinc"

const float MAX_DISTANCE = 1e24;

struct Ray
{
	vec3 origin;
	vec3 dir;
	vec3 invdir;
};

struct HitInfo
{
	bool hit;
	float dist;
	vec3 point;
	vec3 normal;
	vec2 uv;
	bool front_face;
};

struct SpherePrimitive
{
	vec3 center;
	float radius;
};

struct Box
{
	vec3 center;
	vec3 extent;
};

struct TrianglePrimitive
{
	vec3 pointA, pointB, pointC;
	vec3 normalA, normalB, normalC;
	vec2 uvA, uvB, uvC;
};

HitInfo get_empty_hit()
{
	return HitInfo(
		false, // hit
		MAX_DISTANCE * 2.0, // dist
		vec3(0.0), // point
		vec3(0.0), // normal
		vec2(0.0), // uv
		false // front_face
	);
}

HitInfo ray_sphere(in Ray ray, in SpherePrimitive sphere)
{
	HitInfo hit_info = get_empty_hit();
	
	vec3 origin = ray.origin - sphere.center;
	float b = 2.0 * dot(ray.dir, origin);
	float c = dot(origin, origin) - sphere.radius * sphere.radius;
	float delta = b * b - 4.0 * c;
	
	// No intersection
	if (delta < 0.0)
	{
		return hit_info;
	}
	
	float sqrt_delta = sqrt(delta);
	
	// dist is < 0
	if (-b < sqrt_delta)
	{
		return hit_info;
	}
	
	hit_info.hit = true;
	hit_info.dist = -(b + sqrt_delta) * 0.5;
	hit_info.point = ray.origin + ray.dir * hit_info.dist;
	hit_info.normal = (hit_info.point - sphere.center) / sphere.radius;
	
	float face_direction = sign(dot(ray.dir, hit_info.normal));
	hit_info.normal *= -face_direction;
	hit_info.front_face = face_direction < 0.0;
	
	return hit_info;
}

HitInfo ray_triangle(in Ray ray, in TrianglePrimitive triangle)
{
	vec3 ab = triangle.pointB - triangle.pointA;
	vec3 ac = triangle.pointC - triangle.pointA;
	vec3 normal = cross(ab, ac);
	vec3 ao = ray.origin - triangle.pointA;
	vec3 dao = cross(ao, ray.dir);
	
	float delta = -dot(ray.dir, normal);
	float inv_delta = 1.0 / delta;
	
	float dist = dot(ao, normal) * inv_delta;
	float u = dot(ac, dao) * inv_delta;
	float v = -dot(ab, dao) * inv_delta;
	float w = 1.0 - u - v;
	
	HitInfo hit_info = get_empty_hit();
	hit_info.hit = delta >= EPSILON && dist >= 0.0 && u >= 0.0 && v >= 0.0 && w >= 0.0;
	
	if (!hit_info.hit)
	{
		return hit_info;
	}
	
	hit_info.dist = dist;
	hit_info.point = ray.origin + ray.dir * hit_info.dist;
	hit_info.normal = normalize(triangle.normalA * w + triangle.normalB * u + triangle.normalC * v);
	hit_info.uv = triangle.uvA * w + triangle.uvB * u + triangle.uvC * v;
	
	//float face_direction = sign(dot(ray.dir, hit_info.normal));
	//hit_info.normal *= -face_direction;
	//hit_info.front_face = face_direction < 0.0;
	
	return hit_info;
}

bool ray_box(in Ray ray, in vec3 bound_min, in vec3 bound_max, in float min_dist)
{	
	vec3 tmin = (bound_min - ray.origin) * ray.invdir;
	vec3 tmax = (bound_max - ray.origin) * ray.invdir;
	vec3 t0 = min(tmin, tmax);
	vec3 t1 = max(tmin, tmax);
	float tnear = max(max(t0.x, t0.y), t0.z);
	float tfar = min(min(t1.x, t1.y), t1.z);
	
	return tnear <= tfar && tfar >= 0.0 && tnear < min_dist;
}

HitInfo ray_plane(in Ray ray, in vec3 plane_point, in vec3 plane_normal)
{
	HitInfo hit_info = get_empty_hit();
	
	float denom = dot(ray.dir, plane_normal);
	
	if (denom < EPSILON)
	{
		return hit_info;
	}
	
	float dist = dot(plane_point - ray.origin, plane_normal) / denom;
	
	if (dist < 0.0)
	{
		return hit_info;
	}
	
	hit_info.hit = true;
	hit_info.dist = dist;
	hit_info.point = ray.origin + ray.dir * hit_info.dist;
	hit_info.normal = plane_normal;
	hit_info.front_face = true;
	return hit_info;
}

vec3 random_dir(inout uint state)
{
	return normalize(vec3(normal_randf(state), normal_randf(state), normal_randf(state)));
}