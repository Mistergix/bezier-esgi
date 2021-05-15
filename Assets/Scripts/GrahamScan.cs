using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Esgi.Bezier
{
    public class GrahamScan
    {
        public static List<Vector3> ComputeGrahamScan(List<Vector3> originalPoints)
        {
            if (originalPoints.Count < 2)
            {
                return originalPoints.ToList();
            }
            
            // sort point find min y, then min x if equals
            var indexMin = Enumerable.Range(0, originalPoints.Count).Aggregate((iMin, iCur) =>
            {
                if (originalPoints[iCur].y < originalPoints[iMin].y)
                {
                    return iCur;
                }

                if (originalPoints[iCur].y > originalPoints[iMin].y)
                {
                    return iMin;
                }

                if (originalPoints[iCur].x < originalPoints[iMin].x)
                {
                    return iCur;
                }

                return iMin;
            });
            
            // sort points by angle (from pivot)
            var sort = Enumerable.Range(0, originalPoints.Count)
                .Where(i => i != indexMin) // skip pivot
                .Select(i =>
                    new KeyValuePair<float, Vector3>(
                        Mathf.Atan2(originalPoints[i].y - originalPoints[indexMin].y, originalPoints[i].x - originalPoints[indexMin].x), originalPoints[i]))
                .OrderBy(pair => pair.Key)
                .Select(pair => pair.Value);

            var points = new List<Vector3>();
            points.Add(originalPoints[indexMin]);
            points.AddRange(sort);

            int M = 0;

            for (int i = 1, N = points.Count; i < N; i++)
            {
                bool keepNewPoint = true;

                if (M == 0)
                {
                    keepNewPoint = !NearlyEqual(points[0], points[i]);
                }
                else
                {
                    while (true)
                    {
                        var flag = WhichToRemoveFromBoundary(points[M - 1], points[M], points[i]);
                        if (flag == RemovalFlag.None)
                        {
                            break;
                        }
                        else if (flag == RemovalFlag.MidPoint)
                        {
                            if (M > 0)
                            {
                                M--;
                            }

                            if (M == 0)
                            {
                                break;
                            }
                        }
                        else if (flag == RemovalFlag.EndPoint)
                        {
                            keepNewPoint = false;
                            break;
                        }
                        else
                        {
                            throw new UnityException("IMPOSSIBLE");
                        }
                    }
                }

                if (keepNewPoint)
                {
                    M++;
                    Swap(points, M, i);
                }
            }
            
            points.RemoveRange(M + 1, points.Count - M - 1);
            return points;
        }

        static void Swap<T>(IList<T> list, int i, int j)
        {
            if (i != j)
            {
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        enum RemovalFlag
        {
            None,
            MidPoint,
            EndPoint
        };
        
        static double CCW(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Compute (p2 - p1) X (p3 - p1)
            float cross1 = (p2.x - p1.x) * (p3.y - p1.y);
            float cross2 = (p2.y - p1.y) * (p3.x - p1.x);
            if (Mathf.Approximately(cross1,cross2))
                return 0;
            return cross1 - cross2;
        }

        static RemovalFlag WhichToRemoveFromBoundary(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var cross = CCW(p1, p2, p3);
            if (cross < 0)
                // Remove p2
                return RemovalFlag.MidPoint;
            if (cross > 0)
                // Remove none.
                return RemovalFlag.None;
            // Check for being reversed using the dot product off the difference vectors.
            var dotp = (p3.x - p2.x) * (p2.x - p1.x) + (p3.y - p2.y) * (p2.y - p1.y);
            if (Mathf.Approximately(dotp, 0.0f))
                // Remove p2
                return RemovalFlag.MidPoint;
            if (dotp < 0)
                // Remove p3
                return RemovalFlag.EndPoint;
            else
                // Remove p2
                return RemovalFlag.MidPoint;
        }

        private static bool NearlyEqual(Vector3 point, Vector3 vector3)
        {
            return Mathf.Approximately(point.x, point.y) && Mathf.Approximately(vector3.x, vector3.y);
        }
    }
}
