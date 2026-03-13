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
				// Box shape
				lWriter.Write(GlobalPosition.X);
				lWriter.Write(GlobalPosition.Y);
				lWriter.Write(GlobalPosition.Z);

				lWriter.Write(GlobalRotation.X);
				lWriter.Write(GlobalRotation.Y);
				lWriter.Write(GlobalRotation.Z);

				lWriter.Write(GlobalBasis.Scale.X);
				lWriter.Write(GlobalBasis.Scale.Y);
				lWriter.Write(GlobalBasis.Scale.Z);

				lWriter.Write(Size.X * .5f);
				lWriter.Write(Size.Y * .5f);
				lWriter.Write(Size.Z * .5f);

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