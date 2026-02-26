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
	const float SHAPE_DATA_SIZE = .75f;
	const float INV_SHAPE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SHAPE_DATA_SIZE);

	ERaytracedShapeType Type { get; }
	RaytracedMaterial Material { get; }
}

public enum ERaytracedShapeType
{
	Sphere = 0,
	Box = 1,
	Mesh = 2,
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