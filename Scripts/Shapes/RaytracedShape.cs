using System;
using Godot;

namespace Astral.Raytracer;

public interface IRaytracedShape
{
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
	public static void AddToRaytracer<T>(this T pShape, ref Raytracer pRaytracer, Action<T, Raytracer> pAdder) where T : Node3D, IRaytracedShape
	{
		pRaytracer ??= pShape.GetTree().Root.GetNodeOrNull<Raytracer>(Raytracer.SCENE_PATH);

		if (pRaytracer == null)
			return;

		pAdder?.Invoke(pShape, pRaytracer);
	}

	public static void RemoveFromRaytracer<T>(this T pShape, Raytracer pRaytracer, Action<T, Raytracer> pRemover) where T : IRaytracedShape
	{
		if (pRaytracer == null)
			return;

		pRemover?.Invoke(pShape, pRaytracer);
	}
}