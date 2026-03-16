using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Astral.Raytracer;

public struct SphereData : IShaderData
{
	public vec3 center;
	public float radius;
	public vec3 scale;
	public int materialIndex;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(center.x);
				lWriter.Write(center.y);
				lWriter.Write(center.z);
				lWriter.Write(radius);

				lWriter.Write(scale.x);
				lWriter.Write(scale.y);
				lWriter.Write(scale.z);
				lWriter.Write(materialIndex);

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
		return Marshal.SizeOf<SphereData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTexelSize()
	{
		// Ceiled to take padding into account
		return Mathf.CeilToInt(GetMarshalSize() / (float)Raytracer.TEXEL_SIZE);
	}
}