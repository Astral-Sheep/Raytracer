using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedBox : CsgBox3D, IRaytracedShape
{
	public static uint ByteSize => 80;

	public ERaytracedShapeType Type => ERaytracedShapeType.Box;
	public ShapeBounds Bounds { get; private set; }

	// [Export] public new RaytracedMaterial Material { get; protected set; }
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
			type = (int)ERaytracedShapeType.Box,
			dataTexelIndex = pTexelIndex,
			boundMin = Bounds.min,
			boundMax = Bounds.max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void AddToRaytracer()
	{
		this.AddToRaytracer(
			ref raytracer,
			(b, r) => r.AddBox(b)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveFromRaytracer(
			raytracer,
			(b, r) => r.RemoveBox(b)
		);
	}
}