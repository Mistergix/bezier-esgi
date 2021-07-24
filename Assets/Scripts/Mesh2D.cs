using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Esgi.Bezier
{
    [TypeInfoBox("Cross section of the extruded primitive")]
    public abstract class Mesh2D : ScriptableObject
    {
        [Serializable]
        public class Vertex
        {
            public Vector2 point;
            public Vector2 normal;
            public float uv;
            public int multiplicity;
        }
        
        public abstract Vertex[] Vertices { get; }

        public abstract int[] LineIndices { get; }
        public int VertexCount => Vertices.Length;
        public int LineCount => LineIndices.Length;

        public virtual void UpdateData()
        {
            
        }
    }
}
