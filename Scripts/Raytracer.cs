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
		shapes.Clear();
		updateRequested = true;
	});

	[ExportToolButton("Reset frame count")]
	protected Callable ResetFrameCount => Callable.From(() => frameCount = 0);

	protected List<RaytracedSun> suns = new List<RaytracedSun>();

	// protected List<RaytracedSphere> spheres = new List<RaytracedSphere>();
	// protected List<RaytracedBox> boxes = new List<RaytracedBox>();
	// protected List<RaytracedMesh> meshes = new List<RaytracedMesh>();
	protected List<IRaytracedShape> shapes = new List<IRaytracedShape>();

	private DataBuffer shapeBuffer = DataBuffer.New("shape_buffer");
	private DataBuffer dataBuffer = DataBuffer.New("data_buffer");
	private DataBuffer vertexBuffer = DataBuffer.New("vertex_buffer");
	private DataBuffer triangleBuffer = DataBuffer.New("triangle_buffer");
	private DataBuffer materialBuffer = DataBuffer.New("material_buffer");
	private TextureBuffer textureBuffer = TextureBuffer.New("texture_buffer");
	private bool updateRequested = false;

	private (DataBuffer data, bool updateRequested) sunBuffer = (DataBuffer.New("sun_buffer"), false);

	protected uint frameCount = 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddShape(IRaytracedShape pShape)
	{
		AddObject(pShape, shapes);
		updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddSun(RaytracedSun pSun)
	{
		AddObject(pSun, suns);
		sunBuffer.updateRequested = true;
	}

	protected void AddObject<T>(T pObject, List<T> pObjectContainer) where T : IRaytracedObject
	{
		if (pObject == null || pObjectContainer.Contains(pObject))
			return;

		pObjectContainer.Add(pObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveShape(IRaytracedShape pShape)
	{
		RemoveObject(pShape, shapes);
		updateRequested = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveSun(RaytracedSun pSun)
	{
		RemoveObject(pSun, suns);
		sunBuffer.updateRequested = true;
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

			if (updateRequested)
			{
				Dictionary<Material, int> lMaterialMap = UpdateMaterials();
				UpdateBVH(lMaterialMap);
				updateRequested = false;
			}
		}

		if (print)
		{
			if (printOnce)
			{
				print = false;
			}
		}
	}

	protected void UpdateBVH(Dictionary<Material, int> pMaterialMap)
	{
		BVHResult lResult = BVHBuilder.BuildBVH(shapes.ToArray(), pMaterialMap);

		shapeBuffer.RawData.Clear();
		dataBuffer.RawData.Clear();
		vertexBuffer.RawData.Clear();
		triangleBuffer.RawData.Clear();

		shapeBuffer.RawData.AddRange(lResult.shapeBuffer);
		dataBuffer.RawData.AddRange(lResult.dataBuffer);
		vertexBuffer.RawData.AddRange(lResult.vertexBuffer);
		triangleBuffer.RawData.AddRange(lResult.triangleBuffer);

		shapeBuffer.SendData(material);
		dataBuffer.SendData(material);
		vertexBuffer.SendData(material);
		triangleBuffer.SendData(material);
	}

	protected Dictionary<Material, int> UpdateMaterials()
	{
		Dictionary<Material, int> lMaterialMap = new Dictionary<Material, int>();
		Dictionary<Texture2D, int> lTextureMap = new Dictionary<Texture2D, int>();

		materialBuffer.RawData.Clear();
		textureBuffer.Textures.Clear();

		for (int i = 0; i < shapes.Count; i++)
		{
			Material[] lMaterials = shapes[i].Materials;

			for (int j = 0; j < lMaterials.Length; j++)
			{
				Material lMaterial = lMaterials[j] ?? DefaultObjectMaterial;

				if (!MaterialData.CanHandleResource(lMaterial))
					continue;

				if (lMaterialMap.ContainsKey(lMaterial))
					continue;

				switch (lMaterial)
				{
					case RaytracedMaterial lRaytracedMaterial:
					{
						int lMatIndex = Mathf.FloorToInt(materialBuffer.RawData.Count * RaytracedMaterial.INV_BYTE_SIZE);
						int lTexIndex = -1;

						if (lRaytracedMaterial.texture != null && !lTextureMap.TryGetValue(lRaytracedMaterial.texture, out lTexIndex))
						{
							lTexIndex = textureBuffer.Textures.Count;
							textureBuffer.Textures.Add(lRaytracedMaterial.texture.GetImage());
							lTextureMap.Add(lRaytracedMaterial.texture, lTexIndex);
						}

						(bool _, MaterialData lData) = MaterialData.FromResource(lRaytracedMaterial, lTextureMap);
						materialBuffer.RawData.AddRange(lData.GetBytes());

						lMaterialMap.Add(lRaytracedMaterial, lMatIndex);
						break;
					}
					case BaseMaterial3D lBaseMaterial:
					{
						int lMatIndex = Mathf.FloorToInt(materialBuffer.RawData.Count * RaytracedMaterial.INV_BYTE_SIZE);
						int lTexIndex = -1;

						if (lBaseMaterial.AlbedoTexture != null && !lTextureMap.TryGetValue(lBaseMaterial.AlbedoTexture, out lTexIndex))
						{
							lTexIndex = textureBuffer.Textures.Count;
							textureBuffer.Textures.Add(lBaseMaterial.AlbedoTexture.GetImage());
							lTextureMap.Add(lBaseMaterial.AlbedoTexture, lTexIndex);
						}

						(bool _, MaterialData lData) = MaterialData.FromResource(lBaseMaterial, lTextureMap);
						materialBuffer.RawData.AddRange(lData.GetBytes());

						lMaterialMap.Add(lBaseMaterial, lMatIndex);
						break;
					}
					default:
					{
						GD.PushWarning($"Unhandled material type: {lMaterial.GetType().Name}");
						break;
					}
				}
			}
		}

		materialBuffer.SendData(material);
		textureBuffer.SendData(material);
		return lMaterialMap;
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

		sunBuffer.data.SendData(material);
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