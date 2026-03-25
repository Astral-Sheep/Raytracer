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

public class BVHMesh : IBVHVolume
{
	public static int MaxDepth => GodotUtility.GetSetting<int>("rendering/pathtracing/bvh_mesh_depth");

	public int ChildCount { get; }

	public readonly int[] materials = Array.Empty<int>();
	public readonly List<BVHMeshVolume> children = new List<BVHMeshVolume>();

	public readonly CookData cookData = null;
	public readonly ImmutableArray<int> surfaceData = ImmutableArray<int>.Empty;
	public readonly Bounds bounds = new Bounds();
	public readonly mat4 transform = new mat4(1f);

	protected bool split = false;

	public BVHMesh(
		RaytracedMesh pMesh,
		CookData pCookData,
		ImmutableDictionary<Material, int> pMaterialTable,
		Material pDefaultMaterial
	)
	{
		if (pMesh?.Mesh == null || pCookData == null)
		{
			split = true;
			return;
		}

		cookData = pCookData;
		surfaceData = cookData.meshTable.GetValueOrDefault(pMesh.Mesh, new ImmutableArray<int>());
		ChildCount = surfaceData.Length > 1 ? surfaceData[^1] - surfaceData[0] : 0;

		transform = pMesh.GlobalTransform;
		materials = pMesh.Materials.Select(m => pMaterialTable.GetValueNoError(m ?? pDefaultMaterial, -1)).ToArray();

		int lSurfaceCount = pMesh.Mesh.GetSurfaceCount();

		if (lSurfaceCount <= 0)
		{
			bounds = new Bounds(new vec3(0f), new vec3(0f));
			return;
		}

		vec3[] lMinArray = new vec3[lSurfaceCount];
		vec3[] lMaxArray = new vec3[lSurfaceCount];

		Parallel.For(0, surfaceData.Length - 1, i => {
			int lStart = surfaceData[i];
			int lEnd = surfaceData[i + 1];
			int lCount = lEnd - lStart;

			vec3[] lMins = new vec3[lCount];
			vec3[] lMaxs = new vec3[lCount];

			Parallel.For(lStart, lEnd, j => {
				TriangleData lTriangle = cookData.triangleBuffer[j];
				vec3 lV0 = (transform * new vec4(cookData.vertexBuffer[lTriangle.v0].position, 1f)).xyz;
				vec3 lV1 = (transform * new vec4(cookData.vertexBuffer[lTriangle.v1].position, 1f)).xyz;
				vec3 lV2 = (transform * new vec4(cookData.vertexBuffer[lTriangle.v2].position, 1f)).xyz;

				lMins[j - lStart] = min(lV0, min(lV1, lV2));
				lMaxs[j - lStart] = max(lV0, max(lV1, lV2));
			});

			vec3 lMin = new vec3(float.PositiveInfinity);
			vec3 lMax = new vec3(float.NegativeInfinity);

			for (int j = 0; j < lCount; j++)
			{
				lMin = min(lMin, lMins[j]);
				lMax = max(lMax, lMaxs[j]);
			}

			lMinArray[i] = lMin;
			lMaxArray[i] = lMax;
		});

		vec3 lMin = new vec3(float.PositiveInfinity);
		vec3 lMax = new vec3(float.NegativeInfinity);

		for (int i = 0; i < surfaceData.Length - 1; i++)
		{
			lMin = min(lMin, lMinArray[i]);
			lMax = max(lMax, lMaxArray[i]);
		}

		bounds = new Bounds(lMin, lMax);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public Bounds GetBounds()
	{
		return bounds;
	}

	public void Split(int pMaxDepth = -1)
	{
		if (split || cookData == null || children.Count > 0)
			return;

		pMaxDepth = pMaxDepth < 0 ? MaxDepth : pMaxDepth;

		if (pMaxDepth <= 0)
			return;

		// More than one surface
		if (surfaceData.Length > 2)
		{
			SplitInSubmeshes(pMaxDepth);
		}
		else
		{
			SplitInSubvolumes(pMaxDepth, 0);
		}
	}

	private void SplitInSubmeshes(int pMaxDepth)
	{
		for (int i = 0; i < surfaceData.Length - 1; i++)
		{
			int lStart = surfaceData[i];
			int lEnd = surfaceData[i + 1];

			if (surfaceData[i + 1] - surfaceData[i] <= 0)
				continue;

			BVHSubmesh lSubmesh = new BVHSubmesh(cookData, lStart, lEnd, i, materials[i]);
			children.Add(lSubmesh);
		}

		if (children.Count == 1)
		{
			// Only child is a basic volume: splitting brings nothing so we just remove the child and mark the mesh as split
			if (children[0] is not BVHSubmesh lSubmesh)
			{
				children.Clear();
				split = true;
				return;
			}

			// Split the surface as if it was the only one in the mesh
			int lSurfaceIndex = lSubmesh.surfaceIndex;
			materials[0] = materials[lSurfaceIndex];
			SplitInSubvolumes(pMaxDepth, lSurfaceIndex);
		}
		else
		{
			GD.Print("Splitting mesh in submeshes\n");
			Task.WaitAll(children
				.AsParallel()
				.Select(c => Task.Run(() => c.Split(pMaxDepth - 1)))
				.ToArray()
			);
			split = true;
		}
	}

	private void SplitInSubvolumes(int pMaxDepth, int pSurfaceIndex)
	{
		if (ChildCount < 2)
			return;

		(bool lSplittable, vec3 lSplitAxis) = BVHBuilder.GetSplitAxis(this);

		if (!lSplittable)
		{
			split = true;
			return;
		}

		lSplitAxis = BVHBuilder.TransformSplitAxis(lSplitAxis, inverse(transform));
		int lStart = surfaceData[pSurfaceIndex];
		int lEnd = surfaceData[pSurfaceIndex + 1];

		// Acts as Volume0's end as well
		int lVolume1Start = lStart;
		int lVolume1Count = 0;

		for (int i = lStart; i < lEnd; i++)
		{
			TriangleData lTriangle = cookData.triangleBuffer[i];
			vec3 lCenter = (
				cookData.vertexBuffer[lTriangle.v0].position
				+ cookData.vertexBuffer[lTriangle.v1].position
				+ cookData.vertexBuffer[lTriangle.v2].position
			) / 3f;

			if (all(lessThanEqual(lCenter, lSplitAxis)))
			{
				if (lVolume1Count > 0)
				{
					// Swap triangles for contiguity
					TriangleData lSwappedTriangle = cookData.triangleBuffer[lVolume1Start];
					cookData.triangleBuffer[lVolume1Start] = lTriangle;
					cookData.triangleBuffer[i] = lSwappedTriangle;
				}

				++lVolume1Start;
			}
			else
			{
				++lVolume1Count;
			}
		}

		// Volume 0 or volume 1 has all triangles, making the split useless
		if (lVolume1Start == lStart || lVolume1Count == 0)
		{
			split = true;
			return;
		}

		children.Clear();
		children.AddRange(new BVHMeshVolume[] {
			new BVHMeshVolume(cookData, lStart, lVolume1Start),
			new BVHMeshVolume(cookData, lVolume1Start, lVolume1Start + lVolume1Count),
		});
		children[0].Split(pMaxDepth - 1);
		children[1].Split(pMaxDepth - 1);
		// Task.WaitAll(children.Select(c => Task.Run(() => c.Split(pMaxDepth - 1))).ToArray());
		split = true;
	}

	public void SetSplitData(ImmutableArray<BVHMeshVolume> pChildren)
	{
		if (split || pChildren is not { Length: > 0 })
			return;

		children.Clear();
		children.AddRange(pChildren);

		// Generate new submesh with this mesh's material override
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] is not BVHSubmesh lSubmesh || lSubmesh.material == (materials.IndexIsValid(i) ? materials[i] : -1))
				continue;

