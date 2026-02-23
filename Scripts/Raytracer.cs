using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class Raytracer : PostProcessLayer
{
	private struct SphereData
	{
		public ImageTexture buffer;
		public bool updateRequested;
	}

	private struct BoxData
	{
		public ImageTexture buffer;
		public bool updateRequested;
	}

	private struct MeshData
	{
		public ImageTexture meshBuffer;
		public ImageTexture trianglesBuffer;
		public bool updateRequested;
	}

	public const string SCENE_PATH = "Main/Camera/Raytracer";

	private const string PLANE_SIZE = "plane_size";
	private const string NEAR_CLIP_PLANE = "near_clip_plane";
	private const string LOCAL_TO_WORLD_MATRIX = "local_to_world_matrix";
	private const string ENABLE_ACCUMULATION = "enable_accumulation";
	private const string LAST_FRAME_TEXTURE = "last_frame_texture";
	private const string FRAME_COUNT = "frame_count";

	[ExportGroup("Editor debug")]
	[Export] protected bool print = false;
	[Export] protected bool printOnce = false;

	[ExportGroup("")]
	[Export]
	public bool EnableAccumulation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _enableAccumulation;
		set
		{
			if (_enableAccumulation == value)
				return;

			_enableAccumulation = value;
			frameCount = 0;
		}
	}
	protected bool _enableAccumulation = false;

	[Export] public RaytracedMaterial DefaultObjectMaterial { get; protected set; }
	[Export] protected ShaderMaterial material;

	[ExportToolButton("Clear shapes")]
	protected Callable ClearShapes => Callable.From(() => {
		spheres.Clear();
		boxes.Clear();
		meshes.Clear();
	});

	[ExportToolButton("Reset frame count")]
	protected Callable ResetFrameCount => Callable.From(() => frameCount = 0);

	protected List<RaytracedSphere> spheres = new List<RaytracedSphere>();
	private SphereData sphereData = new SphereData {
		buffer = ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		updateRequested = false,
	};

	protected List<RaytracedBox> boxes = new List<RaytracedBox>();
	private BoxData boxData = new BoxData {
		buffer = ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		updateRequested = false,
	};

	protected List<RaytracedMesh> meshes = new List<RaytracedMesh>();
	private MeshData meshData = new MeshData {
		meshBuffer = ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		trianglesBuffer = ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		updateRequested = false,
	};

	protected uint frameCount = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSphere(RaytracedSphere pSphere)
	{
		AddShape(pSphere, spheres);
		sphereData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddBox(RaytracedBox pBox)
	{
		AddShape(pBox, boxes);
		boxData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMesh(RaytracedMesh pMesh)
	{
		AddShape(pMesh, meshes);
		meshData.updateRequested = true;
	}

	protected void AddShape<T>(T pShape, List<T> pShapeContainer) where T : IRaytracedShape
	{
		if (pShape == null || pShapeContainer.Contains(pShape))
			return;

		pShapeContainer.Add(pShape);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveSphere(RaytracedSphere pSphere)
	{
		RemoveShape(pSphere, spheres);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveBox(RaytracedBox pBox)
	{
		RemoveShape(pBox, boxes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveMesh(RaytracedMesh pMesh)
	{
		RemoveShape(pMesh, meshes);
	}

	protected void RemoveShape<T>(T pShape, List<T> pShapeContainer) where T : IRaytracedShape
	{
		if (pShape == null || !pShapeContainer.Contains(pShape))
			return;

		pShapeContainer.Remove(pShape);
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
		material.SetShaderParameter(ENABLE_ACCUMULATION, EnableAccumulation);
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

		if (EnableAccumulation)
		{
			RenderingServer.FramePostDraw += UpdateFrame;
		}

		if (printOnce)
		{
			print = false;
		}
	}

	protected void UpdateSpheres()
	{
		List<byte> lRawData = new List<byte>();

		for (int i = 0; i < spheres.Count; i++)
		{
			RaytracedSphere lSphere = spheres[i];

			if (lSphere is { Visible: true })
			{
				lRawData.AddRange(lSphere.GetBytes());
			}
		}

		if (lRawData.Count > 0)
		{
			SetBufferData(sphereData.buffer, lRawData.ToArray());
			material.SetShaderParameter("sphere_buffer", sphereData.buffer);
			material.SetShaderParameter("draw_spheres", true);
		}
		else
		{
			material.SetShaderParameter("sphere_buffer", default(ImageTexture));
			material.SetShaderParameter("draw_spheres", false);
		}

		sphereData.updateRequested = lRawData.Count > 0;
	}

	protected void UpdateBoxes()
	{
		List<byte> lRawData = new List<byte>();

		for (int i = 0; i < boxes.Count; i++)
		{
			RaytracedBox lBox = boxes[i];

			if (lBox is { Visible: true })
			{
				lRawData.AddRange(lBox.GetBytes());
			}
		}

		if (lRawData.Count > 0)
		{
			SetBufferData(boxData.buffer, lRawData.ToArray());
			material.SetShaderParameter("box_buffer", boxData.buffer);
			material.SetShaderParameter("draw_boxes", true);
		}
		else
		{
			material.SetShaderParameter("box_buffer", default(ImageTexture));
			material.SetShaderParameter("draw_boxes", false);
		}

		boxData.updateRequested = lRawData.Count > 0;
	}

	protected void UpdateMeshes()
	{
		List<byte> lRawMeshes = new List<byte>();
		List<byte> lRawTriangles = new List<byte>();

		for (int i = 0; i < meshes.Count; i++)
		{
			RaytracedMesh lMesh = meshes[i];

			if (lMesh is { Visible: true, Mesh: not null })
			{
				lRawMeshes.AddRange(lMesh.GetMeshBytes(lRawTriangles.Count / (5 * 16)));
				lRawTriangles.AddRange(lMesh.GetTrianglesBytes());
			}
		}

		if (lRawMeshes.Count > 0)
		{
			SetBufferData(meshData.meshBuffer, lRawMeshes.ToArray());
			SetBufferData(meshData.trianglesBuffer, lRawTriangles.ToArray());
			material.SetShaderParameter("mesh_buffer", meshData.meshBuffer);
			material.SetShaderParameter("triangle_buffer", meshData.trianglesBuffer);
			material.SetShaderParameter("draw_meshes", true);
		}
		else
		{
			material.SetShaderParameter("mesh_buffer", default(ImageTexture));
			material.SetShaderParameter("triangle_buffer", default(ImageTexture));
			material.SetShaderParameter("draw_meshes", false);
		}

		if (print)
		{
			GD.Print(
				$"Meshes: {meshes.Count}\n" +
				$"Raw mesh data: {lRawMeshes.Count} bytes ({lRawMeshes.Count / (7 * 16)} meshes)\n" +
				$"Raw triangle data: {lRawTriangles.Count} bytes ({lRawTriangles.Count / (5 * 16)} triangles)\n"
			);
		}

		meshData.updateRequested = lRawMeshes.Count > 0;
	}

	protected void UpdateFrame()
	{
		if (Camera == null)
			return;

		material.SetShaderParameter(LAST_FRAME_TEXTURE, Camera.GetViewport().GetTexture());
		RenderingServer.FramePostDraw -= UpdateFrame;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void SetBufferData(ImageTexture pBuffer, byte[] pRawData)
	{
		Image lImage = pBuffer.Image;
		lImage.SetData(pRawData.Length / 16, 1, false, Image.Format.Rgbaf, pRawData);
		pBuffer.SetImage(lImage);
	}
}