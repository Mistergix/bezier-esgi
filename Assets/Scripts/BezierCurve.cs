using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes;

namespace Esgi.Bezier
{
    public class BezierCurve : ImmediateModeShapeDrawer
    {
        private List<ControlPoint> controlPoints;
        public bool IsMainCurve { get; set; }

        public int PointCount => controlPoints.Count;


        private void Awake()
        {
            controlPoints = new List<ControlPoint>();
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                if (controlPoints.Count >= 2)
                {
                    float precision = 1f / BezierCurveManager.Instance.Steps;
                    var coordinates =
                        DeCasteljau.GetBezierCurve(controlPoints.Select(point => point.position).ToList(), precision);

                    for (int i = 0; i < coordinates.Count; i++)
                    {
                        Draw.Disc(coordinates[i], BezierCurveManager.Instance.CurvePointRadius,
                            CurveColor);

                        if (BezierCurveManager.Instance.CompleteCasteljauLines && i + 1 < coordinates.Count)
                        {
                            Draw.Line(coordinates[i], coordinates[i + 1], BezierCurveManager.Instance.CurvePointRadius * 2, CurveColor);
                        }
                    }
                }

                if (!BezierCurveManager.Instance.HideMetaData)
                {
                    for (int i = 0; i < controlPoints.Count; i++)
                    {
                        DrawControlPolygons(i);
                        DrawControlPointsHandles(i);
                    }
                }

                if (BezierCurveManager.Instance.ShowConvexHull)
                {
                    var hull = GrahamScan.ComputeGrahamScan(controlPoints.Select(point => (Vector3)point.position).ToList());
                    for (int i = 0; i < hull.Count; i++)
                    {
                        DrawConvexHull(hull, i);
                    }
                }
            }
        }

        private void DrawConvexHull(List<Vector3> hull, int i)
        {
            Draw.Line(hull[i], hull[(i + 1) >= hull.Count ? 0 : i + 1], Color.black);
        }

        private Color CurveColor => IsMainCurve ? BezierCurveManager.Instance.MainCurveColor : BezierCurveManager.Instance.CurveColor;

        public List<ControlPoint> ControlPoints => controlPoints;

        private void DrawControlPointsHandles(int i)
        {
            Draw.Ring(controlPoints[i].position, BezierCurveManager.Instance.ControlPointRadius, BezierCurveManager.Instance.HandleColor);
        }

        private void DrawControlPolygons(int i)
        {
            if (i + 1 >= controlPoints.Count)
            {
                if (BezierCurveManager.Instance.LoopControlPolygon)
                {
                    Draw.LineDashed(controlPoints[i].position, controlPoints[0].position);
                }
            }
            else
            {
                Draw.LineDashed(controlPoints[i].position, controlPoints[i + 1].position);
            }
        }


        public ControlPoint AppendPoint(Vector3 clickInWorldPos)
        {
            ControlPoint cp = new ControlPoint(clickInWorldPos);
            controlPoints.Add(cp);

            return cp;
        }

        public ControlPoint TryDestroyPoint(Vector3 clickInWorldPos)
        {
            var cps = ControlPointsInRadius(controlPoints, clickInWorldPos);

            if (cps.Count > 0)
            {
                controlPoints.Remove(cps[0]);
                return cps[0];
            }

            return null;
        }

        public static List<ControlPoint> ControlPointsInRadius(List<ControlPoint> points, Vector3 clickInWorldPos)
        {
            List<ControlPoint> cps = points.Where(point =>
                Vector2.Distance(point.position, clickInWorldPos) <= BezierCurveManager.Instance.ControlPointRadius).ToList();

            cps.Sort((cp1, cp2) =>
            {
                float dist1 = Vector2.Distance(cp1.position, clickInWorldPos);
                float dist2 = Vector2.Distance(cp2.position, clickInWorldPos);

                if (dist1 > dist2)
                {
                    return 1;
                }

                if (dist2 > dist1)
                {
                    return -1;
                }

                return 0;
            });
            return cps;
        }

        public void DestroyPoint(ControlPoint closestPoint)
        {
            controlPoints.Remove(closestPoint);
        }
    }
}
