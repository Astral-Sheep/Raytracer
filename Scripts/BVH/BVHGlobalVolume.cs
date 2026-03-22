using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

namespace Astral.Raytracer;

public class BVHGlobalVolume : IBVHVolume
{
	public vec3 Min { get; private set; }
	public vec3 Max { get; private set; }
	public int ChildCount { get; private set; }

	public readonly List<IRaytracedShape> childShapes = new List<IRaytracedShape>();
	public readonly List<IBVHVolume> childVolumes = new List<IBVHVolume>();

	protected Dictionary<Mesh, BVHMesh> builtMeshes = new Dictionary<Mesh, BVHMesh>();
	protected List<ShapeBounds> childBounds = new List<ShapeBounds>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHGlobalVolume()
	{
		Min = new vec3(0f);
		Max = new vec3(0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHGlobalVolume(IRaytracedShape[] pShapes = null, ShapeBounds[] pShapeBounds = null, Dictionary<Mesh, BVHMesh> pBuiltMeshes = null)
	{
		Min = new vec3(0f);
		Max = new vec3(0f);
		childShapes = new List<IRaytracedShape>(pShapes ?? Array.Empty<IRaytracedShape>());
		ChildCount = childShapes.Count;
		builtMeshes = pBuiltMeshes ?? new Dictionary<Mesh, BVHMesh>();
		childBounds = new List<ShapeBounds>(pShapeBounds ?? Array.Empty<ShapeBounds>());

		if (childBounds.Count <= 0 && childShapes.Count > 0)
		{
			childBounds = new List<ShapeBounds>(childShapes.Select(s => s.Bounds));

			// Parallel.For(0, childShapes.Count, i => {
			// 	childBounds[i] = childShapes[i].Bounds;
			// });

			// childBounds ??= childShapes != null
			// 	? childShapes.AsParallel().Select(s => s.Bounds).ToArray()
			// 	: Array.Empty<ShapeBounds>();
		}

		for (int i = 0; i < childBounds.Count; i++)
		{
			ShapeBounds lBounds = childBounds[i];
			Min = min(Min, lBounds.min);
			Max = max(Max, lBounds.max);
		}
	}

	public void Include(IRaytracedShape pShape, ShapeBounds pBounds)
	{
		if (childShapes.Contains(pShape))
			return;

		IncludeNoCheck(pShape, pBounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void IncludeNoCheck(IRaytracedShape pShape, ShapeBounds pBounds)
	{
		Min = min(Min, pBounds.min);
		Max = max(Max, pBounds.max);
		childShapes.Add(pShape);
		childBounds.Add(pBounds);
		++ChildCount;
	}

	public int Split(int pMaxDepth = 1, int pVertexIndexOffset = 0)
	{
		if (childVolumes.Count > 0)
		{
			return pVertexIndexOffset;
		}

		if (pMaxDepth <= 0 || childShapes.Count <= 2)
		{
			return SplitLeafMeshes(pVertexIndexOffset);
		}

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			return SplitLeafMeshes(pVertexIndexOffset);
		}

		BVHGlobalVolume lChild0 = new BVHGlobalVolume(pBuiltMeshes: builtMeshes);
		BVHGlobalVolume lChild1 = new BVHGlobalVolume(pBuiltMeshes: builtMeshes);

		for (int i = 0; i < childShapes.Count; i++)
		{
			IRaytracedShape lShape = childShapes[i];

			if (all(lessThanEqual(lShape.Bounds.Center, lSplitAxis)))
			{
				lChild0.IncludeNoCheck(lShape, childBounds[i]);
			}
			else
			{
				lChild1.IncludeNoCheck(lShape, childBounds[i]);
			}
		}

		childShapes.Clear();
		AddSubvolume(lChild0, pMaxDepth - 1, ref pVertexIndexOffset);
		AddSubvolume(lChild1, pMaxDepth - 1, ref pVertexIndexOffset);

		return pVertexIndexOffset;
	}

	private void AddSubvolume(BVHGlobalVolume pVolume, int pMaxDepth, ref int pVertexIndexOffset)
	{
		if (pVolume.childShapes.Count < 2)
		{
			if (pVolume.childShapes.Count > 0 && pVolume.childShapes[0] is RaytracedMesh lMesh)
			{
				pVertexIndexOffset = SplitMesh(lMesh, pVertexIndexOffset);
			}
			else
			{
				childShapes.AddRange(pVolume.childShapes);
			}
		}
		else
		{
			pVertexIndexOffset = pVolume.Split(pMaxDepth - 1, pVertexIndexOffset);
			childVolumes.Add(pVolume);
		}
	}

	private int SplitLeafMeshes(int pVertexIndexOffset)
	{
		for (int i = childShapes.Count - 1; i >= 0; i--)
		{
			if (childShapes[i] is not RaytracedMesh lMesh)
				continue;

			pVertexIndexOffset = SplitMesh(lMesh, pVertexIndexOffset);
			childShapes.RemoveAt(i);
		}

		return pVertexIndexOffset;
	}

	private int SplitMesh(RaytracedMesh pMesh, int pVertexIndexOffset)
	{
		BVHMesh lBVHMesh = new BVHMesh(pMesh);

		if (builtMeshes.TryGetValue(pMesh.Mesh, out BVHMesh lBuiltMesh))
		{
			lBVHMesh.GenerateTriangleBuffer(lBuiltMesh.VertexOffset);
		}
		else
		{
			int lIndexOffset = pVertexIndexOffset;
			pVertexIndexOffset = lBVHMesh.GenerateTriangleBuffer(pVertexIndexOffset);
			lBVHMesh.Split(BVHMesh.MaxDepth, lIndexOffset);
			builtMeshes.Add(pMesh.Mesh, lBVHMesh);
		}

		childVolumes.Add(lBVHMesh);
		return pVertexIndexOffset;
	}

	// public float GetSplitScore(vec3 pAxis)
	// {
	// 	return GetSplitScoreThreadSafe(pAxis, childBounds);
	// 	// (int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(0f), new vec3(0f));
	// 	// (int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(0f), new vec3(0f));
	// 	//
	// 	// for (int i = 0; i < childShapes.Count; i++)
	// 	// {
	// 	// 	IRaytracedShape lShape = childShapes[i];
	// 	//
	// 	// 	if (all(lessThanEqual(lShape.Bounds.Center, pAxis)))
	// 	// 	{
	// 	// 		++lVolume0.count;
	// 	// 		lVolume0.min = min(lVolume0.min, lShape.Bounds.min);
	// 	// 		lVolume0.max = max(lVolume0.max, lShape.Bounds.max);
	// 	// 	}
	// 	// 	else
	// 	// 	{
	// 	// 		++lVolume1.count;
	// 	// 		lVolume1.min = min(lVolume1.min, lShape.Bounds.min);
	// 	// 		lVolume1.max = max(lVolume1.max, lShape.Bounds.max);
	// 	// 	}
	// 	// }
	// 	//
	// 	// return lVolume0.count * (lVolume0.max.x - lVolume0.min.x) * (lVolume0.max.y - lVolume0.min.y) * (lVolume0.max.z - lVolume0.min.z)
	// 	// 	+ lVolume1.count * (lVolume1.max.x - lVolume1.min.x) * (lVolume1.max.y - lVolume1.min.y) * (lVolume1.max.z - lVolume1.min.z);
	// }

	public float GetSplitScore(vec3 pAxis)
	{
		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(0f), new vec3(0f));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(0f), new vec3(0f));

		for (int i = 0; i < childBounds.Count; i++)
		{
			ShapeBounds lBounds = childBounds[i];

			if (all(lessThanEqual(lBounds.Center, pAxis)))
			{
				++lVolume0.count;
				lVolume0.min = min(lVolume0.min, lBounds.min);
				lVolume0.max = max(lVolume0.max, lBounds.max);
			}
			else
			{
				++lVolume1.count;
				lVolume1.min = min(lVolume1.min, lBounds.min);
				lVolume1.max = max(lVolume1.max, lBounds.max);
			}
		}

		return lVolume0.count * (lVolume0.max.x - lVolume0.min.x) * (lVolume0.max.y - lVolume0.min.y) * (lVolume0.max.z - lVolume0.min.z)
			   + lVolume1.count * (lVolume1.max.x - lVolume1.min.x) * (lVolume1.max.y - lVolume1.min.y) * (lVolume1.max.z - lVolume1.min.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShaderShape(int pTexelIndex)
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
			startIndex = pChildOffset,
			count = childShapes.Count + childVolumes.Count,
		};
	}

	public virtual string ToString(int pDepth)
	{
		string lString = $"{GetType().Name}: {childShapes.Count} shapes & {childVolumes.Count} volumes";

		for (int i = 0; i < childShapes.Count; i++)
		{
			lString += $"\n{new string(' ', pDepth * 5)}---> {childShapes[i].GetType().Name}";
		}

		for (int i = 0; i < childVolumes.Count; i++)
		{
			lString += $"\n{new string(' ', pDepth * 5)}---> {childVolumes[i].ToString(pDepth + 1)}";
		}

		return lString;
	}
}