using Astral.Tools;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedSun : DirectionalLight3D, IRaytracedObject
{
	[Export] public bool Trace { get; set; }
	[Export] protected float focus = 1000f;
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
				Vector3 lNormalizedSkyPosition = -Transform.Basis.Z;
				lWriter.Write(lNormalizedSkyPosition.X);
				lWriter.Write(lNormalizedSkyPosition.Y);
				lWriter.Write(lNormalizedSkyPosition.Z);
				lWriter.Write(focus);

				lWriter.Write(LightColor.R);
				lWriter.Write(LightColor.G);
				lWriter.Write(LightColor.B);
				lWriter.Write(LightEnergy * 10f);
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddToRaytracer()
	{
		raytracer ??= this.FindNode<Raytracer>();

		if (raytracer == null)
			return;

		raytracer.AddSun(this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveFromRaytracer()
	{
		if (raytracer == null)
			return;

		raytracer.RemoveSun(this);
	}
}