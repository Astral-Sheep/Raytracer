using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Astral.Raytracer;

public class BVHGlobalVolume : IBVHVolume
{
	public int ChildCount { get; }

	public readonly VolumeChildArray children = new VolumeChildArray();
	public readonly List<IBVHVolume> childVolumes = new List<IBVHVolume>();

	protected Bounds bounds = new Bounds(new vec3(0f), new vec3(0f));
	protected bool split = false;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHGlobalVolume()
	{
		ChildCount = 0;
	}

	public BVHGlobalVolume(IPureShape[] pShapes, BVHMesh[] pMeshes)
	{
		children = new VolumeChildArray(pShapes, pMeshes);
		ChildCount = children.Count + childVolumes.Count;
		split = ChildCount <= 2;

		if (ChildCount <= 0)
		{
			bounds = new Bounds(new vec3(0f), new vec3(0f));
			return;
		}

		if (split && pMeshes.Length > 0)
		{
			childVolumes.AddRange(children.meshes);
			children.meshes.Clear();
		}

		vec3 lMin = new vec3(float.PositiveInfinity);
		vec3 lMax = new vec3(float.NegativeInfinity);

		foreach (IBounded lChild in children)
		{
			lMin = min(lMin, lChild.GetBounds().Min);
			lMax = min(lMax, lChild.GetBounds().Max);
		}

		bounds = new Bounds(lMin, lMax);
	}

	public BVHGlobalVolume(VolumeChildArray pChildren)
	{
		children = pChildren;
		ChildCount = children.Count + childVolumes.Count;
		split = ChildCount <= 2;

		if (ChildCount <= 0)
		{
			bounds = new Bounds(new vec3(0f), new vec3(0f));
			return;
		}

		if (split && children.meshes.Count > 0)
		{
			childVolumes.AddRange(children.meshes);
			children.meshes.Clear();
		}

		vec3 lMin = new vec3(float.PositiveInfinity);
		vec3 lMax = new vec3(float.NegativeInfinity);

		foreach (IBounded lChild in children)
		{
			lMin = min(lMin, lChild.GetBounds().Min);
			lMax = min(lMax, lChild.GetBounds().Max);
		}

		bounds = new Bounds(lMin, lMax);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds GetBounds()
	{
		return bounds;
	}

	public void Split(int pMaxDepth = 1)
	{
		if (split || pMaxDepth <= 0 || childVolumes.Count > 0 || children.Count <= 2)
			return;

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			split = true;
			return;
		}

		VolumeChildArray lVolume0Children = new VolumeChildArray();
		VolumeChildArray lVolume1Children = new VolumeChildArray();

		foreach (IBounded lChild in children)
		{
			if (all(lessThanEqual(lChild.GetBounds().Center, lSplitAxis)))
			{
				lVolume0Children.Add(lChild);
			}
			else
			{
				lVolume1Children.Add(lChild);
			}
		}

		children.Clear();

		BVHGlobalVolume lChild0 = AddSubvolumes(lVolume0Children);
		BVHGlobalVolume lChild1 = AddSubvolumes(lVolume1Children);

		// List<Task> lTasks = new List<Task>();

		if (lChild0 != null)
		{
			lChild0.Split(pMaxDepth - 1);
			// lTasks.Add(Task.Run(() => lChild0.Split(pMaxDepth - 1)));
		}

		if (lChild1 != null)
		{
			lChild1.Split(pMaxDepth - 1);
			// lTasks.Add(Task.Run(() => lChild1.Split(pMaxDepth - 1)));
		}

		// Parallel.For(0, lChildren.Count, i => {
		// 	lChildren[i].Split(pMaxDepth - 1);
		// });
		// Task.WaitAll(lTasks.ToArray());
		split = true;
	}

	private BVHGlobalVolume AddSubvolumes(VolumeChildArray pVolumeChildren)
	{
		if (pVolumeChildren.Count < 2)
		{
			children.shapes.AddRange(pVolumeChildren.shapes);
			childVolumes.AddRange(pVolumeChildren.meshes);
			return null;
		}
		else
		{
			BVHGlobalVolume lSubvolume = new BVHGlobalVolume(pVolumeChildren);
			childVolumes.Add(lSubvolume);
			return lSubvolume;
		}
	}

	public float GetSplitCost(vec3 pAxis)
	{
		if (split)
		{
			return float.PositiveInfinity;
		}

		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));

		foreach (IBounded lChild in children)
		{
			Bounds lBounds = lChild.GetBounds();

			if (all(lessThanEqual(lBounds.Center, pAxis)))
			{
				++lVolume0.count;
				lVolume0.min = min(lVolume0.min, lBounds.Min);
				lVolume0.max = max(lVolume0.max, lBounds.Max);
			}
			else
			{
				++lVolume1.count;
				lVolume1.min = min(lVolume1.min, lBounds.Min);
				lVolume1.max = max(lVolume1.max, lBounds.Max);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.BoundingVolume,
			dataTexelIndex = pTexelIndex,
			boundMin = bounds.Min,
			boundMax = bounds.Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BoundingVolumeData GetShaderData(int pChildOffset = 0)
	{
		return new BoundingVolumeData {
			startIndex = pChildOffset,
			count = children.Count + childVolumes.Count,
		};
	}

	public virtual string ToString(int pDepth)
	{
		string lString = $"{GetType().Name}: {children.Count} shapes & {childVolumes.Count} volumes";

		foreach (IBounded lChild in children)
		{
			lString += $"\n{new string(' ', pDepth * 5)}---> {lChild.GetType().Name}";
		}

		for (int i = 0; i < childVolumes.Count; i++)
		{
			lString += $"\n{new string(' ', pDepth * 5)}---> {childVolumes[i].ToString(pDepth + 1)}";
		}

		return lString;
	}
}