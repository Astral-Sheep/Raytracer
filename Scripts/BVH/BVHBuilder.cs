using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Godot;

namespace Astral.Raytracer;

public static class BVHBuilder
{
	public static byte[] BuildBVH(IRaytracedShape[] pShapes, Dictionary<Godot.Material, int> pMaterialMap, int pDepth = 1)
	{
		BVHBoundingVolume lRoot = new BVHBoundingVolume(pShapes);
		lRoot.Split(pDepth);

		using (MemoryStream lShapeStream = new MemoryStream(), lDataStream = new MemoryStream())
		{
			using (BinaryWriter lShapeWriter = new BinaryWriter(lShapeStream), lDataWriter = new BinaryWriter(lDataStream))
			{
				List<BVHBoundingVolume> lVolumes = new List<BVHBoundingVolume>();
				int lCurrentVolume = 0;
				int lChildOffset = 0;
				lVolumes.Add(lRoot);

				while (lCurrentVolume < lVolumes.Count)
				{
					BVHBoundingVolume lVolume = lVolumes[lCurrentVolume];

					lShapeWriter.Write(new Shape {
						type = (int)ERaytracedShapeType.BoundingVolume,
						dataTexelIndex = (int)(lDataStream.Length / Raytracer.TEXEL_SIZE),
						boundMin = lVolume.min,
						boundMax = lVolume.max,
					}.GetBytes());

					lDataWriter.Write(new BoundingVolume {
						child0 = lVolumes.Count + lChildOffset,
						child1 = lVolumes.Count + lChildOffset + 1,
					}.GetBytes());

					Debug.Assert(
						lVolume.childVolumes.Count <= 0 || lVolume.childShapes.Count + lVolume.childVolumes.Count <= 2,
						$"BVH build assertion failed: total bounding volume child count is equal to {lVolume.childShapes.Count + lVolume.childVolumes.Count} ({lVolume.childShapes.Count} shapes and {lVolume.childVolumes.Count} volumes)"
					);

					for (int i = 0; i < lVolume.childShapes.Count; i++)
					{
						IRaytracedShape lShape = lVolume.childShapes[i];

						lShapeWriter.Write(new Shape {
							type = (int)lShape.Type,
							dataTexelIndex = (int)(lDataStream.Length / Raytracer.TEXEL_SIZE)
						}.GetBytes());

						switch (lShape.Type)
						{
							case ERaytracedShapeType.BoundingVolume:
								GD.PushError($"A bounding volume was pushed in the childShapes List of BoundingVolume {lCurrentVolume} at index {i}");
								break;
							case ERaytracedShapeType.Sphere:
								if (lShape is not RaytracedSphere lSphere)
									break;

								lDataWriter.Write(lSphere.GetShaderData(pMaterialMap).GetBytes());
								++lChildOffset;
								break;
							case ERaytracedShapeType.Mesh:

								++lChildOffset;
								break;
							default:
								GD.PushWarning($"Shape type {lShape.Type} is not supported");
								break;
						}
					}

					lVolumes.AddRange(lVolume.childVolumes);
					++lCurrentVolume;
				}
			}

			return lShapeStream.ToArray();
		}
	}

	public static byte[] BuildBVH(RaytracedMesh pMesh, Dictionary<Godot.Material, int> pMaterialMap, int pDepth = 1)
	{
		return null;
	}
}