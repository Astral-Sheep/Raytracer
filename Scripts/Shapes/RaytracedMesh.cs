using System;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;

using GArray = Godot.Collections.Array;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMesh : MeshInstance3D, IRaytracedShape
{
	public const int MESH_DATA_SIZE = 10;
	public const int TRIANGLE_DATA_SIZE = 6;

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

	public byte[] GetMeshBytes(int pTriangleStartIndex = 0, int pTextureIndex = 0)
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
						lWriter.Write(GlobalTransform[i, j]);
					}

					lWriter.Write(Convert.ToSingle(i == 3));
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
				lWriter.Write(lMaterial.texture != null ? (float)pTextureIndex : -1f); // 37

				// Padding
				lWriter.Write(0f); // 38
				lWriter.Write(0f); // 39
			}

			return lStream.ToArray();
		}
	}

	public byte[] GetTrianglesBytes()
	{
		if (Mesh == null)
		{
			return Array.Empty<byte>();
		}

		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// Vector3[] lFaces = Mesh.GetFaces();
				//
				// for (int i = 0; i < lFaces.Length; i += 3)
				// {
				// 	Vector3 lVertex0 = lFaces[i];
				// 	Vector3 lVertex1 = lFaces[i + 2];
				// 	Vector3 lVertex2 = lFaces[i + 1];
				//
				// 	Vector3 lNormal = (lVertex1 - lVertex0).Cross(lVertex2 - lVertex0);
				//
				// 		// Vertices
				// 		lWriter.Write(lVertex0.X); // 0
				// 		lWriter.Write(lVertex0.Y); // 1
				// 		lWriter.Write(lVertex0.Z); // 2
				//
				// 		lWriter.Write(lVertex1.X); // 3
				// 		lWriter.Write(lVertex1.Y); // 4
				// 		lWriter.Write(lVertex1.Z); // 5
				//
				// 		lWriter.Write(lVertex2.X); // 6
				// 		lWriter.Write(lVertex2.Y); // 7
				// 		lWriter.Write(lVertex2.Z); // 8
				//
				// 		// Normals
				// 		lWriter.Write(lNormal.X); // 9
				// 		lWriter.Write(lNormal.Y); // 10
				// 		lWriter.Write(lNormal.Z); // 11
				//
				// 		lWriter.Write(lNormal.X); // 12
				// 		lWriter.Write(lNormal.Y); // 13
				// 		lWriter.Write(lNormal.Z); // 14
				//
				// 		lWriter.Write(lNormal.X); // 15
				// 		lWriter.Write(lNormal.Y); // 16
				// 		lWriter.Write(lNormal.Z); // 17
				//
				// 		// UVs
				// 		lWriter.Write(0f); // 18
				// 		lWriter.Write(0f); // 19
				//
				// 		lWriter.Write(0f); // 20
				// 		lWriter.Write(0f); // 21
				//
				// 		lWriter.Write(0f); // 22
				// 		lWriter.Write(0f); // 23
				// }

				for (int i = 0; i < Mesh.GetSurfaceCount(); i++)
				{
					GArray lSurface = Mesh.SurfaceGetArrays(i);

					GArray lVertices = lSurface[(int)Mesh.ArrayType.Vertex].As<GArray>();
					GArray lNormals = lSurface[(int)Mesh.ArrayType.Normal].As<GArray>();
					GArray lUVs = lSurface[(int)Mesh.ArrayType.TexUV].As<GArray>();
					GArray lTriangles = lSurface[(int)Mesh.ArrayType.Index].As<GArray>();

					for (int j = 0; j < lTriangles.Count; j += 3)
					{
						int lIndex0 = lTriangles[j].As<int>();
						int lIndex1 = lTriangles[j + 2].As<int>();
						int lIndex2 = lTriangles[j + 1].As<int>();

						Vector3 lVertex0 = lVertices[lIndex0].As<Vector3>();
						Vector3 lVertex1 = lVertices[lIndex1].As<Vector3>();
						Vector3 lVertex2 = lVertices[lIndex2].As<Vector3>();

						Vector3 lNormal0 = lNormals[lIndex0].As<Vector3>();
						Vector3 lNormal1 = lNormals[lIndex1].As<Vector3>();
						Vector3 lNormal2 = lNormals[lIndex2].As<Vector3>();

						Vector2 lUV0 = lUVs[lIndex0].As<Vector2>();
						Vector2 lUV1 = lUVs[lIndex1].As<Vector2>();
						Vector2 lUV2 = lUVs[lIndex2].As<Vector2>();

						// Vertices
						lWriter.Write(lVertex0.X); // 0
						lWriter.Write(lVertex0.Y); // 1
						lWriter.Write(lVertex0.Z); // 2

						lWriter.Write(lVertex1.X); // 3
						lWriter.Write(lVertex1.Y); // 4
						lWriter.Write(lVertex1.Z); // 5

						lWriter.Write(lVertex2.X); // 6
						lWriter.Write(lVertex2.Y); // 7
						lWriter.Write(lVertex2.Z); // 8

						// Normals
						lWriter.Write(lNormal0.X); // 9
						lWriter.Write(lNormal0.Y); // 10
						lWriter.Write(lNormal0.Z); // 11

						lWriter.Write(lNormal1.X); // 12
						lWriter.Write(lNormal1.Y); // 13
						lWriter.Write(lNormal1.Z); // 14

						lWriter.Write(lNormal2.X); // 15
						lWriter.Write(lNormal2.Y); // 16
						lWriter.Write(lNormal2.Z); // 17

						// UVs
						lWriter.Write(lUV0.X); // 18
						lWriter.Write(lUV0.Y); // 19

						lWriter.Write(lUV1.X); // 20
						lWriter.Write(lUV1.Y); // 21

						lWriter.Write(lUV2.X); // 22
						lWriter.Write(lUV2.Y); // 23
					}
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