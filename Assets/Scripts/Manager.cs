using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shapes;

namespace Esgi.Bezier
{
    public class Manager : ImmediateModeShapeDrawer
    {
        private List<ControlPoint> controlPoints;
        
        private void Awake()
        {
            controlPoints = new List<ControlPoint>();
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    Draw.LineDashed(controlPoints[i].position, controlPoints[(i + 1 >= controlPoints.Count) ? 0 : i + 1].position);
                }
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ControlPoint cp = new ControlPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                controlPoints.Add(cp);
            }
        }
    }
}
