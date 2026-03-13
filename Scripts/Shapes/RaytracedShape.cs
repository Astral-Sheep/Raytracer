using System;
using System.IO;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public interface IRaytracedShape : IRaytracedObject
{
	/// <summary>
	/// The size of the shape's data in texels
	/// </summary>
	const int SHAPE_DATA_SIZE = 2;
	const float INV_SHAPE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SHAPE_DATA_SIZE);

	ERaytracedShapeType Type { get; }
	RaytracedMaterial Material { get; }
	ShapeBounds Bounds { get; }
}

public enum ERaytracedShapeType : byte
{
	BoundingVolume = 0,
	Sphere = 1,
	Mesh = 2,
	Box = 3,
}

public struct ShapeBounds
{
	public vec3 Center => (min + max) * .5f;

	public vec3 min;
	public vec3 max;
}

public static class RaytracedShapeExtensions
{
	public static void AddToRaytracer<T>(this T pShape, ref Raytracer pRaytracer, Action<T, Raytracer> pAdder) where T : Node3D, IRaytracedObject
	{
		pRaytracer ??= pShape.FindNode<Raytracer>();

		if (pRaytracer == null)
			return;

		pAdder?.Invoke(pShape, pRaytracer);
	}

	public static void RemoveFromRaytracer<T>(this T pShape, Raytracer pRaytracer, Action<T, Raytracer> pRemover) where T : Node3D, IRaytracedObject
	{
		if (pRaytracer == null)
			return;

		pRemover?.Invoke(pShape, pRaytracer);
	}

	public static byte[] GetShapeBytes(this IRaytracedShape pShape, int pDataIndex, int pMaterialIndex)
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// No padding needed: the shader handles the data as an array of ints and not an array of texels
				lWriter.Write((int)pShape.Type + 1);
				lWriter.Write(pDataIndex);
				lWriter.Write(pMaterialIndex);
			}

			return lStream.ToArray();
		}
	}
}