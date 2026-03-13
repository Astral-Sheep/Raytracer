using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Astral.Raytracer;

public struct Material : IShaderData
{
	public static Material Default => new Material {
		type = (int)EMaterialType.Opaque,
		color = new vec4(1f),
		emissive = new vec3(0f),
		emissiveIntensity = 0f,
		smoothness = 0f,
		specularColor = new vec3(0f),
		specularProbability = 0f,
		textureIndex = -1,
	};

	public int type;
	public vec4 color;
	public vec3 emissive;
	public float emissiveIntensity;
	public float smoothness;
	public vec3 specularColor;
	public float specularProbability;
	public int textureIndex;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(type);

				lWriter.Write(color.x);
				lWriter.Write(color.y);
				lWriter.Write(color.z);
				lWriter.Write(color.w);

				lWriter.Write(emissive.x);
				lWriter.Write(emissive.y);
				lWriter.Write(emissive.z);
				lWriter.Write(emissiveIntensity);

				lWriter.Write(smoothness);
				lWriter.Write(specularColor.x);
				lWriter.Write(specularColor.y);
				lWriter.Write(specularColor.z);
				lWriter.Write(specularProbability);

				lWriter.Write(textureIndex);

				int lSize = GetMarshalSize();

				if (lSize % Raytracer.TEXEL_SIZE != 0)
				{
					lWriter.Write(new byte[Raytracer.TEXEL_SIZE - lSize % Raytracer.TEXEL_SIZE]);
				}
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMarshalSize()
	{
		return Marshal.SizeOf<Material>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}

	public static (bool, Material) FromResource(Godot.Material pMaterial, Dictionary<Texture2D, int> pTextureMap)
	{
		switch (pMaterial)
		{
			case BaseMaterial3D lBaseMaterial:
				return (true, FromBaseMaterial3D(lBaseMaterial, pTextureMap));
			case RaytracedMaterial lRaytracedMaterial:
				return (true, lRaytracedMaterial.GetShaderData(pTextureMap));
			default:
				GD.PushWarning($"Shaders of type {pMaterial.GetType().Name} is not supported for raytracing");
				return (false, Default);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Material FromBaseMaterial3D(BaseMaterial3D pMaterial, Dictionary<Texture2D, int> pTextureMap)
	{
		return new Material {
			// TODO: handle other types like transparent materials
			type = (int)EMaterialType.Opaque,
			color = fromVariant(pMaterial.AlbedoColor),
			emissive = pMaterial.EmissionEnabled ? fromVariant(pMaterial.Emission).rgb : new vec3(0f),
			emissiveIntensity = pMaterial.EmissionIntensity,
			smoothness = 1f - pMaterial.Roughness,
			specularColor = mix(new vec3(1f), fromVariant(pMaterial.AlbedoColor).rgb, pMaterial.Metallic),
			specularProbability = pMaterial.MetallicSpecular,
			textureIndex = pTextureMap.GetValueOrDefault(pMaterial.AlbedoTexture, -1),
		};
	}
}