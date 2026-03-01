using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

[InlineArray(3)]
public struct mat3
{
	public float this[int col, int row]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[col][row];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => this[col][row] = value;
	}

	private vec3 column;

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(float value)
	{
		this[0] = new vec3(value, 0, 0);
		this[1] = new vec3(0, value, 0);
		this[2] = new vec3(0, 0, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(vec3 a, vec3 b, vec3 c)
	{
		this[0] = a;
		this[1] = b;
		this[2] = c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(mat2 mat)
	{
		this[0] = new vec3(mat[0], 0);
		this[1] = new vec3(mat[1], 0);
		this[2] = new vec3(0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(mat3 mat)
	{
		this[0] = mat[0];
		this[1] = mat[1];
		this[2] = mat[2];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(mat4 mat)
	{
		this[0] = mat[0].xyz;
		this[1] = mat[1].xyz;
		this[2] = mat[2].xyz;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public mat3(Basis basis)
	{
		this[0] = new vec3(basis[0].X, basis[0].Y, basis[0].Z);
		this[1] = new vec3(basis[1].X, basis[1].Y, basis[1].Z);
		this[2] = new vec3(basis[2].X, basis[2].Y, basis[2].Z);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator+(mat3 a, mat3 b)
	{
		return new mat3(
			a[0] + b[0],
			a[1] + b[1],
			a[2] + b[2]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator-(mat3 a, mat3 b)
	{
		return new mat3(
			a[0] - b[0],
			a[1] - b[1],
			a[2] - b[2]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator*(mat3 a, mat3 b)
	{
		float m0 = (a[0, 0] + a[0, 1] + a[0, 2] - a[1, 0] - a[1, 1] - a[2, 1] - a[2, 2]) * b[1, 1];
		float m1 = (a[0, 0] - a[1, 0]) * (-b[0, 1] + b[1, 1]);
		float m2 = a[1, 1] * (-b[0, 0] + b[0, 1] + b[1, 0] - b[1, 1] - b[1, 2] - b[2, 0] + b[2, 2]);
		float m3 = (-a[0, 0] + a[1, 0] + a[1, 1]) * (b[0, 0] - b[0, 1] + b[1, 1]);
		float m4 = (a[1, 0] + a[1, 1]) * (-b[0, 0] + b[0, 1]);
		float m5 = a[0, 0] * b[0, 0];
		float m6 = (-a[0, 0] + a[2, 0] + a[2, 1]) * (b[0, 0] - b[0, 2] + b[1, 2]);
		float m7 = (-a[0, 0] + a[2, 0]) * (b[0, 2] - b[1, 2]);
		float m8 = (a[2, 0] + a[2, 1]) * (-b[0, 0] + b[0, 2]);
		float m9 = (a[0, 0] + a[0, 1] + a[0, 2] - a[1, 1] - a[1, 2] - a[2, 0] - a[2, 1]) * b[1, 2];
		float m10 = a[2, 1] * (-b[0, 0] + b[0, 2] + b[1, 0] - b[1, 1] - b[1, 2] - b[2, 0] + b[2, 1]);
		float m11 = (-a[0, 2] + a[2, 1] + a[2, 2]) * (b[1, 1] + b[2, 0] - b[2, 1]);
		float m12 = (a[0, 2] - a[2, 2]) * (b[1, 1] - b[2, 1]);
		float m13 = a[0, 2] * b[2, 0];
		float m14 = (a[2, 1] + a[2, 2]) * (-b[2, 0] * b[2, 1]);
		float m15 = (-a[0, 2] + a[1, 1] + a[1, 2]) * (b[1, 2] + b[2, 0] - b[2, 2]);
		float m16 = (a[0, 2] - a[1, 2]) * (b[1, 2] - b[2, 2]);
		float m17 = (a[1, 1] + a[1, 2]) * (-b[2, 0] + b[2, 2]);
		float m18 = a[0, 1] * b[1, 0];
		float m19 = a[1, 2] * b[2, 1];
		float m20 = a[1, 0] * b[0, 2];
		float m21 = a[2, 0] * b[0, 1];
		float m22 = a[2, 2] * b[2, 2];

		return new mat3(
			new vec3(
				m5 + m13 + m18,
				m0 + m3 + m4 + m5 + m11 + m13 + m14,
				m5 + m6 + m8 + m9 + m13 + m15 + m17
			),
			new vec3(
				m1 + m2 + m3 + m5 + m13 + m15 + m16,
				m1 + m3 + m4 + m5 + m19,
				m13 + m15 + m16 + m17 + m20
			),
			new vec3(
				m5 + m6 + m7 + m10 + m11 + m12 + m13,
				m11 + m12 + m13 + m14 + m21,
				m5 + m6 + m7 + m8 + m22
			)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 operator*(mat3 a, vec3 b)
	{
		return new vec3(
			a[0, 0] * b.x + a[1, 0] * b.y + a[2, 0] * b.z,
			a[0, 1] * b.x + a[1, 1] * b.y + a[2, 1] * b.z,
			a[0, 2] * b.x + a[1, 2] * b.y + a[2, 2] * b.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator*(mat3 a, float b)
	{
		return new mat3(
			a[0] * b,
			a[1] * b,
			a[2] * b
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator*(float a, mat3 b)
	{
		return new mat3(
			a * b[0],
			a * b[1],
			a * b[2]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator/(mat3 a, mat3 b)
	{
		return a * inverse(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator/(mat3 a, float b)
	{
		return a * (1f / b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 operator/(float a, mat3 b)
	{
		return a * inverse(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(mat3 a, mat3 b)
	{
		return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(mat3 a, mat3 b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Basis(mat3 mat)
	{
		return new Basis(toVariant(mat[0]), toVariant(mat[1]), toVariant(mat[2]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat3(Basis basis)
	{
		return new mat3(basis);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(mat3 mat)
	{
		return (Basis)mat;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator mat3(Variant basis)
	{
		return (mat3)basis.AsBasis();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is mat3 lMat && lMat == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return this[0].GetHashCode() ^ this[1].GetHashCode() ^ this[2].GetHashCode();
	}
}