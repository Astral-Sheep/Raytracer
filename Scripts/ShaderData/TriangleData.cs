using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Astral.Raytracer;

public struct TriangleData : IShaderData
{
	public int v0;
	public int v1;
	public int v2;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// No padding: the triangle buffer handles subtexel overlaps correctly
				lWriter.Write(v0);
				lWriter.Write(v1);
				lWriter.Write(v2);
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMarshalSize()
	{
		return Marshal.SizeOf<TriangleData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / (float)Raytracer.TEXEL_SIZE;
	}
}