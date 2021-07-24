using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Esgi.Bezier
{
    [CreateAssetMenu(menuName = "Bezier/ Mesh 2D/With Formulas (NEW)")]
    public class ComputedMesh2D : Mesh2D
    {
        public List<Circle> circles;
        public List<HandShape> handShapes;

        private List<Shape> shapes
        {
            get
            {
                var res = new List<Shape>();
                res.AddRange(circles);
                res.AddRange(handShapes);

                return res;
            }
        }

        public override Vertex[] Vertices
        {
            get
            {
                var res = new List<Vertex>();

                foreach (var shape in shapes)    
                {
                    res.AddRange(shape.GetVertices());
                }

                return res.ToArray();
            }
        }

        public override int[] LineIndices
        {
            get
            {
                var res = new List<int>();
                var count = 0;

                foreach (var shape in shapes)
                {
                    var linesList = shape.GetLineIndices().Select(index => index + count).ToList();
                    res.AddRange(linesList);
                    count += linesList.Count - 1;
                }

                return res.ToArray();
            }
        }
    }

    [Serializable]
    public abstract class Shape
    {
        public abstract Mesh2D.Vertex[] GetVertices();
        public abstract int[] GetLineIndices();
    }

    [Serializable]
    public class Circle : Shape
    {
        public Vector2 center;
        public float radius;
        public int pointCount = 4;
        
        public override Mesh2D.Vertex[] GetVertices()
        {
            var res = new List<Mesh2D.Vertex>();
            var angleIncrement = 360f / pointCount;
            for (var angle = 0f; angle < 360f; angle += angleIncrement)
            {
                var angleRad = angle * Mathf.Deg2Rad;
                var dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                var point = center + dir * radius;
                res.Add(new Mesh2D.Vertex()
                {
                    normal = dir,
                    point = point,
                    multiplicity = 1
                });
            }

            res.Reverse();

            return res.ToArray();
        }

        public override int[] GetLineIndices()
        {
            var l = Enumerable.Range(0, GetVertices().Length).ToList();
            l.Add(0);
            return l.ToArray();
            
        }
    }

    [Serializable]
    public class HandShape : Shape
    {
        public int multiplicity;
        [SerializeField]
        private Mesh2D.Vertex[] _vertices;

        [SerializeField]
        private int[] _lineIndices;

        public override Mesh2D.Vertex[] GetVertices()
        {
            return _vertices;
        }

        public override int[] GetLineIndices()
        {
            return _lineIndices;
        }
    }
}
