using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes;

namespace Esgi.Bezier
{
    public class Manager : ImmediateModeShapeDrawer
    {
        [SerializeField, Range(2, 250)] private int steps = 2;
        [SerializeField] private float controlPointRadius = .5f;
        [SerializeField, Min(0)] private float curvePointRadius = .1f;
        [SerializeField] private bool loopControlPolygon = true;
        private List<ControlPoint> controlPoints;
        
        
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
                    float precision = 1f / steps;
                    var coordinates =
                        DeCasteljau.GetBezierCurve(controlPoints.Select(point => point.position).ToList(), precision);

                    for (int i = 0; i < coordinates.Count; i++)
                    {
                        Draw.Disc(coordinates[i], curvePointRadius, Color.blue); 
                    }
                }
                
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    DrawControlPolygons(i);
                    DrawControlPointsHandles(i);
                }
            }
        }

        private void DrawControlPointsHandles(int i)
        {
            Draw.Ring(controlPoints[i].position, controlPointRadius, Color.red);
        }

        private void DrawControlPolygons(int i)
        {
            if (i + 1 >= controlPoints.Count)
            {
                if (loopControlPolygon)
                {
                    Draw.LineDashed(controlPoints[i].position, controlPoints[0].position);
                }
            }
            else
            {
                Draw.LineDashed(controlPoints[i].position, controlPoints[i + 1].position);
            }
        }

        private void Update()
        {
            Vector3 clickInWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                AppendPoint(clickInWorldPos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                TryDestroyPoint(clickInWorldPos);
            }
        }

        private void AppendPoint(Vector3 clickInWorldPos)
        {
            ControlPoint cp = new ControlPoint(clickInWorldPos);
            controlPoints.Add(cp);
        }

        private void TryDestroyPoint(Vector3 clickInWorldPos)
        {
            List<ControlPoint> cps = controlPoints.Where(point =>
                Vector2.Distance(point.position, clickInWorldPos) <= controlPointRadius).ToList();

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

            if (cps.Count > 0)
            {
                controlPoints.Remove(cps[0]);
            }
        }
    }
}
