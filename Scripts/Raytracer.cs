using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class Raytracer : PostProcessLayer
{
	public struct BoundingBox
	{
		public vec3 min;
		public vec3 max;
		public readonly List<IRaytracedShape> childShapes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BoundingBox()
		{
			min = new vec3(0f);
			max = new vec3(0f);
			childShapes = new List<IRaytracedShape>();
		}
	}

	public const int TEXEL_SIZE = 16;
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

			if (material != null)
			{
				material.SetShaderParameter(ENABLE_ACCUMULATION, _enableAccumulation);
				material.SetShaderParameter(FRAME_COUNT, frameCount);
			}
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

	protected List<RaytracedSphere> spheres = new List<RaytracedSphere>();
	protected List<RaytracedBox> boxes = new List<RaytracedBox>();
	protected List<RaytracedMesh> meshes = new List<RaytracedMesh>();

	private DataBuffer shapeBuffer = DataBuffer.New("shape_buffer");
	private DataBuffer sphereBuffer = DataBuffer.New("sphere_buffer");
	private DataBuffer meshBuffer = DataBuffer.New("mesh_buffer");
	private DataBuffer vertexBuffer = DataBuffer.New("vertex_buffer");
	private DataBuffer triangleBuffer = DataBuffer.New("triangle_buffer");
	private DataBuffer materialBuffer = DataBuffer.New("material_buffer");
	private TextureBuffer textureBuffer = TextureBuffer.New("texture_buffer");
	private bool updateRequested = false;

	private (DataBuffer data, bool updateRequested) sunBuffer = (DataBuffer.New("sun_buffer"), false);

	protected uint frameCount = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSun(RaytracedSun pSun)
	{
		AddObject(pSun, suns);
		sunBuffer.updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSphere(RaytracedSphere pSphere)
	{
		AddObject(pSphere, spheres);
		updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddBox(RaytracedBox pBox)
	{
		AddObject(pBox, boxes);
		updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddMesh(RaytracedMesh pMesh)
	{
		AddObject(pMesh, meshes);
		updateRequested = true;
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

		if (!Engine.IsEditorHint())
		{
			EnableAccumulation = true;
		}

		VisibilityChanged += OnVisibilityChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		VisibilityChanged -= OnVisibilityChanged;
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

		if (Mesh == null || Camera == null || !Visible)
			return;

#if DEBUG
		Refresh();
#endif //DEBUG

		if (material == null || material != GetSurfaceOverrideMaterial(0))
		{
			if (GetSurfaceOverrideMaterial(0) is not ShaderMaterial lMaterial)
				return;

			material = lMaterial;
			material.SetShaderParameter(ENABLE_ACCUMULATION, EnableAccumulation);
		}

		// Accumulation disables objects update
		if (EnableAccumulation)
		{
			material.SetShaderParameter(FRAME_COUNT, frameCount);
			++frameCount;

			RenderingServer.FramePostDraw += UpdateFrame;
		}
		else
		{
			material.SetShaderParameter(PLANE_SIZE, NearPlaneSize);
			material.SetShaderParameter(NEAR_CLIP_PLANE, Camera.Near);
			material.SetShaderParameter(LOCAL_TO_WORLD_MATRIX, Camera.GlobalTransform);

			if (print)
			{
				GD.Print(
					$"Shader parameters:\n" +
					$"Plane size: {NearPlaneSize}\n" +
					$"Near clip plane: {Camera.Near}\n" +
					$"Local to world matrix: {Camera.GlobalTransform}\n"
				);
			}

			// UpdateShapes();
			UpdateSuns();
		}

		if (print)
		{
			for (int i = 0; i < meshes.Count; i++)
			{
				GD.Print($"Mesh {i + 1}: {meshes[i].Name}");
			}

			if (printOnce)
			{
				print = false;
			}
		}
	}

	protected void UpdateShapes()
	{
		Dictionary<Godot.Mesh, int> lImportedMeshes = new Dictionary<Godot.Mesh, int>();
		Dictionary<RaytracedMaterial, int> lImportedMaterials = new Dictionary<RaytracedMaterial, int>();
		Dictionary<Texture2D, int> lImportedTextures = new Dictionary<Texture2D, int>();

		shapeBuffer.RawData.Clear();
		sphereBuffer.RawData.Clear();
		meshBuffer.RawData.Clear();
		vertexBuffer.RawData.Clear();
		triangleBuffer.RawData.Clear();
		materialBuffer.RawData.Clear();
		textureBuffer.Textures.Clear();

		UpdateSpheres(lImportedMaterials, lImportedTextures);
		UpdateMeshes(lImportedMeshes, lImportedMaterials, lImportedTextures);

		if (print)
		{
			GD.Print(
				$"Shapes: {shapeBuffer.RawData.Count} bytes\n" +
				$"Spheres: {sphereBuffer.RawData.Count} bytes\n" +
				$"Meshes: {meshBuffer.RawData.Count} bytes\n" +
				$"Vertices: {vertexBuffer.RawData.Count} bytes\n" +
				$"Triangles: {triangleBuffer.RawData.Count} bytes\n" +
				$"Materials: {materialBuffer.RawData.Count} bytes"
			);
		}

		shapeBuffer.SendData(material);
		sphereBuffer.SendData(material);
		meshBuffer.SendData(material);
		vertexBuffer.SendData(material);
		triangleBuffer.SendData(material);
		materialBuffer.SendData(material);
		textureBuffer.SendData(material);

		material.SetShaderParameter("draw_shapes", shapeBuffer.RawData.Count > 0);
	}

	protected void UpdateSpheres(Dictionary<RaytracedMaterial, int> pImportedMaterials, Dictionary<Texture2D, int> pImportedTextures)
	{
		for (int i = 0; i < spheres.Count; i++)
		{
			RaytracedSphere lSphere = spheres[i];

			if (lSphere is not { Visible: true })
				continue;

			RaytracedMaterial lMaterial = lSphere.Material ?? DefaultObjectMaterial;

			if (lMaterial == null)
				continue;

			int lSphereIndex = Mathf.FloorToInt(sphereBuffer.RawData.Count * RaytracedSphere.INV_SPHERE_BYTE_SIZE);
			sphereBuffer.RawData.AddRange(lSphere.GetBytes());

			int lMatIndex = UpdateMaterial(lMaterial, pImportedMaterials, pImportedTextures);

			shapeBuffer.RawData.AddRange(lSphere.GetShapeBytes(lSphereIndex, lMatIndex));
		}
	}

	protected void UpdateMeshes(Dictionary<Godot.Mesh, int> pImportedMeshes, Dictionary<RaytracedMaterial, int> pImportedMaterials, Dictionary<Texture2D, int> pImportedTextures)
	{
		for (int i = 0; i < meshes.Count; i++)
		{
			RaytracedMesh lMesh = meshes[i];

			if (lMesh is not { Visible: true, Mesh: not null })
				continue;

			RaytracedMaterial lMaterial = lMesh.Material ?? DefaultObjectMaterial;

			if (lMaterial == null)
				continue;

			if (!pImportedMeshes.TryGetValue(lMesh.Mesh, out int lTriStart))
			{
				int lVertexOffset = Mathf.FloorToInt(vertexBuffer.RawData.Count * RaytracedMesh.INV_VERTEX_BYTE_SIZE);
				lTriStart = Mathf.FloorToInt(triangleBuffer.RawData.Count * RaytracedMesh.INV_TRIANGLE_BYTE_SIZE);

				(byte[] lVertices, byte[] lTriangles) = lMesh.GetPrimitiveBytes(lVertexOffset);

				vertexBuffer.RawData.AddRange(lVertices);
				triangleBuffer.RawData.AddRange(lTriangles);
				pImportedMeshes.Add(lMesh.Mesh, lTriStart);
			}

			int lDataIndex = Mathf.FloorToInt(meshBuffer.RawData.Count * RaytracedMesh.INV_MESH_BYTE_SIZE);
			meshBuffer.RawData.AddRange(lMesh.GetMeshBytes(lTriStart));

			int lMatIndex = UpdateMaterial(lMaterial, pImportedMaterials, pImportedTextures);

			shapeBuffer.RawData.AddRange(lMesh.GetShapeBytes(lDataIndex, lMatIndex));
		}
	}

	protected int UpdateMaterial(RaytracedMaterial pMaterial, Dictionary<RaytracedMaterial, int> pImportedMaterials, Dictionary<Texture2D, int> pImportedTextures)
	{
		if (!pImportedMaterials.TryGetValue(pMaterial, out int lMatIndex))
		{
			lMatIndex = Mathf.FloorToInt(materialBuffer.RawData.Count * RaytracedMaterial.INV_BYTE_SIZE);
			int lTexIndex = -1;

			if (pMaterial.texture != null && !pImportedTextures.TryGetValue(pMaterial.texture, out lTexIndex))
			{
				lTexIndex = textureBuffer.Textures.Count;
				textureBuffer.Textures.Add(pMaterial.texture.GetImage());
				pImportedTextures.Add(pMaterial.texture, lTexIndex);
			}

			materialBuffer.RawData.AddRange(pMaterial.GetBytes(lTexIndex));
			pImportedMaterials.Add(pMaterial, lMatIndex);
		}

		return lMatIndex;
	}

	protected void UpdateSuns()
	{
		sunBuffer.data.RawData.Clear();

		for (int i = 0; i < suns.Count; i++)
		{
			RaytracedSun lSun = suns[i];

			if (lSun is { Visible: true })
			{
				sunBuffer.data.RawData.AddRange(lSun.GetBytes());
			}
		}

		if (sunBuffer.data.RawData.Count > 0)
		{
			material.SetShaderParameter("draw_suns", true);
			sunBuffer.data.SendData(material);
		}
		else
		{
			material.SetShaderParameter("draw_suns", false);
			material.SetShaderParameter(sunBuffer.data.Name, Variant.From<ImageTexture>(null));
		}

		sunBuffer.updateRequested = sunBuffer.data.RawData.Count > 0;
	}

	protected void UpdateFrame()
	{
		if (Camera == null)
			return;

		material.SetShaderParameter(LAST_FRAME_TEXTURE, Camera.GetViewport().GetTexture());
		RenderingServer.FramePostDraw -= UpdateFrame;
	}
}