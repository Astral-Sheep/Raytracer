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
			GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] No shapes to order. Returning empty buffers");
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

		if (lRoot.childVolumes.Count == 0 && lRoot.childShapes.Count == 1)
		{
			GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] Only 1 shape ordered. Returning shape without hierarchy");
			IRaytracedShape lShape = lRoot.childShapes[0];
			return new BVHResult {
				shapeBuffer = lShape.GetShapeData(0).GetBytes(),
				dataBuffer = lShape switch {
					RaytracedSphere lSphere => lSphere.GetShaderData(pMaterialMap).GetBytes(),
					_ => Array.Empty<byte>(),
				},
				vertexBuffer = Array.Empty<byte>(),
				triangleBuffer = Array.Empty<byte>(),
			};
		}

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
					lData.AddRange(lGlobalVolume.GetShaderData(lVolumes.Count + lChildOffset - 1).GetBytes());

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

		GD.PrintRich($"[color=#34EBEB][lb]BVH[rb][/color] Ordered {pShapes.Length} shapes into {lVolumes.Count} volumes and {lChildOffset} primitives");

		return new BVHResult {
			shapeBuffer = lShapes.ToArray(),
			dataBuffer = lData.ToArray(),
			vertexBuffer = lVertices.ToArray(),
			triangleBuffer = lTriangles.ToArray(),
		};
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