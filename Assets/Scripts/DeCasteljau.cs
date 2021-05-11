using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    public class DeCasteljau
    {
        public static List<Vector2> GetBezierCurve(List<Vector2> controlPoints, float precision)
        {
            List<Vector2> points = new List<Vector2>();
            for (float t = 0; t <= 1; t += precision)
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
                
                points.Add(approximatedPoints[0]);
            }

            return points;
        }
    }
}
