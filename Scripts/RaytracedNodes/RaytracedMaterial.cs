using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMaterial : Material
{
	public const int DATA_SIZE = 4;
	public const float INV_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * DATA_SIZE);

	[Export] public EMaterialType type = EMaterialType.Opaque;
	[Export] public bool flatShading = false;

	[ExportGroup("Albedo")]
	[Export(PropertyHint.ColorNoAlpha)] public Color color = Colors.Gray;
	[Export] public Texture2D texture;

	[ExportGroup("Emissive")]
	[Export(PropertyHint.ColorNoAlpha)] public Color emissive;
	[Export] public float emissiveIntensity;

	[ExportGroup("Reflection")]
	[Export(PropertyHint.Range, "0,1,0.01")] public float smoothness;
	[Export(PropertyHint.ColorNoAlpha)] public Color specularColor = Colors.White;
	[Export(PropertyHint.Range, "0,1,0.01")] public float specularProbability;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MaterialData GetShaderData(Dictionary<Texture2D, int> pTextureMap)
	{
		return MaterialData.FromResource(this, pTextureMap).material;
	}

	public override Shader.Mode _GetShaderMode()
	{
		return Shader.Mode.Spatial;
	}
}

public enum EMaterialType : byte
{
	Opaque = 0,
}