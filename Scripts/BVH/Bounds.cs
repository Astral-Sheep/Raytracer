using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Astral.Raytracer;

public struct Bounds
{
	public vec3 Min
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
		get => _min;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_min = value;
			_max = max(_min, _max);
			_center = (_max + _min) * .5f;
			_extent = (_max - _min) * .5f;
		}
	}

	public vec3 Max
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
		get => _max;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_max = value;
			_min = min(_min, _max);
			_center = (_max + _min) * .5f;
			_extent = (_max - _min) * .5f;
		}
	}

	public vec3 Center
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
		get => _center;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_center = value;
			_min = _center - _extent;
			_max = _center + _extent;
		}
	}

	public vec3 Extent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
		get => _extent;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_extent = value;
			_min = _center - _extent;
			_max = _center + _extent;
		}
	}

	private vec3 _min;
	private vec3 _max;
	private vec3 _center;
	private vec3 _extent;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds()
	{
		_min = new vec3(0f);
		_max = new vec3(0f);
		_center = new vec3(0f);
		_extent = new vec3(0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds(vec3 pMin, vec3 pMax)
	{
		_min = pMin;
		_max = pMax;
		_center = (_max + _min) * .5f;
		_extent = (_max - _min) * .5f;
	}

	public Bounds(Bounds pBounds)
	{
		_min = pBounds._min;
		_max = pBounds._max;
		_center = pBounds._center;
		_extent = pBounds._extent;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Bounds FromExtent(vec3 pExtent, vec3 pCenter)
	{
		return new Bounds {
			_min = pCenter - pExtent,
			_max = pCenter + pExtent,
			_center = pCenter,
			_extent = pExtent,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Include(vec3 pPoint)
	{
		_min = min(_min, pPoint);
		_max = max(_max, pPoint);
		_center = (_max + _min) * .5f;
		_extent = (_max - _min) * .5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Include(Bounds pBounds)
	{
		_min = min(_min, pBounds.Min);
		_max = max(_max, pBounds.Max);
		_center = (_max - _min) * .5f;
		_extent = (_max - _min) * .5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
	public float GetVolume()
	{
		return _extent.x * _extent.y * _extent.z * 8f;
	}
}