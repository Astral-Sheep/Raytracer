global using static Astral.Tools.GlobalShaderFunctions;
using System;
using System.Runtime.CompilerServices;
using Godot;

namespace Astral.Tools;

public static class GlobalShaderFunctions
{
	private const float RAD2DEG = Mathf.Pi / 180f;
	private const float DEG2RAD = 180f / Mathf.Pi;
	private const float LOG_2 = 0.69314718056f;
	private const float INV_LOG_2 = 1f / LOG_2;

	#region TRIGONOMETRIC_FUNCTIONS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float radians(float degrees)
	{
		return degrees * RAD2DEG;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 radians(vec2 degrees)
	{
		return degrees * RAD2DEG;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 radians(vec3 degrees)
	{
		return degrees * RAD2DEG;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 radians(vec4 degrees)
	{
		return degrees * RAD2DEG;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float degrees(float radians)
	{
		return radians * DEG2RAD;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 degrees(vec2 radians)
	{
		return radians * DEG2RAD;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 degrees(vec3 radians)
	{
		return radians * DEG2RAD;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 degrees(vec4 radians)
	{
		return radians * DEG2RAD;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float sin(float x)
	{
		return Mathf.Sin(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 sin(vec2 x)
	{
		return new vec2(sin(x.x), sin(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 sin(vec3 x)
	{
		return new vec3(sin(x.x), sin(x.y), sin(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 sin(vec4 x)
	{
		return new vec4(sin(x.x), sin(x.y), sin(x.z), sin(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float cos(float x)
	{
		return Mathf.Cos(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 cos(vec2 x)
	{
		return new vec2(cos(x.x), cos(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 cos(vec3 x)
	{
		return new vec3(cos(x.x), cos(x.y), cos(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 cos(vec4 x)
	{
		return new vec4(cos(x.x), cos(x.y), cos(x.z), cos(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float tan(float x)
	{
		return Mathf.Tan(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 tan(vec2 x)
	{
		return new vec2(tan(x.x), tan(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 tan(vec3 x)
	{
		return new vec3(tan(x.x), tan(x.y), tan(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 tan(vec4 x)
	{
		return new vec4(tan(x.x), tan(x.y), tan(x.z), tan(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float asin(float x)
	{
		return Mathf.Asin(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 asin(vec2 x)
	{
		return new vec2(asin(x.x), asin(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 asin(vec3 x)
	{
		return new vec3(asin(x.x), asin(x.y), asin(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 asin(vec4 x)
	{
		return new vec4(asin(x.x), asin(x.y), asin(x.z), asin(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float acos(float x)
	{
		return Mathf.Acos(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 acos(vec2 x)
	{
		return new vec2(acos(x.x), acos(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 acos(vec3 x)
	{
		return new vec3(acos(x.x), acos(x.y), acos(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 acos(vec4 x)
	{
		return new vec4(acos(x.x), acos(x.y), acos(x.z), acos(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float atan(float y_over_x)
	{
		return Mathf.Atan(y_over_x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 atan(vec2 y_over_x)
	{
		return new vec2(atan(y_over_x.x), atan(y_over_x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 atan(vec3 y_over_x)
	{
		return new vec3(atan(y_over_x.x), atan(y_over_x.y), atan(y_over_x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 atan(vec4 y_over_x)
	{
		return new vec4(atan(y_over_x.x), atan(y_over_x.y), atan(y_over_x.z), atan(y_over_x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float atan(float y, float x)
	{
		return Mathf.Atan2(y, x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 atan(vec2 y, vec2 x)
	{
		return new vec2(atan(y.x, x.x), atan(y.y, x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 atan(vec3 y, vec3 x)
	{
		return new vec3(atan(y.x, x.x), atan(y.y, x.y), atan(y.z, x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 atan(vec4 y, vec4 x)
	{
		return new vec4(atan(y.x, x.x), atan(y.y, x.y), atan(y.z, x.z), atan(y.w, x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float sinh(float x)
	{
		return Mathf.Sinh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 sinh(vec2 x)
	{
		return new vec2(sinh(x.x), sinh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 sinh(vec3 x)
	{
		return new vec3(sinh(x.x), sinh(x.y), sinh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 sinh(vec4 x)
	{
		return new vec4(sinh(x.x), sinh(x.y), sinh(x.z), sinh(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float cosh(float x)
	{
		return Mathf.Cosh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 cosh(vec2 x)
	{
		return new vec2(cosh(x.x), cosh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 cosh(vec3 x)
	{
		return new vec3(cosh(x.x), cosh(x.y), cosh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 cosh(vec4 x)
	{
		return new vec4(cosh(x.x), cosh(x.y), cosh(x.z), cosh(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float tanh(float x)
	{
		return Mathf.Tanh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 tanh(vec2 x)
	{
		return new vec2(tanh(x.x), tanh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 tanh(vec3 x)
	{
		return new vec3(tanh(x.x), tanh(x.y), tanh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 tanh(vec4 x)
	{
		return new vec4(tanh(x.x), tanh(x.y), tanh(x.z), tanh(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float asinh(float x)
	{
		return Mathf.Asinh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 asinh(vec2 x)
	{
		return new vec2(asinh(x.x), asinh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 asinh(vec3 x)
	{
		return new vec3(asinh(x.x), asinh(x.y), asinh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 asinh(vec4 x)
	{
		return new vec4(asinh(x.x), asinh(x.y), asinh(x.z), asinh(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float acosh(float x)
	{
		return Mathf.Acosh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 acosh(vec2 x)
	{
		return new vec2(acosh(x.x), acosh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 acosh(vec3 x)
	{
		return new vec3(acosh(x.x), acosh(x.y), acosh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 acosh(vec4 x)
	{
		return new vec4(acosh(x.x), acosh(x.y), acosh(x.z), acosh(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float atanh(float x)
	{
		return Mathf.Atanh(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 atanh(vec2 x)
	{
		return new vec2(atanh(x.x), atanh(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 atanh(vec3 x)
	{
		return new vec3(atanh(x.x), atanh(x.y), atanh(x.z));
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 atanh(vec4 x)
	{
		return new vec4(atanh(x.x), atanh(x.y), atanh(x.z), atanh(x.w));
	}

	#endregion //TRIGONOMETRIC_FUNCTIONS

	#region MATH_FUNCTIONS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float pow(float x, float y)
	{
		return Mathf.Pow(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 pow(vec2 x, vec2 y)
	{
		return new vec2(pow(x.x, y.x), pow(x.y, y.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 pow(vec3 x, vec3 y)
	{
		return new vec3(pow(x.x, y.x), pow(x.y, y.y), pow(x.z, y.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 pow(vec4 x, vec4 y)
	{
		return new vec4(pow(x.x, y.x), pow(x.y, y.y), pow(x.z, y.z), pow(x.w, y.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float exp(float x)
	{
		return Mathf.Exp(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 exp(vec2 x)
	{
		return new vec2(exp(x.x), exp(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 exp(vec3 x)
	{
		return new vec3(exp(x.x), exp(x.y), exp(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 exp(vec4 x)
	{
		return new vec4(exp(x.x), exp(x.y), exp(x.z), exp(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float exp2(float x)
	{
		return Mathf.Pow(2, x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 exp2(vec2 x)
	{
		return new vec2(exp2(x.x), exp2(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 exp2(vec3 x)
	{
		return new vec3(exp2(x.x), exp2(x.y), exp2(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 exp2(vec4 x)
	{
		return new vec4(exp2(x.x), exp2(x.y), exp2(x.z), exp2(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float log(float x)
	{
		return Mathf.Log(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 log(vec2 x)
	{
		return new vec2(log(x.x), log(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 log(vec3 x)
	{
		return new vec3(log(x.x), log(x.y), log(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 log(vec4 x)
	{
		return new vec4(log(x.x), log(x.y), log(x.z), log(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float log2(float x)
	{
		return Mathf.Log(x) * INV_LOG_2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 log2(vec2 x)
	{
		return new vec2(log2(x.x), log2(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 log2(vec3 x)
	{
		return new vec3(log2(x.x), log2(x.y), log2(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 log2(vec4 x)
	{
		return new vec4(log2(x.x), log2(x.y), log2(x.z), log2(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float sqrt(float x)
	{
		return Mathf.Sqrt(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 sqrt(vec2 x)
	{
		return new vec2(sqrt(x.x), sqrt(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 sqrt(vec3 x)
	{
		return new vec3(sqrt(x.x), sqrt(x.y), sqrt(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 sqrt(vec4 x)
	{
		return new vec4(sqrt(x.x), sqrt(x.y), sqrt(x.z), sqrt(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float inversesqrt(float x)
	{
		return 1f / Mathf.Sqrt(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 inversesqrt(vec2 x)
	{
		return new vec2(inversesqrt(x.x), inversesqrt(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 inversesqrt(vec3 x)
	{
		return new vec3(inversesqrt(x.x), inversesqrt(x.y), inversesqrt(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 inversesqrt(vec4 x)
	{
		return new vec4(inversesqrt(x.x), inversesqrt(x.y), inversesqrt(x.z), inversesqrt(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float abs(float x)
	{
		return Mathf.Abs(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 abs(vec2 x)
	{
		return new vec2(abs(x.x), abs(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 abs(vec3 x)
	{
		return new vec3(abs(x.x), abs(x.y), abs(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 abs(vec4 x)
	{
		return new vec4(abs(x.x), abs(x.y), abs(x.z), abs(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int abs(int x)
	{
		return Mathf.Abs(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 abs(ivec2 x)
	{
		return new ivec2(abs(x.x), abs(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 abs(ivec3 x)
	{
		return new ivec3(abs(x.x), abs(x.y), abs(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 abs(ivec4 x)
	{
		return new ivec4(abs(x.x), abs(x.y), abs(x.z), abs(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float sign(float x)
	{
		return Mathf.Sign(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 sign(vec2 x)
	{
		return new vec2(sign(x.x), sign(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 sign(vec3 x)
	{
		return new vec3(sign(x.x), sign(x.y), sign(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 sign(vec4 x)
	{
		return new vec4(sign(x.x), sign(x.y), sign(x.z), sign(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int sign(int x)
	{
		return Mathf.Sign(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 sign(ivec2 x)
	{
		return new ivec2(sign(x.x), sign(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 sign(ivec3 x)
	{
		return new ivec3(sign(x.x), sign(x.y), sign(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 sign(ivec4 x)
	{
		return new ivec4(sign(x.x), sign(x.y), sign(x.z), sign(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float floor(float x)
	{
		return Mathf.Floor(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 floor(vec2 x)
	{
		return new vec2(floor(x.x), floor(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 floor(vec3 x)
	{
		return new vec3(floor(x.x), floor(x.y), floor(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 floor(vec4 x)
	{
		return new vec4(floor(x.x), floor(x.y), floor(x.z), floor(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float round(float x)
	{
		return Mathf.Round(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 round(vec2 x)
	{
		return new vec2(round(x.x), round(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 round(vec3 x)
	{
		return new vec3(round(x.x), round(x.y), round(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 round(vec4 x)
	{
		return new vec4(round(x.x), round(x.y), round(x.z), round(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float roundEven(float x)
	{
		return 2f * round(x * .5f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 roundEven(vec2 x)
	{
		return new vec2(roundEven(x.x), roundEven(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 roundEven(vec3 x)
	{
		return new vec3(roundEven(x.x), roundEven(x.y), roundEven(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 roundEven(vec4 x)
	{
		return new vec4(roundEven(x.x), roundEven(x.y), roundEven(x.z), roundEven(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float trunc(float x)
	{
		return (float)Math.Truncate(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 trunc(vec2 x)
	{
		return new vec2(trunc(x.x), trunc(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 trunc(vec3 x)
	{
		return new vec3(trunc(x.x), trunc(x.y), trunc(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 trunc(vec4 x)
	{
		return new vec4(trunc(x.x), trunc(x.y), trunc(x.z), trunc(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ceil(float x)
	{
		return Mathf.Ceil(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 ceil(vec2 x)
	{
		return new vec2(ceil(x.x), ceil(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 ceil(vec3 x)
	{
		return new vec3(ceil(x.x), ceil(x.y), ceil(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 ceil(vec4 x)
	{
		return new vec4(ceil(x.x), ceil(x.y), ceil(x.z), ceil(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float fract(float x)
	{
		return x - floor(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 fract(vec2 x)
	{
		return x - floor(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 fract(vec3 x)
	{
		return x - floor(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 fract(vec4 x)
	{
		return x - floor(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float mod(float x, float y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 mod(vec2 x, vec2 y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 mod(vec2 x, float y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 mod(vec3 x, vec3 y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 mod(vec3 x, float y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 mod(vec4 x, vec4 y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 mod(vec4 x, float y)
	{
		return x - y * floor(x / y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float modf(float x, out float i)
	{
		i = mod(x, 1);
		return x - i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 modf(vec2 x, out vec2 i)
	{
		i = mod(x, 1);
		return x - i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 modf(vec3 x, out vec3 i)
	{
		i = mod(x, 1);
		return x - i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 modf(vec4 x, out vec4 i)
	{
		i = mod(x, 1);
		return x - i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float min(float a, float b)
	{
		return Mathf.Min(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 min(vec2 a, vec2 b)
	{
		return new vec2(min(a.x, b.x), min(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 min(vec2 a, float b)
	{
		return new vec2(min(a.x, b), min(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 min(vec3 a, vec3 b)
	{
		return new vec3(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 min(vec3 a, float b)
	{
		return new vec3(min(a.x, b), min(a.y, b), min(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 min(vec4 a, vec4 b)
	{
		return new vec4(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z), min(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 min(vec4 a, float b)
	{
		return new vec4(min(a.x, b), min(a.y, b), min(a.z, b), min(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int min(int a, int b)
	{
		return Mathf.Min(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 min(ivec2 a, ivec2 b)
	{
		return new ivec2(min(a.x, b.x), min(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 min(ivec2 a, int b)
	{
		return new ivec2(min(a.x, b), min(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 min(ivec3 a, ivec3 b)
	{
		return new ivec3(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 min(ivec3 a, int b)
	{
		return new ivec3(min(a.x, b), min(a.y, b), min(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 min(ivec4 a, ivec4 b)
	{
		return new ivec4(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z), min(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 min(ivec4 a, int b)
	{
		return new ivec4(min(a.x, b), min(a.y, b), min(a.z, b), min(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint min(uint a, uint b)
	{
		return Math.Min(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 min(uvec2 a, uvec2 b)
	{
		return new uvec2(min(a.x, b.x), min(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 min(uvec2 a, uint b)
	{
		return new uvec2(min(a.x, b), min(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 min(uvec3 a, uvec3 b)
	{
		return new uvec3(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 min(uvec3 a, uint b)
	{
		return new uvec3(min(a.x, b), min(a.y, b), min(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 min(uvec4 a, uvec4 b)
	{
		return new uvec4(min(a.x, b.x), min(a.y, b.y), min(a.z, b.z), min(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 min(uvec4 a, uint b)
	{
		return new uvec4(min(a.x, b), min(a.y, b), min(a.z, b), min(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float max(float a, float b)
	{
		return Mathf.Max(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 max(vec2 a, vec2 b)
	{
		return new vec2(max(a.x, b.x), max(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 max(vec2 a, float b)
	{
		return new vec2(max(a.x, b), max(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 max(vec3 a, vec3 b)
	{
		return new vec3(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 max(vec3 a, float b)
	{
		return new vec3(max(a.x, b), max(a.y, b), max(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 max(vec4 a, vec4 b)
	{
		return new vec4(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z), max(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 max(vec4 a, float b)
	{
		return new vec4(max(a.x, b), max(a.y, b), max(a.z, b), max(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int max(int a, int b)
	{
		return Mathf.Max(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 max(ivec2 a, ivec2 b)
	{
		return new ivec2(max(a.x, b.x), max(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 max(ivec2 a, int b)
	{
		return new ivec2(max(a.x, b), max(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 max(ivec3 a, ivec3 b)
	{
		return new ivec3(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 max(ivec3 a, int b)
	{
		return new ivec3(max(a.x, b), max(a.y, b), max(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 max(ivec4 a, ivec4 b)
	{
		return new ivec4(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z), max(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 max(ivec4 a, int b)
	{
		return new ivec4(max(a.x, b), max(a.y, b), max(a.z, b), max(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint max(uint a, uint b)
	{
		return Math.Max(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 max(uvec2 a, uvec2 b)
	{
		return new uvec2(max(a.x, b.x), max(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 max(uvec2 a, uint b)
	{
		return new uvec2(max(a.x, b), max(a.y, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 max(uvec3 a, uvec3 b)
	{
		return new uvec3(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 max(uvec3 a, uint b)
	{
		return new uvec3(max(a.x, b), max(a.y, b), max(a.z, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 max(uvec4 a, uvec4 b)
	{
		return new uvec4(max(a.x, b.x), max(a.y, b.y), max(a.z, b.z), max(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 max(uvec4 a, uint b)
	{
		return new uvec4(max(a.x, b), max(a.y, b), max(a.z, b), max(a.w, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float clamp(float x, float min, float max)
	{
		return Mathf.Clamp(x, min, max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 clamp(vec2 x, vec2 min, vec2 max)
	{
		return new vec2(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 clamp(vec2 x, float min, float max)
	{
		return new vec2(clamp(x.x, min, max), clamp(x.y, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 clamp(vec3 x, vec3 min, vec3 max)
	{
		return new vec3(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 clamp(vec3 x, float min, float max)
	{
		return new vec3(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 clamp(vec4 x, vec4 min, vec4 max)
	{
		return new vec4(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z), clamp(x.w, min.w, max.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 clamp(vec4 x, float min, float max)
	{
		return new vec4(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max), clamp(x.w, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int clamp(int x, int min, int max)
	{
		return Mathf.Clamp(x, min, max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 clamp(ivec2 x, ivec2 min, ivec2 max)
	{
		return new ivec2(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 clamp(ivec2 x, int min, int max)
	{
		return new ivec2(clamp(x.x, min, max), clamp(x.y, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 clamp(ivec3 x, ivec3 min, ivec3 max)
	{
		return new ivec3(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 clamp(ivec3 x, int min, int max)
	{
		return new ivec3(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 clamp(ivec4 x, ivec4 min, ivec4 max)
	{
		return new ivec4(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z), clamp(x.w, min.w, max.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 clamp(ivec4 x, int min, int max)
	{
		return new ivec4(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max), clamp(x.w, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint clamp(uint x, uint min, uint max)
	{
		return Math.Clamp(x, min, max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 clamp(uvec2 x, uvec2 min, uvec2 max)
	{
		return new uvec2(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 clamp(uvec2 x, uint min, uint max)
	{
		return new uvec2(clamp(x.x, min, max), clamp(x.y, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 clamp(uvec3 x, uvec3 min, uvec3 max)
	{
		return new uvec3(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 clamp(uvec3 x, uint min, uint max)
	{
		return new uvec3(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 clamp(uvec4 x, uvec4 min, uvec4 max)
	{
		return new uvec4(clamp(x.x, min.x, max.x), clamp(x.y, min.y, max.y), clamp(x.z, min.z, max.z), clamp(x.w, min.w, max.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 clamp(uvec4 x, uint min, uint max)
	{
		return new uvec4(clamp(x.x, min, max), clamp(x.y, min, max), clamp(x.z, min, max), clamp(x.w, min, max));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float mix(float a, float b, float c)
	{
		return a + (b - a) * c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float mix(float a, float b, bool c)
	{
		return a + (b - a) * Convert.ToSingle(c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 mix(vec2 a, vec2 b, vec2 c)
	{
		return new vec2(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 mix(vec2 a, vec2 b, float c)
	{
		return new vec2(mix(a.x, b.x, c), mix(a.y, b.y, c));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 mix(vec2 a, vec2 b, bvec2 c)
	{
		return new vec2(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 mix(vec3 a, vec3 b, vec3 c)
	{
		return new vec3(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y), mix(a.z, b.z, c.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 mix(vec3 a, vec3 b, float c)
	{
		return new vec3(mix(a.x, b.x, c), mix(a.y, b.y, c), mix(a.z, b.z, c));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 mix(vec3 a, vec3 b, bvec3 c)
	{
		return new vec3(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y), mix(a.z, b.z, c.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 mix(vec4 a, vec4 b, vec4 c)
	{
		return new vec4(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y), mix(a.z, b.z, c.z), mix(a.w, b.w, c.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 mix(vec4 a, vec4 b, float c)
	{
		return new vec4(mix(a.x, b.x, c), mix(a.y, b.y, c), mix(a.z, b.z, c), mix(a.w, b.w, c));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 mix(vec4 a, vec4 b, bvec4 c)
	{
		return new vec4(mix(a.x, b.x, c.x), mix(a.y, b.y, c.y), mix(a.z, b.z, c.z), mix(a.w, b.w, c.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float step(float a, float b)
	{
		return Convert.ToSingle(b >= a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 step(vec2 a, vec2 b)
	{
		return new vec2(step(a.x, b.x), step(a.y, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 step(float a, vec2 b)
	{
		return new vec2(step(a, b.x), step(a, b.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 step(vec3 a, vec3 b)
	{
		return new vec3(step(a.x, b.x), step(a.y, b.y), step(a.z, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 step(float a, vec3 b)
	{
		return new vec3(step(a, b.x), step(a, b.y), step(a, b.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 step(vec4 a, vec4 b)
	{
		return new vec4(step(a.x, b.x), step(a.y, b.y), step(a.z, b.z), step(a.w, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 step(float a, vec4 b)
	{
		return new vec4(step(a, b.x), step(a, b.y), step(a, b.z), step(a, b.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float smoothstep(float a, float b, float c)
	{
		float t = clamp((c - a) / (b - a), 0f, 1f);
		return t * t * (3f - 2f * t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 smoothstep(vec2 a, vec2 b, vec2 c)
	{
		return new vec2(smoothstep(a.x, b.x, c.x), smoothstep(a.y, b.y, c.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 smoothstep(float a, float b, vec2 c)
	{
		return new vec2(smoothstep(a, b, c.x), smoothstep(a, b, c.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 smoothstep(vec3 a, vec3 b, vec3 c)
	{
		return new vec3(smoothstep(a.x, b.x, c.x), smoothstep(a.y, b.y, c.y), smoothstep(a.z, b.z, c.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 smoothstep(float a, float b, vec3 c)
	{
		return new vec3(smoothstep(a, b, c.x), smoothstep(a, b, c.y), smoothstep(a, b, c.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 smoothstep(vec4 a, vec4 b, vec4 c)
	{
		return new vec4(smoothstep(a.x, b.x, c.x), smoothstep(a.y, b.y, c.y), smoothstep(a.z, b.z, c.z), smoothstep(a.w, b.w, c.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 smoothstep(float a, float b, vec4 c)
	{
		return new vec4(smoothstep(a, b, c.x), smoothstep(a, b, c.y), smoothstep(a, b, c.z), smoothstep(a, b, c.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool isnan(float x)
	{
		return float.IsNaN(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 isnan(vec2 x)
	{
		return new bvec2(isnan(x.x), isnan(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 isnan(vec3 x)
	{
		return new bvec3(isnan(x.x), isnan(x.y), isnan(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 isnan(vec4 x)
	{
		return new bvec4(isnan(x.x), isnan(x.y), isnan(x.z), isnan(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool isinf(float x)
	{
		return float.IsInfinity(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 isinf(vec2 x)
	{
		return new bvec2(isinf(x.x), isinf(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 isinf(vec3 x)
	{
		return new bvec3(isinf(x.x), isinf(x.y), isinf(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 isinf(vec4 x)
	{
		return new bvec4(isinf(x.x), isinf(x.y), isinf(x.z), isinf(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int floatBitsToInt(float x)
	{
		return BitConverter.ToInt32(BitConverter.GetBytes(x));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec2 floatBitsToInt(vec2 x)
	{
		return new ivec2(floatBitsToInt(x.x), floatBitsToInt(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec3 floatBitsToInt(vec3 x)
	{
		return new ivec3(floatBitsToInt(x.x), floatBitsToInt(x.y), floatBitsToInt(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ivec4 floatBitsToInt(vec4 x)
	{
		return new ivec4(floatBitsToInt(x.x), floatBitsToInt(x.y), floatBitsToInt(x.z), floatBitsToInt(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint floatBitsToUint(float x)
	{
		return BitConverter.ToUInt32(BitConverter.GetBytes(x));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec2 floatBitsToUint(vec2 x)
	{
		return new uvec2(floatBitsToUint(x.x), floatBitsToUint(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec3 floatBitsToUint(vec3 x)
	{
		return new uvec3(floatBitsToUint(x.x), floatBitsToUint(x.y), floatBitsToUint(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uvec4 floatBitsToUint(vec4 x)
	{
		return new uvec4(floatBitsToUint(x.x), floatBitsToUint(x.y), floatBitsToUint(x.z), floatBitsToUint(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float intBitsToFloat(int x)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(x));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 intBitsToFloat(ivec2 x)
	{
		return new vec2(intBitsToFloat(x.x), intBitsToFloat(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 intBitsToFloat(ivec3 x)
	{
		return new vec3(intBitsToFloat(x.x), intBitsToFloat(x.y), intBitsToFloat(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 intBitsToFloat(ivec4 x)
	{
		return new vec4(floatBitsToInt(x.x), floatBitsToInt(x.y), floatBitsToInt(x.z), floatBitsToInt(x.w));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float uintBitsToFloat(uint x)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(x));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 uintBitsToFloat(uvec2 x)
	{
		return new vec2(uintBitsToFloat(x.x), uintBitsToFloat(x.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 uintBitsToFloat(uvec3 x)
	{
		return new vec3(uintBitsToFloat(x.x), uintBitsToFloat(x.y), uintBitsToFloat(x.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 uintBitsToFloat(uvec4 x)
	{
		return new vec4(uintBitsToFloat(x.x), uintBitsToFloat(x.y), uintBitsToFloat(x.z), uintBitsToFloat(x.w));
	}

	#endregion //MATH_FUNCTIONS

	#region GEOMETRIC_FUNCTIONS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float length(float x)
	{
		return abs(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float length(vec2 x)
	{
		return sqrt(x.x * x.x + x.y * x.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float length(vec3 x)
	{
		return sqrt(x.x * x.x + x.y * x.y + x.z * x.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float length(vec4 x)
	{
		return sqrt(x.x * x.x + x.y * x.y + x.z * x.z + x.w * x.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float distance(float a, float b)
	{
		return length(a - b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float distance(vec2 a, vec2 b)
	{
		return length(a - b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float distance(vec3 a, vec3 b)
	{
		return length(a - b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float distance(vec4 a, vec4 b)
	{
		return length(a - b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float dot(float a, float b)
	{
		return a * b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float dot(vec2 a, vec2 b)
	{
		return a.x * b.x + a.y * b.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float dot(vec3 a, vec3 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float dot(vec4 a, vec4 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 cross(vec3 a, vec3 b)
	{
		return new vec3(
			a.y * b.z - b.y * a.z,
			a.z * b.x - b.z * a.x,
			a.x * b.z - b.x * a.y
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float normalize(float x)
	{
		return 1f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 normalize(vec2 x)
	{
		return x / length(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 normalize(vec3 x)
	{
		return x / length(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 normalize(vec4 x)
	{
		return x / length(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 reflect(vec3 I, vec3 N)
	{
		return I * 2f - dot(N, I) * N;
	}

	public static vec3 refract(vec3 I, vec3 N, float eta)
	{
		float k = 1f - eta * eta * (1f - dot(N, I) * dot(N, I));
		return k < 0f
			? new vec3(0f)
			: eta * I - (eta * dot(N, I) + sqrt(k)) * N;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float faceforward(float N, float I, float Nref)
	{
		return dot(Nref, I) < 0f ? N : -N;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec2 faceforward(vec2 N, vec2 I, vec2 Nref)
	{
		return dot(Nref, I) < 0f ? N : -N;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec3 faceforward(vec3 N, vec3 I, vec3 Nref)
	{
		return dot(Nref, I) < 0f ? N : -N;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static vec4 faceforward(vec4 N, vec4 I, vec4 Nref)
	{
		return dot(Nref, I) < 0f ? N : -N;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 matrixCompMult(mat2 x, mat2 y)
	{
		return new mat2(
			x[0] * y[0],
			x[1] * y[1]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 matrixCompMult(mat3 x, mat3 y)
	{
		return new mat3(
			x[0] * y[0],
			x[1] * y[1],
			x[2] * y[2]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 matrixCompMult(mat4 x, mat4 y)
	{
		return new mat4(
			x[0] * y[0],
			x[1] * y[1],
			x[2] * y[2],
			x[3] * y[3]
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 outerProduct(vec2 column, vec2 row)
	{
		return new mat2(
			new vec2(row.x * column.x, row.x * column.y),
			new vec2(row.y * column.x, row.y * column.y)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 outerProduct(vec3 column, vec3 row)
	{
		return new mat3(
			new vec3(row.x * column.x, row.x * column.y, row.x * column.z),
			new vec3(row.y * column.x, row.y * column.y, row.y * column.z),
			new vec3(row.z * column.x, row.z * column.y, row.z * column.z)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 outerProduct(vec4 column, vec4 row)
	{
		return new mat4(
			new vec4(row.x * column.x, row.x * column.y, row.x * column.z, row.x * column.w),
			new vec4(row.y * column.x, row.y * column.y, row.y * column.z, row.y * column.w),
			new vec4(row.z * column.x, row.z * column.y, row.z * column.z, row.z * column.w),
			new vec4(row.w * column.x, row.w * column.y, row.w * column.z, row.w * column.w)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 transpose(mat2 mat)
	{
		return new mat2(
			new vec2(mat[0, 0], mat[1, 0]),
			new vec2(mat[0, 1], mat[1, 1])
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 transpose(mat3 mat)
	{
		return new mat3(
			new vec3(mat[0, 0], mat[1, 0], mat[2, 0]),
			new vec3(mat[0, 1], mat[1, 1], mat[2, 1]),
			new vec3(mat[0, 2], mat[1, 2], mat[2, 2])
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 transpose(mat4 mat)
	{
		return new mat4(
			new vec4(mat[0, 0], mat[1, 0], mat[2, 0], mat[3, 0]),
			new vec4(mat[0, 1], mat[1, 1], mat[2, 1], mat[3, 1]),
			new vec4(mat[0, 2], mat[1, 2], mat[2, 2], mat[3, 2]),
			new vec4(mat[0, 3], mat[1, 3], mat[2, 3], mat[3, 3])
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float determinant(mat2 mat)
	{
		return mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float determinant(mat3 mat)
	{
		return mat[0, 0] * (mat[1, 1] * mat[2, 2] - mat[2, 1] * mat[1, 2])
			- mat[1, 0] * (mat[0, 1] * mat[2, 2] - mat[2, 1] * mat[0, 2])
			+ mat[2, 0] * (mat[0, 1] * mat[1, 2] - mat[1, 1] * mat[0, 2]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float determinant(mat4 mat)
	{
		return mat[0, 0] * determinant(new mat3(mat[1].yzw, mat[2].yzw, mat[3].yzw))
			- mat[1, 0] * determinant(new mat3(mat[0].yzw, mat[2].yzw, mat[3].yzw))
			+ mat[2, 0] * determinant(new mat3(mat[0].yzw, mat[1].yzw, mat[3].yzw))
			- mat[3, 0] * determinant(new mat3(mat[0].yzw, mat[1].yzw, mat[2].yzw));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat2 inverse(mat2 mat)
	{
		return new mat2(new vec2(mat[1, 1], -mat[0, 1]), new vec2(-mat[1, 0], mat[0, 0])) / determinant(mat);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat3 inverse(mat3 mat)
	{
		return new mat3(
			new vec3(mat[1, 1] * mat[2, 2] - mat[2, 1] * mat[1, 2], mat[2, 0] * mat[1, 2] - mat[1, 0] * mat[2, 2], mat[1, 0] * mat[2, 1] - mat[2, 0] * mat[1, 1]),
			new vec3(mat[2, 1] * mat[0, 2] - mat[0, 1] * mat[2, 2], mat[0, 0] * mat[2, 2] - mat[2, 0] * mat[0, 2], mat[2, 0] * mat[0, 2] - mat[0, 0] * mat[2, 1]),
			new vec3(mat[0, 1] * mat[1, 2] - mat[1, 1] * mat[0, 2], mat[1, 0] * mat[0, 2] - mat[0, 0] * mat[1, 2], mat[0, 0] * mat[1, 1] - mat[1, 0] * mat[0, 1])
		) / determinant(mat);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static mat4 inverse(mat4 mat)
	{
		mat4 lResult = new mat4(
			new vec4(
				determinant(new mat3(mat[1].yzw, mat[2].yzw, mat[3].yzw)),
				determinant(new mat3(mat[0].yzw, mat[2].yzw, mat[3].yzw)),
				determinant(new mat3(mat[0].yzw, mat[1].yzw, mat[3].yzw)),
				determinant(new mat3(mat[0].yzw, mat[1].yzw, mat[2].yzw))
			),
			new vec4(
				determinant(new mat3(mat[1].xzw, mat[2].xzw, mat[3].xzw)),
				determinant(new mat3(mat[0].xzw, mat[2].xzw, mat[3].xzw)),
				determinant(new mat3(mat[0].xzw, mat[1].xzw, mat[3].xzw)),
				determinant(new mat3(mat[0].xzw, mat[1].xzw, mat[2].xzw))
			),
			new vec4(
				determinant(new mat3(mat[1].xyw, mat[2].xyw, mat[3].xyw)),
				determinant(new mat3(mat[0].xyw, mat[2].xyw, mat[3].xyw)),
				determinant(new mat3(mat[0].xyw, mat[1].xyw, mat[3].xyw)),
				determinant(new mat3(mat[0].xyw, mat[1].xyw, mat[2].xyw))
			),
			new vec4(
				determinant(new mat3(mat[1].xyz, mat[2].xyz, mat[3].xyz)),
				determinant(new mat3(mat[0].xyz, mat[2].xyz, mat[3].xyz)),
				determinant(new mat3(mat[0].xyz, mat[1].xyz, mat[3].xyz)),
				determinant(new mat3(mat[0].xyz, mat[1].xyz, mat[2].xyz))
			)
		);

		float lDet = mat[0, 0] * lResult[0, 0]
		           - mat[1, 0] * lResult[1, 0]
				   + mat[2, 0] * lResult[2, 0]
				   - mat[3, 0] * lResult[3, 0];

		lResult /= lDet;
		return lResult;
	}

	#endregion //GEOMETRIC_FUNCTIONS

	#region COMPARISON_FUNCTIONS

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThan(float a, float b)
	{
		return a < b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThan(int a, int b)
	{
		return a < b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThan(uint a, uint b)
	{
		return a < b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThan(vec2 a, vec2 b)
	{
		return new bvec2(a.x < b.x, a.y < b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThan(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x < b.x, a.y < b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThan(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x < b.x, a.y < b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThan(vec3 a, vec3 b)
	{
		return new bvec3(a.x < b.x, a.y < b.y, a.z < b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThan(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x < b.x, a.y < b.y, a.z < b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThan(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x < b.x, a.y < b.y, a.z < b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThan(vec4 a, vec4 b)
	{
		return new bvec4(a.x < b.x, a.y < b.y, a.z < b.z, a.w < b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThan(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x < b.x, a.y < b.y, a.z < b.z, a.w < b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThan(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x < b.x, a.y < b.y, a.z < b.z, a.w < b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThan(float a, float b)
	{
		return a > b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThan(int a, int b)
	{
		return a > b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThan(uint a, uint b)
	{
		return a > b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThan(vec2 a, vec2 b)
	{
		return new bvec2(a.x > b.x, a.y > b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThan(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x > b.x, a.y > b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThan(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x > b.x, a.y > b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThan(vec3 a, vec3 b)
	{
		return new bvec3(a.x > b.x, a.y > b.y, a.z > b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThan(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x > b.x, a.y > b.y, a.z > b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThan(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x > b.x, a.y > b.y, a.z > b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThan(vec4 a, vec4 b)
	{
		return new bvec4(a.x > b.x, a.y > b.y, a.z > b.z, a.w > b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThan(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x > b.x, a.y > b.y, a.z > b.z, a.w > b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThan(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x > b.x, a.y > b.y, a.z > b.z, a.w > b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThanEqual(float a, float b)
	{
		return a <= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThanEqual(int a, int b)
	{
		return a <= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool lessThanEqual(uint a, uint b)
	{
		return a <= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThanEqual(vec2 a, vec2 b)
	{
		return new bvec2(a.x <= b.x, a.y <= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThanEqual(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x <= b.x, a.y <= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 lessThanEqual(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x <= b.x, a.y <= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThanEqual(vec3 a, vec3 b)
	{
		return new bvec3(a.x <= b.x, a.y <= b.y, a.z <= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThanEqual(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x <= b.x, a.y <= b.y, a.z <= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 lessThanEqual(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x <= b.x, a.y <= b.y, a.z <= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThanEqual(vec4 a, vec4 b)
	{
		return new bvec4(a.x <= b.x, a.y <= b.y, a.z <= b.z, a.w <= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThanEqual(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x <= b.x, a.y <= b.y, a.z <= b.z, a.w <= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 lessThanEqual(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x <= b.x, a.y <= b.y, a.z <= b.z, a.w <= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThanEqual(float a, float b)
	{
		return a >= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThanEqual(int a, int b)
	{
		return a >= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool greaterThanEqual(uint a, uint b)
	{
		return a >= b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThanEqual(vec2 a, vec2 b)
	{
		return new bvec2(a.x >= b.x, a.y >= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThanEqual(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x >= b.x, a.y >= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 greaterThanEqual(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x >= b.x, a.y >= b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThanEqual(vec3 a, vec3 b)
	{
		return new bvec3(a.x >= b.x, a.y >= b.y, a.z >= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThanEqual(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x >= b.x, a.y >= b.y, a.z >= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 greaterThanEqual(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x >= b.x, a.y >= b.y, a.z >= b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThanEqual(vec4 a, vec4 b)
	{
		return new bvec4(a.x >= b.x, a.y >= b.y, a.z >= b.z, a.w >= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThanEqual(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x >= b.x, a.y >= b.y, a.z >= b.z, a.w >= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 greaterThanEqual(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x >= b.x, a.y >= b.y, a.z >= b.z, a.w >= b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool equal(float a, float b)
	{
		return a == b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool equal(int a, int b)
	{
		return a == b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool equal(uint a, uint b)
	{
		return a == b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 equal(vec2 a, vec2 b)
	{
		return new bvec2(a.x == b.x, a.y == b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 equal(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x == b.x, a.y == b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 equal(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x == b.x, a.y == b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 equal(vec3 a, vec3 b)
	{
		return new bvec3(a.x == b.x, a.y == b.y, a.z == b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 equal(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x == b.x, a.y == b.y, a.z == b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 equal(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x == b.x, a.y == b.y, a.z == b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 equal(vec4 a, vec4 b)
	{
		return new bvec4(a.x == b.x, a.y == b.y, a.z == b.z, a.w == b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 equal(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x == b.x, a.y == b.y, a.z == b.z, a.w == b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 equal(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x == b.x, a.y == b.y, a.z == b.z, a.w == b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool notEqual(float a, float b)
	{
		return a != b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool notEqual(int a, int b)
	{
		return a != b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool notEqual(uint a, uint b)
	{
		return a != b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 notEqual(vec2 a, vec2 b)
	{
		return new bvec2(a.x != b.x, a.y != b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 notEqual(ivec2 a, ivec2 b)
	{
		return new bvec2(a.x != b.x, a.y != b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 notEqual(uvec2 a, uvec2 b)
	{
		return new bvec2(a.x != b.x, a.y != b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 notEqual(vec3 a, vec3 b)
	{
		return new bvec3(a.x != b.x, a.y != b.y, a.z != b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 notEqual(ivec3 a, ivec3 b)
	{
		return new bvec3(a.x != b.x, a.y != b.y, a.z != b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 notEqual(uvec3 a, uvec3 b)
	{
		return new bvec3(a.x != b.x, a.y != b.y, a.z != b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 notEqual(vec4 a, vec4 b)
	{
		return new bvec4(a.x != b.x, a.y != b.y, a.z != b.z, a.w != b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 notEqual(ivec4 a, ivec4 b)
	{
		return new bvec4(a.x != b.x, a.y != b.y, a.z != b.z, a.w != b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 notEqual(uvec4 a, uvec4 b)
	{
		return new bvec4(a.x != b.x, a.y != b.y, a.z != b.z, a.w != b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool any(bool x)
	{
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool any(bvec2 x)
	{
		return x.x || x.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool any(bvec3 x)
	{
		return x.x || x.y || x.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool any(bvec4 x)
	{
		return x.x || x.y || x.z || x.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool all(bool x)
	{
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool all(bvec2 x)
	{
		return x.x && x.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool all(bvec3 x)
	{
		return x.x && x.y && x.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool all(bvec4 x)
	{
		return x.x && x.y && x.z && x.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool not(bool x)
	{
		return !x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec2 not(bvec2 x)
	{
		return !x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec3 not(bvec3 x)
	{
		return !x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bvec4 not(bvec4 x)
	{
		return !x;
	}

	#endregion //COMPARISON_FUNCTIONS
}