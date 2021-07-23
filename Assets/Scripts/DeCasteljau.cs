using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    public class DeCasteljau
    {
        public static List<Vector2> GetBezierCurve(List<Vector2> controlPoints, float precision)
        {
            var points = new List<Vector2>();
            for (float t = 0; t <= 1; t += precision)
            {
                var point = GetBezierPointAt(controlPoints, t);

                points.Add(point);
            }

            return points;
        }

        public static Vector2 GetBezierPointAt(List<Vector2> controlPoints, float t)
        {
            var oneMinusT = 1 - t;
            var approximatedPoints = new List<Vector2>(controlPoints);

            while (approximatedPoints.Count > 1)
            {
                for (int i = 0, e = approximatedPoints.Count - 1; i < e; i++)
                {
                    approximatedPoints[i] = oneMinusT * approximatedPoints[i] + t * approximatedPoints[i + 1];
                }

                approximatedPoints.RemoveAt(approximatedPoints.Count - 1);
            }

            var point = approximatedPoints[0];
            return point;
        }
        
        public static Vector2 GetBezierTangentAt(List<Vector2> controlPoints, float t)
        {
            var oneMinusT = 1 - t;
            var approximatedPoints = new List<Vector2>(controlPoints);

            while (approximatedPoints.Count > 2)
            {
                for (int i = 0, e = approximatedPoints.Count - 1; i < e; i++)
                {
                    approximatedPoints[i] = oneMinusT * approximatedPoints[i] + t * approximatedPoints[i + 1];
                }

                approximatedPoints.RemoveAt(approximatedPoints.Count - 1);
            }
            
            return (approximatedPoints[1] - approximatedPoints[0]).normalized;
        }
    }
}
