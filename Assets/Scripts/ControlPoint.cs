using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    public class ControlPoint
    {
        public Vector2 position;

        public ControlPoint(Vector2 position)
        {
            this.position = position;
        }
        
        public static implicit operator Vector2(ControlPoint cp) => cp.position;
        public static implicit operator Vector3(ControlPoint cp) => cp.position;
    }

    public struct OrientatedPoint
    {
        public Vector2 position;
        public Vector2 up;
        public Vector2 forward;
        public Quaternion rotation;
        public Vector2 right;

        public Vector3 LocalToWorldPosition(Vector3 localPoint)
        {
            return (Vector3)position + LocalToWorldDirection(localPoint);
        }
        
        public Vector3 LocalToWorldDirection(Vector3 localPoint)
        {
            return rotation * localPoint;
        }
    }
}
