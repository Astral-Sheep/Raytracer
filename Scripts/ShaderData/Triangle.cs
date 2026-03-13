using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Astral.Raytracer;

public struct Triangle : IShaderData
{
	public int v0Index;
	public int v1Index;
	public int v2Index;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(v0Index);
				lWriter.Write(v1Index);
				lWriter.Write(v2Index);
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetMarshalSize()
	{
		return Marshal.SizeOf<Triangle>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}
}