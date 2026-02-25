using System;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public interface IRaytracedShape : IRaytracedObject
{
	RaytracedMaterial Material { get; }
}

public enum ERaytracedShapeType
{
	Sphere = 0,
	Box = 1,
	Cylinder = 2,
	Disk = 3,
	Cone = 4,
	Mesh = 5,
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
}