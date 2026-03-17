using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

public class BVHResult
{
	public static BVHResult Empty => new BVHResult {
		shapeBuffer = Array.Empty<byte>(),
		dataBuffer = Array.Empty<byte>(),
		vertexBuffer = Array.Empty<byte>(),
		triangleBuffer = Array.Empty<byte>(),
	};

	public byte[] shapeBuffer;
	public byte[] dataBuffer;
	public byte[] vertexBuffer;
	public byte[] triangleBuffer;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BVHResult()
	{
		shapeBuffer = Array.Empty<byte>();
		dataBuffer = Array.Empty<byte>();
		vertexBuffer = Array.Empty<byte>();
		triangleBuffer = Array.Empty<byte>();
	}

	public BVHResult(ShapeData[] pShapes, IShaderData[] pData, VertexData[] pVertices, TriangleData[] pTriangles)
	{
		// Shape buffer
		int lShapeByteSize = (int)(ShapeData.GetTexelSize() * Raytracer.TEXEL_SIZE);
		shapeBuffer = new byte[pShapes.Length * lShapeByteSize];

		for (int i = 0; i < pShapes.Length; i++)
		{
			Array.Copy(pShapes[i].GetBytes(), 0, shapeBuffer, i * lShapeByteSize, lShapeByteSize);
		}

		// Data buffer
		List<byte> lData = new List<byte>(pData.Length);

		for (int i = 0; i < pData.Length; i++)
		{
			lData.AddRange(pData[i].GetBytes());
		}

		dataBuffer = lData.ToArray();

		// Vertex buffer
		int lVertexByteSize = VertexData.GetTexelSize() * Raytracer.TEXEL_SIZE;
		vertexBuffer = new byte[pVertices.Length * lVertexByteSize];

		for (int i = 0; i < pVertices.Length; i++)
		{
			Array.Copy(pVertices[i].GetBytes(), 0, vertexBuffer, i * lVertexByteSize, lVertexByteSize);
		}

		// Triangle buffer
		int lTriangleByteSize = Mathf.CeilToInt(TriangleData.GetTexelSize() * Raytracer.TEXEL_SIZE);
		triangleBuffer = new byte[pTriangles.Length * lTriangleByteSize];

		for (int i = 0; i < pTriangles.Length; i++)
		{
			Array.Copy(pTriangles[i].GetBytes(), 0, triangleBuffer, i * lTriangleByteSize, lTriangleByteSize);
		}
	}
}