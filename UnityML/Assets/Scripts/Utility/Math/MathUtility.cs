namespace Utility.Math
{
	public static class MathUtility
	{
		public static bool IsInRange(float value, float min, float max)
		{
			return min <= value && value <= max;
		}
	}
}