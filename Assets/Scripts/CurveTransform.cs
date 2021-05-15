using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Shapes;
using UnityEditor.UIElements;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Esgi.Bezier
{
    public class CurveTransform : ImmediateModeShapeDrawer
    {
        
        [SerializeField] private Color pointColor = Color.yellow;
        [SerializeField] private float translateSpeed = 1f;
        [SerializeField] private float scaleSpeed = 1;
        [SerializeField] private float shearSpeed = 1;

        private delegate void OnDraw();

        private OnDraw onDraw;
        
        private Vector2 transformPoint;
        
        public bool ShowPoint { get; set; }

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

        public void Translate(Vector3 clickInWorldPos, List<ControlPoint> points, List<Vector2> originalPositions)
        {
            var delta = clickInWorldPos - (Vector3)transformPoint;
            delta *= translateSpeed;
            
            Matrix3x3 translateMat = Matrix3x3.TranslateMatrix(delta.x, delta.y);

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pos = translateMat * originalPositions[i];
                point.position = pos;
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
            
            Matrix3x3 scaleMat = Matrix3x3.TranslateMatrix(transformPoint.x, transformPoint.y) * Matrix3x3.ScaleMatrix(1 + distance, 1 + distance) * Matrix3x3.TranslateMatrix(-transformPoint.x, -transformPoint.y);
            
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pos = scaleMat * originalPositions[i];
                //point.position = originalPositions[i] + (originalPositions[i] - transformPoint).normalized * (Vector2.Distance(originalPositions[i], transformPoint) * distance);
                point.position = pos;
            }

            onDraw = () =>
            {
                Draw.Line(transformPoint, clickInWorldPos, .2f, pointColor);
            };
        }

        public void Shear(Vector3 clickInWorldPos, List<ControlPoint> points, List<Vector2> originalPositions)
        {
            var delta = clickInWorldPos - (Vector3)transformPoint;
            delta *= shearSpeed;
            
            Matrix3x3 shearMat = new Matrix3x3(new Vector3(1, delta.y, 0), new Vector3(delta.x, 1, 0), Vector3.zero);

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pos = originalPositions[i];
                pos = shearMat * pos;
                point.position = pos;
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
            
            Matrix3x3 rotateMat = Matrix3x3.TranslateMatrix(transformPoint.x, transformPoint.y) * Matrix3x3.RotationZMatrix(angle) * Matrix3x3.TranslateMatrix(-transformPoint.x, -transformPoint.y);

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pos = rotateMat * originalPositions[i];
                //point.position = RotatePointAroundPivot(originalPositions[i], transformPoint, new Vector3(0, 0, angle));
                point.position = pos;
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
