namespace Astral.Raytracer;

public interface IRaytracedObject
{
	bool Trace { get; }

	void AddToRaytracer();
	void RemoveFromRaytracer();
}