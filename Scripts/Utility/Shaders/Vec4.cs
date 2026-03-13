global using vec4 = Astral.Tools.Vec4<float>;
global using ivec4 = Astral.Tools.Vec4<int>;
global using uvec4 = Astral.Tools.Vec4<uint>;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

public struct Vec4<T> where T : unmanaged, INumber<T>
{
	public T x;
	public T y;
	public T z;
	public T w;

	public T this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			2 => z,
			3 => w,
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
				case 3:
					w = value;
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
	public Vec2<T> ww => new Vec2<T>(w, w);

	public Vec3<T> xxx => new Vec3<T>(x, x, x);
	public Vec3<T> xxy => new Vec3<T>(x, x, y);
	public Vec3<T> xxz => new Vec3<T>(x, x, z);
	public Vec3<T> xxw => new Vec3<T>(x, x, w);
	public Vec3<T> xyx => new Vec3<T>(x, y, x);
	public Vec3<T> xyy => new Vec3<T>(x, y, y);
	public Vec3<T> xzx => new Vec3<T>(x, z, x);
	public Vec3<T> xzz => new Vec3<T>(x, z, z);
	public Vec3<T> xwx => new Vec3<T>(x, w, x);
	public Vec3<T> xww => new Vec3<T>(x, w, w);
	public Vec3<T> yxx => new Vec3<T>(y, x, x);
	public Vec3<T> yxy => new Vec3<T>(y, x, y);
	public Vec3<T> yyx => new Vec3<T>(y, y, x);
	public Vec3<T> yyy => new Vec3<T>(y, y, y);
	public Vec3<T> yyz => new Vec3<T>(y, y, z);
	public Vec3<T> yyw => new Vec3<T>(y, y, w);
	public Vec3<T> yzy => new Vec3<T>(y, z, y);
	public Vec3<T> yzz => new Vec3<T>(y, z, z);
	public Vec3<T> ywy => new Vec3<T>(y, w, y);
	public Vec3<T> yww => new Vec3<T>(y, w, w);
	public Vec3<T> zxx => new Vec3<T>(z, x, x);
	public Vec3<T> zxz => new Vec3<T>(z, x, z);
	public Vec3<T> zyy => new Vec3<T>(z, y, y);
	public Vec3<T> zyz => new Vec3<T>(z, y, z);
	public Vec3<T> zzx => new Vec3<T>(z, z, x);
	public Vec3<T> zzy => new Vec3<T>(z, z, y);
	public Vec3<T> zzz => new Vec3<T>(z, z, z);
	public Vec3<T> zzw => new Vec3<T>(z, z, w);
	public Vec3<T> zwz => new Vec3<T>(z, w, z);
	public Vec3<T> zww => new Vec3<T>(z, w, w);
	public Vec3<T> wxx => new Vec3<T>(w, x, x);
	public Vec3<T> wxw => new Vec3<T>(w, x, w);
	public Vec3<T> wyy => new Vec3<T>(w, y, y);
	public Vec3<T> wyw => new Vec3<T>(w, y, w);
	public Vec3<T> wzz => new Vec3<T>(w, z, z);
	public Vec3<T> wzw => new Vec3<T>(w, z, w);
	public Vec3<T> wwx => new Vec3<T>(w, w, x);
	public Vec3<T> wwy => new Vec3<T>(w, w, y);
	public Vec3<T> wwz => new Vec3<T>(w, w, z);
	public Vec3<T> www => new Vec3<T>(w, w, w);

