namespace Astral.Raytracer;

public interface IBVHVolume
{
	vec3 Min { get; }
	vec3 Max { get; }
	int ChildCount { get; }

	int Split(int pMaxDepth, int pVertexIndexOffset);
	float GetSplitScore(vec3 pAxis);
}