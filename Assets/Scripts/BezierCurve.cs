using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PGSauce.Core.PGDebugging;
using Shapes;
using UnityEditor;

namespace Esgi.Bezier
{
    public class BezierCurve : ImmediateModeShapeDrawer
    {
        private float tTest => Manager.Instance.tTest;
        private List<ControlPoint> controlPoints;
        public bool IsMainCurve { get; set; }

        private void OnDrawGizmos()
        {
            if(ControlPoints.Count < 2 || Mode != Manager.Mode.SweepPath){return;}
            var p = GetOrientedPointAt(tTest);
            Handles.PositionHandle(p.position, p.rotation);
        }

        public int PointCount => ControlPoints.Count;
        
        public Vector3 Right => Vector3.back;

        public OrientatedPoint GetOrientedPointAt(float t)
        {

            if (Profile2D == Manager.Profile2D.Bezier)
            {
                return new OrientatedPoint()
                {
                    forward = GetTangentAt(t),
                    position = GetPosAt(t),
                    right = Right,
                    rotation = GetOrientationAt(t),
                    up = GetUpAt(t)
                };
            }
            
            return new OrientatedPoint()
            {
                forward = GetTangentAtPolygon(t),
                position = GetPosAtPolygon(t),
                right = Right,
                rotation = GetOrientationAtPolygon(t),
                up = GetUpAtPolygon(t)
            };
            
        }

        private Quaternion GetOrientationAtPolygon(float t)
        {
            var forward = GetTangentAtPolygon(t);
            var up = GetUpAtPolygon(t);
            
            var m = new Matrix4x4();
            m.SetColumn(0, Right);
            m.SetColumn(1, up);
            m.SetColumn(2, forward);

            return QuaternionFromMatrix(m);
        }

        private Vector2 GetUpAtPolygon(float t)
        {
            return Vector3.Cross(GetTangentAtPolygon(t), Right);
        }

        private Vector2 GetTangentAtPolygon(float t)
        {
            var totalLength = PolygonLength();
            var goalLength = t * totalLength;
            var currentLength = 0f;
            var currentIndex = 0;

            while (currentLength <= goalLength && currentIndex < ControlPoints.Count - 1)
            {
                Vector3 currentPoint = ControlPoints[currentIndex];
                Vector3 nextPoint = ControlPoints[currentIndex + 1 >= ControlPoints.Count ? 0 : currentIndex + 1];
                var nextDiffLength = Vector3.Distance(currentPoint, nextPoint);
                if (currentLength + nextDiffLength < goalLength)
                {
                    currentLength += nextDiffLength;
                    currentIndex++;
                }
                else
                {

                    var diff = goalLength - currentLength;
                    var newT = diff / nextDiffLength;
                    return (nextPoint - currentPoint).normalized;
                }
            }

            var pos1 = (Vector3) ControlPoints[ControlPoints.Count - 1];
            var pos2 = (Vector3) ControlPoints[ControlPoints.Count - 2];

            return (pos1 - pos2).normalized;
        }

        private Vector2 GetPosAtPolygon(float t)
        {
            var totalLength = PolygonLength();
            var goalLength = t * totalLength;
            var currentLength = 0f;
            var currentIndex = 0;

            while (currentLength <= goalLength && currentIndex < ControlPoints.Count - 1)
            {
                Vector3 currentPoint = ControlPoints[currentIndex];
                Vector3 nextPoint = ControlPoints[currentIndex + 1 >= ControlPoints.Count ? 0 : currentIndex + 1];
                var nextDiffLength = Vector3.Distance(currentPoint, nextPoint);
                if (currentLength + nextDiffLength < goalLength)
                {
                    currentLength += nextDiffLength;
                    currentIndex++;
                }
                else
                {

                    var diff = goalLength - currentLength;
                    var newT = diff / nextDiffLength;
                    return Vector3.Lerp(currentPoint, nextPoint, newT);
                }
            }

            return ControlPoints[ControlPoints.Count - 1];
        }

        private float PolygonLength()
        {
            var length = 0f;
            for (var i = 0; i < ControlPoints.Count - 1; i++)
            {
                length += Vector3.Distance(ControlPoints[i], ControlPoints[i + 1]);
            }

            return length;
        }

        public Vector2 GetPosAt(float t)
        {
            if(ControlPoints.Count <= 1){return Vector2.zero;}
            return DeCasteljau.GetBezierPointAt(ControlPointsPositions(), t);
        }

        public Vector2 GetTangentAt(float t)
        {
            if(ControlPoints.Count <= 2){return Vector2.zero;}
            return DeCasteljau.GetBezierTangentAt(ControlPointsPositions(), t);
        }

        public Quaternion GetOrientationAt(float t)
        {
            var forward = GetTangentAt(t);
            var up = GetUpAt(t);
            
            var m = new Matrix4x4();
            m.SetColumn(0, Right);
            m.SetColumn(1, up);
            m.SetColumn(2, forward);

            return QuaternionFromMatrix(m);
            //return Quaternion.LookRotation(Vector3.forward);
        }

        public Vector3 GetUpAt(float t)
        {
            return Vector3.Cross(GetTangentAt(t), Right);
        }

        public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2;
            q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2;
            q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2;
            q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2;
            q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
            q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
            q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
            return q;
        }

