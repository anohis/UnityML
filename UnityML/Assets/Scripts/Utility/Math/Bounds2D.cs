using UnityEngine;

namespace Utility.Math
{
    public struct Bounds2D
	{
		public Vector2 Min;
		public Vector2 Max;

		public Bounds2D(Vector2 min, Vector2 max)
		{
			Min = min;
			Max = max;
		}

		public bool IsContain(Vector2 pos)
		{
			return MathUtility.IsInRange(pos.x, Min.x, Max.x)
				&& MathUtility.IsInRange(pos.y, Min.y, Max.y);
		}

		public Vector2 ToLocalPos(Vector2 pos) 
		{
			return pos - Min;
		}
	}
}