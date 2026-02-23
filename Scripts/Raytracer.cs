using System.Linq;
using System.Runtime.CompilerServices;
using Astral.Tools;
using Godot;
using Godot.Collections;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class Raytracer : PostProcessLayer
{
	private struct SphereData
	{
		public Array<Vector3> centers;
		public Array<float> radiuses;
		public Array<Color> colors;
		public Array<Color> emissives;
		public Array<float> emissiveIntensities;
		public int count;
		public bool updateRequested;
	}

	private struct BoxData
	{
		public Array<Vector3> centers;
		public Array<Vector3> extents;
		public Array<Color> colors;
		public Array<Color> emissives;
		public Array<float> emissiveIntensities;
		public int count;
		public bool updateRequested;
	}

	public const int MAX_SHAPES = 16;
	public const string SCENE_PATH = "Main/Camera/Raytracer";

	private const string PLANE_SIZE = "plane_size";
	private const string NEAR_CLIP_PLANE = "near_clip_plane";
	private const string LOCAL_TO_WORLD_MATRIX = "local_to_world_matrix";
	private const string FRAME_COUNT = "frame_count";

	[ExportGroup("Editor debug")]
	[Export] protected bool print = false;
	[Export] protected bool printOnce = false;

	[ExportGroup("")]
	[Export] protected ShaderMaterial material;
	[Export] protected RaytracedMaterial defaultObjectMaterial;

	[ExportToolButton("Clear shapes")]
	protected Callable ClearShapes => Callable.From(() => {
		System.Array.Clear(spheres);
		sphereSlotIndex = 0;

		System.Array.Clear(boxes);
		boxSlotIndex = 0;

		System.Array.Clear(meshes);
		meshSlotIndex = 0;
	});

	[ExportToolButton("Reset frame count")]
	protected Callable ResetFrameCount => Callable.From(() => frameCount = 0);

	protected RaytracedSphere[] spheres = new RaytracedSphere[MAX_SHAPES];
	protected int sphereSlotIndex = 0;
	private SphereData sphereData = new SphereData {
		centers = new Array<Vector3>(new Vector3[MAX_SHAPES]),
		radiuses = new Array<float>(new float[MAX_SHAPES]),
		colors = new Array<Color>(new Color[MAX_SHAPES]),
		emissives = new Array<Color>(new Color[MAX_SHAPES]),
		emissiveIntensities = new Array<float>(new float[MAX_SHAPES]),
		count = 0,
	};

	protected RaytracedBox[] boxes = new RaytracedBox[MAX_SHAPES];
	protected int boxSlotIndex = 0;
	private BoxData boxData = new BoxData {
		centers = new Array<Vector3>(new Vector3[MAX_SHAPES]),
		extents = new Array<Vector3>(new Vector3[MAX_SHAPES]),
		colors = new Array<Color>(new Color[MAX_SHAPES]),
		emissives = new Array<Color>(new Color[MAX_SHAPES]),
		emissiveIntensities = new Array<float>(new float[MAX_SHAPES]),
		count = 0,
	};

	protected RaytracedMesh[] meshes = new RaytracedMesh[MAX_SHAPES];
	protected int meshSlotIndex = 0;

	protected uint frameCount = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSphere(RaytracedSphere pSphere)
	{
		AddShape(pSphere, spheres, ref sphereSlotIndex);
		sphereData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddBox(RaytracedBox pBox)
	{
		AddShape(pBox, boxes, ref boxSlotIndex);
		boxData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMesh(RaytracedMesh pMesh)
	{
		AddShape(pMesh, meshes, ref meshSlotIndex);
	}

	protected void AddShape<T>(T pShape, T[] pShapeContainer, ref int pSlot) where T : IRaytracedShape
	{
		if (pShape == null || pSlot >= MAX_SHAPES || pShapeContainer.Contains(pShape))
			return;

		pShapeContainer[pSlot] = pShape;

		for (int i = pSlot + 1; i < MAX_SHAPES; i++)
		{
			if (pShapeContainer[i] != null)
				continue;

			pSlot = i;
			return;
		}

		pSlot = MAX_SHAPES;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveSphere(RaytracedSphere pSphere)
	{
		RemoveShape(pSphere, spheres, ref sphereSlotIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveBox(RaytracedBox pBox)
	{
		RemoveShape(pBox, boxes, ref boxSlotIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveMesh(RaytracedMesh pMesh)
	{
		RemoveShape(pMesh, meshes, ref meshSlotIndex);
	}

	protected void RemoveShape<T>(T pShape, T[] pShapeContainer, ref int pSlot) where T : IRaytracedShape
	{
		if (pShape == null || !(pShapeContainer.IndexOf(pShape) is var lIndex && lIndex >= 0))
			return;

		pShapeContainer[lIndex] = default;

		if (pSlot > lIndex)
		{
			pSlot = lIndex;
		}
	}

	protected override void Init()
	{
		base.Init();

		if (material == null && GetSurfaceOverrideMaterial(0) is ShaderMaterial lMaterial)
		{
			material = lMaterial;
		}
	}

	protected override void Dispose(bool pDisposing)
	{
		base.Dispose(pDisposing);

		if (!pDisposing)
			return;

		RenderingServer.FramePostDraw -= UpdateFrame;
	}

	public override void _Process(double pDelta)
	{
		base._Process(pDelta);

		if (Mesh == null || Camera == null)
			return;

#if DEBUG
		Refresh();
#endif //DEBUG

		if (material == null)
		{
			if (GetSurfaceOverrideMaterial(0) is not ShaderMaterial lMaterial)
				return;

			material = lMaterial;
		}

		material.SetShaderParameter(PLANE_SIZE, NearPlaneSize);
		material.SetShaderParameter(NEAR_CLIP_PLANE, Camera.Near);
		material.SetShaderParameter(LOCAL_TO_WORLD_MATRIX, Camera.GlobalTransform);
		material.SetShaderParameter(FRAME_COUNT, frameCount);

		if (print)
		{
			GD.Print(
				$"Shader parameters:\n" +
				$"Plane size: {NearPlaneSize}\n" +
				$"Near clip plane: {Camera.Near}\n" +
				$"Local to world matrix: {Camera.GlobalTransform}\n"
			);
		}

		UpdateSpheres();
		UpdateBoxes();
		UpdateMeshes();

		++frameCount;
		RenderingServer.FramePostDraw += UpdateFrame;

		if (printOnce)
		{
			print = false;
		}
	}

	protected void UpdateSpheres()
	{
		sphereData.count = 0;

		for (int i = 0; i < spheres.Length; i++)
		{
			RaytracedSphere lSphere = spheres[i];

			if (lSphere is { Visible: true })
			{
				sphereData.centers[sphereData.count] = lSphere.GlobalPosition;
				sphereData.radiuses[sphereData.count] = lSphere.Radius;

				if (lSphere.Material != null)
				{
					sphereData.colors[sphereData.count] = lSphere.Material.color;
					sphereData.emissives[sphereData.count] = lSphere.Material.emissive;
					sphereData.emissiveIntensities[sphereData.count] = lSphere.Material.emissiveIntensity;
				}
				else
				{
					sphereData.colors[sphereData.count] = defaultObjectMaterial.color;
					sphereData.emissives[sphereData.count] = defaultObjectMaterial.emissive;
					sphereData.emissiveIntensities[sphereData.count] = defaultObjectMaterial.emissiveIntensity;
				}

				++sphereData.count;
			}
			else
			{
				int lIndex = spheres.Length - 1 - (i - sphereData.count);
				sphereData.centers[lIndex] = Vector3.Zero;
				sphereData.radiuses[lIndex] = 0f;
				sphereData.colors[lIndex] = new Color(0x00000000);
				sphereData.emissives[lIndex] = new Color(0x00000000);
				sphereData.emissiveIntensities[lIndex] = 0f;
			}
		}

		material.SetShaderParameter("sphere_centers", sphereData.centers);
		material.SetShaderParameter("sphere_radiuses", sphereData.radiuses);
		material.SetShaderParameter("sphere_colors", sphereData.colors);
		material.SetShaderParameter("sphere_emissives", sphereData.emissives);
		material.SetShaderParameter("sphere_emissive_intensities", sphereData.emissiveIntensities);
		material.SetShaderParameter("sphere_count", sphereData.count);

		if (print)
		{
			GD.Print(
				$"Spheres data:\n" +
				$"Centers: {sphereData.centers}\n" +
				$"Radiuses: {sphereData.radiuses}\n" +
				$"Colors: {sphereData.colors}\n" +
				$"Emissives: {sphereData.emissives}\n" +
				$"Emissive intensities: {sphereData.emissiveIntensities}\n" +
				$"Count: {sphereData.count}"
			);
		}

		sphereData.updateRequested = sphereData.count > 0;
	}

	protected void UpdateBoxes()
	{
		boxData.count = 0;

		for (int i = 0; i < boxes.Length; i++)
		{
			RaytracedBox lBox = boxes[i];

			if (lBox is { Visible: true })
			{
				boxData.centers[boxData.count] = lBox.GlobalPosition;
				boxData.extents[boxData.count] = lBox.Size * .5f;

				if (lBox.Material != null)
				{
					boxData.colors[boxData.count] = lBox.Material.color;
					boxData.emissives[boxData.count] = lBox.Material.emissive;
					boxData.emissiveIntensities[boxData.count] = lBox.Material.emissiveIntensity;
				}
				else
				{
					boxData.colors[boxData.count] = defaultObjectMaterial.color;
					boxData.emissives[boxData.count] = defaultObjectMaterial.emissive;
					boxData.emissiveIntensities[boxData.count] = defaultObjectMaterial.emissiveIntensity;
				}

				++boxData.count;
			}
			else
			{
				int lIndex = spheres.Length - 1 - (i - boxData.count);
				boxData.centers[lIndex] = Vector3.Zero;
				boxData.extents[lIndex] = Vector3.Zero;
				boxData.colors[lIndex] = new Color(0);
				boxData.emissives[lIndex] = new Color(0);
				boxData.emissiveIntensities[lIndex] = 0f;
			}
		}

		material.SetShaderParameter("box_centers", boxData.centers);
		material.SetShaderParameter("box_extents", boxData.extents);
		material.SetShaderParameter("box_colors", boxData.colors);
		material.SetShaderParameter("box_emissives", boxData.emissives);
		material.SetShaderParameter("box_emissive_intensities", boxData.emissiveIntensities);
		material.SetShaderParameter("box_count", boxData.count);

		if (print)
		{
			GD.Print(
				$"Boxes data:\n" +
				$"Centers: {boxData.centers}\n" +
				$"Extents: {boxData.extents}\n" +
				$"Colors: {boxData.colors}\n" +
				$"Emissives: {boxData.emissives}\n" +
				$"Emissive intensities: {boxData.emissiveIntensities}\n" +
				$"Count: {boxData.count}"
			);
		}

		boxData.updateRequested = boxData.count > 0;
	}

	protected void UpdateMeshes()
	{
		
	}

	protected void UpdateFrame()
	{
		if (Camera == null)
			return;

		material.SetShaderParameter("last_frame_texture", Camera.GetViewport().GetTexture());
		RenderingServer.FramePostDraw -= UpdateFrame;
	}
}