using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Tools
    {
        public static bool IsHit(float percentage)
        {
            float rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < percentage)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
