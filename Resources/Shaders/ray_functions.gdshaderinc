#include "shader_utilities.gdshaderinc"

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
	bool front_face;
};

struct Sphere
{
	vec3 center;
	float radius;
};

struct Box
{
	vec3 center;
	vec3 extent;
};

struct Triangle
{
	vec3 pointA, pointB, pointC;
	vec3 normalA, normalB, normalC;
};

HitInfo ray_sphere(in Ray ray, in Sphere sphere)
{
	HitInfo hit_info = HitInfo(false, 0.0, vec3(0.0), vec3(0.0), false);
	
	vec3 origin = ray.origin - sphere.center;
	float a = dot(ray.dir, ray.dir);
	float b = 2.0 * dot(ray.dir, origin);
	float c = dot(origin, origin) - sphere.radius * sphere.radius;
	float delta = b * b - 4.0 * a * c;
	
	if (delta < 0.0)
	{
		return hit_info;
	}
	
	float dist = -1.0;
	
	if (delta == 0.0)
	{
		dist = -b / (2.0 * a);
	}
	else
	{
		float sqrt_delta = sqrt(delta);
		float inv_a = 1.0 / a;
		float d0 = -(b - sqrt_delta) * inv_a * 0.5;
		float d1 = -(b + sqrt_delta) * inv_a * 0.5;
		dist = d0 > d1 ? d1 : d0;
	}
	
	if (dist < 0.0)
	{
		return hit_info;
	}
	
	hit_info.hit = true;
	hit_info.dist = dist;
	hit_info.point = ray.origin + ray.dir * hit_info.dist;
	hit_info.normal = (hit_info.point - sphere.center) / sphere.radius;
	
	float face_direction = sign(dot(ray.dir, hit_info.normal));
	hit_info.normal *= -face_direction;
	hit_info.front_face = face_direction < 0.0;
	
	return hit_info;
}

HitInfo ray_triangle(Ray ray, Triangle triangle)
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
	
	HitInfo hit_info = HitInfo(false, 0.0, vec3(0.0), vec3(0.0), false);
	hit_info.hit = delta >= 1e-6 && dist >= 0.0 && u >= 0.0 && v >= 0.0 && w >= 0.0;
	
	if (!hit_info.hit)
	{
		return hit_info;
	}
	
	hit_info.dist = dist;
	hit_info.point = ray.origin + ray.dir * hit_info.dist;
	hit_info.normal = normalize(triangle.normalA * w + triangle.normalB * u + triangle.normalC * v);
	return hit_info;
}

vec3 random_dir(inout uint state)
{
	return normalize(vec3(normal_randf(state), normal_randf(state), normal_randf(state)));
}

//HitInfo ray_box(Ray ray, Box box)
//{
	//HitInfo hit_info = HitInfo(false, 0.0, vec3(0.0), vec3(0.0), false);
	//vec3 bounds[2] = {
		//-box.extent,
		//box.extent
	//};
	//
	//vec3 origin = ray.origin - box.center;
	//vec3 tmin = vec3(
		//(bounds[int(ray.invdir.x < 0.0)].x - origin.x) * ray.invdir.x,
		//(bounds[int(ray.invdir.y < 0.0)].y - origin.y) * ray.invdir.y,
		//0.0
	//);
	//vec3 tmax = vec3(
		//(bounds[1 - int(ray.invdir.x < 0.0)].x - origin.x) * ray.invdir.x,
		//(bounds[1 - int(ray.invdir.y < 0.0)].y - origin.y) * ray.invdir.y,
		//0.0
	//);
	//
	//if (tmin.x > tmax.y || tmin.y > tmax.x)
	//{
		//return hit_info;
	//}
	//
	//if (tmin.y > tmin.x)
	//{
		//tmin.x = tmin.y;
	//}
	//
	//if (tmax.y < tmax.x)
	//{
		//tmax.x = tmax.y;
	//}
	//
	//tmin.z = (bounds[int(ray.invdir.z < 0.0)].z - origin.z) * ray.invdir.z;
	//tmax.z = (bounds[1 - int(ray.invdir.z < 0.0)].z - origin.z) * ray.invdir.z;
	//
	//if (tmin.z > tmin.x)
	//{
		//tmin.x = tmin.z;
	//}
	//
	//if (tmax.z < tmax.x)
	//{
		//tmax.x = tmax.z;
	//}
	//
	//hit_info.hit = true;
	//hit_info.front_face = tmin.x > 0.0;
	//hit_info.dist = float(hit_info.front_face) * tmin.x + (1.0 - float(hit_info.front_face)) * tmax.x;
	//hit_info.point = ray.origin + ray.dir * hit_info.dist;
	//
	//return hit_info;
//}