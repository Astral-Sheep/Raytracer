using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Astral.Raytracer;

public class BVHSubmesh : BVHMeshVolume
{
	public int surfaceIndex;
	public int material;

	public BVHSubmesh(CookData pCookData, int pStart, int pEnd, int pSurfaceIndex, int pMaterialIndex)
		: base(pCookData, pStart, pEnd)
	{
		surfaceIndex = pSurfaceIndex;
		material = pMaterialIndex;
	}

	public void SetSplitData(ImmutableArray<BVHMeshVolume> pChildren)
	{
		if (split)
			return;

		Children = pChildren;
		split = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.Submesh,
			dataTexelIndex = pTexelIndex,
			boundMin = GetBounds().Min,
			boundMax = GetBounds().Max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override IShaderData GetShaderData(int pChildOffset = 0)
	{
		return new SubmeshData {
			startIndex = Children.Length > 0 ? pChildOffset : start,
			count = Children.Length > 0 ? 0 : ChildCount,
			materialIndex = material,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public IShaderData GetShaderData(List<IBounded> pObjects, out bool pCanAddChildren)
	{
		int lChildIndex = pObjects.IndexOf(Children[0]);
		pCanAddChildren = lChildIndex < 0;

		return new SubmeshData {
			startIndex = Children.Length > 0
				? lChildIndex >= 0 ? lChildIndex : pObjects.Count
				: start,
			count = Children.Length > 0 ? 0 : ChildCount,
			materialIndex = material,
		};
	}
}