        public List<Vector2> ControlPointsPositions()
        {
            return ControlPoints.Select(point => point.position).ToList();
        }

        public List<ControlPoint> PolygonPoints()
        {
            var points =new List<ControlPoint>(controlPoints);
            bool close = Mode == Manager.Mode.SweepPath
                ? Manager.Instance.closeSweepCurve
                : Manager.Instance.closeProfile2DCurve;
            if (close && controlPoints.Count >= 1)
            {
                points.Add(controlPoints[0]);
            }

            return points;
        }

        private void Awake()
        {
            controlPoints = new List<ControlPoint>();
        }

        public override void DrawShapes(Camera cam)
        {
            if(!CanDraw && Manager.Instance.Is2DEditing) {return;}
            using (Draw.Command(cam))
            {
                if (Profile2D == Manager.Profile2D.Bezier)
                {
                    if (Manager.Instance.Is2DEditing)
                    {
                        DrawBezierCurve();
                    }
                    else
                    {
                        if(Manager.Instance.currentMode != Manager.Mode.Revolution || Mode != Manager.Mode.SweepPath)
                        {
                            DrawBezierCurveOrientated();
                        }
                    }
                }
                
                if (Profile2D == Manager.Profile2D.Polygon || Manager.Instance.Is2DEditing)
                {
                    if (Manager.Instance.Is2DEditing)
                    {
                        DrawControlPolygon();
                    }
                    else
                    {
                        if(Manager.Instance.currentMode != Manager.Mode.Revolution || Mode != Manager.Mode.SweepPath)
                        {
                            DrawControlPolygon();
                        }
                    }
                    
                }

                if (Manager.Instance.ShowConvexHull)
                {
                    var hull = GrahamScan.ComputeGrahamScan(ControlPoints.Select(point => (Vector3)point.position).ToList());
                    for (int i = 0; i < hull.Count; i++)
                    {
                        DrawConvexHull(hull, i);
                    }
                }
            }
        }

        private void DrawBezierCurveOrientated()
        {
            var positions = Manager.Instance.CurrentExtrusion.GetPositions(Manager.Instance.tTest);
            for (var i = 0; i < positions.Count; i++)
            {
                Draw.Disc(positions[i], BezierCurveManager.Instance.CurvePointRadius,
                    CurveColor);

                if (BezierCurveManager.Instance.CompleteCasteljauLines && i + 1 < positions.Count)
                {
                    Draw.Line(positions[i], positions[i + 1], BezierCurveManager.Instance.CurvePointRadius * 2,
                        CurveColor);
                }
            }
        }

        private void DrawBezierCurve()
        {
            if (ControlPoints.Count >= 2)
            {
                float precision = 1f / BezierCurveManager.Instance.Steps;
                var coordinates =
                    DeCasteljau.GetBezierCurve(ControlPointsPositions(), precision);

                for (int i = 0; i < coordinates.Count; i++)
                {
                    Draw.Disc(coordinates[i], BezierCurveManager.Instance.CurvePointRadius,
                        CurveColor);

                    if (BezierCurveManager.Instance.CompleteCasteljauLines && i + 1 < coordinates.Count)
                    {
                        Draw.Line(coordinates[i], coordinates[i + 1], BezierCurveManager.Instance.CurvePointRadius * 2,
                            CurveColor);
                    }
                }
            }
        }

        private void DrawControlPolygon()
        {
            for (var i = 0; i < ControlPoints.Count - 1; i++)
            {
                DrawControlPolygons(i);
                if (Manager.Instance.Is2DEditing)
                {
                    DrawControlPointsHandles(i);
                    if (i == ControlPoints.Count - 2)
                    {
                        DrawControlPointsHandles(i + 1);
                    }
                }
            }
            
            if (Manager.Instance.Is2DEditing)
            {
                if (ControlPoints.Count >= 1)
                {
                    DrawControlPointsHandles(ControlPoints.Count - 1);
                }
            }
        }

        private void DrawConvexHull(List<Vector3> hull, int i)
        {
            Draw.Line(hull[i], hull[(i + 1) >= hull.Count ? 0 : i + 1], Color.black);
        }

        private Color CurveColor => IsMainCurve ? BezierCurveManager.Instance.MainCurveColor : BezierCurveManager.Instance.CurveColor;

        public List<ControlPoint> ControlPoints => PolygonPoints();
        public int ControlPointsCount => ControlPoints.Count;
        public bool CanDraw => Mode == Manager.Instance.currentMode;
        public Manager.Mode Mode { get; set; }

        public Manager.Profile2D Profile2D => Mode == Manager.Mode.SweepPath
            ? Manager.Instance.sweepProfile2D
            : Manager.Instance.profile2DProfile2D;

        private void DrawControlPointsHandles(int i)
        {
            Draw.Ring(ControlPoints[i].position, BezierCurveManager.Instance.ControlPointRadius, BezierCurveManager.Instance.HandleColor);
        }

        private void DrawControlPolygons(int i)
        {
            Draw.LineDashed(ControlPoints[i].position, ControlPoints[i + 1].position);
        }
        
        public ControlPoint AppendPoint(Vector3 clickInWorldPos)
        {
            var cp = new ControlPoint(clickInWorldPos);
            controlPoints.Add(cp);

            return cp;
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
