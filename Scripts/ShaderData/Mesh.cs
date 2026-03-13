using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Astral.Tools;

namespace Astral.Raytracer;

public struct Mesh : IShaderData
{
	public int triStart;
	public int triCount;
	public mat4 transform;
	public int materialIndex;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(triStart);
				lWriter.Write(triCount);

				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						lWriter.Write(transform[i, j]);
					}
				}

				lWriter.Write(materialIndex);

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
		return Marshal.SizeOf<Mesh>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}
}