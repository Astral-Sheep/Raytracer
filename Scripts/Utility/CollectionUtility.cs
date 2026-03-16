using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Astral.Tools;

public static class CollectionExtensions
{
	public static T[] Append<T>(this T[] pArray, T pItem)
	{
		if (pArray == null)
		{
			return [pItem];
		}

		return Enumerable.Append(pArray, pItem) as T[];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf<T>(this T[] pArray, T pItem)
	{
		return Array.IndexOf(pArray, pItem);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IndexIsValid<T>(this IEnumerable<T> pEnumerable, int pIndex)
	{
		return pIndex >= 0 && pEnumerable != null && pIndex < pEnumerable.Count();
	}

	public static T[] RemoveAt<T>(T[] pArray, int pIndex)
	{
		if (!IndexIsValid(pArray, pIndex))
			return pArray;

		T[] lDest = new T[pArray.Length - 1];

		if (pIndex > 0)
		{
			Array.Copy(pArray, 0, lDest, 0, pIndex);
		}

		if (pIndex < pArray.Length - 1)
		{
			Array.Copy(pArray, pIndex + 1, lDest, pIndex, pArray.Length - 1 - pIndex);
		}

		return lDest;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Remove<T>(T[] pArray, T pItem)
	{
		return RemoveAt(pArray, IndexOf(pArray, pItem));
	}

	public static IEnumerable<int> GetSteppedRange(int pFromInclusive, int pToExclusive, int pStep)
	{
		int[] lRange = new int[(int)Math.Ceiling((pToExclusive - pFromInclusive) / (float)pStep)];

		for (int i = pFromInclusive; i < pToExclusive; i += pStep)
		{
			lRange[i / pStep] = i;
		}

		return lRange;
	}
}