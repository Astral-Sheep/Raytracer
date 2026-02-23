using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedBox : CsgBox3D, IRaytracedShape
{
	public ERaytracedShapeType Type => ERaytracedShapeType.Box;

	[Export] public new RaytracedMaterial Material { get; protected set; }
	[Export] protected Raytracer raytracer;

	[ExportToolButton("Add to Raytracer")]
	protected Callable AddButton => Callable.From(AddToRaytracer);

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
	protected virtual void AddToRaytracer()
	{
		this.AddToRaytracer(
			ref raytracer,
			(b, r) => r.AddBox(b)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void RemoveFromRaytracer()
	{
		this.RemoveFromRaytracer(
			raytracer,
			(b, r) => r.RemoveBox(b)
		);
	}
}