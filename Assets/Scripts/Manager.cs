using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.Utilities;

namespace Esgi.Bezier
{
    public class Manager : MonoSingleton<Manager>
    {
        [Range(2, 50)]
        [SerializeField] public int edgeRingCount = 20;
        [SerializeField] private bool showConvexHull;
        public bool closeSweepCurve;
        public bool closeProfile2DCurve;
        [SerializeField] public float starScale = 1; 
        [SerializeField] public float finalScale = 0.2f;
        [SerializeField] public Vector3 revolutionAxis = Vector3.up;
        [Range(0, 1f)] public float tTest;
        
        public bool ShowConvexHull => showConvexHull;
        public bool Is2DEditing => Instance.currentMode == Mode.SweepPath || Instance.currentMode == Mode.Profile2D;

        public enum Profile2D
        {
            Polygon,
            Bezier
        }
        
        public enum Mode
        {
            SweepPath,
            Profile2D,
            Revolution,
            General
        }

        public Profile2D sweepProfile2D;
        public Profile2D profile2DProfile2D;
        public Mode currentMode;
        
    }
}
