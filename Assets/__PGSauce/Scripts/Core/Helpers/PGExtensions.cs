using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.Utilities
{
    public static class PGExtensions
    {
        public static float Remap(this float value, float inputA, float inputB, float outputA, float outputB)
        {
            return (value - inputA) / (inputB - inputA) * (outputB - outputA) + outputA;
        }

        public static float Remap(this int value, float inputA, float inputB, float outputA, float outputB)
        {
            return Remap((float)value, inputA, inputB, outputA, outputB);
        }

        /// 
        /// Is the object left, right, or in front ?
        /// 
        /// 
        /// 
        /// 
        /// -1 = left, 1 = right, 0 = in front (or behind)
        public static float RelativeOrientation(this Transform transform, Vector3 targetDir)
        {
            Vector3 up = transform.up;
            Vector3 forward = transform.forward;

            Vector3 perp = Vector3.Cross(forward, targetDir);
            float dir = Vector3.Dot(perp, up);
            if (dir > 0f)
            {
                return 1f;
            }
            else if (dir < 0f)
            {
                return -1f;
            }
            else
            {
                return 0f;
            }
        }
    }
}
