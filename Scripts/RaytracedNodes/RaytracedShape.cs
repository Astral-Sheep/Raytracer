using System;
using System.Collections.Immutable;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public interface IRaytracedShape : IRaytracedObject, IBounded
{
	/// <summary>
	/// The size of the shape's data in texels
	/// </summary>
	const int SHAPE_DATA_SIZE = 2;
	const float INV_SHAPE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SHAPE_DATA_SIZE);

	static readonly Type[] primitiveTypes = new Type[] {
		typeof(RaytracedSphere),
		typeof(RaytracedBox),
		typeof(RaytracedCylinder),
		typeof(RaytracedTorus),
	};

	ERaytracedShapeType Type { get; }
	Material[] Materials { get; }

	IPureShape AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial = null);
}

public interface IRaytracedShape<T> : IRaytracedShape where T : IShaderData
{
	new IPureShape<T> AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial = null);
}

public enum ERaytracedShapeType : byte
{
	None = 0,
	BoundingVolume = 1,
	Mesh = 2,
	LeafMesh = 3,
	MeshVolume = 4,
	Submesh = 5,
	Sphere = 6,
	/// <summary>
	/// Not handled currently
	/// </summary>
	Box = 7,
	Cylinder = 8,
	/// <summary>
	/// Not handled currently
	/// </summary>
	Torus = 9,
}

public static class RaytracedShapeExtensions
{
	public static void AddShapeToRaytracer<T>(this T pShape, ref Raytracer pRaytracer) where T : Node3D, IRaytracedShape
	{
		pRaytracer ??= pShape.FindNode<Raytracer>();

		if (pRaytracer == null)
			return;

		pRaytracer.AddShape(pShape);
	}

	public static void RemoveShapeFromRaytracer<T>(this T pShape, Raytracer pRaytracer) where T : Node3D, IRaytracedShape
	{
		if (pRaytracer == null)
			return;

		pRaytracer.RemoveShape(pShape);
	}
}