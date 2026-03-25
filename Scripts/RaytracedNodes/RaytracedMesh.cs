using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Astral.Tools;
using Godot;

using GArray = Godot.Collections.Array;

namespace Astral.Raytracer;

[GlobalClass, Tool]
public partial class RaytracedMesh : MeshInstance3D, IRaytracedShape
{
	public const int MESH_DATA_SIZE = 5;
	public const int VERTEX_DATA_SIZE = 2;
	public const float TRIANGLE_DATA_SIZE = .75f;

	public const float INV_MESH_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * MESH_DATA_SIZE);
	public const float INV_VERTEX_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * VERTEX_DATA_SIZE);
	public const float INV_TRIANGLE_BYTE_SIZE = 1f / (Raytracer.TEXEL_SIZE * TRIANGLE_DATA_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Mesh;

	public Material[] Materials
	{
		get
		{
			if (Mesh == null)
				return [];

			Material[] lMaterials = new Material[Mesh.GetSurfaceCount()];

			for (int i = 0; i < lMaterials.Length; i++)
			{
				if (materialOverrides.Length > i && materialOverrides[i] != null)
				{
					lMaterials[i] = materialOverrides[i];
				}
				else
				{
					lMaterials[i] = GetSurfaceOverrideMaterial(i) ?? Mesh.SurfaceGetMaterial(i);
				}
			}

			return lMaterials;
		}
	}

	[Export] public bool Trace { get; set; }
	[Export] protected Material[] materialOverrides = Array.Empty<Material>();
	[Export] protected Raytracer raytracer;

	[ExportToolButton("Add to Raytracer")]
	protected Callable AddButton => Callable.From(AddToRaytracer);

	[ExportToolButton("Remove from Raytracer")]
	protected Callable RemoveButton => Callable.From(RemoveFromRaytracer);

	public override void _Ready()
	{
		base._Ready();
		AddToRaytracer();
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		if (!IsNodeReady())
			return;

		AddToRaytracer();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		RemoveFromRaytracer();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void AddToRaytracer()
	{
		this.AddShapeToRaytracer(ref raytracer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void RemoveFromRaytracer()
	{
		this.RemoveShapeFromRaytracer(raytracer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds GetBounds()
	{
		Aabb lBounds = GetAabb();
		return new Bounds(fromVariant(ToGlobal(lBounds.Position)), fromVariant(ToGlobal(lBounds.End)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	IPureShape IRaytracedShape.AsPureShape(ImmutableDictionary<Material, int> pMaterialTable, Material pDefaultMaterial)
	{
		return null;
	}

	public VertexData[] GetVertices()
	{
		if (Mesh == null || Mesh.GetSurfaceCount() <= 0)
		{
			return Array.Empty<VertexData>();
		}

		List<VertexData> lVertices = new List<VertexData>();

		for (int i = 0; i < Mesh.GetSurfaceCount(); i++)
		{
			GArray lSurface = Mesh.SurfaceGetArrays(i);
			GArray lSurfVertices = lSurface[(int)Mesh.ArrayType.Vertex].As<GArray>();
			GArray lSurfNormals = lSurface[(int)Mesh.ArrayType.Normal].As<GArray>();
			GArray lSurfUV = lSurface[(int)Mesh.ArrayType.TexUV].As<GArray>();
			VertexData[] lArray = new VertexData[lSurfVertices.Count];

			Parallel.For(0, lArray.Length, j => {
				lArray[j] = new VertexData {
					position = fromVariant(lSurfVertices[j].As<Vector3>()),
					normal = lSurfNormals.IndexIsValid(j) ? fromVariant(lSurfNormals[j].As<Vector3>()) : new vec3(0f),
					uv = lSurfUV.IndexIsValid(j) ? fromVariant(lSurfUV[j].As<Vector2>()) : new vec2(0f)
				};
			});

			lVertices.AddRange(lArray);
		}

		return lVertices.ToArray();
	}

	public (TriangleData[], int[]) GetTriangles(int pVertexOffset, int pTriangleOffset)
	{
		if (Mesh == null || Mesh.GetSurfaceCount() <= 0)
		{
			return (Array.Empty<TriangleData>(), Array.Empty<int>());
		}

		List<TriangleData> lTriangles = new List<TriangleData>();
		int[] lSurfaceIndices = new int[Mesh.GetSurfaceCount() + 1];

		for (int i = 0; i < Mesh.GetSurfaceCount(); i++)
		{
			GArray lSurface = Mesh.SurfaceGetArrays(i);
			GArray lSurfVertices = lSurface[(int)Mesh.ArrayType.Vertex].As<GArray>();
			GArray lSurfTriangles = lSurface[(int)Mesh.ArrayType.Index].As<GArray>();
			TriangleData[] lArray = new TriangleData[lSurfTriangles.Count / 3];

			lSurfaceIndices[i] = pTriangleOffset + lTriangles.Count;
			Parallel.For(0, lArray.Length, j => {
				int lT0 = lSurfTriangles[j * 3].As<int>();
				int lT1 = lSurfTriangles[j * 3 + 1].As<int>();
				int lT2 = lSurfTriangles[j * 3 + 2].As<int>();

				vec3 lV0 = fromVariant(lSurfVertices[lT0].As<Vector3>());
				vec3 lV1 = fromVariant(lSurfVertices[lT1].As<Vector3>());
				vec3 lV2 = fromVariant(lSurfVertices[lT2].As<Vector3>());

				lArray[j] = new TriangleData {
					v0 = lT0 + pVertexOffset,
					v1 = lT1 + pVertexOffset,
					v2 = lT2 + pVertexOffset,
					bounds = new Bounds(min(lV0, min(lV1, lV2)), max(lV0, max(lV1, lV2))),
				};
			});

			lTriangles.AddRange(lArray);
		}

		lSurfaceIndices[^1] = pTriangleOffset + lTriangles.Count;
		return (lTriangles.ToArray(), lSurfaceIndices);
	}
}