using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

namespace Astral.Raytracer;

public class BVHMeshVolume : IBVHVolume
{
	public vec3 Min { get; private set; }
	public vec3 Max { get; private set; }

	public readonly List<BVHMeshVolume> children = new List<BVHMeshVolume>();

	// References to BVHMesh data arrays
	public VertexData[] vertices;
	public TriangleData[] triangles;

	public int startIndex;
	public int count;
	public int vertexOffset;

	public BVHMeshVolume(VertexData[] pVertices, TriangleData[] pTriangles, int pStart, int pCount, int pVertexOffset)
	{
		startIndex = pStart;
		count = pCount;
		vertexOffset = pVertexOffset;
		vertices = pVertices;
		triangles = pTriangles;

		for (int i = startIndex; i < startIndex + count; i++)
		{
			TriangleData lTriangle = triangles[i];
			vec3 v0 = vertices[lTriangle.v0 - vertexOffset].position;
			vec3 v1 = vertices[lTriangle.v1 - vertexOffset].position;
			vec3 v2 = vertices[lTriangle.v2 - vertexOffset].position;

			Min = min(min(v0, v1), v2);
			Max = max(max(v0, v1), v2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddTriangle()
	{
		++count;
		TriangleData lTriangle = triangles[startIndex + count - 1];
		vec3 v0 = vertices[lTriangle.v0 - vertexOffset].position;
		vec3 v1 = vertices[lTriangle.v1 - vertexOffset].position;
		vec3 v2 = vertices[lTriangle.v2 - vertexOffset].position;

		Min = min(min(min(v0, v1), v2), Min);
		Max = max(max(max(v0, v1), v2), Max);
	}

	public virtual int Split(int pMaxDepth = 1, int pVertexIndexOffset = 0)
	{
		if (triangles is not { Length: > 0 })
		{
			return vertexOffset;
		}

		BVHMeshVolume lChild0 = new BVHMeshVolume(vertices, triangles, startIndex, 0, vertexOffset);
		BVHMeshVolume lChild1 = new BVHMeshVolume(vertices, triangles, startIndex, 0, vertexOffset);

		vec3 lSplitAxis = BVHBuilder.GetSplitAxis(this);

		for (int i = startIndex; i < count; i++)
		{
			TriangleData lTriangle = triangles[i];
			vec3 lCenter = (
				vertices[lTriangle.v0 - vertexOffset].position +
				vertices[lTriangle.v1 - vertexOffset].position +
				vertices[lTriangle.v2 - vertexOffset].position
			) / 3f;

			if (all(lessThanEqual(lCenter, lSplitAxis)))
			{
				if (lChild1.count > 0)
				{
					// Swap triangles for contiguity
					TriangleData lSwappedTriangle = triangles[lChild1.startIndex];
					triangles[lChild1.startIndex] = lTriangle;
					triangles[i] = lSwappedTriangle;
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

		return pVertexIndexOffset;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.BoundingVolume,
			dataTexelIndex = pTexelIndex,
			boundMin = Min,
			boundMax = Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BoundingVolumeData GetShaderData(int pChildOffset = 0)
	{
		return new BoundingVolumeData {
			startIndex = children.Count > 0 ? pChildOffset : startIndex,
			count = children.Count > 0 ? 0 : count,
		};
	}
}