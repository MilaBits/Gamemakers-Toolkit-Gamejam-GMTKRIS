using System.Collections.Generic;
using UnityEngine;

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

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}