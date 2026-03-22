using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

public static class BVHBuilder
{
	public static int MaxDepth => GodotUtility.GetSetting<int>("rendering/pathtracing/bvh_global_depth");
	public static int SplitTests => GodotUtility.GetSetting<int>("rendering/pathtracing/bvh_split_tests");

	public static IBVHVolume GenerateBVH(IRaytracedShape[] pShapes)
	{
		if (pShapes is not { Length: > 0 })
		{
			GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] No shapes to order. Returning empty hierarchy");
			return null;
		}

		BVHGlobalVolume lRoot = new BVHGlobalVolume(pShapes);
		lRoot.Split(MaxDepth);

		if (lRoot.childVolumes.Count == 1 && lRoot.childShapes.Count <= 0)
		{
			return lRoot.childVolumes[0];
		}

		return lRoot;
	}

	public static BVHResult BuildBVH(IBVHVolume pRoot, Dictionary<Material, int> pMaterialMap, Material pDefaultMaterial)
	{
		if (pRoot == null)
		{
			return BVHResult.Empty;
		}

		switch (pRoot)
		{
			case BVHGlobalVolume lVolume:
			{
				if (lVolume.childVolumes.Count > 0 || lVolume.childShapes.Count != 1)
					break;

				GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] Only 1 shape ordered. Returning shape without hierarchy");
				IRaytracedShape lShape = lVolume.childShapes[0];
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
			case BVHMeshVolume or BVHSubmesh:
				GD.PushError($"Ill formed BVH: root is {pRoot.GetType().Name}");
				return BVHResult.Empty;
			default:
				break;
		}

		// Buffers
		List<byte> lShapes = new List<byte>();
		List<byte> lData = new List<byte>();
		List<byte> lVertices = new List<byte>();
		List<byte> lTriangles = new List<byte>();

		// Hierarchy
		List<object> lObjectsToAdd = new List<object>();
		Dictionary<Mesh, (int start, int count)> lBuiltMeshes = new Dictionary<Mesh, (int start, int count)>();
		lObjectsToAdd.Add(pRoot);

		int lCurrentObject = 0;

		while (lCurrentObject < lObjectsToAdd.Count)
		{
			object lObject = lObjectsToAdd[lCurrentObject];

			switch (lObject)
			{
				case BVHGlobalVolume lGlobalVolume:
				{
					lShapes.AddRange(lGlobalVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lGlobalVolume.GetShaderData(lObjectsToAdd.Count).GetBytes());
					lObjectsToAdd.AddRange(lGlobalVolume.childShapes);
					lObjectsToAdd.AddRange(lGlobalVolume.childVolumes);
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
						lObjectsToAdd.Count,
						(int)(lTriangles.Count / TriangleData.GetTexelSize()),
						pMaterialMap,
						pDefaultMaterial
					).GetBytes());
					lObjectsToAdd.AddRange(lMesh.children);
					break;
				}
				case BVHSubmesh lSubmesh:
				{
					lShapes.AddRange(lSubmesh.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lSubmesh.GetShaderData(lObjectsToAdd.Count, pMaterialMap, pDefaultMaterial).GetBytes());
					lObjectsToAdd.AddRange(lSubmesh.children);
					break;
				}
				case BVHMeshVolume lMeshVolume:
				{
					lShapes.AddRange(lMeshVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lMeshVolume.GetShaderData(lObjectsToAdd.Count).GetBytes());
					lObjectsToAdd.AddRange(lMeshVolume.children);
					break;
				}
				case IRaytracedShape lShape:
				{
					lShapes.AddRange(lShape.GetShapeData(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());

					switch (lShape)
					{
						case RaytracedSphere lSphere:
							lData.AddRange(lSphere.GetShaderData(pMaterialMap).GetBytes());
							break;
						case RaytracedCylinder lCylinder:
							lData.AddRange(lCylinder.GetShaderData(pMaterialMap).GetBytes());
							break;
						case RaytracedTorus lTorus:
							lData.AddRange(lTorus.GetShaderData(pMaterialMap).GetBytes());
							break;
						case RaytracedMesh:
							GD.PushError("Raytraced mesh handled as shape instead of volume");
							break;
						default:
							GD.PushError($"Unhandled raytraced shape found in hierarchy: {lShape.GetType().Name}");
							break;
					}

					break;
				}
				default:
					GD.PushError($"Unhandled object found in hierarchy: {lObject?.GetType().Name}");
					break;
			}

			++lCurrentObject;
		}

		return new BVHResult {
			shapeBuffer = lShapes.ToArray(),
			dataBuffer = lData.ToArray(),
			vertexBuffer = lVertices.ToArray(),
			triangleBuffer = lTriangles.ToArray(),
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVHResult BuildBVH(IRaytracedShape[] pShapes, Dictionary<Material, int> pMaterialMap, Material pDefaultMaterial)
	{
		return BuildBVH(GenerateBVH(pShapes), pMaterialMap, pDefaultMaterial);
	}

	public static (bool splittable, vec3 axis) GetSplitAxis(IBVHVolume pVolume)
	{
		int lSplitTests = SplitTests;

		vec3 lStep = (pVolume.Max - pVolume.Min) / (lSplitTests + 1f);
		vec3 lStart = pVolume.Min + lStep;
		vec3 lSize = abs(pVolume.Max - pVolume.Min);

		// vec3 lBestAxis = new vec3(float.NaN);
		float lBestScore = lSize.x * lSize.y * lSize.z * pVolume.ChildCount;

		// DateTime lTimeStart = DateTime.UtcNow;
		Task<(float, vec3)[]> lTests = Task.WhenAll(
			Task.Run(() => {
				(float score, vec3 axis)[] lScores = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = lStart + new vec3(lStep.x * i, Mathf.Inf, Mathf.Inf);
					lScores[i] = (pVolume.GetSplitScore(lAxis), lAxis);
				});
				return lScores.AsParallel().MinBy(s => s.score);
			}),
			Task.Run(() => {
				(float score, vec3 axis)[] lScores = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = lStart + new vec3(Mathf.Inf, lStep.y * i, Mathf.Inf);
					lScores[i] = (pVolume.GetSplitScore(lAxis), lAxis);
				});
				return lScores.AsParallel().MinBy(s => s.score);
			}),
			Task.Run(() => {
				(float score, vec3 axis)[] lScores = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = lStart + new vec3(Mathf.Inf, Mathf.Inf, lStep.z * i);
					lScores[i] = (pVolume.GetSplitScore(lAxis), lAxis);
				});
				return lScores.AsParallel().MinBy(s => s.score);
			})
		);
		lTests.Wait();
		(float score, vec3 axis) = lTests.Result.AsParallel().MinBy(s => s.Item1);
		return (score < lBestScore, axis);
		// TimeSpan lMultiDuration = DateTime.UtcNow - lTimeStart;
		//
		// lTimeStart = DateTime.UtcNow;
		// for (int i = 0; i < lSplitTests; i++)
		// {
		// 	vec3 lAxis = lStart + new vec3(lStep.x * i, Mathf.Inf, Mathf.Inf);
		// 	float lScore = pVolume.GetSplitScore(lAxis);
		//
		// 	if (lScore < lBestScore)
		// 	{
		// 		lBestAxis = lAxis;
		// 		lBestScore = lScore;
		// 	}
		// }
		//
		// for (int i = 0; i < lSplitTests; i++)
		// {
		// 	vec3 lAxis = lStart + new vec3(Mathf.Inf, lStep.y * i, Mathf.Inf);
		// 	float lScore = pVolume.GetSplitScore(lAxis);
		//
		// 	if (lScore < lBestScore)
		// 	{
		// 		lBestAxis = lAxis;
		// 		lBestScore = lScore;
		// 	}
		// }
		//
		// for (int i = 0; i < lSplitTests; i++)
		// {
		// 	vec3 lAxis = lStart + new vec3(Mathf.Inf, Mathf.Inf, lStep.z * i);
		// 	float lScore = pVolume.GetSplitScore(lAxis);
		//
		// 	if (lScore < lBestScore)
		// 	{
		// 		lBestAxis = lAxis;
		// 		lBestScore = lScore;
		// 	}
		// }
		// TimeSpan lSingleDuration = DateTime.UtcNow - lTimeStart;

		// GD.PrintRich(
		// 	$"[color=#{(lSingleDuration < lMultiDuration ? "33e88e" : "eb4034")}]Main thread score: {lBestScore} with axis ({lBestAxis.x}, {lBestAxis.y}, {lBestAxis.z}) in {lSingleDuration}[/color]\n" +
		// 	$"[color=#{(lSingleDuration >= lMultiDuration ? "33e88e" : "eb4034")}]Multithreaded score: {score} with axis ({axis.x}, {axis.y}, {axis.z}) in {lMultiDuration}[/color]\n"
		// );
		// return (score < lBestScore || !float.IsNaN(lBestAxis.x), score < lBestScore ? axis : lBestAxis);
		// return (!float.IsNaN(lBestAxis.x), lBestAxis);
	}
}