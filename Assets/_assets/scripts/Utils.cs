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
        public static List<Bounds> SplitBounds(Bounds boundsIn, int amountRow, int amountColumn)
        {
            List<Bounds> boundsList = new List<Bounds>();
            float intervalRow = boundsIn.size.x / amountRow;
            float intervalColumn = boundsIn.size.z / amountColumn;
            Vector3 size = new Vector3(intervalRow, boundsIn.size.y, intervalColumn);
            for (float x = boundsIn.min.x; x < boundsIn.max.x; x+=intervalRow)
            {
                for (float z = boundsIn.min.z; z < boundsIn.max.z; z+=intervalColumn)
                {
                    Bounds bounds = new Bounds();
                    bounds.center = new Vector3(x + (intervalRow/2f), boundsIn.center.y, z + (intervalColumn/2f));
                    bounds.size = size;
                    boundsList.Add(bounds);
                }
            }
            return boundsList;
        }
        public static Vector3Int VectorToInt(Vector3 vectorIn)
        {
            return new Vector3Int((int)vectorIn.x, (int)vectorIn.y, (int)vectorIn.z);
        }
    }
}
