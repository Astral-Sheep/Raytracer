global using vec3 = Astral.Tools.Vec3<float>;
global using ivec3 = Astral.Tools.Vec3<int>;
global using uvec3 = Astral.Tools.Vec3<uint>;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

public struct Vec3<T> where T : unmanaged, INumber<T>
{
	public T x;
	public T y;
	public T z;

	public T this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			2 => z,
			_ => default,
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
				case 2:
					z = value;
					break;
				default:
					break;
			}
		}
	}

	#region COORDINATES

	public Vec2<T> xx => new Vec2<T>(x, x);
	public Vec2<T> yy => new Vec2<T>(y, y);
	public Vec2<T> zz => new Vec2<T>(z, z);

	public Vec3<T> xxx => new Vec3<T>(x, x, x);
	public Vec3<T> xxy => new Vec3<T>(x, x, y);
	public Vec3<T> xxz => new Vec3<T>(x, x, z);
	public Vec3<T> xyx => new Vec3<T>(x, y, x);
	public Vec3<T> xyy => new Vec3<T>(x, y, y);
	public Vec3<T> xzx => new Vec3<T>(x, z, x);
	public Vec3<T> xzz => new Vec3<T>(x, z, z);
	public Vec3<T> yxx => new Vec3<T>(y, x, x);
	public Vec3<T> yxy => new Vec3<T>(y, x, y);
	public Vec3<T> yyx => new Vec3<T>(y, y, x);
	public Vec3<T> yyy => new Vec3<T>(y, y, y);
	public Vec3<T> yyz => new Vec3<T>(y, y, z);
	public Vec3<T> yzy => new Vec3<T>(y, z, y);
	public Vec3<T> yzz => new Vec3<T>(y, z, z);
	public Vec3<T> zxx => new Vec3<T>(z, x, x);
	public Vec3<T> zxz => new Vec3<T>(z, x, z);
	public Vec3<T> zyy => new Vec3<T>(z, y, y);
	public Vec3<T> zyz => new Vec3<T>(z, y, z);
	public Vec3<T> zzx => new Vec3<T>(z, z, x);
	public Vec3<T> zzy => new Vec3<T>(z, z, y);
	public Vec3<T> zzz => new Vec3<T>(z, z, z);

	public Vec2<T> xy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
		}
	}

	public Vec2<T> xz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
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

	public Vec2<T> yz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
		}
	}

	public Vec2<T> zx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
		}
	}

	public Vec2<T> zy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
		}
	}

	public Vec3<T> xyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public Vec3<T> xzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			y = value.z;
		}
	}

	public Vec3<T> yxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			z = value.z;
		}
	}

	public Vec3<T> yzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			x = value.z;
		}
	}

	public Vec3<T> zxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			y = value.z;
		}
	}

	public Vec3<T> zyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			x = value.z;
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

	public T b
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => z;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => z = value;
	}


	public Vec2<T> rr => new Vec2<T>(r, r);
	public Vec2<T> gg => new Vec2<T>(g, g);
	public Vec2<T> bb => new Vec2<T>(b, b);

	public Vec3<T> rrr => new Vec3<T>(r, r, r);
	public Vec3<T> rrg => new Vec3<T>(r, r, g);
	public Vec3<T> rrb => new Vec3<T>(r, r, b);
	public Vec3<T> rgr => new Vec3<T>(r, g, r);
	public Vec3<T> rgg => new Vec3<T>(r, g, g);
	public Vec3<T> rbr => new Vec3<T>(r, b, r);
	public Vec3<T> rbb => new Vec3<T>(r, b, b);
	public Vec3<T> grr => new Vec3<T>(g, r, r);
	public Vec3<T> grg => new Vec3<T>(g, r, g);
	public Vec3<T> ggr => new Vec3<T>(g, g, r);
	public Vec3<T> ggg => new Vec3<T>(g, g, g);
	public Vec3<T> ggb => new Vec3<T>(g, g, b);
	public Vec3<T> gbg => new Vec3<T>(g, b, g);
	public Vec3<T> gbb => new Vec3<T>(g, b, b);
	public Vec3<T> brr => new Vec3<T>(b, r, r);
	public Vec3<T> brb => new Vec3<T>(b, r, b);
	public Vec3<T> bgg => new Vec3<T>(b, g, g);
	public Vec3<T> bgb => new Vec3<T>(b, g, b);
	public Vec3<T> bbr => new Vec3<T>(b, b, r);
	public Vec3<T> bbg => new Vec3<T>(b, b, g);
	public Vec3<T> bbb => new Vec3<T>(b, b, b);

	public Vec2<T> rg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(r, g);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.x;
			g = value.y;
		}
	}

	public Vec2<T> rb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(r, b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.x;
			b = value.y;
		}
	}

	public Vec2<T> gr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(g, r);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			g = value.x;
			r = value.y;
		}
	}

	public Vec2<T> gb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(g, b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			g = value.x;
			b = value.y;
		}
	}

	public Vec2<T> br
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(b, r);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			b = value.x;
			r = value.y;
		}
	}

	public Vec2<T> bg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(b, g);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			b = value.x;
			g = value.y;
		}
	}

	public Vec3<T> rgb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.x;
			g = value.y;
			b = value.z;
		}
	}

	public Vec3<T> rbg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(r, b, g);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			r = value.x;
			b = value.y;
			g = value.z;
		}
	}

	public Vec3<T> grb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(g, r, b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			g = value.x;
			r = value.y;
			b = value.z;
		}
	}

	public Vec3<T> gbr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(g, b, r);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			g = value.x;
			b = value.y;
			r = value.z;
		}
	}

	public Vec3<T> brg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(b, r, g);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			b = value.x;
			r = value.y;
			g = value.z;
		}
	}

	public Vec3<T> bgr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(b, g, r);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			b = value.x;
			g = value.y;
			r = value.z;
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

	public T p
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => z;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => z = value;
	}


	public Vec2<T> ss => new Vec2<T>(s, s);
	public Vec2<T> tt => new Vec2<T>(t, t);
	public Vec2<T> pp => new Vec2<T>(p, p);

	public Vec3<T> sss => new Vec3<T>(s, s, s);
	public Vec3<T> sst => new Vec3<T>(s, s, t);
	public Vec3<T> ssp => new Vec3<T>(s, s, p);
	public Vec3<T> sts => new Vec3<T>(s, t, s);
	public Vec3<T> stt => new Vec3<T>(s, t, t);
	public Vec3<T> sps => new Vec3<T>(s, p, s);
	public Vec3<T> spp => new Vec3<T>(s, p, p);
	public Vec3<T> tss => new Vec3<T>(t, s, s);
	public Vec3<T> tst => new Vec3<T>(t, s, t);
	public Vec3<T> tts => new Vec3<T>(t, t, s);
	public Vec3<T> ttt => new Vec3<T>(t, t, t);
	public Vec3<T> ttp => new Vec3<T>(t, t, p);
	public Vec3<T> tpt => new Vec3<T>(t, p, t);
	public Vec3<T> tpp => new Vec3<T>(t, p, p);
	public Vec3<T> pss => new Vec3<T>(p, s, s);
	public Vec3<T> psp => new Vec3<T>(p, s, p);
	public Vec3<T> ptt => new Vec3<T>(p, t, t);
	public Vec3<T> ptp => new Vec3<T>(p, t, p);
	public Vec3<T> pps => new Vec3<T>(p, p, s);
	public Vec3<T> ppt => new Vec3<T>(p, p, t);
	public Vec3<T> ppp => new Vec3<T>(p, p, p);

	public Vec2<T> st
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(s, t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.x;
			t = value.y;
		}
	}

	public Vec2<T> sp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(s, p);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.x;
			p = value.y;
		}
	}

	public Vec2<T> ts
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(t, s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			t = value.x;
			s = value.y;
		}
	}

	public Vec2<T> tp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(t, p);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.x;
			p = value.y;
		}
	}

	public Vec2<T> ps
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(p, s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			p = value.x;
			s = value.y;
		}
	}

	public Vec2<T> pt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(p, t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			p = value.x;
			t = value.y;
		}
	}

	public Vec3<T> stp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.x;
			t = value.y;
			p = value.z;
		}
	}

	public Vec3<T> spt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(s, p, t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			s = value.x;
			p = value.y;
			t = value.z;
		}
	}

	public Vec3<T> tsp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(t, s, p);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			t = value.x;
			s = value.y;
			p = value.z;
		}
	}

	public Vec3<T> tps
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(t, p, s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			t = value.x;
			p = value.y;
			s = value.z;
		}
	}

	public Vec3<T> pst
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(p, s, t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			p = value.x;
			s = value.y;
			t = value.z;
		}
	}

	public Vec3<T> pts
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(p, t, s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			p = value.x;
			t = value.y;
			s = value.z;
		}
	}

	#endregion //TEXTURE

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(T value)
	{
		x = value;
		y = value;
		z = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(T x, T y, T z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(T x, Vec2<T> yz)
	{
		this.x = x;
		y = yz.x;
		z = yz.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(Vec2<T> xy, T z)
	{
		x = xy.x;
		y = xy.y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec3(Vec3<T> vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator+(Vec3<T> a, Vec3<T> b)
	{
		return new Vec3<T>(
			a.x + b.x,
			a.y + b.y,
			a.z + b.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator-(Vec3<T> a, Vec3<T> b)
	{
		return new Vec3<T>(
			a.x - b.x,
			a.y - b.y,
			a.z - b.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator*(Vec3<T> a, Vec3<T> b)
	{
		return new Vec3<T>(
			a.x * b.x,
			a.y * b.y,
			a.z * b.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator/(Vec3<T> a, Vec3<T> b)
	{
		return new Vec3<T>(
			a.x / b.x,
			a.y / b.y,
			a.z / b.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator+(Vec3<T> vec)
	{
		return vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator-(Vec3<T> vec)
	{
		return new Vec3<T>(
			-vec.x,
			-vec.y,
			-vec.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator*(Vec3<T> vec, T scalar)
	{
		return new Vec3<T>(
			vec.x * scalar,
			vec.y * scalar,
			vec.z * scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator*(T scalar, Vec3<T> vec)
	{
		return new Vec3<T>(
			scalar * vec.x,
			scalar * vec.y,
			scalar * vec.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator/(Vec3<T> vec, T scalar)
	{
		return new Vec3<T>(
			vec.x / scalar,
			vec.y / scalar,
			vec.z / scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3<T> operator/(T scalar, Vec3<T> vec)
	{
		return new Vec3<T>(
			scalar / vec.x,
			scalar / vec.y,
			scalar / vec.z
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(Vec3<T> a, Vec3<T> b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(Vec3<T> a, Vec3<T> b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is Vec3<T> lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
	}
}

public struct bvec3
{
	public bool x;
	public bool y;
	public bool z;

	public bool this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			2 => z,
			_ => default,
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
				case 2:
					z = value;
					break;
				default:
					break;
			}
		}
	}

	#region COORDINATES

	public bvec2 xx => new bvec2(x, x);
	public bvec2 yy => new bvec2(y, y);
	public bvec2 zz => new bvec2(z, z);

	public bvec3 xxx => new bvec3(x, x, x);
	public bvec3 xxy => new bvec3(x, x, y);
	public bvec3 xxz => new bvec3(x, x, z);
	public bvec3 xyx => new bvec3(x, y, x);
	public bvec3 xyy => new bvec3(x, y, y);
	public bvec3 xzx => new bvec3(x, z, x);
	public bvec3 xzz => new bvec3(x, z, z);
	public bvec3 yxx => new bvec3(y, x, x);
	public bvec3 yxy => new bvec3(y, x, y);
	public bvec3 yyx => new bvec3(y, y, x);
	public bvec3 yyy => new bvec3(y, y, y);
	public bvec3 yyz => new bvec3(y, y, z);
	public bvec3 yzy => new bvec3(y, z, y);
	public bvec3 yzz => new bvec3(y, z, z);
	public bvec3 zxx => new bvec3(z, x, x);
	public bvec3 zxz => new bvec3(z, x, z);
	public bvec3 zyy => new bvec3(z, y, y);
	public bvec3 zyz => new bvec3(z, y, z);
	public bvec3 zzx => new bvec3(z, z, x);
	public bvec3 zzy => new bvec3(z, z, y);
	public bvec3 zzz => new bvec3(z, z, z);

	public bvec2 xy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
		}
	}

	public bvec2 xz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
		}
	}

	public bvec2 yx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
		}
	}

	public bvec2 yz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
		}
	}

	public bvec2 zx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
		}
	}

	public bvec2 zy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
		}
	}

	public bvec3 xyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public bvec3 xzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			y = value.z;
		}
	}

	public bvec3 yxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			z = value.z;
		}
	}

	public bvec3 yzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			x = value.z;
		}
	}

	public bvec3 zxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			y = value.z;
		}
	}

	public bvec3 zyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			x = value.z;
		}
	}

	#endregion //CORDINATES

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(bool value)
	{
		x = value;
		y = value;
		z = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(bool x, bool y, bool z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(bool x, bvec2 yz)
	{
		this.x = x;
		y = yz.x;
		z = yz.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(bvec2 xy, bool z)
	{
		x = xy.x;
		y = xy.y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(bvec3 vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec3(int vec)
	{
		x = Convert.ToBoolean(vec & 0b001);
		y = Convert.ToBoolean(vec & 0b010);
		z = Convert.ToBoolean(vec & 0b100);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 operator!(bvec3 x)
	{
		return new bvec3(!x.x, !x.y, !x.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 operator&(bvec3 a, bvec3 b)
	{
		return new bvec3(a.x && b.x, a.y && b.y, a.z && b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 operator|(bvec3 a, bvec3 b)
	{
		return new bvec3(a.x || b.x, a.y || b.y, a.z || b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 operator^(bvec3 a, bvec3 b)
	{
		return new bvec3(a.x != b.x, a.y != b.y, a.z != b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(bvec3 a, bvec3 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(bvec3 a, bvec3 b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec3(int vec)
	{
		return new bvec3(vec);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int(bvec3 vec)
	{
		return Convert.ToInt32(vec.x) & (Convert.ToInt32(vec.y) << 1) & (Convert.ToInt32(vec.z) << 2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec3(Variant vec)
	{
		return (bvec3)vec.AsInt32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(bvec3 vec)
	{
		return (int)vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is bvec3 lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
	}
}