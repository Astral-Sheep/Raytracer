namespace Astral.Raytracer;

public interface IBVHVolume : IBounded
{
	int ChildCount { get; }

	void Split(int pMaxDepth);
	float GetSplitCost(vec3 pAxis);
	string ToString(int pDepth);
}