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
        private Vector2 transformPoint;
        
        public bool ShowPoint { get; set; }

        public Vector2 TransformPoint => transformPoint;

        public float TransformPointLerpRatio => transformPointLerpRatio;

        public override void DrawShapes(Camera cam)
        {
            if(!ShowPoint) { return; }
            
            using (Draw.Command(cam))
            {
                Draw.Ring(transformPoint, BezierCurveManager.Instance.ControlPointRadius, pointColor);
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
                point.position += (Vector2)delta * Time.deltaTime * translateSpeed;
            }
        }
    }
}
