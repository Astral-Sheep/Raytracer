using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Astral.Raytracer;

public struct Vertex : IShaderData
{
	public vec3 position;
	public vec3 normal;
	public vec2 uv;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(position.x);
				lWriter.Write(position.y);
				lWriter.Write(position.z);

				lWriter.Write(normal.x);
				lWriter.Write(normal.y);
				lWriter.Write(normal.z);

				lWriter.Write(uv.x);
				lWriter.Write(uv.y);

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
		return Marshal.SizeOf<Vertex>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}
}