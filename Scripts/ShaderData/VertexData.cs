using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot;

namespace Astral.Raytracer;

public struct VertexData : IShaderData
{
	public vec3 position;
	public vec3 normal;
	public vec2 uv;

	public byte[] GetBytes()
	{
		// Use byte array directly if this causes a performance bottleneck
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
		return Marshal.SizeOf<VertexData>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTexelSize()
	{
		// Ceiled to take padding into account
		return Mathf.CeilToInt(GetMarshalSize() / (float)Raytracer.TEXEL_SIZE);
	}
}