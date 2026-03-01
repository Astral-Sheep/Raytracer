using System.IO;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMaterial : Resource
{
	public const int DATA_SIZE = 4;
	public const float INV_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * DATA_SIZE);

	[Export] public EMaterialType type = EMaterialType.Diffuse;
	[Export] public Color color = Colors.Gray;
	[Export] public Texture2D texture;
	[Export] public Color emissive;
	[Export] public float emissiveIntensity;
	[Export(PropertyHint.Range, "0,1,0.01")] public float smoothness;
	[Export] public Color specularColor = Colors.White;
	[Export(PropertyHint.Range, "0,1,0.01")] public float specularProbability;

	public byte[] GetBytes(int pTextureIndex = -1)
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write((int)type); // 0

				lWriter.Write(color.R); // 1
				lWriter.Write(color.G); // 2
				lWriter.Write(color.B); // 3
				lWriter.Write(color.A); // 4

				lWriter.Write(emissive.R); // 5
				lWriter.Write(emissive.G); // 6
				lWriter.Write(emissive.B); // 7
				lWriter.Write(emissiveIntensity); // 8

				lWriter.Write(smoothness); // 9
				lWriter.Write(specularColor.R); // 10
				lWriter.Write(specularColor.G); // 11
				lWriter.Write(specularColor.B); // 12
				lWriter.Write(specularProbability); // 13

				lWriter.Write(texture == null ? -1 : pTextureIndex); // 14

				// Padding
				lWriter.Write(0); // 15
			}

			return lStream.ToArray();
		}
	}
}

public enum EMaterialType
{
	Diffuse = 0,
}