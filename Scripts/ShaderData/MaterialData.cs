using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public struct MaterialData : IShaderData
{
	public static MaterialData Default => new MaterialData {
		type = (int)EMaterialType.Opaque,
		textureIndex = -1,
		color = new vec3(1f),
		emissive = new vec3(0f),
		emissiveIntensity = 0f,
		smoothness = 0f,
		specularColor = new vec3(0f),
		specularProbability = 0f,
		flatShading = false,
	};

	public int type;
	public int textureIndex;
	public vec3 color;
	public vec3 emissive;
	public float emissiveIntensity;
	public float smoothness;
	public vec3 specularColor;
	public float specularProbability;
	public bool flatShading;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(type); // 4: 4 bytes
				lWriter.Write(textureIndex); // 8: 8 bytes

				lWriter.Write(color.r); // 12: 4 bytes
				lWriter.Write(color.g); // 16: 4 bytes
				lWriter.Write(color.b); // 20: 4 bytes

				lWriter.Write(emissive.r); // 24: 4 bytes
				lWriter.Write(emissive.g); // 28: 4 bytes
				lWriter.Write(emissive.b); // 32: 4 bytes
				lWriter.Write(emissiveIntensity); // 36: 4 bytes

				lWriter.Write(smoothness); // 40: 4 bytes

				lWriter.Write(specularColor.r); // 44: 4 bytes
				lWriter.Write(specularColor.g); // 48: 4 bytes
				lWriter.Write(specularColor.b); // 52: 4 bytes
				lWriter.Write(specularProbability); // 56: 4 bytes

				lWriter.Write(Convert.ToInt32(flatShading)); // 60: 4 bytes

				int lSize = GetMarshalSize();

				if (lSize % Raytracer.TEXEL_SIZE != 0)
				{
					// byte[] lPadding = new byte[Raytracer.TEXEL_SIZE - lSize % Raytracer.TEXEL_SIZE];
					// lWriter.Write(lPadding);
					lWriter.Write(new byte[Raytracer.TEXEL_SIZE - lSize % Raytracer.TEXEL_SIZE]);
				}
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMarshalSize()
	{
		return Marshal.SizeOf<MaterialData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / (float)Raytracer.TEXEL_SIZE;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanHandleResource(Material pMaterial)
	{
		return pMaterial is RaytracedMaterial or BaseMaterial3D;
	}

	public static (bool handled, MaterialData material) FromResource(Material pMaterial, Dictionary<Texture2D, int> pTextureMap)
	{
		switch (pMaterial)
		{
			case BaseMaterial3D lBaseMaterial:
				return (true, FromBaseMaterial3D(lBaseMaterial, pTextureMap));
			case RaytracedMaterial lRaytracedMaterial:
				return (true, FromRaytracedMaterial(lRaytracedMaterial, pTextureMap));
			default:
				GD.PushWarning($"Shaders of type {pMaterial.GetType().Name} are not supported for raytracing.\nSupported types are: {nameof(RaytracedMaterial)} and {nameof(BaseMaterial3D)}");
				return (false, Default);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static MaterialData FromBaseMaterial3D(BaseMaterial3D pMaterial, Dictionary<Texture2D, int> pTextureMap)
	{
		return new MaterialData {
			// TODO: handle other types like transparent materials
			type = (int)EMaterialType.Opaque,
			textureIndex = pTextureMap.GetValueNoError(pMaterial.AlbedoTexture, -1),
			color = fromVariant(pMaterial.AlbedoColor).rgb,
			emissive = pMaterial.EmissionEnabled ? fromVariant(pMaterial.Emission).rgb : new vec3(0f),
			emissiveIntensity = pMaterial.EmissionIntensity,
			smoothness = Mathf.Clamp(1f - pMaterial.Roughness, 0f, 1f),
			specularColor = fromVariant(pMaterial.AlbedoColor.Lerp(Colors.White, 1f - pMaterial.Metallic)).rgb,
			specularProbability = Mathf.Clamp(pMaterial.MetallicSpecular, 0, 1),
			flatShading = false,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static MaterialData FromRaytracedMaterial(RaytracedMaterial pMaterial, Dictionary<Texture2D, int> pTextureMap)
	{
		return new MaterialData {
			// TODO: handle other types like transparent materials
			type = (int)pMaterial.type,
			textureIndex = pTextureMap.GetValueNoError(pMaterial.texture, -1),
			color = fromVariant(pMaterial.color).rgb,
			emissive = fromVariant(pMaterial.emissive).rgb,
			emissiveIntensity = pMaterial.emissiveIntensity,
			smoothness = pMaterial.smoothness,
			specularColor = fromVariant(pMaterial.specularColor).rgb,
			specularProbability = pMaterial.specularProbability,
			flatShading = pMaterial.flatShading,
		};
	}
}