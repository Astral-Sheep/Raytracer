using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Astral.Raytracer;

public struct PureSphere : IPureShape<SphereData>
{
	public Bounds bounds;
	public vec3 scale;
	public float radius;
	public int material;

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public ERaytracedShapeType GetShapeType()
	{
		return ERaytracedShapeType.Sphere;
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
	public (ShapeData, SphereData) GetShaderData(int pTexelIndex)
	{
		return (
			new ShapeData {
				type = (int)GetShapeType(),
				dataTexelIndex = pTexelIndex,
				boundMin = bounds.Min,
				boundMax = bounds.Max,
			},
			new SphereData {
				center = bounds.Center,
				radius = radius,
				scale = scale,
				materialIndex = material,
			}
		);
	}
}