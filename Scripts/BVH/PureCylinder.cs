using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Astral.Tools;

namespace Astral.Raytracer;

public struct PureCylinder : IPureShape<CylinderData>
{
	public mat4 transform;
	public Bounds bounds;
	public float radius;
	public float halfHeight;
	public int material;

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public ERaytracedShapeType GetShapeType()
	{
		return ERaytracedShapeType.Cylinder;
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
	public (ShapeData, CylinderData) GetShaderData(int pTexelIndex)
	{
		return (
			new ShapeData {
				type = (int)GetShapeType(),
				dataTexelIndex = pTexelIndex,
				boundMin = bounds.Min,
				boundMax = bounds.Max,
			},
			new CylinderData {
				radius = radius,
				halfHeight = halfHeight,
				transform = transform,
				materialIndex = material,
			}
		);
	}
}