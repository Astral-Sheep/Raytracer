using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	public vec3 Min => Bounds.min;
	public vec3 Max => Bounds.max;

	public ShapeBounds Bounds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Aabb lBounds = GetAabb();
			return new ShapeBounds {
				min = fromVariant(ToGlobal(lBounds.Position)),
				max = fromVariant(ToGlobal(lBounds.End)),
			};
		}
	}

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
	public ShapeData GetShapeData(int pTexelIndex)
	{
		return new ShapeData {
			type = (int)ERaytracedShapeType.Mesh,
			dataTexelIndex = pTexelIndex,
			boundMin = Bounds.min,
			boundMax = Bounds.max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MeshData GetShaderData(Dictionary<Material, int> pMaterialMap, int pTriangleOffset = 0)
	{
		return new MeshData {
			startIndex = 0,
			count = 0,
			transform = new mat4(1f),
			materialIndex = -1,
		};
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
}