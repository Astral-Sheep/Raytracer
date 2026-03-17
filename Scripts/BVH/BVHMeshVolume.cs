using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public class BVHMeshVolume : IBVHVolume
{
	public vec3 Min { get; private set; }
	public vec3 Max { get; private set; }
	public int ChildCount { get; private set; }

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
		ChildCount = count;

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
		++ChildCount;

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

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			return pVertexIndexOffset;
		}

		BVHMeshVolume lChild0 = new BVHMeshVolume(vertices, triangles, startIndex, 0, vertexOffset);
		BVHMeshVolume lChild1 = new BVHMeshVolume(vertices, triangles, startIndex, 0, vertexOffset);

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

	public float GetSplitScore(vec3 pAxis)
	{
		if (count < 2)
			return -1f;

		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(0f), new vec3(0f));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(0f), new vec3(0f));

		for (int i = 0; i < triangles.Length; i++)
		{
			TriangleData lTriangle = triangles[i];
			vec3 lV0 = vertices[lTriangle.v0 - vertexOffset].position;
			vec3 lV1 = vertices[lTriangle.v1 - vertexOffset].position;
			vec3 lV2 = vertices[lTriangle.v2 - vertexOffset].position;

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