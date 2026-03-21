using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Astral.Raytracer;

public interface IBuffer
{
	string Name { get; }

	bool HasData();
	void SendData(ShaderMaterial pMaterial);
}

public struct DataBuffer : IBuffer
{
	public const int MAX_SIZE = 16384;
	public const int BIG_TEXTURE_THRESHOLD = MAX_SIZE * MAX_SIZE / 64;

	public string Name { get; private init; }
	public List<byte> RawData { get; private init; }
	public ImageTexture Buffer { get; private init; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DataBuffer New(string pBufferName)
	{
		return new DataBuffer {
			Name = pBufferName,
			RawData = new List<byte>(),
			Buffer = ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasData()
	{
		return RawData.Count > 0;
	}

	public void SendData(ShaderMaterial pMaterial)
	{
		if (!HasData())
		{
			pMaterial.SetShaderParameter(Name, Variant.From<ImageTexture>(null));
			pMaterial.SetShaderParameter($"{Name}_size", Vector2I.Zero);
			return;
		}

		Vector2I lTextureSize = GetTextureSize();

		// Add padding if needed
		if (RawData.Count < lTextureSize.X * lTextureSize.Y * Raytracer.TEXEL_SIZE)
		{
			RawData.AddRange(new byte[lTextureSize.X * lTextureSize.Y * Raytracer.TEXEL_SIZE - RawData.Count]);
		}

		Image lImage = Buffer.Image;
		lImage.SetData(lTextureSize.X, lTextureSize.Y, false, lImage.GetFormat(), RawData.ToArray());
		Buffer.SetImage(lImage);

		pMaterial.SetShaderParameter(Name, Buffer);
		pMaterial.SetShaderParameter($"{Name}_size", lTextureSize);
	}

	private Vector2I GetTextureSize()
	{
		int lTexelCount = Mathf.CeilToInt(RawData.Count / (float)Raytracer.TEXEL_SIZE);
		int lMaxZ = lTexelCount >= BIG_TEXTURE_THRESHOLD ? Mathf.CeilToInt(lTexelCount * .6f) : lTexelCount;
		int z = Mathf.CeilToInt(Mathf.Sqrt(lTexelCount));

		while (lTexelCount % z != 0 && z < lMaxZ) { ++z; }

		return new Vector2I(z, Mathf.CeilToInt(lTexelCount / (float)z));
	}
}

public struct TextureBuffer : IBuffer
{
	public string Name { get; private init; }
	public Godot.Collections.Array<Image> Textures { get; private init; }
	public Texture2DArray Buffer { get; private init; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextureBuffer New(string pBufferName)
	{
		return new TextureBuffer {
			Name = pBufferName,
			Textures = new Godot.Collections.Array<Image>(),
			Buffer = new Texture2DArray(),
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasData()
	{
		return Textures.Count > 0;
	}

	public void SendData(ShaderMaterial pMaterial)
	{
		if (!HasData())
		{
			pMaterial.SetShaderParameter(Name, Variant.From<Texture2DArray>(null));
			return;
		}

		Buffer.CreateFromImages(Textures);
		pMaterial.SetShaderParameter(Name, Buffer);
	}
}