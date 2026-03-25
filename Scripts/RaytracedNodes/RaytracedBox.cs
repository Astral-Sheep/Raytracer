using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedBox : CsgBox3D, IRaytracedShape
{
	public static uint ByteSize => 80;

	public ERaytracedShapeType Type => ERaytracedShapeType.Box;
	// public Bounds Bounds { get; private set; }

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

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// public ShapeData GetShapeData(int pTexelIndex)
	// {
	// 	return new ShapeData {
	// 		type = (int)ERaytracedShapeType.Box,
	// 		dataTexelIndex = pTexelIndex,
	// 		boundMin = Bounds.min,
	// 		boundMax = Bounds.max,
	// 	};
	// }

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
		// TO FIX: handle rotation
		return Bounds.FromExtent(fromVariant(Size * GlobalBasis.Scale) * .5f, fromVariant(GlobalPosition));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	IPureShape IRaytracedShape.AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		return new PureSphere {
			bounds = GetBounds(),
			radius = 1f,
			scale = new vec3(1f),
			material = pMaterialTable.GetValueNoError(Materials[0] ?? pDefaultMaterial, -1),
		};
	}
}