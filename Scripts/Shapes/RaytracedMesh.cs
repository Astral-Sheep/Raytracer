using System.IO;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMesh : MeshInstance3D, IRaytracedShape
{
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
				lWriter.Write((float)pTriangleStartIndex); // 0
				lWriter.Write((float)(Mesh.GetFaces().Length / 3)); // 1

				Aabb lBounds = GetAabb();
				Vector3 lGlobalMin = ToGlobal(lBounds.Position);
				Vector3 lGlobalMax = ToGlobal(lBounds.End);

				lWriter.Write(lGlobalMin.X); // 2
				lWriter.Write(lGlobalMin.Y); // 3
				lWriter.Write(lGlobalMin.Z); // 4

				lWriter.Write(lGlobalMax.X); // 5
				lWriter.Write(lGlobalMax.Y); // 6
				lWriter.Write(lGlobalMax.Z); // 7

				// 8 to 23
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						lWriter.Write(Transform[i][j]);
					}

					lWriter.Write(i == 3 ? 1f : 0f);
				}

				// Material
				RaytracedMaterial lMaterial = Material ?? raytracer.DefaultObjectMaterial;

				lWriter.Write(lMaterial.color.R); // 24
				lWriter.Write(lMaterial.color.G); // 25
				lWriter.Write(lMaterial.color.B); // 26
				lWriter.Write(lMaterial.color.A); // 27

				lWriter.Write(lMaterial.emissive.R); // 28
				lWriter.Write(lMaterial.emissive.G); // 29
				lWriter.Write(lMaterial.emissive.B); // 30
				lWriter.Write(lMaterial.emissiveIntensity); // 31

				lWriter.Write(lMaterial.smoothness); // 32
				lWriter.Write(lMaterial.specularColor.R); // 33
				lWriter.Write(lMaterial.specularColor.G); // 34
				lWriter.Write(lMaterial.specularColor.B); // 35
				lWriter.Write(lMaterial.specularProbability); // 36

				// Padding
				lWriter.Write(0f); // 37
				lWriter.Write(0f); // 38
				lWriter.Write(0f); // 39
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
				// switch (Mesh)
				// {
				// 	case PrimitiveMesh lPrimitive:
				// 		
				// 		break;
				// 	default:
				// 		break;
				// }
				//
				// Vector3[] lTriangles;

				Vector3[] lFaces = Mesh.GetFaces();

				for (int i = 0; i < lFaces.Length; i += 3)
				{
					Vector3 lPointA = lFaces[i];
					Vector3 lPointB = lFaces[i + 2];
					Vector3 lPointC = lFaces[i + 1];

					Vector3 lNormal = (lPointB - lPointA).Cross(lPointC - lPointA).Normalized();

					// Positions
					lWriter.Write(lPointA.X); // 0
					lWriter.Write(lPointA.Y); // 1
					lWriter.Write(lPointA.Z); // 2

					lWriter.Write(lPointB.X); // 3
					lWriter.Write(lPointB.Y); // 4
					lWriter.Write(lPointB.Z); // 5

					lWriter.Write(lPointC.X); // 6
					lWriter.Write(lPointC.Y); // 7
					lWriter.Write(lPointC.Z); // 8

					// Normals
					lWriter.Write(lNormal.X); // 9
					lWriter.Write(lNormal.Y); // 10
					lWriter.Write(lNormal.Z); // 11

					lWriter.Write(lNormal.X); // 12
					lWriter.Write(lNormal.Y); // 13
					lWriter.Write(lNormal.Z); // 14

					lWriter.Write(lNormal.X); // 15
					lWriter.Write(lNormal.Y); // 16
					lWriter.Write(lNormal.Z); // 17

					// Padding
					lWriter.Write(0f); // 18
					lWriter.Write(0f); // 19
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