	public Vec4<T> xxxx => new Vec4<T>(x, x, x, x);
	public Vec4<T> xxxy => new Vec4<T>(x, x, x, y);
	public Vec4<T> xxxz => new Vec4<T>(x, x, x, z);
	public Vec4<T> xxxw => new Vec4<T>(x, x, x, w);
	public Vec4<T> xxyx => new Vec4<T>(x, x, y, x);
	public Vec4<T> xxyy => new Vec4<T>(x, x, y, y);
	public Vec4<T> xxyz => new Vec4<T>(x, x, y, z);
	public Vec4<T> xxyw => new Vec4<T>(x, x, y, w);
	public Vec4<T> xxzx => new Vec4<T>(x, x, z, x);
	public Vec4<T> xxzy => new Vec4<T>(x, x, z, y);
	public Vec4<T> xxzz => new Vec4<T>(x, x, z, z);
	public Vec4<T> xxzw => new Vec4<T>(x, x, z, w);
	public Vec4<T> xxwx => new Vec4<T>(x, x, w, x);
	public Vec4<T> xxwy => new Vec4<T>(x, x, w, y);
	public Vec4<T> xxwz => new Vec4<T>(x, x, w, z);
	public Vec4<T> xxww => new Vec4<T>(x, x, w, w);
	public Vec4<T> xyxx => new Vec4<T>(x, y, x, x);
	public Vec4<T> xyxy => new Vec4<T>(x, y, x, y);
	public Vec4<T> xyxz => new Vec4<T>(x, y, x, z);
	public Vec4<T> xyxw => new Vec4<T>(x, y, x, w);
	public Vec4<T> xyyx => new Vec4<T>(x, y, y, x);
	public Vec4<T> xyyy => new Vec4<T>(x, y, y, y);
	public Vec4<T> xyyz => new Vec4<T>(x, y, y, z);
	public Vec4<T> xyyw => new Vec4<T>(x, y, y, w);
	public Vec4<T> xyzx => new Vec4<T>(x, y, z, x);
	public Vec4<T> xyzy => new Vec4<T>(x, y, z, y);
	public Vec4<T> xyzz => new Vec4<T>(x, y, z, z);
	public Vec4<T> xywx => new Vec4<T>(x, y, w, x);
	public Vec4<T> xywy => new Vec4<T>(x, y, w, y);
	public Vec4<T> xyww => new Vec4<T>(x, y, w, w);
	public Vec4<T> xzxx => new Vec4<T>(x, z, x, x);
	public Vec4<T> xzxy => new Vec4<T>(x, z, x, y);
	public Vec4<T> xzxz => new Vec4<T>(x, z, x, z);
	public Vec4<T> xzxw => new Vec4<T>(x, z, x, w);
	public Vec4<T> xzyx => new Vec4<T>(x, z, y, x);
	public Vec4<T> xzyy => new Vec4<T>(x, z, y, y);
	public Vec4<T> xzyz => new Vec4<T>(x, z, y, z);
	public Vec4<T> xzzx => new Vec4<T>(x, z, z, x);
	public Vec4<T> xzzy => new Vec4<T>(x, z, z, y);
	public Vec4<T> xzzz => new Vec4<T>(x, z, z, z);
	public Vec4<T> xzzw => new Vec4<T>(x, z, z, w);
	public Vec4<T> xzwx => new Vec4<T>(x, z, w, x);
	public Vec4<T> xzwz => new Vec4<T>(x, z, w, z);
	public Vec4<T> xzww => new Vec4<T>(x, z, w, w);
	public Vec4<T> xwxx => new Vec4<T>(x, w, x, x);
	public Vec4<T> xwxy => new Vec4<T>(x, w, x, y);
	public Vec4<T> xwxz => new Vec4<T>(x, w, x, z);
	public Vec4<T> xwxw => new Vec4<T>(x, w, x, w);
	public Vec4<T> xwyx => new Vec4<T>(x, w, y, x);
	public Vec4<T> xwyy => new Vec4<T>(x, w, y, y);
	public Vec4<T> xwyw => new Vec4<T>(x, w, y, w);
	public Vec4<T> xwzx => new Vec4<T>(x, w, z, x);
	public Vec4<T> xwzz => new Vec4<T>(x, w, z, z);
	public Vec4<T> xwzw => new Vec4<T>(x, w, z, w);
	public Vec4<T> xwwx => new Vec4<T>(x, w, w, x);
	public Vec4<T> xwwy => new Vec4<T>(x, w, w, y);
	public Vec4<T> xwwz => new Vec4<T>(x, w, w, z);
	public Vec4<T> xwww => new Vec4<T>(x, w, w, w);
	public Vec4<T> yxxx => new Vec4<T>(y, x, x, x);
	public Vec4<T> yxxy => new Vec4<T>(y, x, x, y);
	public Vec4<T> yxxz => new Vec4<T>(y, x, x, z);
	public Vec4<T> yxxw => new Vec4<T>(y, x, x, w);
	public Vec4<T> yxyx => new Vec4<T>(y, x, y, x);
	public Vec4<T> yxyy => new Vec4<T>(y, x, y, y);
	public Vec4<T> yxyz => new Vec4<T>(y, x, y, z);
	public Vec4<T> yxyw => new Vec4<T>(y, x, y, w);
	public Vec4<T> yxzx => new Vec4<T>(y, x, z, x);
	public Vec4<T> yxzy => new Vec4<T>(y, x, z, y);
	public Vec4<T> yxzz => new Vec4<T>(y, x, z, z);
	public Vec4<T> yxwx => new Vec4<T>(y, x, w, x);
	public Vec4<T> yxwy => new Vec4<T>(y, x, w, y);
	public Vec4<T> yxww => new Vec4<T>(y, x, w, w);
	public Vec4<T> yyxx => new Vec4<T>(y, y, x, x);
	public Vec4<T> yyxy => new Vec4<T>(y, y, x, y);
	public Vec4<T> yyxz => new Vec4<T>(y, y, x, z);
	public Vec4<T> yyxw => new Vec4<T>(y, y, x, w);
	public Vec4<T> yyyx => new Vec4<T>(y, y, y, x);
	public Vec4<T> yyyy => new Vec4<T>(y, y, y, y);
	public Vec4<T> yyyz => new Vec4<T>(y, y, y, z);
	public Vec4<T> yyyw => new Vec4<T>(y, y, y, w);
	public Vec4<T> yyzx => new Vec4<T>(y, y, z, x);
	public Vec4<T> yyzy => new Vec4<T>(y, y, z, y);
	public Vec4<T> yyzz => new Vec4<T>(y, y, z, z);
	public Vec4<T> yyzw => new Vec4<T>(y, y, z, w);
	public Vec4<T> yywx => new Vec4<T>(y, y, w, x);
	public Vec4<T> yywy => new Vec4<T>(y, y, w, y);
	public Vec4<T> yywz => new Vec4<T>(y, y, w, z);
	public Vec4<T> yyww => new Vec4<T>(y, y, w, w);
	public Vec4<T> yzxx => new Vec4<T>(y, z, x, x);
	public Vec4<T> yzxy => new Vec4<T>(y, z, x, y);
	public Vec4<T> yzxz => new Vec4<T>(y, z, x, z);
	public Vec4<T> yzyx => new Vec4<T>(y, z, y, x);
	public Vec4<T> yzyy => new Vec4<T>(y, z, y, y);
	public Vec4<T> yzyz => new Vec4<T>(y, z, y, z);
	public Vec4<T> yzyw => new Vec4<T>(y, z, y, w);
	public Vec4<T> yzzx => new Vec4<T>(y, z, z, x);
	public Vec4<T> yzzy => new Vec4<T>(y, z, z, y);
	public Vec4<T> yzzz => new Vec4<T>(y, z, z, z);
	public Vec4<T> yzzw => new Vec4<T>(y, z, z, w);
	public Vec4<T> yzwy => new Vec4<T>(y, z, w, y);
	public Vec4<T> yzwz => new Vec4<T>(y, z, w, z);
	public Vec4<T> yzww => new Vec4<T>(y, z, w, w);
	public Vec4<T> ywxx => new Vec4<T>(y, w, x, x);
	public Vec4<T> ywxy => new Vec4<T>(y, w, x, y);
	public Vec4<T> ywxw => new Vec4<T>(y, w, x, w);
	public Vec4<T> ywyx => new Vec4<T>(y, w, y, x);
	public Vec4<T> ywyy => new Vec4<T>(y, w, y, y);
	public Vec4<T> ywyz => new Vec4<T>(y, w, y, z);
	public Vec4<T> ywyw => new Vec4<T>(y, w, y, w);
	public Vec4<T> ywzy => new Vec4<T>(y, w, z, y);
	public Vec4<T> ywzz => new Vec4<T>(y, w, z, z);
	public Vec4<T> ywzw => new Vec4<T>(y, w, z, w);
	public Vec4<T> ywwx => new Vec4<T>(y, w, w, x);
	public Vec4<T> ywwy => new Vec4<T>(y, w, w, y);
	public Vec4<T> ywwz => new Vec4<T>(y, w, w, z);
	public Vec4<T> ywww => new Vec4<T>(y, w, w, w);
	public Vec4<T> zxxx => new Vec4<T>(z, x, x, x);
	public Vec4<T> zxxy => new Vec4<T>(z, x, x, y);
	public Vec4<T> zxxz => new Vec4<T>(z, x, x, z);
	public Vec4<T> zxxw => new Vec4<T>(z, x, x, w);
	public Vec4<T> zxyx => new Vec4<T>(z, x, y, x);
	public Vec4<T> zxyy => new Vec4<T>(z, x, y, y);
	public Vec4<T> zxyz => new Vec4<T>(z, x, y, z);
	public Vec4<T> zxzx => new Vec4<T>(z, x, z, x);
	public Vec4<T> zxzy => new Vec4<T>(z, x, z, y);
	public Vec4<T> zxzz => new Vec4<T>(z, x, z, z);
	public Vec4<T> zxzw => new Vec4<T>(z, x, z, w);
	public Vec4<T> zxwx => new Vec4<T>(z, x, w, x);
	public Vec4<T> zxwz => new Vec4<T>(z, x, w, z);
	public Vec4<T> zxww => new Vec4<T>(z, x, w, w);
	public Vec4<T> zyxx => new Vec4<T>(z, y, x, x);
	public Vec4<T> zyxy => new Vec4<T>(z, y, x, y);
	public Vec4<T> zyxz => new Vec4<T>(z, y, x, z);
	public Vec4<T> zyyx => new Vec4<T>(z, y, y, x);
	public Vec4<T> zyyy => new Vec4<T>(z, y, y, y);
	public Vec4<T> zyyz => new Vec4<T>(z, y, y, z);
	public Vec4<T> zyyw => new Vec4<T>(z, y, y, w);
	public Vec4<T> zyzx => new Vec4<T>(z, y, z, x);
	public Vec4<T> zyzy => new Vec4<T>(z, y, z, y);
	public Vec4<T> zyzz => new Vec4<T>(z, y, z, z);
	public Vec4<T> zyzw => new Vec4<T>(z, y, z, w);
	public Vec4<T> zywy => new Vec4<T>(z, y, w, y);
	public Vec4<T> zywz => new Vec4<T>(z, y, w, z);
	public Vec4<T> zyww => new Vec4<T>(z, y, w, w);
	public Vec4<T> zzxx => new Vec4<T>(z, z, x, x);
	public Vec4<T> zzxy => new Vec4<T>(z, z, x, y);
	public Vec4<T> zzxz => new Vec4<T>(z, z, x, z);
	public Vec4<T> zzxw => new Vec4<T>(z, z, x, w);
	public Vec4<T> zzyx => new Vec4<T>(z, z, y, x);
	public Vec4<T> zzyy => new Vec4<T>(z, z, y, y);
	public Vec4<T> zzyz => new Vec4<T>(z, z, y, z);
	public Vec4<T> zzyw => new Vec4<T>(z, z, y, w);
	public Vec4<T> zzzx => new Vec4<T>(z, z, z, x);
	public Vec4<T> zzzy => new Vec4<T>(z, z, z, y);
	public Vec4<T> zzzz => new Vec4<T>(z, z, z, z);
	public Vec4<T> zzzw => new Vec4<T>(z, z, z, w);
	public Vec4<T> zzwx => new Vec4<T>(z, z, w, x);
	public Vec4<T> zzwy => new Vec4<T>(z, z, w, y);
	public Vec4<T> zzwz => new Vec4<T>(z, z, w, z);
	public Vec4<T> zzww => new Vec4<T>(z, z, w, w);
	public Vec4<T> zwxx => new Vec4<T>(z, w, x, x);
	public Vec4<T> zwxz => new Vec4<T>(z, w, x, z);
	public Vec4<T> zwxw => new Vec4<T>(z, w, x, w);
	public Vec4<T> zwyy => new Vec4<T>(z, w, y, y);
	public Vec4<T> zwyz => new Vec4<T>(z, w, y, z);
	public Vec4<T> zwyw => new Vec4<T>(z, w, y, w);
	public Vec4<T> zwzx => new Vec4<T>(z, w, z, x);
	public Vec4<T> zwzy => new Vec4<T>(z, w, z, y);
	public Vec4<T> zwzz => new Vec4<T>(z, w, z, z);
	public Vec4<T> zwzw => new Vec4<T>(z, w, z, w);
	public Vec4<T> zwwx => new Vec4<T>(z, w, w, x);
	public Vec4<T> zwwy => new Vec4<T>(z, w, w, y);
	public Vec4<T> zwwz => new Vec4<T>(z, w, w, z);
	public Vec4<T> zwww => new Vec4<T>(z, w, w, w);

