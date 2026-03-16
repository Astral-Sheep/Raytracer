using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Astral.Raytracer;

public struct BoundingVolumeData : IShaderData
{
	public int startIndex;
	public int count;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(startIndex);
				lWriter.Write(count);

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
		return Marshal.SizeOf<BoundingVolumeData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTexelSize()
	{
		// Ceiled to take padding into account
		return Mathf.CeilToInt(GetMarshalSize() / (float)Raytracer.TEXEL_SIZE);
	}
}