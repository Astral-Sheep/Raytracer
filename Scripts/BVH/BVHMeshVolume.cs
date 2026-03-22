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

	public vec3 GlobalMin => (localToWorld * new vec4(Min, 1f)).xyz;
	public vec3 GlobalMax => (localToWorld * new vec4(Max, 1f)).xyz;

	public int ChildCount { get; private set; }

	public readonly List<BVHMeshVolume> children = new List<BVHMeshVolume>();

	// References to BVHMesh data arrays
	public VertexData[] vertices;
	public TriangleData[] triangles;

	public int startIndex;
	public int count;
	public int vertexOffset;
	public int triangleOffset;
	public mat4 localToWorld;

	public BVHMeshVolume(VertexData[] pVertices, TriangleData[] pTriangles, mat4 pLocalToWorld, int pStart, int pCount, int pVertexOffset, int pTriangleOffset)
	{
		vertices = pVertices;
		triangles = pTriangles;
		localToWorld = pLocalToWorld;

		startIndex = pStart;
		count = pCount;
		vertexOffset = pVertexOffset;
		triangleOffset = pTriangleOffset;

		ChildCount = count;

		for (int i = startIndex; i < startIndex + count; i++)
		{
			TriangleData lTriangle = triangles[i - triangleOffset];
			vec3 v0 = vertices[lTriangle.v0 - vertexOffset].position;
			vec3 v1 = vertices[lTriangle.v1 - vertexOffset].position;
			vec3 v2 = vertices[lTriangle.v2 - vertexOffset].position;

			Min = min(min(min(v0, v1), v2), Min);
			Max = max(max(max(v0, v1), v2), Max);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddTriangle()
	{
		++count;
		++ChildCount;

		TriangleData lTriangle = triangles[startIndex + count - 1 - triangleOffset];
		vec3 v0 = vertices[lTriangle.v0 - vertexOffset].position;
		vec3 v1 = vertices[lTriangle.v1 - vertexOffset].position;
		vec3 v2 = vertices[lTriangle.v2 - vertexOffset].position;

		Min = min(min(min(v0, v1), v2), Min);
		Max = max(max(max(v0, v1), v2), Max);
	}

	public virtual int Split(int pMaxDepth = 1, int pVertexIndexOffset = 0)
	{
		if (pMaxDepth <= 0 || count <= 1 || triangles is not { Length: > 0 } || startIndex >= triangles.Length)
		{
			return vertexOffset;
		}

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			return pVertexIndexOffset;
		}

		BVHMeshVolume lChild0 = new BVHMeshVolume(vertices, triangles, localToWorld, startIndex, 0, vertexOffset, triangleOffset);
		BVHMeshVolume lChild1 = new BVHMeshVolume(vertices, triangles, localToWorld, startIndex, 0, vertexOffset, triangleOffset);

		for (int i = startIndex; i < startIndex + count; i++)
		{
			TriangleData lTriangle = triangles[i - triangleOffset];
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
					TriangleData lSwappedTriangle = triangles[lChild1.startIndex - triangleOffset];
					triangles[lChild1.startIndex - triangleOffset] = lTriangle;
					triangles[i - triangleOffset] = lSwappedTriangle;
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
			TriangleData lTriangle = triangles[i - triangleOffset];
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
			boundMin = GlobalMin,
			boundMax = GlobalMax,
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