using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class RaytracedTorus : CsgTorus3D, IRaytracedShape
{
	public const int TORUS_DATA_SIZE = 5;
	public const float INV_CYLINDER_BYTE_SIZE = 1f / (TORUS_DATA_SIZE * Raytracer.TEXEL_SIZE);

	public ERaytracedShapeType Type => ERaytracedShapeType.Torus;

	public ShapeBounds Bounds
	{
		get
		{
			mat4 lLocalToWorld = GlobalTransform;
			float lHalfHeight = OuterRadius - InnerRadius * .5f;
			vec3 lMin = new vec3(Mathf.Inf);
			vec3 lMax = new vec3(-Mathf.Inf);

			for (int i = 0; i < 8; i++)
			{
				vec3 lCorner = (lLocalToWorld * new vec4(
					OuterRadius * (i % 2 == 0 ? 1 : -1),
					lHalfHeight * (i % 4 < 2 ? 1 : -1),
					OuterRadius * (i < 4 ? 1 : -1),
					0f
				)).xyz;
				lMin = min(lCorner, lMin);
				lMax = max(lCorner, lMax);
			}

			return new ShapeBounds {
				min = lMin,
				max = lMax,
			};
		}
	}

	public Material[] Materials
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Material[] { materialOverride ?? Material };
		}
	}

	[Export] public bool Trace { get; set; }
	[Export] protected RaytracedMaterial materialOverride;
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
		ShapeBounds lBounds = Bounds;
		return new ShapeData {
			type = (int)ERaytracedShapeType.Cylinder,
			dataTexelIndex = pTexelIndex,
			boundMin = lBounds.min,
			boundMax = lBounds.max,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TorusData GetShaderData(Dictionary<Material, int> pMaterialMap)
	{
		return new TorusData {
			innerRadius = InnerRadius,
			outerRadius = OuterRadius,
			transform = GlobalTransform,
			materialIndex = pMaterialMap.GetValueNoError(Materials[0] ?? raytracer?.DefaultObjectMaterial, -1),
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