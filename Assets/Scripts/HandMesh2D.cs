using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Esgi.Bezier
{
    [CreateAssetMenu(menuName = "Bezier/ Mesh 2D/Hand Crafted (OBSOLETE)")]
    public class HandMesh2D : Mesh2D
    {
        [SerializeField]
        private Vertex[] _vertices;

        [SerializeField]
        private int[] _lineIndices;

        public override Vertex[] Vertices => _vertices;
        public override int[] LineIndices => _lineIndices;
    }
}
