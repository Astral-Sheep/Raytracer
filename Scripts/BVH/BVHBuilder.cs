using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BVHResult BuildBVH(IRaytracedShape[] pShapes, ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		(IBVHVolume lRoot, CookData lData) = GenerateBVH(pShapes, pMaterialTable, pDefaultMaterial);
		return BuildBVH(lRoot, lData);
	}

	public static BVHResult BuildBVH(IBVHVolume pRoot, CookData pMeshCookData)
	{
		if (pRoot == null)
		{
			return BVHResult.Empty;
		}

		GD.Print("BVH build started");

		switch (pRoot)
		{
			case BVHGlobalVolume lVolume:
			{
				if (lVolume.childVolumes.Count > 0 || lVolume.children.Count != 1)
					break;

				if (lVolume.children.meshes.Count > 0)
				{
					pRoot = lVolume.children.meshes[0];
					break;
				}

				GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] Only 1 shape ordered. Returning shape without hierarchy");
				IPureShape lShape = lVolume.children.shapes[0];
				(ShapeData shape, IShaderData data) = lShape.GetShaderData(0);

				return new BVHResult {
					shapeBuffer = shape.GetBytes(),
					dataBuffer = data.GetBytes(),
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

		// Hierarchy
		List<IBounded> lObjectsToAdd = new List<IBounded>();
		lObjectsToAdd.Add(pRoot);
		int lCurrentObject = 0;

		while (lCurrentObject < lObjectsToAdd.Count)
		{
			IBounded lObject = lObjectsToAdd[lCurrentObject];

			switch (lObject)
			{
				case BVHGlobalVolume lGlobalVolume:
				{
					lShapes.AddRange(lGlobalVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lGlobalVolume.GetShaderData(lObjectsToAdd.Count).GetBytes());
					lObjectsToAdd.AddRange(lGlobalVolume.children.shapes);
					lObjectsToAdd.AddRange(lGlobalVolume.children.meshes);
					lObjectsToAdd.AddRange(lGlobalVolume.childVolumes);
					break;
				}
				case BVHMesh lMesh:
				{
					lShapes.AddRange(lMesh.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lMesh.GetShaderData(lObjectsToAdd, out bool lCanAddChildren).GetBytes());

					if (lCanAddChildren)
					{
						lObjectsToAdd.AddRange(lMesh.children);
					}

					break;
				}
				case BVHSubmesh lSubmesh:
				{
					lShapes.AddRange(lSubmesh.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lSubmesh.GetShaderData(lObjectsToAdd, out bool lCanAddChildren).GetBytes());

					if (lCanAddChildren)
					{
						lObjectsToAdd.AddRange(lSubmesh.Children);
					}
					break;
				}
				case BVHMeshVolume lMeshVolume:
				{
					lShapes.AddRange(lMeshVolume.GetShaderShape(lData.Count / Raytracer.TEXEL_SIZE).GetBytes());
					lData.AddRange(lMeshVolume.GetShaderData(lObjectsToAdd.Count).GetBytes());
					lObjectsToAdd.AddRange(lMeshVolume.Children);
					break;
				}
				case IPureShape lShape:
				{
					(ShapeData shape, IShaderData data) = lShape.GetShaderData(lData.Count / Raytracer.TEXEL_SIZE);
					lShapes.AddRange(shape.GetBytes());
					lData.AddRange(data.GetBytes());
					break;
				}
				default:
					GD.PushError($"Unhandled object found in hierarchy: {lObject?.GetType().Name}");
					break;
			}

			++lCurrentObject;
		}

		(byte[] lVertexBuffer, byte[] lTriangleBuffer) = pMeshCookData.Build();

		return new BVHResult {
			shapeBuffer = lShapes.ToArray(),
			dataBuffer = lData.ToArray(),
			vertexBuffer = lVertexBuffer,
			triangleBuffer = lTriangleBuffer,
		};
	}

	public static (IBVHVolume root, CookData data) GenerateBVH(IRaytracedShape[] pShapes, ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		if (pShapes is not { Length: > 0 })
		{
			GD.PrintRich("[color=#34EBEB][lb]BVH[rb][/color] No shapes to order. Returning empty hierarchy");
			return (null, null);
		}

		GD.Print("BVH generation started");
		IPureShape[] lPrimitives = pShapes
			.Where(s => IRaytracedShape.primitiveTypes.Contains(s.GetType()))
			.Select(s => s.AsPureShape(pMaterialTable, pDefaultMaterial))
			.ToArray();
		(BVHMesh[] lMeshes, CookData lCookData) = GenerateMeshes(
			pShapes.OfType<RaytracedMesh>().ToArray(),
			pMaterialTable,
			pDefaultMaterial
		);

		BVHGlobalVolume lRoot = new BVHGlobalVolume(lPrimitives, lMeshes);
		lRoot.Split(MaxDepth);

		if (lRoot.childVolumes.Count == 1 && lRoot.children.Count <= 0)
		{
			return (lRoot.childVolumes[0], lCookData);
		}

		return (lRoot, lCookData);
	}

	private static (BVHMesh[], CookData) GenerateMeshes(RaytracedMesh[] pMeshes, ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		Dictionary<Mesh, RaytracedMesh> lModelsTable = new Dictionary<Mesh, RaytracedMesh>();
		List<RaytracedMesh> lInstances = new List<RaytracedMesh>();

		for (int i = 0; i < pMeshes.Length; i++)
		{
			RaytracedMesh lMesh = pMeshes[i];

			if (lMesh?.Mesh == null)
				continue;

			if (lModelsTable.TryAdd(lMesh.Mesh, lMesh))
				continue;

			lInstances.Add(lMesh);
		}

		RaytracedMesh[] lModels = lModelsTable.Values.ToArray();
		CookData lCookData = CookMeshes(lModels);
		Dictionary<Mesh, ImmutableArray<BVHMeshVolume>> lSplitData = GenerateMeshModels(
			lModels,
			lCookData,
			pMaterialTable,
			pDefaultMaterial,
			out BVHMesh[] lBuiltModels
		);

		BVHMesh[] lBuiltMeshes = new BVHMesh[lBuiltModels.Length + lInstances.Count];
		Array.Copy(lBuiltModels, 0, lBuiltMeshes, 0, lBuiltModels.Length);

		for (int i = lBuiltModels.Length; i < lBuiltMeshes.Length; i++)
		{
			RaytracedMesh lMesh = lInstances[i - lBuiltModels.Length];
			BVHMesh lBuiltMesh = new BVHMesh(lMesh, lCookData, pMaterialTable, pDefaultMaterial);
			lBuiltMesh.SetSplitData(lSplitData[lMesh.Mesh]);
			lBuiltMeshes[i] = lBuiltMesh;
		}

		return (lBuiltMeshes, lCookData);
	}

	/// <summary>
	/// /!\ Warning /!\<br/>
	/// This method is used to build all <see cref="Mesh"/> resources into the BVH, therefore it is expected
	/// to not have 2 <see cref="RaytracedMesh"/> with the same <see cref="Mesh"/> reference to avoid duplicates.<br/>
	/// The verification is expected to be done outside of this method.
	/// </summary>
	private static Dictionary<Mesh, ImmutableArray<BVHMeshVolume>> GenerateMeshModels(
		RaytracedMesh[] pMeshes,
		CookData pCookData,
		ImmutableDictionary<Material, int> pMaterialTable,
		Material pDefaultMaterial,
		out BVHMesh[] pBuiltMeshes
	)
	{
		BVHMesh[] lBuiltMeshes = new BVHMesh[pMeshes.Length];

		for (int i = 0; i < pMeshes.Length; i++)
		{
			lBuiltMeshes[i] = new BVHMesh(pMeshes[i], pCookData, pMaterialTable, pDefaultMaterial);
		}

		Parallel.For(0, lBuiltMeshes.Length, i => {
			lBuiltMeshes[i].Split();
		});

		Dictionary<Mesh, ImmutableArray<BVHMeshVolume>> lSplitData = new Dictionary<Mesh, ImmutableArray<BVHMeshVolume>>(lBuiltMeshes.Length);

		for (int i = 0; i < lBuiltMeshes.Length; i++)
		{
			lSplitData.Add(pMeshes[i].Mesh, lBuiltMeshes[i].children.ToImmutableArray());
		}

		pBuiltMeshes = lBuiltMeshes;
		return lSplitData;
	}

	private static CookData CookMeshes(RaytracedMesh[] pMeshes)
	{
		Dictionary<Mesh, int[]> lCookedMeshes = new Dictionary<Mesh, int[]>();
		List<VertexData> lVertexBuffer = new List<VertexData>();
		List<TriangleData> lTriangleBuffer = new List<TriangleData>();

		for (int i = 0; i < pMeshes.Length; i++)
		{
			RaytracedMesh lMesh = pMeshes[i];

			if (lMesh?.Mesh == null || lCookedMeshes.ContainsKey(lMesh.Mesh))
				continue;

			(TriangleData[] lTriangleData, int[] lSurfaceIndices) = lMesh.GetTriangles(lVertexBuffer.Count, lTriangleBuffer.Count);

			lVertexBuffer.AddRange(lMesh.GetVertices());
			lTriangleBuffer.AddRange(lTriangleData);
			lCookedMeshes.Add(lMesh.Mesh, lSurfaceIndices);
		}

		return new CookData(lVertexBuffer.ToArray(), lTriangleBuffer.ToArray(), lCookedMeshes);
	}

	public static (bool splittable, vec3 axis) GetSplitAxis(IBVHVolume pVolume)
	{
		int lSplitTests = SplitTests;

		Bounds lVolumeBounds = pVolume.GetBounds();
		vec3 lSize = lVolumeBounds.Extent * 2f;
		vec3 lStep = lSize / (lSplitTests + 1f);
		vec3 lStart = lVolumeBounds.Min + lStep;

		float lVolumeCost = GetVolumeCost(pVolume.ChildCount, lSize);

		Task<(float, vec3)[]> lTests = Task.WhenAll(
			Task.Run(() => {
				(float cost, vec3 axis)[] lCosts = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = new vec3(lStart.x + lStep.x * i, float.PositiveInfinity, float.PositiveInfinity);
					lCosts[i] = (pVolume.GetSplitCost(lAxis), lAxis);
				});
				return lCosts.AsParallel().MinBy(s => s.cost);
			}),
			Task.Run(() => {
				(float cost, vec3 axis)[] lCosts = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = new vec3(float.PositiveInfinity, lStart.y + lStep.y * i, float.PositiveInfinity);
					lCosts[i] = (pVolume.GetSplitCost(lAxis), lAxis);
				});
				return lCosts.AsParallel().MinBy(s => s.cost);
			}),
			Task.Run(() => {
				(float cost, vec3 axis)[] lCosts = new (float, vec3)[lSplitTests];

				Parallel.For(0, lSplitTests, i => {
					vec3 lAxis = new vec3(float.PositiveInfinity, float.PositiveInfinity, lStart.z + lStep.z * i);
					lCosts[i] = (pVolume.GetSplitCost(lAxis), lAxis);
				});
				return lCosts.AsParallel().MinBy(s => s.cost);
			})
		);
		lTests.Wait();

		(float cost, vec3 axis) = lTests.Result.AsParallel().MinBy(s => s.Item1);
		return (cost < lVolumeCost, axis);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public static vec3 TransformSplitAxis(vec3 pSplitAxis, mat4 pTransform)
	{
		vec3 lSanitizedAxis = new vec3(
			float.IsFinite(pSplitAxis.x) ? pSplitAxis.x : pTransform[3, 0],
			float.IsFinite(pSplitAxis.y) ? pSplitAxis.y : pTransform[3, 1],
			float.IsFinite(pSplitAxis.z) ? pSplitAxis.z : pTransform[3, 2]
		);
		lSanitizedAxis = (pTransform * new vec4(lSanitizedAxis, 1f)).xyz;

		return new vec3(
			float.IsFinite(pSplitAxis.x) ? lSanitizedAxis.x : float.PositiveInfinity,
			float.IsFinite(pSplitAxis.y) ? lSanitizedAxis.y : float.PositiveInfinity,
			float.IsFinite(pSplitAxis.z) ? lSanitizedAxis.z : float.PositiveInfinity
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public static float GetVolumeCost(int pChildCount, vec3 pSize)
	{
		// return pSize.x * pSize.y * pSize.z * pChildCount;
		return (pSize.x * (pSize.y + pSize.z) + pSize.y * pSize.z) * pChildCount;
	}
}