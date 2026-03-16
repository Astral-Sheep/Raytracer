using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Astral.Tools;
using Godot;

using GArray = Godot.Collections.Array;

namespace Astral.Raytracer;

public class BVHMesh : IBVHVolume
{
	public static int MaxDepth => GodotUtility.GetSetting<int>("rendering/raytracing/bvh_mesh_depth");

	public vec3 Min { get; }
	public vec3 Max { get; }
	public Mesh Mesh => basis?.Mesh;
	public VertexData[][] Vertices { get; }
	public TriangleData[][] Triangles { get; }
	public int VertexOffset { get; protected set; }

	public readonly List<BVHMeshVolume> children;

	protected RaytracedMesh basis;

	public BVHMesh(RaytracedMesh pMesh)
	{
		basis = pMesh;
		children = new List<BVHMeshVolume>();

		if (basis?.Mesh == null)
			return;

		Min = basis.Bounds.min;
		Max = basis.Bounds.max;

		int lSurfaceCount = basis.Mesh.GetSurfaceCount();
		Vertices = new VertexData[][lSurfaceCount];
		Triangles = new TriangleData[][lSurfaceCount];

		Parallel.For(0, lSurfaceCount, i => {
			GArray lSurface = basis.Mesh.SurfaceGetArrays(i);

			GArray lVertexArray = lSurface[(int)Mesh.ArrayType.Vertex].As<GArray>();
			GArray lNormalArray = lSurface[(int)Mesh.ArrayType.Normal].As<GArray>();
			GArray lUVArray = lSurface[(int)Mesh.ArrayType.TexUV].As<GArray>();

			Vertices[i] = lVertexArray.AsParallel().Select((v, j) => new VertexData {
				position = fromVariant(v.As<Vector3>()),
				normal = fromVariant(lNormalArray[j].As<Vector3>()),
				uv = fromVariant(lUVArray[j].As<Vector2>()),
			}).ToArray();
		});
	}

	public int GenerateTriangleBuffer(int pVertexIndexOffset)
	{
		if (basis?.Mesh == null)
		{
			return pVertexIndexOffset;
		}

		VertexOffset = pVertexIndexOffset;

		for (int i = 0; i < basis.Mesh.GetSurfaceCount(); i++)
		{
			GArray lSurface = basis.Mesh.SurfaceGetArrays(i);
			GArray lTriangles = lSurface[(int)Mesh.ArrayType.Index].As<GArray>();
			TriangleData[] lSubmeshTriangles = new TriangleData[lTriangles.Count / 3];

			Parallel.For(
				0,
				lSubmeshTriangles.Length,
				j => {
					lSubmeshTriangles[j] = new TriangleData {
						v0 = pVertexIndexOffset + j * 3,
						v1 = pVertexIndexOffset + j * 3 + 1,
						v2 = pVertexIndexOffset + j * 3 + 2,
					};
				}
			);

			Triangles[i] = lSubmeshTriangles;
			pVertexIndexOffset += Vertices[i].Length;
		}

		return pVertexIndexOffset;
	}

	public int Split(int pMaxDepth = -1, int pVertexIndexOffset = 0)
	{
		if (basis?.Mesh == null || children.Count > 0)
		{
			return pVertexIndexOffset;
		}

		pMaxDepth = pMaxDepth < 0 ? MaxDepth : pMaxDepth;

		if (pMaxDepth <= 0)
		{
			return pVertexIndexOffset;
		}

		// Split in submeshes
		if (basis.Mesh.GetSurfaceCount() > 1)
		{
			SplitInSubmeshes(pMaxDepth);
		}
		else
		{
			SplitInSubvolumes(pMaxDepth, 0);
		}

		return pVertexIndexOffset;
	}

	private void SplitInSubmeshes(int pMaxDepth)
	{
		int lTriangleCount = 0;

		List<Task> lSplits = new List<Task>();

		for (int i = 0; i < basis.Mesh.GetSurfaceCount(); i++)
		{
			if (Triangles[i] is not { Length: > 0 })
				continue;

			BVHSubmesh lSubmesh = new BVHSubmesh(Vertices[i], Triangles[i], lTriangleCount, lTriangleCount + Triangles[i].Length, VertexOffset);
			children.Add(lSubmesh);
			lTriangleCount += Triangles[i].Length;
			lSplits.Add(Task.Run(() => lSubmesh.Split(pMaxDepth - 1, VertexOffset)));
		}

		Task.WaitAll(lSplits.ToArray());

		if (children.Count == 1)
		{
			SplitInSubvolumes(pMaxDepth, Triangles.IndexOf((children[0] as BVHSubmesh).triangles));
		}
	}

