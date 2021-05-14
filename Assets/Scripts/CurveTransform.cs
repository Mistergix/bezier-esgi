using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shapes;

namespace Esgi.Bezier
{
    public class CurveTransform : ImmediateModeShapeDrawer
    {
        
        [SerializeField] private Color pointColor = Color.yellow;
        [SerializeField] private float transformPointLerpRatio = .2f;
        [SerializeField] private float translateSpeed = 1f;
        [SerializeField] private float scaleSpeed = 1;

        private delegate void OnDraw();

        private OnDraw onDraw;
        
        private Vector2 transformPoint;
        
        public bool ShowPoint { get; set; }

        public Vector2 TransformPoint => transformPoint;

        public float TransformPointLerpRatio => transformPointLerpRatio;

        private void Awake()
        {
            onDraw = () => { };
        }

        public override void DrawShapes(Camera cam)
        {
            if(!ShowPoint) { return; }
            
            using (Draw.Command(cam))
            {
                Draw.Ring(transformPoint, BezierCurveManager.Instance.ControlPointRadius, pointColor);
                
                onDraw?.Invoke();
            }
        }

        public void SetPoint(Vector2 point)
        {
            transformPoint = point;
        }

        public void Translate(Vector3 clickInWorldPos, List<ControlPoint> points)
        {
            var delta = clickInWorldPos - (Vector3)transformPoint;
            foreach (var point in points)
            {
                point.position += (Vector2)delta * (Time.deltaTime * translateSpeed);
            }

            onDraw = () =>
            {
                Draw.Line(transformPoint, clickInWorldPos, .2f, pointColor);
            };
        }

        public void Scale(Vector3 clickInWorldPos, List<ControlPoint> points, List<Vector2> originalPositions)
        {
            clickInWorldPos.y = transformPoint.y;
            var delta = clickInWorldPos - (Vector3)transformPoint;
            
            var distance = Vector2.Distance(clickInWorldPos, transformPoint);
            distance *= scaleSpeed * Mathf.Sign(delta.x);
            
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                point.position = originalPositions[i] + (originalPositions[i] - transformPoint).normalized *
                    Vector2.Distance(originalPositions[i], transformPoint) * distance;
            }


            
            onDraw = () =>
            {
                Draw.Line(transformPoint, clickInWorldPos, .2f, pointColor);
            };
        }

        public void Rotate(Vector3 clickInWorldPos, List<ControlPoint> points, List<Vector2> originalPositions)
        {
            var delta = clickInWorldPos - (Vector3)transformPoint;
            
            var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                point.position = RotatePointAroundPivot(originalPositions[i], transformPoint, new Vector3(0, 0, angle));
            }

            if (angle < 0)
            {
                angle += 360;
            }

            onDraw = () =>
            {
                Draw.Arc(transformPoint, BezierCurveManager.Instance.ControlPointRadius * 2, .1f, 0, angle * Mathf.Deg2Rad, pointColor);
            };
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            var dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }
}
