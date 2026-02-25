using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMaterial : Resource
{
	[Export] public EMaterialType type = EMaterialType.Diffuse;
	[Export] public Color color = Colors.Gray;
	[Export] public Texture2D texture;
	[Export] public Color emissive;
	[Export] public float emissiveIntensity;
	[Export(PropertyHint.Range, "0,1,0.01")] public float smoothness;
	[Export] public Color specularColor = Colors.White;
	[Export(PropertyHint.Range, "0,1,0.01")] public float specularProbability;
}

public enum EMaterialType
{
	Diffuse = 0,
}