using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Astral.Tools;

namespace Astral.Raytracer;

public struct PureTorus : IPureShape<TorusData>
{
	public mat4 transform;
	public Bounds bounds;
	public float majorRadius;
	public float minorRadius;
	public int material;

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public ERaytracedShapeType GetShapeType()
	{
		return ERaytracedShapeType.Torus;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public Bounds GetBounds()
	{
		return bounds;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public int[] GetMaterials()
	{
		return [material];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	(ShapeData, IShaderData) IPureShape.GetShaderData(int pTexelIndex)
	{
		return GetShaderData(pTexelIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public (ShapeData, TorusData) GetShaderData(int pTexelIndex)
	{
		return (
			new ShapeData {
				type = (int)GetShapeType(),
				dataTexelIndex = pTexelIndex,
				boundMin = bounds.Min,
				boundMax = bounds.Max,
			},
			new TorusData {
				majorRadius = majorRadius,
				minorRadius = minorRadius,
				transform = transform,
				materialIndex = material,
			}
		);
	}
}