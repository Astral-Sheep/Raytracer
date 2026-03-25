namespace Astral.Raytracer;

public interface IPureShape : IBounded
{
	ERaytracedShapeType GetShapeType();
	int[] GetMaterials();
	(ShapeData, IShaderData) GetShaderData(int pTexelIndex);
}

public interface IPureShape<T> : IPureShape where T : IShaderData
{
	new (ShapeData, T) GetShaderData(int pTexelIndex);
}
