using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

[DebuggerDisplay(
	"({this[0, 0]}, {this[1, 0]}, {this[2, 0]}, {this[3, 0]})\n" +
	"({this[0, 1]}, {this[1, 1]}, {this[2, 1]}, {this[3, 1]})\n" +
	"({this[0, 2]}, {this[1, 2]}, {this[2, 2]}, {this[3, 2]})\n" +
	"({this[0, 3]}, {this[1, 3]}, {this[2, 3]}, {this[3, 3]})"
)]
[InlineArray(4)]
public struct mat4
{
	public float this[int col, int row]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[col][row];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => this[col][row] = value;
	}

	private vec4 column;

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(float value)
	{
		this[0] = new vec4(value, 0, 0, 0);
		this[1] = new vec4(0, value, 0, 0);
		this[2] = new vec4(0, 0, value, 0);
		this[3] = new vec4(0, 0, 0, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(vec4 a, vec4 b, vec4 c, vec4 d)
	{
		this[0] = a;
		this[1] = b;
		this[2] = c;
		this[3] = d;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(mat2 mat)
	{
		this[0] = new vec4(mat[0].xy, 0, 0);
		this[1] = new vec4(mat[1].xy, 0, 0);
		this[2] = new vec4(0);
		this[3] = new vec4(0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(mat3 mat)
	{
		this[0] = new vec4(mat[0].xyz, 0);
		this[1] = new vec4(mat[1].xyz, 0);
		this[2] = new vec4(mat[2].xyz, 0);
		this[3] = new vec4(0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(mat4 mat)
	{
		this[0] = mat[0];
		this[1] = mat[1];
		this[2] = mat[2];
		this[3] = mat[3];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(Transform3D transform)
	{
		this[0] = new vec4(transform[0].X, transform[0].Y, transform[0].Z, 0);
		this[1] = new vec4(transform[1].X, transform[1].Y, transform[1].Z, 0);
		this[2] = new vec4(transform[2].X, transform[2].Y, transform[2].Z, 0);
		this[3] = new vec4(transform[3].X, transform[3].Y, transform[3].Z, 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat4(Projection projection)
	{
		this[0] = new vec4(projection[0].X, projection[0].Y, projection[0].Z, projection[0].W);
		this[1] = new vec4(projection[1].X, projection[1].Y, projection[1].Z, projection[1].W);
		this[2] = new vec4(projection[2].X, projection[2].Y, projection[2].Z, projection[2].W);
		this[3] = new vec4(projection[3].X, projection[3].Y, projection[3].Z, projection[3].W);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator+(mat4 a, mat4 b)
	{
		return new mat4(
			a[0] + b[0],
			a[1] + b[1],
			a[2] + b[2],
			a[3] + b[3]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator-(mat4 a, mat4 b)
	{
		return new mat4(
			a[0] - b[0],
			a[1] - b[1],
			a[2] - b[2],
			a[3] - b[3]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator*(mat4 a, mat4 b)
	{
		mat2 a00 = new mat2(
			new vec2(a[0, 0], a[0, 1]),
			new vec2(a[1, 0], a[1, 1])
		);
		mat2 a01 = new mat2(
			new vec2(a[0, 2], a[0, 3]),
			new vec2(a[1, 2], a[1, 3])
		);
		mat2 a10 = new mat2(
			new vec2(a[2, 0], a[2, 1]),
			new vec2(a[3, 0], a[3, 1])
		);
		mat2 a11 = new mat2(
			new vec2(a[2, 2], a[2, 3]),
			new vec2(a[3, 2], a[3, 3])
		);

		mat2 b00 = new mat2(
			new vec2(b[0, 0], b[0, 1]),
			new vec2(b[1, 0], b[1, 1])
		);
		mat2 b01 = new mat2(
			new vec2(b[0, 2], b[0, 3]),
			new vec2(b[1, 2], b[1, 3])
		);
		mat2 b10 = new mat2(
			new vec2(b[2, 0], b[2, 1]),
			new vec2(b[3, 0], b[3, 1])
		);
		mat2 b11 = new mat2(
			new vec2(b[2, 2], b[2, 3]),
			new vec2(b[3, 2], b[3, 3])
		);

		mat2 p = (a00 + a11) * (b00 + b11);
		mat2 q = (a10 + a11) * b00;
		mat2 r = a00 * (b01 - b11);
		mat2 s = a10 * (b10 - b00);
		mat2 t = (a00 + a01) * b11;
		mat2 u = (a10 - a00) * (b00 + b01);
		mat2 v = (a01 - a11) * (b10 + b11);

		mat2 c00 = p + s - t + v;
		mat2 c01 = r + t;
		mat2 c10 = q + s;
		mat2 c11 = p + r - q + u;

		return new mat4(
			new vec4(c00[0, 0], c00[0, 1], c01[0, 0], c01[0, 1]),
			new vec4(c00[1, 0], c00[1, 1], c01[1, 0], c01[1, 1]),
			new vec4(c10[0, 0], c10[0, 1], c11[0, 0], c11[0, 1]),
			new vec4(c10[1, 0], c10[1, 1], c11[1, 0], c11[1, 1])
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 operator*(mat4 a, vec4 b)
	{
		return new vec4(
			a[0, 0] * b.x + a[1, 0] * b.y + a[2, 0] * b.z + a[3, 0] * b.w,
			a[0, 1] * b.x + a[1, 1] * b.y + a[2, 1] * b.z + a[3, 1] * b.w,
			a[0, 2] * b.x + a[1, 2] * b.y + a[2, 2] * b.z + a[3, 2] * b.w,
			a[0, 3] * b.x + a[1, 3] * b.y + a[2, 3] * b.z + a[3, 3] * b.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator*(mat4 a, float b)
	{
		return new mat4(
			a[0] * b,
			a[1] * b,
			a[2] * b,
			a[3] * b
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator*(float a, mat4 b)
	{
		return new mat4(
			a * b[0],
			a * b[1],
			a * b[2],
			a * b[3]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator/(mat4 a, mat4 b)
	{
		return a * inverse(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator/(mat4 a, float b)
	{
		return a * (1f / b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 operator/(float a, mat4 b)
	{
		return a * inverse(b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Transform3D(mat4 mat)
	{
		return new Transform3D(toVariant(mat[0].xyz), toVariant(mat[1].xyz), toVariant(mat[2].xyz), toVariant(mat[3].xyz));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat4(Transform3D transform)
	{
		return new mat4(transform);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Projection(mat4 mat)
	{
		return new Projection(toVariant(mat[0]), toVariant(mat[1]), toVariant(mat[2]), toVariant(mat[3]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat4(Projection projection)
	{
		return new mat4(projection);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(mat4 mat)
	{
		return (Projection)mat;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat4(Variant variant)
	{
		return new mat4(variant.VariantType switch {
			Variant.Type.Transform3D => variant.AsTransform3D(),
			_ => variant.AsProjection(),
		});
	}
}