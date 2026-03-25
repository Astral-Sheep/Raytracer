using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Astral.Raytracer;

public class CookData
{
	public readonly ImmutableArray<VertexData> vertexBuffer;
	public readonly TriangleData[] triangleBuffer;
	public readonly ImmutableDictionary<Mesh, ImmutableArray<int>> meshTable;

	public CookData(VertexData[] pVertexBuffer, TriangleData[] pTriangleBuffer, Dictionary<Mesh, int[]> pMeshTable)
	{
		vertexBuffer = pVertexBuffer.ToImmutableArray();
		triangleBuffer = pTriangleBuffer;
		meshTable = pMeshTable
			.Select(kvp => new KeyValuePair<Mesh, ImmutableArray<int>>(kvp.Key, kvp.Value.ToImmutableArray()))
			.ToImmutableDictionary();
	}

	public (byte[] vertexBuffer, byte[] triangleBuffer) Build()
	{
		int lVertexSize = VertexData.GetTexelSize();
		int lVertexBufferSize = vertexBuffer.Length * lVertexSize;
		byte[] lVertexBuffer = new byte[lVertexBufferSize];

		int lTriangleSize = Mathf.CeilToInt(TriangleData.GetTexelSize());
		int lTriangleBufferSize = Mathf.CeilToInt(triangleBuffer.Length * lTriangleSize);
		byte[] lTriangleBuffer = new byte[lTriangleBufferSize];

		Parallel.Invoke(
			() => Parallel.For(0, vertexBuffer.Length, i => {
				Array.Copy(vertexBuffer[i].GetBytes(), 0, lVertexBuffer, i * lVertexSize, lVertexSize);
			}),
			() => Parallel.For(0, triangleBuffer.Length, i => {
				Array.Copy(triangleBuffer[i].GetBytes(), 0, lTriangleBuffer, i * lTriangleSize, lTriangleSize);
			})
		);

		return (lVertexBuffer, lTriangleBuffer);
	}
}
