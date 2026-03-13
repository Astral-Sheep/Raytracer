using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Astral.Raytracer;

public struct Sphere : IShaderData
{
	public vec3 center;
	public float radius;
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
	public int GetMarshalSize()
	{
		return Marshal.SizeOf<Sphere>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}
}