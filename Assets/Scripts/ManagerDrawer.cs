using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shapes;

namespace Esgi.Bezier
{
    public class ManagerDrawer : ImmediateModeShapeDrawer
    {
        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.Line(Vector3.down * 20, Vector3.up * 20, BezierCurveManager.Instance.CurvePointRadius * 2,
                    Color.green);
                Draw.Line(Vector3.left * 20, Vector3.right * 20, BezierCurveManager.Instance.CurvePointRadius * 2,
                    Color.red);
                Draw.Line(Vector3.back * 20, Vector3.forward * 20, BezierCurveManager.Instance.CurvePointRadius * 2,
                    Color.blue);
                
                Draw.Line(-Manager.Instance.revolutionAxis * 20, Manager.Instance.revolutionAxis * 20, BezierCurveManager.Instance.CurvePointRadius * 2,
                    Color.white);
            }
        }
    }
}
