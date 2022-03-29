using UnityEngine;

namespace Utility 
{
	public static class VectorExtenstion
	{
		public static Vector2 XY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}
	}
}