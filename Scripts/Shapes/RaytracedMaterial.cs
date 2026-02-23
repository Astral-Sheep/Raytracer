using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMaterial : Resource
{
	[Export] public EMaterialType type;
	[Export] public Color color;
	[Export] public Color emissive;
	[Export] public float emissiveIntensity;
}

public enum EMaterialType
{
	Diffuse = 0,
}