using System;
using System.Collections.Generic;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public static class BVHBuilder
{
	public static int MaxDepth = GodotUtility.GetSetting<int>("rendering/raytracing/bvh_global_depth");

	public static BVHResult BuildBVH(IRaytracedShape[] pShapes, Dictionary<Material, int> pMaterialMap)
	{
		if (pShapes is not { Length: > 0 })
		{
			return new BVHResult {
				shapeBuffer = Array.Empty<byte>(),
				dataBuffer = Array.Empty<byte>(),
				vertexBuffer = Array.Empty<byte>(),
				triangleBuffer = Array.Empty<byte>(),
			};
		}

		// === Build ===
		BVHGlobalVolume lRoot = new BVHGlobalVolume(pShapes);
		lRoot.Split(MaxDepth);

		// === Data formatting ===

		// Buffers
		List<byte> lShapes = new List<byte>();
		List<byte> lData = new List<byte>();
		List<byte> lVertices = new List<byte>();
		List<byte> lTriangles = new List<byte>();

		// Hierarchy
		List<IBVHVolume> lVolumes = new List<IBVHVolume>();
		Dictionary<Mesh, (int start, int count)> lBuiltMeshes = new Dictionary<Mesh, (int start, int count)>();
		lVolumes.Add(lRoot);

		int lCurrentVolume = 0;
		int lChildOffset = 0;

		while (lCurrentVolume < lVolumes.Count)
		{
			IBVHVolume lVolume = lVolumes[lCurrentVolume];

			switch (lVolume)
			{
				case BVHGlobalVolume lGlobalVolume:
				{
					lShapes.AddRange(lGlobalVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lGlobalVolume.GetShaderData(lVolumes.Count + lChildOffset).GetBytes());

					for (int i = 0; i < lGlobalVolume.childShapes.Count; i++)
					{
						IRaytracedShape lShape = lGlobalVolume.childShapes[i];
						lShapes.AddRange(lShape.GetShapeData(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());

						switch (lShape)
						{
							case RaytracedSphere lSphere:
								lData.AddRange(lSphere.GetShaderData(pMaterialMap).GetBytes());
								break;
							case RaytracedMesh:
								GD.PushError("Raytraced mesh handled as shape instead of volume");
								break;
							default:
								GD.PushWarning($"Unhandled raytraced shape: {lShape.GetType().Name}");
								break;
						}
					}

					lVolumes.AddRange(lGlobalVolume.childVolumes);
					lChildOffset += lGlobalVolume.childShapes.Count;
					break;
				}
				case BVHMesh lMesh:
				{
					if (!lBuiltMeshes.ContainsKey(lMesh.Mesh))
					{
						lVertices.AddRange(lMesh.GetVertexBufferData());
						lTriangles.AddRange(lMesh.GetTriangleBufferData());
					}

					lShapes.AddRange(lMesh.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lMesh.GetShaderData(
						lBuiltMeshes,
						pMaterialMap,
						lVolumes.Count + lChildOffset,
						(int)(lTriangles.Count / TriangleData.GetTexelSize())
					).GetBytes());

					lVolumes.AddRange(lMesh.children);
					break;
				}
				case BVHSubmesh lSubmesh:
				{
					lShapes.AddRange(lSubmesh.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lSubmesh.GetShaderData(pMaterialMap, lVolumes.Count + lChildOffset).GetBytes());
					lVolumes.AddRange(lSubmesh.children);
					break;
				}
				case BVHMeshVolume lMeshVolume:
				{
					lShapes.AddRange(lMeshVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lMeshVolume.GetShaderData(lVolumes.Count + lChildOffset).GetBytes());
					lVolumes.AddRange(lMeshVolume.children);
					break;
				}
				default:
				{
					GD.PushError($"Unknown BVH volume type: {lVolume.GetType().Name}");
					break;
				}
			}

			++lCurrentVolume;
		}

		return new BVHResult {
			shapeBuffer = lShapes.ToArray(),
			dataBuffer = lData.ToArray(),
			vertexBuffer = lVertices.ToArray(),
			triangleBuffer = lTriangles.ToArray(),
		};

		// List<byte> lShapeBuffer = new List<byte>();
		// List<byte> lDataBuffer = new List<byte>();
		// List<byte> lVertexBuffer = new List<byte>();
		// List<byte> lTriangleBuffer = new List<byte>();
		//
		// Dictionary<Godot.Mesh, int> lBuiltMeshes = new Dictionary<Godot.Mesh, int>();
		//
		// List<IBVHVolume> lVolumes = new List<IBVHVolume>();
		// int lCurrentVolume = 0;
		// int lChildOffset = 0;
		//
		// BVHGlobalVolume lRoot = new BVHGlobalVolume(pShapes);
		// lRoot.Split(MaxDepth);
		// lVolumes.Add(lRoot);
		//
		// while (lCurrentVolume < lVolumes.Count)
		// {
		// 	IBVHVolume lVolume = lVolumes[lCurrentVolume];
		//
		// 	switch (lVolume)
		// 	{
		// 		case RaytracedMesh lMesh:
		// 			lShapeBuffer.AddRange(new ShapeData {
		// 				type = (int)ERaytracedShapeType.BoundingVolume,
		// 				dataTexelIndex = lDataBuffer.Count / Raytracer.TEXEL_SIZE,
		// 				boundMin = lVolume.Min,
		// 				boundMax = lVolume.Max,
		// 			}.GetBytes());
		//
		// 			if (!lBuiltMeshes.TryGetValue(lMesh.Mesh, out int lTexelIndex))
		// 			{
		// 				lTexelIndex = lDataBuffer.Count / Raytracer.TEXEL_SIZE;
		// 				lDataBuffer.AddRange(lMesh.GetShaderData(pMaterialMap, lTriangleBuffer.Count / TriangleData.GetMarshalSize()).GetBytes());
		// 			}
		//
		// 			break;
		// 		default:
		// 			lShapeBuffer.AddRange(new ShapeData {
		// 				type = (int)ERaytracedShapeType.BoundingVolume,
		// 				dataTexelIndex = lDataBuffer.Count / Raytracer.TEXEL_SIZE,
		// 				boundMin = lVolume.Min,
		// 				boundMax = lVolume.Max,
		// 			}.GetBytes());
		//
		// 			lDataBuffer.AddRange(new BoundingVolumeData {
		// 				startIndex = lVolumes.Count + lChildOffset,
		// 				count = lVolume.ChildShapes.Count + lVolume.ChildVolumes.Count,
		// 			}.GetBytes());
		// 			break;
		// 	}
		//
		// 	Debug.Assert(
		// 		lVolume.ChildVolumes.Count <= 0 || lVolume.ChildShapes.Count + lVolume.ChildVolumes.Count <= 2,
		// 		$"BVH build assertion failed: total bounding volume child count is equal to {lVolume.ChildShapes.Count + lVolume.ChildVolumes.Count} ({lVolume.ChildShapes.Count} shapes and {lVolume.ChildVolumes.Count} volumes)"
		// 	);
		//
		// 	for (int i = 0; i < lVolume.ChildShapes.Count; i++)
		// 	{
		// 		IRaytracedShape lShape = lVolume.ChildShapes[i];
		//
		// 		switch (lShape.Type)
		// 		{
		// 			case ERaytracedShapeType.BoundingVolume:
		// 			{
		// 				GD.PushError($"A bounding volume was pushed in the childShapes List of BoundingVolume {lCurrentVolume} at index {i}");
		// 				break;
		// 			}
		// 			case ERaytracedShapeType.Sphere:
		// 			{
		// 				if (lShape is not RaytracedSphere lSphere)
		// 					break;
		//
		// 				vec3 lExtent = new vec3(lSphere.Radius * lSphere.Basis.Scale.X);
		// 				lShapeBuffer.AddRange(new ShapeData {
		// 					type = (int)lShape.Type,
		// 					dataTexelIndex = lDataBuffer.Count / Raytracer.TEXEL_SIZE,
		// 					boundMin = fromVariant(lSphere.GlobalPosition) - lExtent,
		// 					boundMax = fromVariant(lSphere.GlobalPosition) + lExtent,
		// 				}.GetBytes());
		//
		// 				lDataBuffer.AddRange(lSphere.GetShaderData(pMaterialMap).GetBytes());
		// 				++lChildOffset;
		// 				break;
		// 			}
		// 			case ERaytracedShapeType.Mesh:
		// 			{
		// 				if (lShape is not RaytracedMesh lMesh)
		// 					break;
		//
		// 				BVHResult lMeshBVH = BuildBVH(lMesh, 0, 0, pMaterialMap, MaxDepth);
		//
		// 				lShapeBuffer.AddRange(new ShapeData {
		// 					type = (int)lShape.Type,
		// 					dataTexelIndex = lDataBuffer.Count / Raytracer.TEXEL_SIZE,
		// 					boundMin = lMesh.Bounds.min,
		// 					boundMax = lMesh.Bounds.max,
		// 				}.GetBytes());
		//
		// 				++lChildOffset;
		// 				break;
		// 			}
		// 			default:
		// 			{
		// 				GD.PushWarning($"Shape type {lShape.Type} is not supported");
		// 				break;
		// 			}
		// 		}
		// 	}
		//
		// 	lVolumes.AddRange(lVolume.ChildVolumes);
		// 	++lCurrentVolume;
		// }
		//
		// return new BVHResult {
		// 	shapeBuffer = lShapeBuffer.ToArray(),
		// 	dataBuffer = lDataBuffer.ToArray(),
		// 	vertexBuffer = lVertexBuffer.ToArray(),
		// 	triangleBuffer = lTriangleBuffer.ToArray(),
		// };
	}

	public static vec3 GetSplitAxis(IBVHVolume pVolume)
	{
		vec3 lAxis = (pVolume.Min + pVolume.Max) * .5f;

		// Mathf.Inf is used because we compare with lessThan. -Mathf.Inf should be used if compared with greaterThan
		if (lAxis.x > lAxis.y && lAxis.x > lAxis.z)
		{
			lAxis = new vec3(lAxis.x, Mathf.Inf, Mathf.Inf);
		}
		else if (lAxis.y > lAxis.z)
		{
			lAxis = new vec3(Mathf.Inf, lAxis.y, Mathf.Inf);
		}
		else
		{
			lAxis = new vec3(Mathf.Inf, Mathf.Inf, lAxis.z);
		}

		return lAxis;
	}
}