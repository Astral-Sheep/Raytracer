using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

public class BVHBoundingVolume
{
	public vec3 min;
	public vec3 max;
	public readonly List<IRaytracedShape> childShapes;
	public readonly List<BVHBoundingVolume> childVolumes = new List<BVHBoundingVolume>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHBoundingVolume(IRaytracedShape[] pShapes = null)
	{
		min = new vec3(0f);
		max = new vec3(0f);
		childShapes = new List<IRaytracedShape>(pShapes ?? Array.Empty<IRaytracedShape>());
	}

	public void Include(IRaytracedShape pShape)
	{
		if (childShapes.Contains(pShape))
			return;

		IncludeNoCheck(pShape);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void IncludeNoCheck(IRaytracedShape pShape)
	{
		min = min(min, pShape.Bounds.min);
		max = max(max, pShape.Bounds.max);
		childShapes.Add(pShape);
	}

	public void Split(int pMaxDepth = 0)
	{
		if (pMaxDepth <= 0 || childVolumes.Count > 0 || childShapes.Count <= 2)
			return;

		BVHBoundingVolume lChild0 = new BVHBoundingVolume();
		BVHBoundingVolume lChild1 = new BVHBoundingVolume();

		vec3 lSplitAxis = (min + max) * .5f;

		// Mathf.Inf is used because we compare with lessThan. -Mathf.Inf should be used if compared with greaterThan
		if (lSplitAxis.x > lSplitAxis.y && lSplitAxis.x > lSplitAxis.z)
		{
			lSplitAxis = new vec3(lSplitAxis.x, Mathf.Inf, Mathf.Inf);
		}
		else if (lSplitAxis.y > lSplitAxis.z)
		{
			lSplitAxis = new vec3(Mathf.Inf, lSplitAxis.y, Mathf.Inf);
		}
		else
		{
			lSplitAxis = new vec3(Mathf.Inf, Mathf.Inf, lSplitAxis.z);
		}

		for (int i = 0; i < childShapes.Count; i++)
		{
			IRaytracedShape lShape = childShapes[i];

			if (all(lessThanEqual(lSplitAxis, lShape.Bounds.Center)))
			{
				lChild0.IncludeNoCheck(lShape);
			}
			else
			{
				lChild1.IncludeNoCheck(lShape);
			}
		}

		childShapes.Clear();

		if (lChild0.childShapes.Count < 2)
		{
			childShapes.AddRange(lChild0.childShapes);
		}
		else
		{
			lChild0.Split(pMaxDepth - 1);
			childVolumes.Add(lChild0);
		}

		if (lChild1.childShapes.Count < 2)
		{
			childShapes.AddRange(lChild1.childShapes);
		}
		else
		{
			lChild1.Split(pMaxDepth - 1);
			childVolumes.Add(lChild1);
		}
	}
}