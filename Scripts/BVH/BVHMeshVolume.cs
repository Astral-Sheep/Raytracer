using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

namespace Astral.Raytracer;

public class BVHMeshVolume : IBVHVolume
{
	public int ChildCount { get; }
	public ImmutableArray<BVHMeshVolume> Children { get; protected set; } = ImmutableArray<BVHMeshVolume>.Empty;

	public readonly CookData cookData;
	public readonly int start;
	public readonly int end;
	public readonly Bounds bounds;

	protected bool split = false;

	public BVHMeshVolume(CookData pCookData, int pStart, int pEnd)
	{
		if (pCookData == null)
		{
			split = true;
			return;
		}

		cookData = pCookData;
		start = pStart;
		end = pEnd;
		ChildCount = end - start;

		if (ChildCount <= 0)
		{
			bounds = new Bounds(new vec3(0f), new vec3(0f));
			return;
		}

		vec3[] lMins = new vec3[ChildCount];
		vec3[] lMaxs = new vec3[ChildCount];

		Parallel.For(start, end, j => {
			TriangleData lTriangle = cookData.triangleBuffer[j];
			vec3 lV0 = cookData.vertexBuffer[lTriangle.v0].position;
			vec3 lV1 = cookData.vertexBuffer[lTriangle.v1].position;
			vec3 lV2 = cookData.vertexBuffer[lTriangle.v2].position;

			lMins[j - start] = min(lV0, min(lV1, lV2));
			lMaxs[j - start] = max(lV0, max(lV1, lV2));
		});

		vec3 lMin = new vec3(float.PositiveInfinity);
		vec3 lMax = new vec3(float.NegativeInfinity);

		for (int j = 0; j < ChildCount; j++)
		{
			lMin = min(lMin, lMins[j]);
			lMax = max(lMax, lMaxs[j]);
		}

		bounds = new Bounds(lMin, lMax);
	}

	public Bounds GetBounds()
	{
		return bounds;
	}

	public void Split(int pMaxDepth = 1)
	{
		if (split || pMaxDepth <= 0)
			return;

		if (start < 0 || ChildCount < 2)
		{
			split = true;
			return;
		}

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			// GD.Print($"Subvolume at depth {pMaxDepth} is not splittable. Returning method\n");
			split = true;
			return;
		}

		// GD.Print($"Splitting subvolume at depth {pMaxDepth} in subvolumes with axis {lSplitAxis}");
		// Acts as Volume0's end as well
		int lVolume1Start = start;
		int lVolume1Count = 0;

		for (int i = start; i < end; i++)
		{
			TriangleData lTriangle = cookData.triangleBuffer[i];
			vec3 lCenter = (
				cookData.vertexBuffer[lTriangle.v0].position
				+ cookData.vertexBuffer[lTriangle.v1].position
				+ cookData.vertexBuffer[lTriangle.v2].position
			) / 3f;

			if (all(lessThanEqual(lCenter, lSplitAxis)))
			{
				if (lVolume1Count > 0)
				{
					// Swap triangles for contiguity
					TriangleData lSwappedTriangle = cookData.triangleBuffer[lVolume1Start];
					cookData.triangleBuffer[lVolume1Start] = lTriangle;
					cookData.triangleBuffer[i] = lSwappedTriangle;
				}

				++lVolume1Start;
			}
			else
			{
				++lVolume1Count;
			}
		}

		// Volume 0 or volume 1 has all triangles, making the split useless
		if (lVolume1Start == start || lVolume1Count == 0)
		{
			// GD.Print("Splitting cancelled: all triangles where put in the same subvolume\n");
			split = true;
			return;
		}

		Children = ImmutableArray.Create(
			new BVHMeshVolume(cookData, start, lVolume1Start),
			new BVHMeshVolume(cookData, lVolume1Start, lVolume1Start + lVolume1Count)
		);
		Children[0].Split(pMaxDepth - 1);
		Children[1].Split(pMaxDepth - 1);
		// Task.WaitAll(Children.Select(c => Task.Run(() => c.Split(pMaxDepth - 1))).ToArray());
		split = true;
	}

	public float GetSplitCost(vec3 pAxis)
	{
		if (split)
		{
			return float.PositiveInfinity;
		}

		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));

		for (int i = start; i < end; i++)
		{
			TriangleData lTriangle = cookData.triangleBuffer[i];

			if (all(lessThanEqual(lTriangle.bounds.Center, pAxis)))
			{
				++lVolume0.count;
				lVolume0.min = min(lVolume0.min, lTriangle.bounds.Min);
				lVolume0.max = max(lVolume0.max, lTriangle.bounds.Max);
			}
			else
			{
				++lVolume1.count;
				lVolume1.min = min(lVolume1.min, lTriangle.bounds.Min);
				lVolume1.max = max(lVolume1.max, lTriangle.bounds.Max);
			}
		}

		Bounds lVolume0Bounds = lVolume0.count <= 0
			? new Bounds(new vec3(0f), new vec3(0f))
			: new Bounds(lVolume0.min, lVolume0.max);

		Bounds lVolume1Bounds = lVolume1.count <= 0
			? new Bounds(new vec3(0f), new vec3(0f))
			: new Bounds(lVolume1.min, lVolume1.max);

		return BVHBuilder.GetVolumeCost(lVolume0.count, lVolume0Bounds.Extent * 2f)
			 + BVHBuilder.GetVolumeCost(lVolume1.count, lVolume1Bounds.Extent * 2f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public virtual ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.BoundingVolume,
			dataTexelIndex = pTexelIndex,
			boundMin = bounds.Min,
			boundMax = bounds.Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public virtual IShaderData GetShaderData(int pChildOffset = 0)
	{
		return new BoundingVolumeData {
			startIndex = Children.Length > 0 ? pChildOffset : start,
			count = Children.Length > 0 ? 0 : ChildCount,
		};
	}

	public virtual string ToString(int pDepth)
	{
		if (Children.Length > 0)
		{
			string lString = $"{GetType().Name}";

			for (int i = 0; i < Children.Length; i++)
			{
				lString += $"\n{new string('-', pDepth * 5)}---> {Children[i].ToString(pDepth + 1)}";
			}

			return lString;
		}
		else
		{
			return $"{GetType().Name}: {ChildCount} triangles";
		}
	}
}