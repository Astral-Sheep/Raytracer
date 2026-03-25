using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class RaytracedTorus : CsgTorus3D, IRaytracedShape<TorusData>
{
	public const int TORUS_DATA_SIZE = 5;
	public const float INV_CYLINDER_BYTE_SIZE = 1f / (TORUS_DATA_SIZE * Raytracer.TEXEL_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Torus;

	public Material[] Materials
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	public Bounds GetBounds()
	{
		mat4 lLocalToWorld = GlobalTransform;
		float lHalfHeight = (OuterRadius - InnerRadius) * .5f;
		vec3 lMin = new vec3(Mathf.Inf);
		vec3 lMax = new vec3(-Mathf.Inf);

		for (int i = 0; i < 8; i++)
		{
			vec3 lCorner = (lLocalToWorld * new vec4(
				OuterRadius * (i % 2 == 0 ? 1 : -1),
				lHalfHeight * (i % 4 < 2 ? 1 : -1),
				OuterRadius * (i < 4 ? 1 : -1),
				1f
			)).xyz;
			lMin = min(lCorner, lMin);
			lMax = max(lCorner, lMax);
		}

		return new Bounds(lMin, lMax);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	IPureShape IRaytracedShape.AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		return AsPureShape(pMaterialTable, pDefaultMaterial);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public IPureShape<TorusData> AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial = null)
	{
		return new PureTorus {
			bounds = GetBounds(),
			transform = GlobalTransform,
			minorRadius = (OuterRadius - InnerRadius) * .5f,
			majorRadius = (OuterRadius + InnerRadius) * .5f,
			material = pMaterialTable.GetValueNoError(Materials[0] ?? pDefaultMaterial, -1),
		};
	}
}