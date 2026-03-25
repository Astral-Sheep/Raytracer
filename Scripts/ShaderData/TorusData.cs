using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public struct TorusData : IShaderData
{
	public float minorRadius;
	public float majorRadius;
	public mat4 transform;
	public int materialIndex;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// 64 bytes
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						lWriter.Write(transform[i, j]);
					}
				}

				lWriter.Write(minorRadius); // 68
				lWriter.Write(majorRadius); // 72
				lWriter.Write(materialIndex); // 76

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
		return Marshal.SizeOf<TorusData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTexelSize()
	{
		// Ceiled to take padding into account
		return Mathf.CeilToInt(GetMarshalSize() / (float)Raytracer.TEXEL_SIZE);
	}
}