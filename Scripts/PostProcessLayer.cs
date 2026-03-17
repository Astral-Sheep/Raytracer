using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Raytracer;

/// <summary>
/// Mesh used as a screen post-process layer. Should be added only on cameras
/// </summary>
[Tool, GlobalClass]
public partial class PostProcessLayer : MeshInstance3D
{
	private const float NEAR_CLIP_PLANE_OFFSET = 1.01f;

#if DEBUG
	public bool UseEditorCamera => Engine.IsEditorHint() && _showInSceneView;

	[Export]
	public bool ShowInSceneView
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showInSceneView;
		set
		{
			if (_showInSceneView == value)
				return;

			_showInSceneView = value;
			Camera = UseEditorCamera
				? EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D()
				: GetParent() as Camera3D;
		}
	}
	private bool _showInSceneView = false;
#endif //DEBUG

	public Camera3D Camera
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _camera;
		set
		{
			if (_camera == value)
				return;

			if (_camera != null && GetTree() != null)
			{
				_camera.GetViewport().SizeChanged -= OnSizeChanged;
			}

			_camera = value;
			Refresh();

			if (_camera != null)
			{
				GlobalPosition = _camera.GlobalPosition - _camera.GlobalBasis.Z * (_camera.Near * NEAR_CLIP_PLANE_OFFSET);
				GlobalRotation = _camera.GlobalRotation;

				if (GetTree() != null)
				{
					_camera.GetViewport().SizeChanged += OnSizeChanged;
				}
			}
		}
	}
	private Camera3D _camera;

	public Vector2 NearPlaneSize { get; private set; }
	public Vector2 FarPlaneSize { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		if (!IsNodeReady())
			return;

		Init();
	}

	public override void _Ready()
	{
		base._Ready();

		ProcessPriority = 5;
		Init();

		CastShadow = ShadowCastingSetting.Off;
		GIMode = GIModeEnum.Disabled;
		PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
		SetMeta("_edit_lock_", true);
	}

	public override void _Process(double pDelta)
	{
		base._Process(pDelta);

		if (Camera != null && Camera != GetParent())
		{
			GlobalPosition = Camera.GlobalPosition - Camera.GlobalTransform.Basis.Z * (Camera.Near * NEAR_CLIP_PLANE_OFFSET);
			GlobalRotation = Camera.GlobalRotation;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if (Camera?.GetViewport() != null)
		{
			Camera.GetViewport().SizeChanged -= OnSizeChanged;
		}
	}

	protected virtual void Init()
	{
		if (Mesh is not PlaneMesh)
		{
			PlaneMesh lMesh = new PlaneMesh();
			lMesh.Orientation = PlaneMesh.OrientationEnum.Z;
			lMesh.CenterOffset = Vector3.Zero;
			lMesh.SubdivideDepth = 0;
			lMesh.SubdivideWidth = 0;
			Mesh = lMesh;
		}

#if DEBUG
		bool lUseSceneCamera = Engine.IsEditorHint() && ShowInSceneView;

		Camera = lUseSceneCamera
			? EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D()
			: GetParent() as Camera3D;
#else
		Camera = GetParent() as Camera3D;
#endif //DEBUG
	}

	protected void OnSizeChanged()
	{
		Refresh();
	}

	public virtual void Refresh()
	{
		RefreshPlanes();
		RefreshMesh();
	}

	private void RefreshPlanes()
	{
		if (Camera == null)
		{
			NearPlaneSize = Vector2.Zero;
			FarPlaneSize = Vector2.Zero;
			return;
		}

		Vector2 lScreenSize = Camera.GetViewport().GetVisibleRect().Size;
		float lFovAxisSize = Mathf.Tan(Mathf.DegToRad(Camera.Fov) * .5f) * 2f;
		Vector2 lNearSize = Vector2.Zero;
		Vector2 lFarSize = Vector2.Zero;

		switch (Camera.KeepAspect)
		{
			case Camera3D.KeepAspectEnum.Width:
			{
				float lInvAspectRatio = lScreenSize.Y / lScreenSize.X;

				switch (Camera.Projection)
				{
					case Camera3D.ProjectionType.Perspective:
						lNearSize.X = Camera.Near * lFovAxisSize * 2f;
						lFarSize.X = Camera.Far * lFovAxisSize * 2f;
						break;
					case Camera3D.ProjectionType.Orthogonal:
						lNearSize.X = Camera.Size;
						lFarSize.X = Camera.Size;
						break;
					case Camera3D.ProjectionType.Frustum:
						lNearSize.X = Camera.Size * Camera.Near * lFovAxisSize * 2f;
						lFarSize.X = Camera.Size * Camera.Far * lFovAxisSize * 2f;
						break;
					default:
						lNearSize.X = 1f;
						lFarSize.X = 1f;
						break;
				}

				lNearSize.Y = lNearSize.X * lInvAspectRatio;
				lFarSize.Y = lFarSize.X * lInvAspectRatio;
				break;
			}
			case Camera3D.KeepAspectEnum.Height:
			{
				float lInvAspectRatio = lScreenSize.X / lScreenSize.Y;

				switch (Camera.Projection)
				{
					case Camera3D.ProjectionType.Perspective:
						lNearSize.Y = Camera.Near * lFovAxisSize * 2f;
						lFarSize.Y = Camera.Far * lFovAxisSize * 2f;
						break;
					case Camera3D.ProjectionType.Orthogonal:
						lNearSize.Y = Camera.Size;
						lFarSize.Y = Camera.Size;
						break;
					case Camera3D.ProjectionType.Frustum:
						lNearSize.Y = Camera.Size * Camera.Near * lFovAxisSize * 2f;
						lFarSize.Y = Camera.Size * Camera.Far * lFovAxisSize * 2f;
						break;
					default:
						lNearSize.Y = 1f;
						lFarSize.Y = 1f;
						break;
				}

				lNearSize.X = lNearSize.Y * lInvAspectRatio;
				lFarSize.X = lFarSize.Y * lInvAspectRatio;
				break;
			}
			default:
			{
				lNearSize = Vector2.One;
				lFarSize = Vector2.One;
				break;
			}
		}

		NearPlaneSize = lNearSize;
		FarPlaneSize = lFarSize;
	}

	private void RefreshMesh()
	{
		if (Camera == null || Mesh is not PlaneMesh lMesh)
			return;

		lMesh.Size = NearPlaneSize * NEAR_CLIP_PLANE_OFFSET;
		Mesh = lMesh;
	}

	public override string[] _GetConfigurationWarnings()
	{
		List<string> lWarnings = base._GetConfigurationWarnings()?.ToList() ?? new List<string>();

		if (GetParent() is not Camera3D)
		{
			lWarnings.Add($"Post-process layer should be added only on {nameof(Camera3D)} nodes");
		}

		if (Mesh is not PlaneMesh)
		{
			lWarnings.Add($"Mesh should be a {nameof(PlaneMesh)} but {(Mesh == null ? "null" : $"a {Mesh.GetType().Name}")} was added");
		}

		return lWarnings.ToArray();
	}
}