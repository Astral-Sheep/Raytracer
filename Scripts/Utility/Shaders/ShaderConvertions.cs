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
	public static ushort toVariant(Vec2<byte> vec)
	{
		return (ushort)(vec.x | (vec.y << 8));
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
	public static uint toVariant(Vec3<byte> vec)
	{
		return (uint)(vec.x | (vec.y << 8) | (vec.z << 16));
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint toVariant(Vec4<byte> vec)
	{
		return (uint)(vec.x | (vec.y << 8) | (vec.z << 16) | (vec.w << 24));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 fromVariant(Vector2 vec)
	{
		return new vec2(vec.X, vec.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 fromSVariant(Vector2I vec)
	{
		return new ivec2(vec.X, vec.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 fromUVariant(Vector2I vec)
	{
		return new uvec2((uint)vec.X, (uint)vec.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<byte> fromUVariant(ushort vec)
	{
		return new Vec2<byte>((byte)(vec & 0xFF), (byte)((vec & 0xFF00) >> 8));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 fromVariant(Vector3 vec)
	{
		return new vec3(vec.X, vec.Y, vec.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 fromSVariant(Vector3I vec)
	{
		return new ivec3(vec.X, vec.Y, vec.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 fromUVariant(Vector3I vec)
	{
		return new uvec3((uint)vec.X, (uint)vec.Y, (uint)vec.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fromVariant(Vector4 vec)
	{
		return new vec4(vec.X, vec.Y, vec.Z, vec.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fromVariant(Color color)
	{
		return new vec4(color.R, color.G, color.B, color.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fromVariant(Rect2 rect)
	{
		return new vec4(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fromVariant(Plane plane)
	{
		return new vec4(plane.Normal.X, plane.Normal.Y, plane.Normal.Z, plane.D);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fromVariant(Quaternion quat)
	{
		return new vec4(quat.X, quat.Y, quat.Z, quat.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 fromSVariant(Vector4I vec)
	{
		return new ivec4(vec.X, vec.Y, vec.Z, vec.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 fromUVariant(Vector4I vec)
	{
		return new uvec4((uint)vec.X, (uint)vec.Y, (uint)vec.Z, (uint)vec.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<byte> fromUVariant(uint vec)
	{
		return new Vec4<byte>((byte)(vec & 0xFF), (byte)((vec & 0xFF00) >> 8), (byte)((vec & 0xFF0000) >> 16), (byte)((vec & 0xFF000000) >> 24));
	}
}