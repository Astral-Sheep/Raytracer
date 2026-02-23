using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSphere : CsgSphere3D, IRaytracedShape
{
	public ERaytracedShapeType Type => ERaytracedShapeType.Sphere;
	public static uint ByteSize => 48;

	[Export] public new RaytracedMaterial Material { get; protected set; }
	[Export] protected Raytracer raytracer;

	[ExportToolButton("Add to Raytracer")]
	protected Callable AddButton => Callable.From(AddToRaytracer);

	[ExportToolButton("Remove from Raytracer")]
	protected Callable RemoveButton => Callable.From(AddToRaytracer);

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
				lWriter.Write(GlobalPosition.X);
				lWriter.Write(GlobalPosition.Y);
				lWriter.Write(GlobalPosition.Z);
				lWriter.Write(Radius);

				// Material
				RaytracedMaterial lMaterial = Material ?? raytracer.DefaultObjectMaterial;

				lWriter.Write(lMaterial.color.R);
				lWriter.Write(lMaterial.color.G);
				lWriter.Write(lMaterial.color.B);
				lWriter.Write(lMaterial.color.A);

				lWriter.Write(lMaterial.emissive.R);
				lWriter.Write(lMaterial.emissive.G);
				lWriter.Write(lMaterial.emissive.B);
				lWriter.Write(lMaterial.emissiveIntensity);
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void AddToRaytracer()
	{
		this.AddToRaytracer(
			ref raytracer,
			(s, r) => r.AddSphere(s)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual void RemoveFromRaytracer()
	{
		this.RemoveFromRaytracer(
			raytracer,
			(s, r) => r.RemoveSphere(s)
		);
	}
}