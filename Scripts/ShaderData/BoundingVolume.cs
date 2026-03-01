namespace Astral.Raytracer;

public struct BoundingVolume
{
	public vec3 boundMin;
	public vec3 boundMax;
	public int child0;
	public int child1;
}