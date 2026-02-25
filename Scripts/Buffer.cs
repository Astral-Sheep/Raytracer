using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	public string Name { get; private set; }
	public List<byte> RawData { get; private set; }
	public ImageTexture Buffer { get; private set; }

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
			return;
		}

		// Add padding if needed
		RawData.AddRange(new byte[16 - (RawData.Count % 16)]);

		Image lImage = Buffer.Image;
		lImage.SetData(RawData.Count / Raytracer.TEXEL_SIZE, 1, false, Image.Format.Rgbaf, RawData.ToArray());
		Buffer.SetImage(lImage);
		pMaterial.SetShaderParameter(Name, Buffer);
	}
}

public struct TextureBuffer : IBuffer
{
	public string Name { get; private set; }
	public Godot.Collections.Array<Image> Textures { get; private set; }
	public Texture2DArray Buffer { get; private set; }

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