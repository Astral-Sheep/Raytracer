using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Astral.Raytracer;

public struct Shape : IShaderData
{
	public int type;
	public int dataTexelIndex;
	public vec3 boundMin;
	public vec3 boundMax;

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				lWriter.Write(type);
				lWriter.Write(dataTexelIndex);

				lWriter.Write(boundMin.x);
				lWriter.Write(boundMin.y);
				lWriter.Write(boundMin.z);

				lWriter.Write(boundMax.x);
				lWriter.Write(boundMax.y);
				lWriter.Write(boundMax.z);

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
		return Marshal.SizeOf<Shape>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTexelSize()
	{
		return GetMarshalSize() / 16f;
	}
}