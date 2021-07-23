using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    public class ExtrusionRevolution : MonoBehaviour
    {
        [Range(2, 200)]
        [SerializeField] private int edgeRingCount = 20;
        [SerializeField] private Mesh2D shape;
        public bool doNormals;
        private Mesh mesh;
    }
}