			BVHSubmesh lLocalSubmesh = new BVHSubmesh(
				cookData,
				lSubmesh.start,
				lSubmesh.end,
				i,
				materials.IndexIsValid(i) ? materials[i] : -1
			);
			lLocalSubmesh.SetSplitData(lSubmesh.Children);
			children[i] = lLocalSubmesh;
		}

		split = true;
	}

	/// <summary>
	/// Warning: if there is more than one surface, the result is always -1
	/// </summary>
	public float GetSplitCost(vec3 pAxis)
	{
		// If there is more than 1 surface (or no surface at all), we don't care about the split axis 
		if (surfaceData is not { Length: 2 })
		{
			return float.PositiveInfinity;
		}

		int lStart = surfaceData[0];
		int lEnd = surfaceData[1];

		// To local space
		pAxis = BVHBuilder.TransformSplitAxis(pAxis, inverse(transform));

		(int count, vec3 min, vec3 max) lVolume0 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));
		(int count, vec3 min, vec3 max) lVolume1 = (0, new vec3(float.PositiveInfinity), new vec3(float.NegativeInfinity));

		for (int i = lStart; i < lEnd; i++)
		{
			TriangleData lTriangle = cookData.triangleBuffer[i];

			if (all(lessThanEqual(lTriangle.bounds.Center, pAxis)))
			{
				++lVolume0.count;
				lVolume0.min = min(lVolume0.min, lTriangle.bounds.Min);
				lVolume0.max = max(lVolume0.max, lTriangle.bounds.Max);
			}
			else
			{
				++lVolume1.count;
				lVolume1.min = min(lVolume1.min, lTriangle.bounds.Min);
				lVolume1.max = max(lVolume1.max, lTriangle.bounds.Max);
			}
		}

		Bounds lVolume0Bounds = lVolume0.count <= 0
			? new Bounds(new vec3(0f), new vec3(0f))
			: new Bounds(lVolume0.min, lVolume0.max);

		Bounds lVolume1Bounds = lVolume1.count <= 0
			? new Bounds(new vec3(0f), new vec3(0f))
			: new Bounds(lVolume1.min, lVolume1.max);

		return BVHBuilder.GetVolumeCost(lVolume0.count, lVolume0Bounds.Extent * 2f)
			 + BVHBuilder.GetVolumeCost(lVolume1.count, lVolume1Bounds.Extent * 2f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeData GetShaderShape(int pTexelIndex)
	{
		return new ShapeData {
			// Leaf if no child volumes
			type = (int)(children.Count == 0 ? ERaytracedShapeType.LeafMesh : ERaytracedShapeType.Mesh),
			dataTexelIndex = pTexelIndex,
			boundMin = bounds.Min,
			boundMax = bounds.Max,
		};
	}

	public MeshData GetShaderData(List<IBounded> pObjects, out bool pCanAddChildren)
	{
		pCanAddChildren = false;

		// Invalid surface data: start index with no end or just no indices
		if (surfaceData.Length < 2)
		{
			return new MeshData {
				transform = transform,
				startIndex = -1,
				count = 0,
				materialIndex = -1,
			};
		}

		// Leaf mesh
		if (children.Count <= 0)
		{
			return new MeshData {
				transform = transform,
				startIndex = surfaceData[0],
				count = surfaceData[^1] - surfaceData[0],
				materialIndex = materials[0]
			};
		}
		else
		{
			int lChildIndex = pObjects.IndexOf(children[0]);
			pCanAddChildren = lChildIndex < 0;

			return new MeshData {
				transform = transform,
				startIndex = lChildIndex >= 0 ? lChildIndex : pObjects.Count,
				count = children.Count,
				// If submeshes, material is -2
				materialIndex = children[0] is BVHSubmesh ? -2 : materials[0]
			};
		}
	}

	public virtual string ToString(int pDepth)
	{
		if (children.Count > 0)
		{
			string lString = $"{GetType().Name}";

			for (int i = 0; i < children.Count; i++)
			{
				lString += $"\n{new string('-', pDepth * 5)}---> {children[i].ToString(pDepth + 1)}";
			}

			return lString;
		}
		else
		{
			return $"{GetType().Name}: {ChildCount} triangles";
		}
	}
}