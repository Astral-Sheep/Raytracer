using Godot;

namespace Astral.Tools;

public static class MathUtility
{
	public const float LOG2 = 0.69314718056f;

	public static Vector2 ComputeBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float pTime)
	{
		float lSqrT = pTime * pTime;
		float lOneMinT = 1 - pTime;
		float lOneMinInvT = lOneMinT * lOneMinT;
		return lOneMinT * lOneMinInvT * p0 + 3f * lOneMinInvT * pTime * p1 + 3f * lOneMinT * lSqrT * p2 + pTime * lSqrT * p3;
	}
}