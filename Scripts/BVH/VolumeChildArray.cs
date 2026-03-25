using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Astral.Tools;

namespace Astral.Raytracer;

public class VolumeChildArray : IEnumerable<IBounded>
{
	public int Count => shapes.Count + meshes.Count;
	public int ShapeCount => shapes.Count;
	public int MeshCount => meshes.Count;

	public readonly List<IPureShape> shapes = new List<IPureShape>();
	public readonly List<BVHMesh> meshes = new List<BVHMesh>();

	public VolumeChildArray() {}

	public VolumeChildArray(IPureShape[] pShapes, BVHMesh[] pMeshes)
	{
		shapes = new List<IPureShape>(pShapes);
		meshes = new List<BVHMesh>(pMeshes);
	}

	public void Add(IBounded pChild)
	{
		switch (pChild)
		{
			case IPureShape lShape:
				AddShape(lShape);
				break;
			case BVHMesh lMesh:
				AddMesh(lMesh);
				break;
			default:
				return;
		}
	}

	public bool Contains(IBounded pChild)
	{
		return pChild switch {
			IPureShape lShape => ContainsShape(lShape),
			BVHMesh lMesh => ContainsMesh(lMesh),
			_ => false,
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		shapes.Clear();
		meshes.Clear();
	}

	public bool Remove(IBounded pChild)
	{
		return pChild switch {
			IPureShape lShape => RemoveShape(lShape),
			BVHMesh lMesh => RemoveMesh(lMesh),
			_ => false,
		};
	}

	public void RemoveAt(int pIndex)
	{
		if (pIndex < 0)
			return;

		if (pIndex < shapes.Count)
		{
			shapes.RemoveAt(pIndex);
			return;
		}

		pIndex -= shapes.Count;

		if (pIndex < meshes.Count)
		{
			meshes.RemoveAt(pIndex);
		}
	}

	public void AddShape(IPureShape pShape)
	{
		if (shapes.Contains(pShape))
			return;

		shapes.Add(pShape);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ContainsShape(IPureShape pShape)
	{
		return shapes.Contains(pShape);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearShapes()
	{
		shapes.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveShape(IPureShape pShape)
	{
		return shapes.Remove(pShape);
	}

	public void RemoveShapeAt(int pIndex)
	{
		if (!shapes.IndexIsValid(pIndex))
			return;

		shapes.RemoveAt(pIndex);
	}

	public void AddMesh(BVHMesh pMesh)
	{
		if (meshes.Contains(pMesh))
			return;

		meshes.Add(pMesh);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ContainsMesh(BVHMesh pMesh)
	{
		return meshes.Contains(pMesh);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearMeshes()
	{
		meshes.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveMesh(BVHMesh pMesh)
	{
		return meshes.Remove(pMesh);
	}

	public void RemoveMeshAt(int pIndex)
	{
		if (!meshes.IndexIsValid(pIndex))
			return;

		shapes.RemoveAt(pIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<IBounded> GetEnumerator()
	{
		return shapes.Select(IBounded (s) => s).Union(meshes).GetEnumerator();
	}
}