using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    static class Extensions
    {
        public static Vector3 Copy(this Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static bool NextBool(this System.Random random, double probability = 0.5)
        {
            return random.NextDouble() < probability;
        }

        public static int CountChildren(this Transform transform, Predicate<Transform> predicate)
        {
            int count = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (predicate(transform.GetChild(i))) count++;
            }
            return count;
        }

        public static Transform FirstChild(this Transform transform, Predicate<Transform> predicate)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (predicate(transform.GetChild(i))) return transform.GetChild(i);
            }
            return null;
        }

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }
    }
}