	private void SplitInSubvolumes(int pMaxDepth, int pSurfaceIndex)
	{
		VertexData[] lVertices = Vertices[pSurfaceIndex];
		TriangleData[] lTriangles = Triangles[pSurfaceIndex];

		if (lTriangles is not { Length: > 0 })
			return;

		vec3 lSplitAxis = BVHBuilder.GetSplitAxis(this);

		BVHMeshVolume lChild0 = new BVHMeshVolume(Vertices[pSurfaceIndex], Triangles[pSurfaceIndex], 0, 0, VertexOffset);
		BVHMeshVolume lChild1 = new BVHMeshVolume(Vertices[pSurfaceIndex], Triangles[pSurfaceIndex], 0, 0, VertexOffset);

		for (int i = 0; i < Triangles.Length; i++)
		{
			TriangleData lTriangle = lTriangles[i];
			vec3 lCenter = (
				lVertices[lTriangle.v0 - VertexOffset].position +
				lVertices[lTriangle.v1 - VertexOffset].position +
				lVertices[lTriangle.v2 - VertexOffset].position
			) / 3f;

			if (all(lessThanEqual(lCenter, lSplitAxis)))
			{
				if (lChild1.count > 0)
				{
					// Swap triangles for contiguity
					TriangleData lSwappedTriangle = lTriangles[lChild1.startIndex];
					lTriangles[lChild1.startIndex] = lTriangle;
					lTriangles[i] = lSwappedTriangle;
				}

				++lChild1.startIndex;
				lChild0.AddTriangle();
			}
			else
			{
				lChild1.AddTriangle();
			}
		}

		Task.WaitAll(
			Task.Run(() => lChild0.Split(pMaxDepth - 1)),
			Task.Run(() => lChild1.Split(pMaxDepth - 1))
		);

		children.Add(lChild0);
		children.Add(lChild1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.Mesh,
			dataTexelIndex = pTexelIndex,
			boundMin = Min,
			boundMax = Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MeshData GetShaderData(Dictionary<Mesh, (int start, int count)> pMeshMap, Dictionary<Material, int> pMaterialMap, int pChildOffset = 0, int pTriangleOffset = 0)
	{
		if (basis?.Mesh == null)
		{
			return new MeshData {
				transform = new mat4(1f),
				startIndex = -1,
				count = 0,
				materialIndex = -1,
			};
		}

		bool lHasSubmeshes = basis.Mesh.GetSurfaceCount() > 1;

		if (!pMeshMap.TryGetValue(basis.Mesh, out (int start, int count) lMeshIndices))
		{
			bool lHasTriangles = basis.Mesh.GetSurfaceCount() == 1 && children.Count == 0;
			lHasSubmeshes = lHasSubmeshes && children.Count > 0;

			lMeshIndices = (
				lHasTriangles ? pTriangleOffset : pChildOffset,
				lHasSubmeshes
					? children.Count
					: lHasTriangles ? Triangles[0].Length : 0
			);
			pMeshMap.Add(basis.Mesh, lMeshIndices);
		}

		return new MeshData {
			transform = basis.Transform,
			startIndex = lMeshIndices.start,
			count = lMeshIndices.count,
			materialIndex = lHasSubmeshes ? -1 : pMaterialMap.GetValueOrDefault(basis.GetSurfaceOverrideMaterial(0), -1),
		};
	}

	public byte[] GetVertexBufferData()
	{
		int lVertexByteSize = VertexData.GetTexelSize() * Raytracer.TEXEL_SIZE;
		List<byte> lVertexBufferData = new List<byte>();

		for (int i = 0; i < Vertices.Length; i++)
		{
			VertexData[] lSubmeshVertices = Vertices[i];
			byte[] lRawBytes = new byte[lSubmeshVertices.Length * lVertexByteSize];

			Parallel.For(0, lSubmeshVertices.Length, j => {
				Array.Copy(lSubmeshVertices[j].GetBytes(), 0, lRawBytes, j * lVertexByteSize, lVertexByteSize);
			});

			lVertexBufferData.AddRange(lRawBytes);
		}

		return lVertexBufferData.ToArray();
	}

	public byte[] GetTriangleBufferData()
	{
		int lTriangleByteSize = (int)(TriangleData.GetTexelSize() * Raytracer.TEXEL_SIZE);
		List<byte> lTriangleBufferData = new List<byte>();

		for (int i = 0; i < Triangles.Length; i++)
		{
			TriangleData[] lSubmeshTriangles = Triangles[i];
			byte[] lRawBytes = new byte[lSubmeshTriangles.Length * lTriangleByteSize];

			Parallel.For(0, lSubmeshTriangles.Length, j => {
				Array.Copy(lSubmeshTriangles[j].GetBytes(), 0, lRawBytes, j * lTriangleByteSize, lTriangleByteSize);
			});

			lTriangleBufferData.AddRange(lRawBytes);
		}

		return lTriangleBufferData.ToArray();
	}
}