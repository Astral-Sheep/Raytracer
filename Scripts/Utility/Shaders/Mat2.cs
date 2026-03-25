using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

[DebuggerDisplay(
	"({this[0, 0]}, {this[1, 0]})\n" +
	"({this[0, 1]}, {this[1, 1]})"
)]
[InlineArray(2)]
public struct mat2
{
	public float this[int col, int row]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[col][row];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => this[col][row] = value;
	}

	private vec2 column;

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(float value)
	{
		this[0] = new vec2(value, 0);
		this[1] = new vec2(0, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(vec2 a, vec2 b)
	{
		this[0] = a;
		this[1] = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(mat2 mat)
	{
		this[0] = mat[0];
		this[1] = mat[1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(mat3 mat)
	{
		this[0] = new vec2(mat[0][0], mat[0][1]);
		this[1] = new vec2(mat[1][0], mat[1][1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(mat4 mat)
	{
		this[0] = new vec2(mat[0][0], mat[0][1]);
		this[1] = new vec2(mat[1][0], mat[1][1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat2(Transform2D transform)
	{
		this[0] = new vec2(transform[0][0], transform[0][1]);
		this[1] = new vec2(transform[1][0], transform[1][1]);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator+(mat2 a, mat2 b)
	{
		return new mat2(
			a[0] + b[0],
			a[1] + b[1]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator-(mat2 a, mat2 b)
	{
		return new mat2(
			a[0] - b[0],
			a[1] - b[1]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator*(mat2 a, mat2 b)
	{
		float m0 = (a[0, 0] + a[1, 1]) * (b[0, 0] + b[1, 1]);
		float m1 = (a[1, 0] + a[1, 1]) * b[0, 0];
		float m2 = a[0, 0] * (b[0, 1] - b[1, 1]);
		float m3 = a[1, 1] * (b[1, 0] - b[0, 0]);
		float m4 = (a[0, 0] + a[0, 1]) * b[1, 1];
		float m5 = (a[1, 0] - a[0, 0]) * (b[0, 0] + b[0, 1]);
		float m6 = (a[0, 1] - a[1, 1]) * (b[1, 0] + b[1, 1]);

		return new mat2(
			new vec2(m0 + m3 - m4 + m6, m2 + m4),
			new vec2(m1 + m3, m0 - m1 + m2 + m5)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 operator*(mat2 a, vec2 b)
	{
		return new vec2(
			a[0, 0] * b.x + a[1, 0] * b.y,
			a[0, 1] * b.x + a[1, 1] * b.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator*(mat2 a, float b)
	{
		return new mat2(a[0] * b, a[1] * b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator*(float a, mat2 b)
	{
		return new mat2(a * b[0], a * b[1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator/(mat2 a, mat2 b)
	{
		return a * inverse(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator/(mat2 a, float b)
	{
		return a * (1f / b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 operator/(float a, mat2 b)
	{
		return a * inverse(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(mat2 a, mat2 b)
	{
		return a[0] == b[0] && a[1] == b[1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(mat2 a, mat2 b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Transform2D(mat2 mat)
	{
		return new Transform2D(
			mat[0, 0], mat[0, 1],
			mat[1, 0], mat[1, 1],
			0f, 0f
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat2(Transform2D transform)
	{
		return new mat2(
			new vec2(transform[0][0], transform[0][1]),
			new vec2(transform[1][0], transform[1][1])
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(mat2 mat)
	{
		return (Transform2D)mat;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat2(Variant transform)
	{
		return (mat2)transform.AsTransform2D();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is mat2 lMat && lMat == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return this[0].GetHashCode() ^ this[1].GetHashCode();
	}
}