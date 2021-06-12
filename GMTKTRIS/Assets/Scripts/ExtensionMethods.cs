using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace DefaultNamespace
{
    public static class ExtensionMethods
    {
        public static Vector2Int Rotate(this Vector2Int v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (int) (cos * tx) - (int) (sin * ty);
            v.y = (int) (sin * tx) + (int) (cos * ty);
            return v;
        }
    }
}