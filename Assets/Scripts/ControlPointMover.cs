using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    public class ControlPointMover
    {
        public ControlPoint controlPointBeingMoved;
        public bool IsHoldingControlPoint { get; set; }

        public void MoveControlPoint(Vector3 clickInWorldPos)
        {
            controlPointBeingMoved.position = clickInWorldPos;
        }

        public void HoldPoint(ControlPoint closestPoint)
        {
            controlPointBeingMoved = closestPoint;
            IsHoldingControlPoint = true;
        }

        public void Release()
        {
            controlPointBeingMoved = null;
            IsHoldingControlPoint = false;
        }
    }
}
