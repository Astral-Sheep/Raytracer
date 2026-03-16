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

	public readonly List<IRaytracedShape> childShapes = new List<IRaytracedShape>();
	public readonly List<IBVHVolume> childVolumes = new List<IBVHVolume>();
	protected Dictionary<Mesh, BVHMesh> builtMeshes = new Dictionary<Mesh, BVHMesh>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHGlobalVolume()
	{
		Min = new vec3(0f);
		Max = new vec3(0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHGlobalVolume(IRaytracedShape[] pShapes = null, Dictionary<Mesh, BVHMesh> pBuiltMeshes = null)
	{
		Min = new vec3(0f);
		Max = new vec3(0f);
		childShapes = new List<IRaytracedShape>(pShapes ?? Array.Empty<IRaytracedShape>());
		builtMeshes = pBuiltMeshes ?? new Dictionary<Mesh, BVHMesh>();

		for (int i = 0; i < childShapes.Count; i++)
		{
			IRaytracedShape lShape = childShapes[i];
			Min = min(Min, lShape.Bounds.min);
			Max = max(Max, lShape.Bounds.max);
		}
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
		Min = min(Min, pShape.Bounds.min);
		Max = max(Max, pShape.Bounds.max);
		childShapes.Add(pShape);
	}

	public int Split(int pMaxDepth = 1, int pVertexIndexOffset = 0)
	{
		if (pMaxDepth <= 0 || childVolumes.Count > 0 || childShapes.Count <= 2)
		{
			return pVertexIndexOffset;
		}

		BVHGlobalVolume lChild0 = new BVHGlobalVolume(pBuiltMeshes: builtMeshes);
		BVHGlobalVolume lChild1 = new BVHGlobalVolume(pBuiltMeshes: builtMeshes);

		vec3 lSplitAxis = BVHBuilder.GetSplitAxis(this);

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
		AddSubvolume(lChild0, ref pVertexIndexOffset);
		AddSubvolume(lChild1, ref pVertexIndexOffset);

		return pVertexIndexOffset;
	}

	private void AddSubvolume(BVHGlobalVolume pVolume, ref int pVertexIndexOffset)
	{
		if (pVolume.childShapes.Count < 2)
		{
			if (pVolume.childShapes.Count > 0 && pVolume.childShapes[0] is RaytracedMesh lMesh)
			{
				BVHMesh lBVHMesh = new BVHMesh(lMesh);

				if (builtMeshes.TryGetValue(lMesh.Mesh, out BVHMesh lBuiltMesh))
				{
					lBVHMesh.GenerateTriangleBuffer(lBuiltMesh.VertexOffset);
				}
				else
				{
					pVertexIndexOffset = lBVHMesh.GenerateTriangleBuffer(pVertexIndexOffset);
					pVertexIndexOffset = lBVHMesh.Split(pVertexIndexOffset: pVertexIndexOffset);
					builtMeshes.Add(lMesh.Mesh, lBVHMesh);
				}

				childVolumes.Add(lBVHMesh);
			}
			else
			{
				childShapes.AddRange(pVolume.childShapes);
			}
		}
		else
		{
			pVertexIndexOffset = pVolume.Split(pVertexIndexOffset: pVertexIndexOffset);
			childVolumes.Add(pVolume);
		}
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
}