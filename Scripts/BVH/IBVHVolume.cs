namespace Astral.Raytracer;

public interface IBVHVolume
{
	vec3 Min { get; }
	vec3 Max { get; }

	int Split(int pMaxDepth, int pVertexIndexOffset);
}