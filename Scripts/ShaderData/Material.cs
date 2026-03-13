using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public struct Material : IShaderData
{
	public static Material Default => new Material {
		type = (int)EMaterialType.Opaque,
		textureIndex = short.MinValue,
		color = new Vec3<byte>(255),
		emissive = new Vec3<byte>(0),
		emissiveIntensity = (Half)0f,
		smoothness = 0,
		specularColor = new Vec3<byte>(0),
		specularProbability = 0,
	};

	public byte type;
	public short textureIndex;
	public Vec3<byte> color;
	public Vec3<byte> emissive;
	public Half emissiveIntensity;
	public byte smoothness;
	public Vec3<byte> specularColor;
	public byte specularProbability;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(type); // 1: 1 byte
				lWriter.Write(textureIndex); // 3: 2 bytes

				lWriter.Write(color.x); // 4: 1 byte
				lWriter.Write(color.y); // 5: 1 byte
				lWriter.Write(color.z); // 6: 1 byte

				lWriter.Write(emissive.x); // 7: 1 byte
				lWriter.Write(emissive.y); // 8: 1 byte
				lWriter.Write(emissive.z); // 9: 1 byte
				lWriter.Write(emissiveIntensity); // 11: 2 bytes

				lWriter.Write(smoothness); // 12: 1 byte
				lWriter.Write(specularColor.x); // 13: 1 byte
				lWriter.Write(specularColor.y); // 14: 1 byte
				lWriter.Write(specularColor.z); // 15: 1 byte
				lWriter.Write(specularProbability); // 16: 1 byte

				// Uncomment this if the struct is not properly aligned
				// int lSize = GetMarshalSize();
				//
				// if (lSize % Raytracer.TEXEL_SIZE != 0)
				// {
				// 	lWriter.Write(new byte[Raytracer.TEXEL_SIZE - lSize % Raytracer.TEXEL_SIZE]);
				// }
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
		uint lAlbedo = pMaterial.AlbedoColor.ToRgba32();
		uint lEmissive = pMaterial.Emission.ToRgba32();
		uint lSpecularColor = pMaterial.AlbedoColor.Lerp(Colors.White, 1f - pMaterial.Metallic).ToRgba32();

		return new Material {
			// TODO: handle other types like transparent materials
			type = (int)EMaterialType.Opaque,
			textureIndex = (short)(pTextureMap.GetValueOrDefault(pMaterial.AlbedoTexture, -1) + (short.MinValue + 1)),
			color = fromUVariant(lAlbedo).rgb,
			emissive = pMaterial.EmissionEnabled ? fromUVariant(lEmissive).rgb : new Vec3<byte>(0),
			emissiveIntensity = (Half)pMaterial.EmissionIntensity,
			smoothness = (byte)Mathf.RoundToInt(Mathf.Clamp(1f - pMaterial.Roughness, 0f, 1f) * 255f),
			specularColor = fromUVariant(lSpecularColor).rgb,
			specularProbability = (byte)Mathf.RoundToInt(Mathf.Clamp(pMaterial.MetallicSpecular, 0, 1) * 255f),
		};
	}
}