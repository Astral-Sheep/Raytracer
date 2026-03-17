using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSphere : CsgSphere3D, IRaytracedShape
{
	public const int SPHERE_DATA_SIZE = 2;
	public const float INV_SPHERE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SPHERE_DATA_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Sphere;

	public ShapeBounds Bounds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			vec3 lOrigin = fromVariant(GlobalPosition);
			vec3 lExtent = new vec3(Radius) * fromVariant(GlobalTransform.Basis.Scale);
			return new ShapeBounds {
				min = lOrigin - lExtent,
				max = lOrigin + lExtent,
			};
		}
	}

	public Material[] Materials
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Material[] { materialOverride ?? Material };
		}
	}

	[Export] protected RaytracedMaterial materialOverride;
	[Export] protected Raytracer raytracer;

	[ExportToolButton("Add to Raytracer")]
	protected Callable AddButton => Callable.From(AddToRaytracer);

	[ExportToolButton("Remove from Raytracer")]
	protected Callable RemoveButton => Callable.From(RemoveFromRaytracer);

	public override void _Ready()
	{
		base._Ready();
		AddToRaytracer();
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		if (!IsNodeReady())
			return;

		AddToRaytracer();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		RemoveFromRaytracer();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShapeData(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.Sphere,
			dataTexelIndex = pTexelIndex,
			boundMin = Bounds.min,
			boundMax = Bounds.max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SphereData GetShaderData(Dictionary<Material, int> pMaterialMap)
	{
		return new SphereData {
			center = new vec3(GlobalPosition.X, GlobalPosition.Y, GlobalPosition.Z),
			radius = Radius,
			scale = fromVariant(GlobalBasis.Scale),
			materialIndex = pMaterialMap.GetValueNoError(Material, 0),
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void AddToRaytracer()
	{
		this.AddShapeToRaytracer(ref raytracer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveShapeFromRaytracer(raytracer);
	}
}