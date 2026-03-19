using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public class BVHSubmesh : BVHMeshVolume
{
	public Material material;

	public BVHSubmesh(VertexData[] pVertices, TriangleData[] pTriangles, mat4 pLocalToWorld, int pStart, int pCount, int pVertexOffset, int pTriangleOffset)
		: base(pVertices, pTriangles, pLocalToWorld, pStart, pCount, pVertexOffset, pTriangleOffset) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.Submesh,
			dataTexelIndex = pTexelIndex,
			boundMin = GlobalMin,
			boundMax = GlobalMax,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SubmeshData GetShaderData(Dictionary<Material, int> pMaterialMap, int pChildOffset = 0)
	{
		return new SubmeshData {
			startIndex = children.Count > 0 ? pChildOffset : startIndex,
			count = children.Count > 0 ? 0 : count,
			materialIndex = pMaterialMap.GetValueNoError(material, -1),
		};
	}
}