using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

[Tool, GlobalClass]
public partial class Raytracer : PostProcessLayer
{
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
	// 	= new ShaderData {
	// 	buffers = [ ImageTexture.CreateFromImage(Image.CreateEmpty(1, 1, false, Image.Format.Rgbaf)) ],
	// 	updateRequested = false,
	// };

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
			material.SetShaderParameter(ENABLE_ACCUMULATION, EnableAccumulation);
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

		UpdateShapes();
		UpdateSuns();

		++frameCount;

		if (EnableAccumulation)
		{
			RenderingServer.FramePostDraw += UpdateFrame;
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
		Dictionary<Mesh, int> lImportedMeshes = new Dictionary<Mesh, int>();
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

	protected void UpdateMeshes(Dictionary<Mesh, int> pImportedMeshes, Dictionary<RaytracedMaterial, int> pImportedMaterials, Dictionary<Texture2D, int> pImportedTextures)
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
			// SetBufferData(sunBuffer.data.Buffer[0], lRawData.ToArray());
			material.SetShaderParameter("draw_suns", true);
			sunBuffer.data.SendData(material);
			// material.SetShaderParameter("sun_buffer", sunBuffer.buffers[0]);
			// material.SetShaderParameter("draw_suns", true);
		}
		else
		{
			material.SetShaderParameter("draw_suns", false);
			material.SetShaderParameter(sunBuffer.data.Name, Variant.From<ImageTexture>(null));
		}

		sunBuffer.updateRequested = sunBuffer.data.RawData.Count > 0;
	}

	// protected void UpdateSpheres()
	// {
	// 	List<byte> lRawData = new List<byte>();
	//
	// 	for (int i = 0; i < spheres.Count; i++)
	// 	{
	// 		RaytracedSphere lSphere = spheres[i];
	//
	// 		if (lSphere is { Visible: true })
	// 		{
	// 			lRawData.AddRange(lSphere.GetBytes());
	// 		}
	// 	}
	//
	// 	if (lRawData.Count > 0)
	// 	{
	// 		SetBufferData(sphereData.buffers[0], lRawData.ToArray());
	// 		material.SetShaderParameter("sphere_buffer", sphereData.buffers[0]);
	// 		material.SetShaderParameter("draw_spheres", true);
	// 	}
	// 	else
	// 	{
	// 		material.SetShaderParameter("sphere_buffer", Variant.From<ImageTexture>(null));
	// 		material.SetShaderParameter("draw_spheres", false);
	// 	}
	//
	// 	sphereData.updateRequested = lRawData.Count > 0;
	// }
	//
	// protected void UpdateBoxes()
	// {
	// 	List<byte> lRawData = new List<byte>();
	//
	// 	for (int i = 0; i < boxes.Count; i++)
	// 	{
	// 		RaytracedBox lBox = boxes[i];
	//
	// 		if (lBox is { Visible: true })
	// 		{
	// 			lRawData.AddRange(lBox.GetBytes());
	// 		}
	// 	}
	//
	// 	if (lRawData.Count > 0)
	// 	{
	// 		SetBufferData(boxData.buffers[0], lRawData.ToArray());
	// 		material.SetShaderParameter("box_buffer", boxData.buffers[0]);
	// 		material.SetShaderParameter("draw_boxes", true);
	// 	}
	// 	else
	// 	{
	// 		material.SetShaderParameter("box_buffer", Variant.From<ImageTexture>(null));
	// 		material.SetShaderParameter("draw_boxes", false);
	// 	}
	//
	// 	boxData.updateRequested = lRawData.Count > 0;
	// }
	//
	// protected void UpdateMeshes()
	// {
	// 	List<byte> lRawMeshes = new List<byte>();
	// 	List<byte> lRawTriangles = new List<byte>();
	// 	Godot.Collections.Array<Image> lMaterialTextures = new Godot.Collections.Array<Image>();
	// 	float lInvTriangleTexelSize = 1f / (RaytracedMesh.TRIANGLE_DATA_SIZE * TEXEL_SIZE);
	//
	// 	for (int i = 0; i < meshes.Count; i++)
	// 	{
	// 		RaytracedMesh lMesh = meshes[i];
	//
	// 		if (lMesh is { Visible: true, Mesh: not null })
	// 		{
	// 			lRawMeshes.AddRange(lMesh.GetMeshBytes(Mathf.FloorToInt(lRawTriangles.Count * lInvTriangleTexelSize), lMaterialTextures.Count));
	// 			lRawTriangles.AddRange(lMesh.GetTrianglesBytes());
	//
	// 			RaytracedMaterial lMeshMaterial = lMesh.Material ?? DefaultObjectMaterial;
	//
	// 			if (lMeshMaterial?.texture != null)
	// 			{
	// 				lMaterialTextures.Add(lMeshMaterial.texture.GetImage());
	// 			}
	// 		}
	// 	}
	//
	// 	if (lRawMeshes.Count > 0)
	// 	{
	// 		SetBufferData(meshData.buffers[0], lRawMeshes.ToArray());
	// 		SetBufferData(meshData.buffers[1], lRawTriangles.ToArray());
	//
	// 		material.SetShaderParameter("mesh_buffer", meshData.buffers[0]);
	// 		material.SetShaderParameter("triangle_buffer", meshData.buffers[1]);
	// 		material.SetShaderParameter("draw_meshes", true);
	//
	// 		if (lMaterialTextures.Count > 0)
	// 		{
	// 			Texture2DArray lTextures = new Texture2DArray();
	// 			lTextures.CreateFromImages(lMaterialTextures);
	// 			material.SetShaderParameter("mesh_textures", lTextures);
	// 		}
	// 	}
	// 	else
	// 	{
	// 		material.SetShaderParameter("mesh_buffer", Variant.From<ImageTexture>(null));
	// 		material.SetShaderParameter("triangle_buffer", Variant.From<ImageTexture>(null));
	// 		material.SetShaderParameter("mesh_textures", Variant.From<Texture2DArray>(null));
	// 		material.SetShaderParameter("draw_meshes", false);
	// 	}
	//
	// 	if (print)
	// 	{
	// 		GD.Print(
	// 			$"Meshes: {meshes.Count}\n" +
	// 			$"Raw mesh data: {lRawMeshes.Count} bytes ({lRawMeshes.Count / (7 * 16)} meshes)\n" +
	// 			$"Raw triangle data: {lRawTriangles.Count} bytes ({lRawTriangles.Count / (5 * 16)} triangles)\n"
	// 		);
	// 	}
	//
	// 	meshData.updateRequested = lRawMeshes.Count > 0;
	// }

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