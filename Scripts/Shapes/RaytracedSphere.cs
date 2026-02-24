using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSphere : CsgSphere3D, IRaytracedShape
{
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

				// Material
				RaytracedMaterial lMaterial = Material ?? raytracer.DefaultObjectMaterial;

				lWriter.Write(lMaterial.color.R); // 4
				lWriter.Write(lMaterial.color.G); // 5
				lWriter.Write(lMaterial.color.B); // 6
				lWriter.Write(lMaterial.color.A); // 7

				lWriter.Write(lMaterial.emissive.R); // 8
				lWriter.Write(lMaterial.emissive.G); // 9
				lWriter.Write(lMaterial.emissive.B); // 10
				lWriter.Write(lMaterial.emissiveIntensity); // 11

				lWriter.Write(lMaterial.smoothness); // 12
				lWriter.Write(lMaterial.specularColor.R); // 13
				lWriter.Write(lMaterial.specularColor.G); // 14
				lWriter.Write(lMaterial.specularColor.B); // 15
				lWriter.Write(lMaterial.specularProbability); // 16

				// Padding
				lWriter.Write(0f); // 17
				lWriter.Write(0f); // 18
				lWriter.Write(0f); // 19
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