	public Vec4<T> wxxx => new Vec4<T>(w, x, x, x);
	public Vec4<T> wxxy => new Vec4<T>(w, x, x, y);
	public Vec4<T> wxxz => new Vec4<T>(w, x, x, z);
	public Vec4<T> wxxw => new Vec4<T>(w, x, x, w);
	public Vec4<T> wxyx => new Vec4<T>(w, x, y, x);
	public Vec4<T> wxyy => new Vec4<T>(w, x, y, y);
	public Vec4<T> wxyw => new Vec4<T>(w, x, y, w);
	public Vec4<T> wxzx => new Vec4<T>(w, x, z, x);
	public Vec4<T> wxzz => new Vec4<T>(w, x, z, z);
	public Vec4<T> wxzw => new Vec4<T>(w, x, z, w);
	public Vec4<T> wxwx => new Vec4<T>(w, x, w, x);
	public Vec4<T> wxwy => new Vec4<T>(w, x, w, y);
	public Vec4<T> wxwz => new Vec4<T>(w, x, w, z);
	public Vec4<T> wxww => new Vec4<T>(w, x, w, w);
	public Vec4<T> wyxx => new Vec4<T>(w, y, x, x);
	public Vec4<T> wyxy => new Vec4<T>(w, y, x, y);
	public Vec4<T> wyxw => new Vec4<T>(w, y, x, w);
	public Vec4<T> wyyx => new Vec4<T>(w, y, y, x);
	public Vec4<T> wyyy => new Vec4<T>(w, y, y, y);
	public Vec4<T> wyyz => new Vec4<T>(w, y, y, z);
	public Vec4<T> wyyw => new Vec4<T>(w, y, y, w);
	public Vec4<T> wyzy => new Vec4<T>(w, y, z, y);
	public Vec4<T> wyzz => new Vec4<T>(w, y, z, z);
	public Vec4<T> wyzw => new Vec4<T>(w, y, z, w);
	public Vec4<T> wywx => new Vec4<T>(w, y, w, x);
	public Vec4<T> wywy => new Vec4<T>(w, y, w, y);
	public Vec4<T> wywz => new Vec4<T>(w, y, w, z);
	public Vec4<T> wyww => new Vec4<T>(w, y, w, w);
	public Vec4<T> wzxx => new Vec4<T>(w, z, x, x);
	public Vec4<T> wzxz => new Vec4<T>(w, z, x, z);
	public Vec4<T> wzxw => new Vec4<T>(w, z, x, w);
	public Vec4<T> wzyy => new Vec4<T>(w, z, y, y);
	public Vec4<T> wzyz => new Vec4<T>(w, z, y, z);
	public Vec4<T> wzyw => new Vec4<T>(w, z, y, w);
	public Vec4<T> wzzx => new Vec4<T>(w, z, z, x);
	public Vec4<T> wzzy => new Vec4<T>(w, z, z, y);
	public Vec4<T> wzzz => new Vec4<T>(w, z, z, z);
	public Vec4<T> wzzw => new Vec4<T>(w, z, z, w);
	public Vec4<T> wzwx => new Vec4<T>(w, z, w, x);
	public Vec4<T> wzwy => new Vec4<T>(w, z, w, y);
	public Vec4<T> wzwz => new Vec4<T>(w, z, w, z);
	public Vec4<T> wzww => new Vec4<T>(w, z, w, w);
	public Vec4<T> wwxx => new Vec4<T>(w, w, x, x);
	public Vec4<T> wwxy => new Vec4<T>(w, w, x, y);
	public Vec4<T> wwxz => new Vec4<T>(w, w, x, z);
	public Vec4<T> wwxw => new Vec4<T>(w, w, x, w);
	public Vec4<T> wwyx => new Vec4<T>(w, w, y, x);
	public Vec4<T> wwyy => new Vec4<T>(w, w, y, y);
	public Vec4<T> wwyz => new Vec4<T>(w, w, y, z);
	public Vec4<T> wwyw => new Vec4<T>(w, w, y, w);
	public Vec4<T> wwzx => new Vec4<T>(w, w, z, x);
	public Vec4<T> wwzy => new Vec4<T>(w, w, z, y);
	public Vec4<T> wwzz => new Vec4<T>(w, w, z, z);
	public Vec4<T> wwzw => new Vec4<T>(w, w, z, w);
	public Vec4<T> wwwx => new Vec4<T>(w, w, w, x);
	public Vec4<T> wwwy => new Vec4<T>(w, w, w, y);
	public Vec4<T> wwwz => new Vec4<T>(w, w, w, z);
	public Vec4<T> wwww => new Vec4<T>(w, w, w, w);

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

