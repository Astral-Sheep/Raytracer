global using static Astral.Tools.ShaderConvertions;

using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

public static class ShaderConvertions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 toVariant(vec2 vec)
	{
		return new Vector2(vec.x, vec.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I toVariant(ivec2 vec)
	{
		return new Vector2I(vec.x, vec.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I toVariant(uvec2 vec)
	{
		return new Vector2I((int)vec.x, (int)vec.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 toVariant(vec3 vec)
	{
		return new Vector3(vec.x, vec.y, vec.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3I toVariant(ivec3 vec)
	{
		return new Vector3I(vec.x, vec.y, vec.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3I toVariant(uvec3 vec)
	{
		return new Vector3I((int)vec.x, (int)vec.y, (int)vec.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 toVariant(vec4 vec)
	{
		return new Vector4(vec.x, vec.y, vec.z, vec.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color toColor(vec4 vec)
	{
		return new Color(vec.r, vec.g, vec.b, vec.a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rect2 toRect(vec4 vec)
	{
		return new Rect2(vec.x, vec.y, vec.z, vec.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane toPlane(vec4 vec)
	{
		return new Plane(vec.x, vec.y, vec.z, vec.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion toQuaternion(vec4 vec)
	{
		return new Quaternion(vec.x, vec.y, vec.z, vec.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4I toVariant(ivec4 vec)
	{
		return new Vector4I(vec.x, vec.y, vec.z, vec.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4I toVariant(uvec4 vec)
	{
		return new Vector4I((int)vec.x, (int)vec.y, (int)vec.z, (int)vec.w);
	}
}