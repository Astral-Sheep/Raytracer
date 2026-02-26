using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSphere : CsgSphere3D, IRaytracedShape
{
	public const int SPHERE_DATA_SIZE = 1;
	public const float INV_SPHERE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * SPHERE_DATA_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Sphere;
	[Export] public new RaytracedMaterial Material { get; protected set; }
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

	public byte[] GetBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// Sphere shape
				lWriter.Write(GlobalPosition.X); // 0
				lWriter.Write(GlobalPosition.Y); // 1
				lWriter.Write(GlobalPosition.Z); // 2
				lWriter.Write(Radius); // 3
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void AddToRaytracer()
	{
		this.AddToRaytracer(
			ref raytracer,
			(s, r) => r.AddSphere(s)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveFromRaytracer(
			raytracer,
			(s, r) => r.RemoveSphere(s)
		);
	}
}