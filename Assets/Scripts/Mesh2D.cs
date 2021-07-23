using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Esgi.Bezier
{
    [CreateAssetMenu(menuName = "Bezier/ Mesh 2D")]
    [TypeInfoBox("Cross section of the extruded primitive")]
    public class Mesh2D : ScriptableObject
    {
        [Serializable]
        public class Vertex
        {
            public Vector2 point;
            public Vector2 normal;
            public float uv;
        }

        [SerializeField]
        private Vertex[] _vertices;

        [SerializeField]
        private int[] _lineIndices;

        public Vertex[] Vertices => _vertices;

        public int[] LineIndices => _lineIndices;
        public int VertexCount => Vertices.Length;
        public int LineCount => LineIndices.Length;
    }
}