	public Vec2<T> xw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
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

	public Vec2<T> yw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
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

	public Vec2<T> zw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec2<T>(z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
		}
	}

	public Vec3<T> xyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public Vec3<T> xyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			w = value.z;
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

	public Vec3<T> xzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			w = value.z;
		}
	}

	public Vec3<T> xwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			y = value.z;
		}
	}

	public Vec3<T> xwz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(x, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			z = value.z;
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

	public Vec3<T> yxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			w = value.z;
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

	public Vec3<T> yzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			w = value.z;
		}
	}

	public Vec3<T> ywx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			x = value.z;
		}
	}

	public Vec3<T> ywz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(y, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			z = value.z;
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

	public Vec3<T> zxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			w = value.z;
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

	public Vec3<T> zyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			w = value.z;
		}
	}

	public Vec3<T> zwx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			x = value.z;
		}
	}

	public Vec3<T> zwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(z, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			y = value.z;
		}
	}

	public Vec3<T> wxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			y = value.z;
		}
	}

	public Vec3<T> wxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			z = value.z;
		}
	}

	public Vec3<T> wyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			x = value.z;
		}
	}

	public Vec3<T> wyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public Vec3<T> wzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			x = value.z;
		}
	}

	public Vec3<T> wzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec3<T>(w, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			y = value.z;
		}
	}

	public Vec4<T> xyzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
			w = value.w;
		}
	}

	public Vec4<T> xywz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(x, y, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			w = value.z;
			z = value.w;
		}
	}

	public Vec4<T> xzyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(x, z, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			y = value.z;
			w = value.w;
		}
	}

	public Vec4<T> xzwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(x, z, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			w = value.z;
			y = value.w;
		}
	}

	public Vec4<T> xwyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(x, w, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			y = value.z;
			z = value.w;
		}
	}

	public Vec4<T> xwzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(x, w, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			z = value.z;
			y = value.w;
		}
	}

	public Vec4<T> yxzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, x, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			z = value.z;
			w = value.w;
		}
	}

	public Vec4<T> yxwz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, x, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			w = value.z;
			z = value.w;
		}
	}

	public Vec4<T> yzxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, z, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			x = value.z;
			w = value.w;
		}
	}

	public Vec4<T> yzwx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, z, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			w = value.z;
			x = value.w;
		}
	}

	public Vec4<T> ywxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, w, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			x = value.z;
			z = value.w;
		}
	}

	public Vec4<T> ywzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(y, w, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			z = value.z;
			x = value.w;
		}
	}

	public Vec4<T> zxyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, x, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			y = value.z;
			w = value.w;
		}
	}

	public Vec4<T> zxwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, x, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			w = value.z;
			y = value.w;
		}
	}

	public Vec4<T> zyxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, y, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			x = value.z;
			w = value.w;
		}
	}

	public Vec4<T> zywx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, y, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			w = value.z;
			x = value.w;
		}
	}

	public Vec4<T> zwxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, w, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			x = value.z;
			y = value.w;
		}
	}

	public Vec4<T> zwyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(z, w, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			y = value.z;
			x = value.w;
		}
	}

	public Vec4<T> wxyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, x, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			y = value.z;
			z = value.w;
		}
	}

	public Vec4<T> wxzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, x, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			z = value.z;
			y = value.w;
		}
	}

	public Vec4<T> wyxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, y, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			x = value.z;
			z = value.w;
		}
	}

	public Vec4<T> wyzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, y, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			z = value.z;
			x = value.w;
		}
	}

	public Vec4<T> wzxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, z, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			x = value.z;
			y = value.w;
		}
	}

	public Vec4<T> wzyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Vec4<T>(w, z, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			y = value.z;
			x = value.w;
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

	public T a
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => w;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => w = value;
	}

	public Vec2<T> rr => new Vec2<T>(r, r);
	public Vec2<T> gg => new Vec2<T>(g, g);
	public Vec2<T> bb => new Vec2<T>(b, b);
	public Vec2<T> aa => new Vec2<T>(a, a);

	public Vec3<T> rrt => new Vec3<T>(r, r, r);
	public Vec3<T> rrg => new Vec3<T>(r, r, g);
	public Vec3<T> rrb => new Vec3<T>(r, r, b);
	public Vec3<T> rra => new Vec3<T>(r, r, a);
	public Vec3<T> rgr => new Vec3<T>(r, g, b);
	public Vec3<T> rgg => new Vec3<T>(r, g, g);
	public Vec3<T> rbr => new Vec3<T>(r, b, r);
	public Vec3<T> rbb => new Vec3<T>(r, b, b);
	public Vec3<T> rar => new Vec3<T>(r, a, r);
	public Vec3<T> raa => new Vec3<T>(r, a, a);
	public Vec3<T> grr => new Vec3<T>(g, r, r);
	public Vec3<T> grg => new Vec3<T>(g, r, g);
	public Vec3<T> ggr => new Vec3<T>(g, g, r);
	public Vec3<T> ggg => new Vec3<T>(g, g, g);
	public Vec3<T> ggb => new Vec3<T>(g, g, b);
	public Vec3<T> gga => new Vec3<T>(g, g, a);
	public Vec3<T> gbg => new Vec3<T>(g, b, g);
	public Vec3<T> gbb => new Vec3<T>(g, b, b);
	public Vec3<T> gag => new Vec3<T>(g, a, g);
	public Vec3<T> gaa => new Vec3<T>(g, a, a);
	public Vec3<T> brr => new Vec3<T>(b, r, r);
	public Vec3<T> brb => new Vec3<T>(b, r, b);
	public Vec3<T> bgg => new Vec3<T>(b, g, g);
	public Vec3<T> bgb => new Vec3<T>(b, g, b);
	public Vec3<T> bbr => new Vec3<T>(b, b, r);
	public Vec3<T> bbg => new Vec3<T>(b, b, g);
	public Vec3<T> bbb => new Vec3<T>(b, b, b);
	public Vec3<T> bba => new Vec3<T>(b, b, a);
	public Vec3<T> bab => new Vec3<T>(b, a, b);
	public Vec3<T> baa => new Vec3<T>(b, a, a);
	public Vec3<T> arr => new Vec3<T>(a, r, r);
	public Vec3<T> ara => new Vec3<T>(a, r, a);
	public Vec3<T> agg => new Vec3<T>(a, g, g);
	public Vec3<T> aga => new Vec3<T>(a, g, a);
	public Vec3<T> abb => new Vec3<T>(a, b, b);
	public Vec3<T> aba => new Vec3<T>(a, b, a);
	public Vec3<T> aar => new Vec3<T>(a, a, r);
	public Vec3<T> aag => new Vec3<T>(a, a, g);
	public Vec3<T> aab => new Vec3<T>(a, a, b);
	public Vec3<T> aaa => new Vec3<T>(a, a, a);

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
		get => new Vec3<T>(r, g, b);

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

	public T q
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => w;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => w = value;
	}

	#endregion //TEXTURE

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(T value)
	{
		x = value;
		y = value;
		z = value;
		w = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(T x, T y, T z, T w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(T x, T y, Vec2<T> zw)
	{
		this.x = x;
		this.y = y;
		z = zw.x;
		w = zw.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(T x, Vec2<T> yz, T w)
	{
		this.x = x;
		y = yz.x;
		z = yz.y;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(Vec2<T> xy, T z, T w)
	{
		x = xy.x;
		y = xy.y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(Vec2<T> xy, Vec2<T> zw)
	{
		x = xy.x;
		y = xy.y;
		z = zw.x;
		w = zw.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(T x, Vec3<T> yzw)
	{
		this.x = x;
		y = yzw.x;
		z = yzw.y;
		w = yzw.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(Vec3<T> xyz, T w)
	{
		x = xyz.x;
		y = xyz.y;
		z = xyz.z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vec4(Vec4<T> vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
		w = vec.w;
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator+(Vec4<T> a, Vec4<T> b)
	{
		return new Vec4<T>(
			a.x + b.x,
			a.y + b.y,
			a.z + b.z,
			a.w + b.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator-(Vec4<T> a, Vec4<T> b)
	{
		return new Vec4<T>(
			a.x - b.x,
			a.y - b.y,
			a.z - b.z,
			a.w - b.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator*(Vec4<T> a, Vec4<T> b)
	{
		return new Vec4<T>(
			a.x * b.x,
			a.y * b.y,
			a.z * b.z,
			a.w * b.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator/(Vec4<T> a, Vec4<T> b)
	{
		return new Vec4<T>(
			a.x / b.x,
			a.y / b.y,
			a.z / b.z,
			a.w / b.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator+(Vec4<T> vec)
	{
		return vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator-(Vec4<T> vec)
	{
		return new Vec4<T>(
			-vec.x,
			-vec.y,
			-vec.z,
			-vec.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator*(Vec4<T> vec, T scalar)
	{
		return new Vec4<T>(
			vec.x * scalar,
			vec.y * scalar,
			vec.z * scalar,
			vec.w * scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator*(T scalar, Vec4<T> vec)
	{
		return new Vec4<T>(
			 scalar * vec.x,
			 scalar * vec.y,
			 scalar * vec.z,
			 scalar * vec.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator/(Vec4<T> vec, T scalar)
	{
		return new Vec4<T>(
			vec.x / scalar,
			vec.y / scalar,
			vec.z / scalar,
			vec.w / scalar
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec4<T> operator/(T scalar, Vec4<T> vec)
	{
		return new Vec4<T>(
			scalar / vec.x,
			scalar / vec.y,
			scalar / vec.z,
			scalar / vec.w
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(Vec4<T> a, Vec4<T> b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(Vec4<T> a, Vec4<T> b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is Vec4<T> lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
	}
}

public struct bvec4
{
	public bool x;
	public bool y;
	public bool z;
	public bool w;

	public bool this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => i switch {
			0 => x,
			1 => y,
			2 => z,
			3 => w,
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
				case 2:
					z = value;
					break;
				case 3:
					w = value;
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
	public bvec2 ww => new bvec2(w, w);

	public bvec3 xxx => new bvec3(x, x, x);
	public bvec3 xxy => new bvec3(x, x, y);
	public bvec3 xxz => new bvec3(x, x, z);
	public bvec3 xxw => new bvec3(x, x, w);
	public bvec3 xyx => new bvec3(x, y, x);
	public bvec3 xyy => new bvec3(x, y, y);
	public bvec3 xzx => new bvec3(x, z, x);
	public bvec3 xzz => new bvec3(x, z, z);
	public bvec3 xwx => new bvec3(x, w, x);
	public bvec3 xww => new bvec3(x, w, w);
	public bvec3 yxx => new bvec3(y, x, x);
	public bvec3 yxy => new bvec3(y, x, y);
	public bvec3 yyx => new bvec3(y, y, x);
	public bvec3 yyy => new bvec3(y, y, y);
	public bvec3 yyz => new bvec3(y, y, z);
	public bvec3 yyw => new bvec3(y, y, w);
	public bvec3 yzy => new bvec3(y, z, y);
	public bvec3 yzz => new bvec3(y, z, z);
	public bvec3 ywy => new bvec3(y, w, y);
	public bvec3 yww => new bvec3(y, w, w);
	public bvec3 zxx => new bvec3(z, x, x);
	public bvec3 zxz => new bvec3(z, x, z);
	public bvec3 zyy => new bvec3(z, y, y);
	public bvec3 zyz => new bvec3(z, y, z);
	public bvec3 zzx => new bvec3(z, z, x);
	public bvec3 zzy => new bvec3(z, z, y);
	public bvec3 zzz => new bvec3(z, z, z);
	public bvec3 zzw => new bvec3(z, z, w);
	public bvec3 zwz => new bvec3(z, w, z);
	public bvec3 zww => new bvec3(z, w, w);
	public bvec3 wxx => new bvec3(w, x, x);
	public bvec3 wxw => new bvec3(w, x, w);
	public bvec3 wyy => new bvec3(w, y, y);
	public bvec3 wyw => new bvec3(w, y, w);
	public bvec3 wzz => new bvec3(w, z, z);
	public bvec3 wzw => new bvec3(w, z, w);
	public bvec3 wwx => new bvec3(w, w, x);
	public bvec3 wwy => new bvec3(w, w, y);
	public bvec3 wwz => new bvec3(w, w, z);
	public bvec3 www => new bvec3(w, w, w);

	public bvec4 xxxx => new bvec4(x, x, x, x);
	public bvec4 xxxy => new bvec4(x, x, x, y);
	public bvec4 xxxz => new bvec4(x, x, x, z);
	public bvec4 xxxw => new bvec4(x, x, x, w);
	public bvec4 xxyx => new bvec4(x, x, y, x);
	public bvec4 xxyy => new bvec4(x, x, y, y);
	public bvec4 xxyz => new bvec4(x, x, y, z);
	public bvec4 xxyw => new bvec4(x, x, y, w);
	public bvec4 xxzx => new bvec4(x, x, z, x);
	public bvec4 xxzy => new bvec4(x, x, z, y);
	public bvec4 xxzz => new bvec4(x, x, z, z);
	public bvec4 xxzw => new bvec4(x, x, z, w);
	public bvec4 xxwx => new bvec4(x, x, w, x);
	public bvec4 xxwy => new bvec4(x, x, w, y);
	public bvec4 xxwz => new bvec4(x, x, w, z);
	public bvec4 xxww => new bvec4(x, x, w, w);
	public bvec4 xyxx => new bvec4(x, y, x, x);
	public bvec4 xyxy => new bvec4(x, y, x, y);
	public bvec4 xyxz => new bvec4(x, y, x, z);
	public bvec4 xyxw => new bvec4(x, y, x, w);
	public bvec4 xyyx => new bvec4(x, y, y, x);
	public bvec4 xyyy => new bvec4(x, y, y, y);
	public bvec4 xyyz => new bvec4(x, y, y, z);
	public bvec4 xyyw => new bvec4(x, y, y, w);
	public bvec4 xyzx => new bvec4(x, y, z, x);
	public bvec4 xyzy => new bvec4(x, y, z, y);
	public bvec4 xyzz => new bvec4(x, y, z, z);
	public bvec4 xywx => new bvec4(x, y, w, x);
	public bvec4 xywy => new bvec4(x, y, w, y);
	public bvec4 xyww => new bvec4(x, y, w, w);
	public bvec4 xzxx => new bvec4(x, z, x, x);
	public bvec4 xzxy => new bvec4(x, z, x, y);
	public bvec4 xzxz => new bvec4(x, z, x, z);
	public bvec4 xzxw => new bvec4(x, z, x, w);
	public bvec4 xzyx => new bvec4(x, z, y, x);
	public bvec4 xzyy => new bvec4(x, z, y, y);
	public bvec4 xzyz => new bvec4(x, z, y, z);
	public bvec4 xzzx => new bvec4(x, z, z, x);
	public bvec4 xzzy => new bvec4(x, z, z, y);
	public bvec4 xzzz => new bvec4(x, z, z, z);
	public bvec4 xzzw => new bvec4(x, z, z, w);
	public bvec4 xzwx => new bvec4(x, z, w, x);
	public bvec4 xzwz => new bvec4(x, z, w, z);
	public bvec4 xzww => new bvec4(x, z, w, w);
	public bvec4 xwxx => new bvec4(x, w, x, x);
	public bvec4 xwxy => new bvec4(x, w, x, y);
	public bvec4 xwxz => new bvec4(x, w, x, z);
	public bvec4 xwxw => new bvec4(x, w, x, w);
	public bvec4 xwyx => new bvec4(x, w, y, x);
	public bvec4 xwyy => new bvec4(x, w, y, y);
	public bvec4 xwyw => new bvec4(x, w, y, w);
	public bvec4 xwzx => new bvec4(x, w, z, x);
	public bvec4 xwzz => new bvec4(x, w, z, z);
	public bvec4 xwzw => new bvec4(x, w, z, w);
	public bvec4 xwwx => new bvec4(x, w, w, x);
	public bvec4 xwwy => new bvec4(x, w, w, y);
	public bvec4 xwwz => new bvec4(x, w, w, z);
	public bvec4 xwww => new bvec4(x, w, w, w);
	public bvec4 yxxx => new bvec4(y, x, x, x);
	public bvec4 yxxy => new bvec4(y, x, x, y);
	public bvec4 yxxz => new bvec4(y, x, x, z);
	public bvec4 yxxw => new bvec4(y, x, x, w);
	public bvec4 yxyx => new bvec4(y, x, y, x);
	public bvec4 yxyy => new bvec4(y, x, y, y);
	public bvec4 yxyz => new bvec4(y, x, y, z);
	public bvec4 yxyw => new bvec4(y, x, y, w);
	public bvec4 yxzx => new bvec4(y, x, z, x);
	public bvec4 yxzy => new bvec4(y, x, z, y);
	public bvec4 yxzz => new bvec4(y, x, z, z);
	public bvec4 yxwx => new bvec4(y, x, w, x);
	public bvec4 yxwy => new bvec4(y, x, w, y);
	public bvec4 yxww => new bvec4(y, x, w, w);
	public bvec4 yyxx => new bvec4(y, y, x, x);
	public bvec4 yyxy => new bvec4(y, y, x, y);
	public bvec4 yyxz => new bvec4(y, y, x, z);
	public bvec4 yyxw => new bvec4(y, y, x, w);
	public bvec4 yyyx => new bvec4(y, y, y, x);
	public bvec4 yyyy => new bvec4(y, y, y, y);
	public bvec4 yyyz => new bvec4(y, y, y, z);
	public bvec4 yyyw => new bvec4(y, y, y, w);
	public bvec4 yyzx => new bvec4(y, y, z, x);
	public bvec4 yyzy => new bvec4(y, y, z, y);
	public bvec4 yyzz => new bvec4(y, y, z, z);
	public bvec4 yyzw => new bvec4(y, y, z, w);
	public bvec4 yywx => new bvec4(y, y, w, x);
	public bvec4 yywy => new bvec4(y, y, w, y);
	public bvec4 yywz => new bvec4(y, y, w, z);
	public bvec4 yyww => new bvec4(y, y, w, w);
	public bvec4 yzxx => new bvec4(y, z, x, x);
	public bvec4 yzxy => new bvec4(y, z, x, y);
	public bvec4 yzxz => new bvec4(y, z, x, z);
	public bvec4 yzyx => new bvec4(y, z, y, x);
	public bvec4 yzyy => new bvec4(y, z, y, y);
	public bvec4 yzyz => new bvec4(y, z, y, z);
	public bvec4 yzyw => new bvec4(y, z, y, w);
	public bvec4 yzzx => new bvec4(y, z, z, x);
	public bvec4 yzzy => new bvec4(y, z, z, y);
	public bvec4 yzzz => new bvec4(y, z, z, z);
	public bvec4 yzzw => new bvec4(y, z, z, w);
	public bvec4 yzwy => new bvec4(y, z, w, y);
	public bvec4 yzwz => new bvec4(y, z, w, z);
	public bvec4 yzww => new bvec4(y, z, w, w);
	public bvec4 ywxx => new bvec4(y, w, x, x);
	public bvec4 ywxy => new bvec4(y, w, x, y);
	public bvec4 ywxw => new bvec4(y, w, x, w);
	public bvec4 ywyx => new bvec4(y, w, y, x);
	public bvec4 ywyy => new bvec4(y, w, y, y);
	public bvec4 ywyz => new bvec4(y, w, y, z);
	public bvec4 ywyw => new bvec4(y, w, y, w);
	public bvec4 ywzy => new bvec4(y, w, z, y);
	public bvec4 ywzz => new bvec4(y, w, z, z);
	public bvec4 ywzw => new bvec4(y, w, z, w);
	public bvec4 ywwx => new bvec4(y, w, w, x);
	public bvec4 ywwy => new bvec4(y, w, w, y);
	public bvec4 ywwz => new bvec4(y, w, w, z);
	public bvec4 ywww => new bvec4(y, w, w, w);
	public bvec4 zxxx => new bvec4(z, x, x, x);
	public bvec4 zxxy => new bvec4(z, x, x, y);
	public bvec4 zxxz => new bvec4(z, x, x, z);
	public bvec4 zxxw => new bvec4(z, x, x, w);
	public bvec4 zxyx => new bvec4(z, x, y, x);
	public bvec4 zxyy => new bvec4(z, x, y, y);
	public bvec4 zxyz => new bvec4(z, x, y, z);
	public bvec4 zxzx => new bvec4(z, x, z, x);
	public bvec4 zxzy => new bvec4(z, x, z, y);
	public bvec4 zxzz => new bvec4(z, x, z, z);
	public bvec4 zxzw => new bvec4(z, x, z, w);
	public bvec4 zxwx => new bvec4(z, x, w, x);
	public bvec4 zxwz => new bvec4(z, x, w, z);
	public bvec4 zxww => new bvec4(z, x, w, w);
	public bvec4 zyxx => new bvec4(z, y, x, x);
	public bvec4 zyxy => new bvec4(z, y, x, y);
	public bvec4 zyxz => new bvec4(z, y, x, z);
	public bvec4 zyyx => new bvec4(z, y, y, x);
	public bvec4 zyyy => new bvec4(z, y, y, y);
	public bvec4 zyyz => new bvec4(z, y, y, z);
	public bvec4 zyyw => new bvec4(z, y, y, w);
	public bvec4 zyzx => new bvec4(z, y, z, x);
	public bvec4 zyzy => new bvec4(z, y, z, y);
	public bvec4 zyzz => new bvec4(z, y, z, z);
	public bvec4 zyzw => new bvec4(z, y, z, w);
	public bvec4 zywy => new bvec4(z, y, w, y);
	public bvec4 zywz => new bvec4(z, y, w, z);
	public bvec4 zyww => new bvec4(z, y, w, w);
	public bvec4 zzxx => new bvec4(z, z, x, x);
	public bvec4 zzxy => new bvec4(z, z, x, y);
	public bvec4 zzxz => new bvec4(z, z, x, z);
	public bvec4 zzxw => new bvec4(z, z, x, w);
	public bvec4 zzyx => new bvec4(z, z, y, x);
	public bvec4 zzyy => new bvec4(z, z, y, y);
	public bvec4 zzyz => new bvec4(z, z, y, z);
	public bvec4 zzyw => new bvec4(z, z, y, w);
	public bvec4 zzzx => new bvec4(z, z, z, x);
	public bvec4 zzzy => new bvec4(z, z, z, y);
	public bvec4 zzzz => new bvec4(z, z, z, z);
	public bvec4 zzzw => new bvec4(z, z, z, w);
	public bvec4 zzwx => new bvec4(z, z, w, x);
	public bvec4 zzwy => new bvec4(z, z, w, y);
	public bvec4 zzwz => new bvec4(z, z, w, z);
	public bvec4 zzww => new bvec4(z, z, w, w);
	public bvec4 zwxx => new bvec4(z, w, x, x);
	public bvec4 zwxz => new bvec4(z, w, x, z);
	public bvec4 zwxw => new bvec4(z, w, x, w);
	public bvec4 zwyy => new bvec4(z, w, y, y);
	public bvec4 zwyz => new bvec4(z, w, y, z);
	public bvec4 zwyw => new bvec4(z, w, y, w);
	public bvec4 zwzx => new bvec4(z, w, z, x);
	public bvec4 zwzy => new bvec4(z, w, z, y);
	public bvec4 zwzz => new bvec4(z, w, z, z);
	public bvec4 zwzw => new bvec4(z, w, z, w);
	public bvec4 zwwx => new bvec4(z, w, w, x);
	public bvec4 zwwy => new bvec4(z, w, w, y);
	public bvec4 zwwz => new bvec4(z, w, w, z);
	public bvec4 zwww => new bvec4(z, w, w, w);

	public bvec4 wxxx => new bvec4(w, x, x, x);
	public bvec4 wxxy => new bvec4(w, x, x, y);
	public bvec4 wxxz => new bvec4(w, x, x, z);
	public bvec4 wxxw => new bvec4(w, x, x, w);
	public bvec4 wxyx => new bvec4(w, x, y, x);
	public bvec4 wxyy => new bvec4(w, x, y, y);
	public bvec4 wxyw => new bvec4(w, x, y, w);
	public bvec4 wxzx => new bvec4(w, x, z, x);
	public bvec4 wxzz => new bvec4(w, x, z, z);
	public bvec4 wxzw => new bvec4(w, x, z, w);
	public bvec4 wxwx => new bvec4(w, x, w, x);
	public bvec4 wxwy => new bvec4(w, x, w, y);
	public bvec4 wxwz => new bvec4(w, x, w, z);
	public bvec4 wxww => new bvec4(w, x, w, w);
	public bvec4 wyxx => new bvec4(w, y, x, x);
	public bvec4 wyxy => new bvec4(w, y, x, y);
	public bvec4 wyxw => new bvec4(w, y, x, w);
	public bvec4 wyyx => new bvec4(w, y, y, x);
	public bvec4 wyyy => new bvec4(w, y, y, y);
	public bvec4 wyyz => new bvec4(w, y, y, z);
	public bvec4 wyyw => new bvec4(w, y, y, w);
	public bvec4 wyzy => new bvec4(w, y, z, y);
	public bvec4 wyzz => new bvec4(w, y, z, z);
	public bvec4 wyzw => new bvec4(w, y, z, w);
	public bvec4 wywx => new bvec4(w, y, w, x);
	public bvec4 wywy => new bvec4(w, y, w, y);
	public bvec4 wywz => new bvec4(w, y, w, z);
	public bvec4 wyww => new bvec4(w, y, w, w);
	public bvec4 wzxx => new bvec4(w, z, x, x);
	public bvec4 wzxz => new bvec4(w, z, x, z);
	public bvec4 wzxw => new bvec4(w, z, x, w);
	public bvec4 wzyy => new bvec4(w, z, y, y);
	public bvec4 wzyz => new bvec4(w, z, y, z);
	public bvec4 wzyw => new bvec4(w, z, y, w);
	public bvec4 wzzx => new bvec4(w, z, z, x);
	public bvec4 wzzy => new bvec4(w, z, z, y);
	public bvec4 wzzz => new bvec4(w, z, z, z);
	public bvec4 wzzw => new bvec4(w, z, z, w);
	public bvec4 wzwx => new bvec4(w, z, w, x);
	public bvec4 wzwy => new bvec4(w, z, w, y);
	public bvec4 wzwz => new bvec4(w, z, w, z);
	public bvec4 wzww => new bvec4(w, z, w, w);
	public bvec4 wwxx => new bvec4(w, w, x, x);
	public bvec4 wwxy => new bvec4(w, w, x, y);
	public bvec4 wwxz => new bvec4(w, w, x, z);
	public bvec4 wwxw => new bvec4(w, w, x, w);
	public bvec4 wwyx => new bvec4(w, w, y, x);
	public bvec4 wwyy => new bvec4(w, w, y, y);
	public bvec4 wwyz => new bvec4(w, w, y, z);
	public bvec4 wwyw => new bvec4(w, w, y, w);
	public bvec4 wwzx => new bvec4(w, w, z, x);
	public bvec4 wwzy => new bvec4(w, w, z, y);
	public bvec4 wwzz => new bvec4(w, w, z, z);
	public bvec4 wwzw => new bvec4(w, w, z, w);
	public bvec4 wwwx => new bvec4(w, w, w, x);
	public bvec4 wwwy => new bvec4(w, w, w, y);
	public bvec4 wwwz => new bvec4(w, w, w, z);
	public bvec4 wwww => new bvec4(w, w, w, w);

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

	public bvec2 xw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
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

	public bvec2 yw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
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

	public bvec2 zw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec2(z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
		}
	}

	public bvec3 xyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public bvec3 xyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			w = value.z;
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

	public bvec3 xzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			w = value.z;
		}
	}

	public bvec3 xwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			y = value.z;
		}
	}

	public bvec3 xwz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(x, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			z = value.z;
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

	public bvec3 yxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			w = value.z;
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

	public bvec3 yzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			w = value.z;
		}
	}

	public bvec3 ywx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			x = value.z;
		}
	}

	public bvec3 ywz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(y, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			z = value.z;
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

	public bvec3 zxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			w = value.z;
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

	public bvec3 zyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			w = value.z;
		}
	}

	public bvec3 zwx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			x = value.z;
		}
	}

	public bvec3 zwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(z, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			y = value.z;
		}
	}

	public bvec3 wxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			y = value.z;
		}
	}

	public bvec3 wxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			z = value.z;
		}
	}

	public bvec3 wyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			x = value.z;
		}
	}

	public bvec3 wyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			z = value.z;
		}
	}

	public bvec3 wzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			x = value.z;
		}
	}

	public bvec3 wzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec3(w, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			y = value.z;
		}
	}

	public bvec4 xyzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			z = value.z;
			w = value.w;
		}
	}

	public bvec4 xywz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(x, y, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			y = value.y;
			w = value.z;
			z = value.w;
		}
	}

	public bvec4 xzyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(x, z, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			y = value.z;
			w = value.w;
		}
	}

	public bvec4 xzwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(x, z, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			z = value.y;
			w = value.z;
			y = value.w;
		}
	}

	public bvec4 xwyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(x, w, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			y = value.z;
			z = value.w;
		}
	}

	public bvec4 xwzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(x, w, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			x = value.x;
			w = value.y;
			z = value.z;
			y = value.w;
		}
	}

	public bvec4 yxzw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, x, z, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			z = value.z;
			w = value.w;
		}
	}

	public bvec4 yxwz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, x, w, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			x = value.y;
			w = value.z;
			z = value.w;
		}
	}

	public bvec4 yzxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, z, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			x = value.z;
			w = value.w;
		}
	}

	public bvec4 yzwx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, z, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			z = value.y;
			w = value.z;
			x = value.w;
		}
	}

	public bvec4 ywxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, w, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			x = value.z;
			z = value.w;
		}
	}

	public bvec4 ywzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(y, w, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			y = value.x;
			w = value.y;
			z = value.z;
			x = value.w;
		}
	}

	public bvec4 zxyw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, x, y, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			y = value.z;
			w = value.w;
		}
	}

	public bvec4 zxwy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, x, w, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			x = value.y;
			w = value.z;
			y = value.w;
		}
	}

	public bvec4 zyxw
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, y, x, w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			x = value.z;
			w = value.w;
		}
	}

	public bvec4 zywx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, y, w, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			y = value.y;
			w = value.z;
			x = value.w;
		}
	}

	public bvec4 zwxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, w, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			x = value.z;
			y = value.w;
		}
	}

	public bvec4 zwyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(z, w, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			z = value.x;
			w = value.y;
			y = value.z;
			x = value.w;
		}
	}

	public bvec4 wxyz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, x, y, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			y = value.z;
			z = value.w;
		}
	}

	public bvec4 wxzy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, x, z, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			x = value.y;
			z = value.z;
			y = value.w;
		}
	}

	public bvec4 wyxz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, y, x, z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			x = value.z;
			z = value.w;
		}
	}

	public bvec4 wyzx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, y, z, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			y = value.y;
			z = value.z;
			x = value.w;
		}
	}

	public bvec4 wzxy
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, z, x, y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			x = value.z;
			y = value.w;
		}
	}

	public bvec4 wzyx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new bvec4(w, z, y, x);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			w = value.x;
			z = value.y;
			y = value.z;
			x = value.w;
		}
	}

	#endregion //COORDINATES

	#region CONSTRUCTORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bool value)
	{
		x = value;
		y = value;
		z = value;
		w = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bool x, bool y, bool z, bool w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bool x, bool y, bvec2 zw)
	{
		this.x = x;
		this.y = y;
		z = zw.x;
		w = zw.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bool x, bvec2 yz, bool w)
	{
		this.x = x;
		y = yz.x;
		z = yz.y;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bvec2 xy, bool z, bool w)
	{
		x = xy.x;
		y = xy.y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bool x, bvec3 yzw)
	{
		this.x = x;
		y = yzw.x;
		z = yzw.y;
		w = yzw.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bvec3 xyz, bool w)
	{
		x = xyz.x;
		y = xyz.y;
		z = xyz.z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(bvec4 vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
		w = vec.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bvec4(int vec)
	{
		x = Convert.ToBoolean(vec & 0b0001);
		y = Convert.ToBoolean(vec & 0b0010);
		z = Convert.ToBoolean(vec & 0b0100);
		w = Convert.ToBoolean(vec & 0b1000);
	}

	#endregion //CONSTRUCTORS

	#region OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 operator!(bvec4 x)
	{
		return new bvec4(!x.x, !x.y, !x.z, !x.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 operator&(bvec4 a, bvec4 b)
	{
		return new bvec4(a.x && b.x, a.y && b.y, a.z && b.z, a.w && b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 operator|(bvec4 a, bvec4 b)
	{
		return new bvec4(a.x || b.x, a.y || b.y, a.z || b.z, a.w || b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 operator^(bvec4 a, bvec4 b)
	{
		return new bvec4(a.x != b.x, a.y != b.y, a.z != b.z, a.w != b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator==(bvec4 a, bvec4 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator!=(bvec4 a, bvec4 b)
	{
		return !(a == b);
	}

	#endregion //OPERATORS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec4(int vec)
	{
		return new bvec4(vec);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int(bvec4 vec)
	{
		return Convert.ToInt32(vec.x) & (Convert.ToInt32(vec.x) << 1) & (Convert.ToInt32(vec.x) << 2) & (Convert.ToInt32(vec.x) << 3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bvec4(Variant vec)
	{
		return (bvec4)vec.AsInt32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Variant(bvec4 vec)
	{
		return (int)vec;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object pObject)
	{
		return pObject is bvec4 lVec && lVec == this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
	}
}