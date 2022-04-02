using System;
using Tensorflow;

namespace Utility.Math
{
	public static class MathUtility
	{
		public static bool IsInRange(float value, float min, float max)
		{
			return min <= value && value <= max;
		}

        public static bool IsNaN(Tensor x)
        {
            var v = math_ops.reduce_sum(x).numpy();
            switch (v.dtype)
            {
                case TF_DataType.TF_INT32:
                    return float.IsNaN((int)v);
                case TF_DataType.TF_FLOAT:
                    return float.IsNaN(v);
                case TF_DataType.TF_DOUBLE:
                    return double.IsNaN(v);
            }
            throw new NotImplementedException("v.dtype");
        }
    }
}