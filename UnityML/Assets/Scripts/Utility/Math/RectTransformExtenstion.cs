using UnityEngine;

namespace Utility.Math
{
    public static class RectTransformExtenstion
    {
        public static Bounds2D GetBounds(this RectTransform rectTransform) 
        {
            var pos = rectTransform.position.XY();
            var rect = rectTransform.rect;
            var min = pos + rect.min;
            var max = pos + rect.max;
            return new Bounds2D(min, max);
        }
    }
}