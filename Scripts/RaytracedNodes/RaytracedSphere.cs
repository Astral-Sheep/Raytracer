using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSphere : CsgSphere3D, IRaytracedShape<SphereData>
{
	public const int SPHERE_DATA_SIZE = 2;
	public const float INV_SPHERE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SPHERE_DATA_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Sphere;

	public Material[] Materials
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
		get
		{
			return new Material[] { materialOverride ?? Material };
		}
	}

	[Export] public bool Trace { get; set; }
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
	public virtual void AddToRaytracer()
	{
		this.AddShapeToRaytracer(ref raytracer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveShapeFromRaytracer(raytracer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public Bounds GetBounds()
	{
		return Bounds.FromExtent(
			new vec3(Radius) * fromVariant(GlobalTransform.Basis.Scale),
			fromVariant(GlobalPosition)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	IPureShape IRaytracedShape.AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		return AsPureShape(pMaterialTable);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public IPureShape<SphereData> AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial = null)
	{
		return new PureSphere {
			bounds = GetBounds(),
			radius = Radius,
			scale = fromVariant(GlobalTransform.Basis.Scale),
			material = pMaterialTable.GetValueNoError(Materials[0] ?? pDefaultMaterial, -1),
		};
	}
}