using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace Esgi.Bezier
{
    public class ExtrusionRevolution : ExtrusionBase
    {
        public override string ExtrusionMeshName => "Extrusion par révolution";
        public override Manager.Mode ExtrusionMode => Manager.Mode.Revolution;
    }
}
