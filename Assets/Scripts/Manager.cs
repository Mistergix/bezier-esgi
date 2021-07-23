using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.Utilities;

namespace Esgi.Bezier
{
    public class Manager : MonoSingleton<Manager>
    {
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
