global using vec2 = Astral.Tools.Vec2<float>;
global using ivec2 = Astral.Tools.Vec2<int>;
global using uvec2 = Astral.Tools.Vec2<uint>;

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

[DebuggerDisplay("Vec2({x}, {y})")]
public struct Vec2<T> where T : unmanaged, INumber<T>
{
	public T x;
	public T y;

	public T this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			_ => default,
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (i) {
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					break;
			}
		}
	}

	#region COORDINATES

	public Vec2<T> xx => new Vec2<T>(x, x);
	public Vec2<T> yy => new Vec2<T>(y, y);

	public Vec2<T> xy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
		}
	}

	public Vec2<T> yx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
		}
	}

	#endregion //COORDINATES

	#region COLOR

	public T r
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => x = value;
	}

	public T g
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => y;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => y = value;
	}

	public Vec2<T> rr => new Vec2<T>(r, r);
	public Vec2<T> gg => new Vec2<T>(g, g);

	public Vec2<T> rg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.r;
			g = value.g;
		}
	}

	public Vec2<T> gr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(g, r);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.g;
			g = value.r;
		}
	}

	#endregion //COLOR

	#region TEXTURE

	public T s
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => x = value;
	}

	public T t
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => y;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => y = value;
	}

	public Vec2<T> ss => new Vec2<T>(s, s);
	public Vec2<T> tt => new Vec2<T>(t, t);

	public Vec2<T> st
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.s;
			t = value.t;
		}
	}

	public Vec2<T> ts
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(t, s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.t;
			t = value.s;
		}
	}

	#endregion //TEXTURE

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2(T value)
	{
		x = value;
		y = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2(T x, T y)
	{
		this.x = x;
		this.y = y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec2(Vec2<T> vec)
	{
		x = vec.x;
		y = vec.y;
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator+(Vec2<T> a, Vec2<T> b)
	{
		return new Vec2<T>(
			a.x + b.x,
			a.y + b.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator-(Vec2<T> a, Vec2<T> b)
	{
		return new Vec2<T>(
			a.x - b.x,
			a.y - b.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator*(Vec2<T> a, Vec2<T> b)
	{
		return new Vec2<T>(
			a.x * b.x,
			a.y * b.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator/(Vec2<T> a, Vec2<T> b)
	{
		return new Vec2<T>(
			a.x / b.x,
			a.y / b.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator+(Vec2<T> vec)
	{
		return vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator-(Vec2<T> vec)
	{
		return new Vec2<T>(
			-vec.x,
			-vec.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator*(Vec2<T> vec, T scalar)
	{
		return new Vec2<T>(
			vec.x * scalar,
			vec.y * scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator*(T scalar, Vec2<T> vec)
	{
		return new Vec2<T>(
			scalar * vec.x,
			scalar * vec.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator/(Vec2<T> vec, T scalar)
	{
		return new Vec2<T>(
			vec.x / scalar,
			vec.y / scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec2<T> operator/(T scalar, Vec2<T> vec)
	{
		return new Vec2<T>(
			scalar / vec.x,
			scalar / vec.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(Vec2<T> a, Vec2<T> b)
	{
		return a.x == b.x && a.y == b.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(Vec2<T> a, Vec2<T> b)
	{
		return !(a == b);
	}

	#endregion // OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override bool Equals(object pObject)
	{
		return pObject is Vec2<T> lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override string ToString()
	{
		return $"Vec2<{typeof(T).Name}>({x}, {y})";
	}
}

[DebuggerDisplay("bvec2({x}, {y})")]
public struct bvec2
{
	public bool x;
	public bool y;

	public bool this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			_ => false,
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (i)
			{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					break;
			}
		}
	}

	#region COORDINATES

	public bvec2 xx => new bvec2(x, x);
	public bvec2 yy => new bvec2(y, y);

	public bvec2 xy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
		}
	}

	public bvec2 yx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
		}
	}

	#endregion //COORDINATES

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec2(bool value)
	{
		x = value;
		y = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec2(bool x, bool y)
	{
		this.x = x;
		this.y = y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec2(bvec2 vec)
	{
		x = vec.x;
		y = vec.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec2(int vec)
	{
		x = Convert.ToBoolean(vec & 0b01);
		y = Convert.ToBoolean(vec & 0b10);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 operator!(bvec2 x)
	{
		return new bvec2(!x.x, !x.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 operator&(bvec2 a, bvec2 b)
	{
		return new bvec2(a.x && b.x, a.y && b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 operator|(bvec2 a, bvec2 b)
	{
		return new bvec2(a.x || b.x, a.y || b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 operator^(bvec2 a, bvec2 b)
	{
		return new bvec2(a.x != b.x, a.y != b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(bvec2 a, bvec2 b)
	{
		return a.x == b.x && a.y == b.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(bvec2 a, bvec2 b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec2(int vec)
	{
		return new bvec2(vec);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int(bvec2 vec)
	{
		return Convert.ToInt32(vec.x) & (Convert.ToInt32(vec.y) << 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec2(Variant vec)
	{
		return (bvec2)vec.AsInt32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(bvec2 vec)
	{
		return (int)vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is bvec2 lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public override string ToString()
	{
		return $"bvec2({x}, {y})";
	}
}