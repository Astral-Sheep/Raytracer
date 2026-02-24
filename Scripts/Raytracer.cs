using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class Raytracer : PostProcessLayer
{
	private struct ShaderData
	{
		public ImageTexture[] buffers;
		public bool updateRequested;
	}

	// private struct SphereData
	// {
	// 	public ImageTexture buffer;
	// 	public bool updateRequested;
	// }
	//
	// private struct BoxData
	// {
	// 	public ImageTexture buffer;
	// 	public bool updateRequested;
	// }
	//
	// private struct MeshData
	// {
	// 	public ImageTexture meshBuffer;
	// 	public ImageTexture trianglesBuffer;
	// 	public bool updateRequested;
	// }

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

	protected List<RaytracedSun> suns = new List<RaytracedSun>();

	private ShaderData sunData = new ShaderData {
		buffers = [ ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)) ],
		updateRequested = false,
	};

	protected List<RaytracedSphere> spheres = new List<RaytracedSphere>();
	private ShaderData sphereData = new ShaderData {
		buffers = [ ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)) ],
		updateRequested = false,
	};

	protected List<RaytracedBox> boxes = new List<RaytracedBox>();
	private ShaderData boxData = new ShaderData {
		buffers = [ ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)) ],
		updateRequested = false,
	};

	protected List<RaytracedMesh> meshes = new List<RaytracedMesh>();
	private ShaderData meshData = new ShaderData {
		buffers = [
			ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
			ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)),
		],
		updateRequested = false,
	};

	protected uint frameCount = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSun(RaytracedSun pSun)
	{
		AddObject(pSun, suns);
		sphereData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSphere(RaytracedSphere pSphere)
	{
		AddObject(pSphere, spheres);
		sphereData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddBox(RaytracedBox pBox)
	{
		AddObject(pBox, boxes);
		boxData.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMesh(RaytracedMesh pMesh)
	{
		AddObject(pMesh, meshes);
		meshData.updateRequested = true;
	}

	protected void AddObject<T>(T pObject, List<T> pObjectContainer) where T : IRaytracedObject
	{
		if (pObject == null || pObjectContainer.Contains(pObject))
			return;

		pObjectContainer.Add(pObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveSun(RaytracedSun pSun)
	{
		RemoveObject(pSun, suns);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveSphere(RaytracedSphere pSphere)
	{
		RemoveObject(pSphere, spheres);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveBox(RaytracedBox pBox)
	{
		RemoveObject(pBox, boxes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveMesh(RaytracedMesh pMesh)
	{
		RemoveObject(pMesh, meshes);
	}

	protected void RemoveObject<T>(T pObject, List<T> pObjectContainer) where T : IRaytracedObject
	{
		if (pObject == null || !pObjectContainer.Contains(pObject))
			return;

		pObjectContainer.Remove(pObject);
	}

	protected override void Init()
	{
		base.Init();

		if (material == null && GetSurfaceOverrideMaterial(0) is ShaderMaterial lMaterial)
		{
			material = lMaterial;
		}

		VisibilityChanged += OnVisibilityChanged;
	}

	protected void OnVisibilityChanged()
	{
		if (Visible)
			return;

		EnableAccumulation = false;
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

		if (material == null || material != GetSurfaceOverrideMaterial(0))
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

		UpdateSuns();
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

	protected void UpdateSuns()
	{
		List<byte> lRawData = new List<byte>();

		for (int i = 0; i < suns.Count; i++)
		{
			RaytracedSun lSun = suns[i];

			if (lSun is { Visible: true })
			{
				lRawData.AddRange(lSun.GetBytes());
			}
		}

		if (lRawData.Count > 0)
		{
			SetBufferData(sunData.buffers[0], lRawData.ToArray());
			material.SetShaderParameter("sun_buffer", sunData.buffers[0]);
			material.SetShaderParameter("draw_suns", true);
		}
		else
		{
			material.SetShaderParameter("sun_buffer", default(ImageTexture));
			material.SetShaderParameter("draw_suns", false);
		}

		sunData.updateRequested = lRawData.Count > 0;
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
			SetBufferData(sphereData.buffers[0], lRawData.ToArray());
			material.SetShaderParameter("sphere_buffer", sphereData.buffers[0]);
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
			SetBufferData(boxData.buffers[0], lRawData.ToArray());
			material.SetShaderParameter("box_buffer", boxData.buffers[0]);
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
			SetBufferData(meshData.buffers[0], lRawMeshes.ToArray());
			SetBufferData(meshData.buffers[1], lRawTriangles.ToArray());
			material.SetShaderParameter("mesh_buffer", meshData.buffers[0]);
			material.SetShaderParameter("triangle_buffer", meshData.buffers[1]);
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