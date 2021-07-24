using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.Utilities;

namespace Esgi.Bezier
{
    public class Manager : MonoSingleton<Manager>
    {
        [SerializeField] private bool showConvexHull;
        public bool closePolygon;
        [SerializeField] public float starScale = 1; 
        [SerializeField] public float finalScale = 0.2f;
        
        public bool ShowConvexHull => showConvexHull;
        
        public enum Profile2D
        {
            Polygon,
            Bezier
        }
        
        public enum Mode
        {
            Profile2D,
            Simple,
            Revolution,
            General
        }

        public Profile2D profile2D;
        public Mode currentMode;
    }
}
