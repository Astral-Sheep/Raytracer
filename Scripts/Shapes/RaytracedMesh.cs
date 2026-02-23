using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMesh : MeshInstance3D, IRaytracedShape
{
	public static uint ByteSize => 112;

	[Export] public RaytracedMaterial Material { get; protected set; }
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

	public byte[] GetMeshBytes(int pTriangleStartIndex = 0)
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// Mesh shape
				lWriter.Write((float)pTriangleStartIndex);
				lWriter.Write((float)(Mesh.GetFaces().Length / 3));

				Aabb lBounds = GetAabb();
				Vector3 lGlobalMin = ToGlobal(lBounds.Position);
				Vector3 lGlobalMax = ToGlobal(lBounds.End);

				lWriter.Write(lGlobalMin.X);
				lWriter.Write(lGlobalMin.Y);
				lWriter.Write(lGlobalMin.Z);

				lWriter.Write(lGlobalMax.X);
				lWriter.Write(lGlobalMax.Y);
				lWriter.Write(lGlobalMax.Z);

				lWriter.Write(GlobalPosition.X);
				lWriter.Write(GlobalPosition.Y);
				lWriter.Write(GlobalPosition.Z);

				lWriter.Write(GlobalRotation.X);
				lWriter.Write(GlobalRotation.Y);
				lWriter.Write(GlobalRotation.Z);

				lWriter.Write(GlobalBasis.Scale.X);
				lWriter.Write(GlobalBasis.Scale.Y);
				lWriter.Write(GlobalBasis.Scale.Z);

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

				// Alignment
				lWriter.Write(0f);
				lWriter.Write(0f);
				lWriter.Write(0f);
			}

			return lStream.ToArray();
		}
	}

	public byte[] GetTrianglesBytes()
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				Vector3[] lFaces = Mesh.GetFaces();

				for (int i = 0; i < lFaces.Length; i += 3)
				{
					Vector3 lPointA = lFaces[i];
					Vector3 lPointB = lFaces[i + 2];
					Vector3 lPointC = lFaces[i + 1];

					Vector3 lNormal = (lPointB - lPointA).Cross(lPointC - lPointA).Normalized();

					// Positions
					lWriter.Write(lPointA.X);
					lWriter.Write(lPointA.Y);
					lWriter.Write(lPointA.Z);

					lWriter.Write(lPointB.X);
					lWriter.Write(lPointB.Y);
					lWriter.Write(lPointB.Z);

					lWriter.Write(lPointC.X);
					lWriter.Write(lPointC.Y);
					lWriter.Write(lPointC.Z);

					// Normals
					lWriter.Write(lNormal.X);
					lWriter.Write(lNormal.Y);
					lWriter.Write(lNormal.Z);

					lWriter.Write(lNormal.X);
					lWriter.Write(lNormal.Y);
					lWriter.Write(lNormal.Z);

					lWriter.Write(lNormal.X);
					lWriter.Write(lNormal.Y);
					lWriter.Write(lNormal.Z);

					// Alignment
					lWriter.Write(0f);
					lWriter.Write(0f);
				}
			}

			return lStream.ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void AddToRaytracer()
	{
		this.AddToRaytracer(
			ref raytracer,
			(m, r) => r.AddMesh(m)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveFromRaytracer(
			raytracer,
			(m, r) => r.RemoveMesh(m)
		);
	}
}