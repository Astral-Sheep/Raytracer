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
	public static int MaxDepth => GodotUtility.GetSetting<int>("rendering/pathtracing/bvh_mesh_depth");

	public vec3 Min { get; }
	public vec3 Max { get; }
	public int ChildCount { get; protected init; }

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

		// TO FIX: store it before multithreading starts
		Min = basis.Bounds.min;
		Max = basis.Bounds.max;

		int lSurfaceCount = basis.Mesh.GetSurfaceCount();
		Vertices = new VertexData[lSurfaceCount][];
		Triangles = new TriangleData[lSurfaceCount][];

		if (lSurfaceCount > 1)
		{
			ChildCount = lSurfaceCount;
		}
		else if (lSurfaceCount == 1)
		{
			ChildCount = basis.Mesh.SurfaceGetArrays(0)[(int)Mesh.ArrayType.Index].As<GArray>().Count / 3;
		}
		else
		{
			ChildCount = 0;
		}

		Parallel.For(0, lSurfaceCount, i => {
			GArray lSurface = basis.Mesh.SurfaceGetArrays(i);

			GArray lVertexArray = lSurface[(int)Mesh.ArrayType.Vertex].As<GArray>();
			GArray lNormalArray = lSurface[(int)Mesh.ArrayType.Normal].As<GArray>();
			GArray lUVArray = lSurface[(int)Mesh.ArrayType.TexUV].As<GArray>();

			Vertices[i] = lVertexArray.AsParallel().Select((v, j) => new VertexData {
				position = fromVariant(v.As<Vector3>()),
				normal = j < lNormalArray.Count ? fromVariant(lNormalArray[j].As<Vector3>()) : new vec3(0f),
				uv = j < lUVArray.Count ? fromVariant(lUVArray[j].As<Vector2>()) : new vec2(0f),
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
						v0 = pVertexIndexOffset + lTriangles[j * 3].As<int>(),
						v1 = pVertexIndexOffset + lTriangles[j * 3 + 1].As<int>(),
						v2 = pVertexIndexOffset + lTriangles[j * 3 + 2].As<int>(),
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

		for (int i = 0; i < basis.Mesh.GetSurfaceCount(); i++)
		{
			if (Triangles[i] is not { Length: > 0 })
				continue;

			BVHSubmesh lSubmesh = new BVHSubmesh(Vertices[i], Triangles[i], basis.GlobalTransform, lTriangleCount, Triangles[i].Length, VertexOffset, lTriangleCount) {
				material = basis.Materials[i],
			};
			children.Add(lSubmesh);
			lTriangleCount += Triangles[i].Length;
		}

		if (children.Count == 1)
		{
			SplitInSubvolumes(pMaxDepth, Triangles.IndexOf((children[0] as BVHSubmesh).triangles));
		}
		else
		{
			Task.WaitAll(
				children
					.AsParallel()
					.Select(c => Task.Run(() => {
						c.Split(pMaxDepth - 1, VertexOffset);
					}))
					.ToArray()
			);
		}
	}

	private void SplitInSubvolumes(int pMaxDepth, int pSurfaceIndex)
	{
		VertexData[] lVertices = Vertices[pSurfaceIndex];
		TriangleData[] lTriangles = Triangles[pSurfaceIndex];

		if (lTriangles is not { Length: > 1 })
			return;

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
			return;

		BVHMeshVolume lChild0 = new BVHMeshVolume(Vertices[pSurfaceIndex], Triangles[pSurfaceIndex], basis.GlobalTransform, 0, 0, VertexOffset, 0);
		BVHMeshVolume lChild1 = new BVHMeshVolume(Vertices[pSurfaceIndex], Triangles[pSurfaceIndex], basis.GlobalTransform, 0, 0, VertexOffset, 0);

		for (int i = 0; i < lTriangles.Length; i++)
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

	/// <summary>
	/// Warning: if there is more than one surface, the result is always -1
	/// </summary>
	public float GetSplitScore(vec3 pAxis)
	{
		// If there is more than 1 surface, we don't care about the split axis 
		if (basis.Mesh.GetSurfaceCount() > 1)
			return -1f;

		VertexData[] lVertices = Vertices[0];
		TriangleData[] lTriangles = Triangles[0];
		pAxis = (inverse(new mat4(basis.Transform)) * new vec4(pAxis, 0f)).xyz;

		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(0f), new vec3(0f));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(0f), new vec3(0f));

		for (int i = 0; i < lTriangles.Length; i++)
		{
			TriangleData lTriangle = lTriangles[i];
			vec3 lV0 = lVertices[lTriangle.v0 - VertexOffset].position;
			vec3 lV1 = lVertices[lTriangle.v1 - VertexOffset].position;
			vec3 lV2 = lVertices[lTriangle.v2 - VertexOffset].position;

			vec3 lMin = min(lV0, min(lV1, lV2));
			vec3 lMax = max(lV0, max(lV1, lV2));
			vec3 lCenter = (lV0 + lV1 + lV2) / 3f;

			if (all(lessThanEqual(lCenter, pAxis)))
			{
				++lVolume0.count;
				lVolume0.min = min(lVolume0.min, lMin);
				lVolume0.max = max(lVolume0.max, lMax);
			}
			else
			{
				++lVolume1.count;
				lVolume1.min = min(lVolume1.min, lMin);
				lVolume1.max = max(lVolume1.max, lMax);
			}
		}

		return lVolume0.count * (lVolume0.max.x - lVolume0.min.x) * (lVolume0.max.y - lVolume0.min.y) * (lVolume0.max.z - lVolume0.min.z)
			   + lVolume1.count * (lVolume1.max.x - lVolume1.min.x) * (lVolume1.max.y - lVolume1.min.y) * (lVolume1.max.z - lVolume1.min.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			// To fix
			type = basis.Mesh.GetSurfaceCount() == 1 && children.Count == 0
				? (int)ERaytracedShapeType.LeafMesh
				: (int)ERaytracedShapeType.Mesh,
			dataTexelIndex = pTexelIndex,
			boundMin = Min,
			boundMax = Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MeshData GetShaderData(Dictionary<Mesh, (int start, int count)> pMeshMap, int pChildOffset, int pTriangleOffset, Dictionary<Material, int> pMaterialMap, Material pDefaultMaterial)
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
			materialIndex = lHasSubmeshes ? -2 : pMaterialMap.GetValueNoError(basis.Materials[0] ?? pDefaultMaterial, -1),
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

	public virtual string ToString(int pDepth)
	{
		if (children.Count > 0)
		{
			string lString = $"{GetType().Name}";

			for (int i = 0; i < children.Count; i++)
			{
				lString += $"\n{new string('-', pDepth * 5)}---> {children[i].ToString(pDepth + 1)}";
			}

			return lString;
		}
		else
		{
			return $"{GetType().Name}: {ChildCount} triangles";
		}
	}
}