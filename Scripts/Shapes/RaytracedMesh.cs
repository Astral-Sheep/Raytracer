using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

using GArray = Godot.Collections.Array;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMesh : MeshInstance3D, IRaytracedShape
{
	public const int MESH_DATA_SIZE = 5;
	public const int VERTEX_DATA_SIZE = 2;
	public const float TRIANGLE_DATA_SIZE = .75f;

	public const float INV_MESH_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * MESH_DATA_SIZE);
	public const float INV_VERTEX_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * VERTEX_DATA_SIZE);
	public const float INV_TRIANGLE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * TRIANGLE_DATA_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Mesh;

	public ShapeBounds Bounds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Aabb lBounds = GetAabb();
			return new ShapeBounds {
				min = fromVariant(ToGlobal(lBounds.Position)),
				max = fromVariant(ToGlobal(lBounds.End)),
			};
		}
	}

	public byte[] BVHData { get; protected set; }

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

	public Mesh[] GetShaderData(Dictionary<Godot.Material, int> pMaterialMap, int pTriangleOffset = 0)
	{
		if (Mesh == null)
		{
			return Array.Empty<Mesh>();
		}

		Mesh[] lSurfaces = new Mesh[Mesh.GetSurfaceCount()];

		for (int i = 0; i < Mesh.GetSurfaceCount(); i++)
		{
			GArray lSurface = Mesh.SurfaceGetArrays(i);
			GArray lTriangles = lSurface[(int)Godot.Mesh.ArrayType.Index].As<GArray>();
			int lTriCount = lTriangles.Count / 3;

			lSurfaces[i] = new Mesh {
				triStart = pTriangleOffset,
				triCount = lTriCount,
				transform = GlobalTransform,
				materialIndex = pMaterialMap.GetValueOrDefault(GetSurfaceOverrideMaterial(i), 0),
			};

			pTriangleOffset += lTriCount;
		}

		return lSurfaces;
	}

	public byte[] GetMeshBytes(int pTriangleStartIndex = 0)
	{
		using (MemoryStream lStream = new MemoryStream())
		{
			using (BinaryWriter lWriter = new BinaryWriter(lStream))
			{
				// Mesh shape
				lWriter.Write(pTriangleStartIndex); // 0
				lWriter.Write(Mesh.GetFaces().Length / 3); // 1

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
			}

			return lStream.ToArray();
		}
	}

	public (byte[], byte[]) GetPrimitiveBytes(int pVertexIndexOffset)
	{
		if (Mesh == null)
		{
			return (Array.Empty<byte>(), Array.Empty<byte>());
		}

		using (MemoryStream lVertexStream = new MemoryStream(), lTriangleStream = new MemoryStream())
		{
			using (BinaryWriter lVertexWriter = new BinaryWriter(lVertexStream), lTriangleWriter = new BinaryWriter(lTriangleStream))
			{
				int lVertexOffset = pVertexIndexOffset;

				for (int i = 0; i < Mesh.GetSurfaceCount(); i++)
				{
					GArray lSurface = Mesh.SurfaceGetArrays(i);

					GArray lVertexArray = lSurface[(int)Godot.Mesh.ArrayType.Vertex].As<GArray>();
					GArray lNormalArray = lSurface[(int)Godot.Mesh.ArrayType.Normal].As<GArray>();
					GArray lUVArray = lSurface[(int)Godot.Mesh.ArrayType.TexUV].As<GArray>();

					for (int j = 0; j < lVertexArray.Count; j++)
					{

						Vector3 lVertex = lVertexArray[j].As<Vector3>();
						lVertexWriter.Write(lVertex.X);
						lVertexWriter.Write(lVertex.Y);
						lVertexWriter.Write(lVertex.Z);

						Vector3 lNormal = lNormalArray[j].As<Vector3>();
						lVertexWriter.Write(lNormal.X);
						lVertexWriter.Write(lNormal.Y);
						lVertexWriter.Write(lNormal.Z);

						if (lUVArray is { Count: > 0 })
						{
							Vector2 lUV = lUVArray[j].As<Vector2>();
							lVertexWriter.Write(lUV.X);
							lVertexWriter.Write(lUV.Y);
						}
						else
						{
							lVertexWriter.Write(0);
							lVertexWriter.Write(0);
						}
					}

					GArray lTriangleArray = lSurface[(int)Godot.Mesh.ArrayType.Index].As<GArray>();

					for (int j = 0; j < lTriangleArray.Count; j += 3)
					{
						lTriangleWriter.Write(lTriangleArray[j].As<int>() + lVertexOffset);
						lTriangleWriter.Write(lTriangleArray[j + 2].As<int>() + lVertexOffset);
						lTriangleWriter.Write(lTriangleArray[j + 1].As<int>() + lVertexOffset);
					}

					lVertexOffset += lVertexArray.Count;
				}
			}

			return (lVertexStream.ToArray(), lTriangleStream.ToArray());
		}
	}

	public void BuildBVH(int pMaxDepth = 10)
	{
		if (Mesh == null || Mesh.GetSurfaceCount() <= 0)
			return;

		GArray lSurface = Mesh.SurfaceGetArrays(0);
		GArray lVertexArray = lSurface[(int)Godot.Mesh.ArrayType.Vertex].As<GArray>();
		GArray lTriangleArray = lSurface[(int)Godot.Mesh.ArrayType.Index].As<GArray>();

		vec3[] lVertices = lVertexArray.Select(v => fromVariant(v.As<Vector3>())).ToArray();
		int[] lTriangles = lTriangleArray.Select(v => v.As<int>()).ToArray();

		for (int i = 0; i < lTriangleArray.Count; i += 3)
		{
			
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