using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Esgi.Bezier
{
    [CreateAssetMenu(menuName = "Bezier/ Mesh 2D/From Bezier")]
    public class BezierCurveMesh2D : Mesh2D
    {
        [SerializeField, Range(2, 50)] private int samples = 10;

        private Vertex[] _vertices;
        private int[] _lineIndices;

        public override void UpdateData()
        {
            base.UpdateData();
            _vertices = ComputeVertices();
            _lineIndices = ComputeLineIndices();
        }

        public override Vertex[] Vertices => _vertices;

        private Vertex[] ComputeVertices()
        {
            var res = new List<Vertex>();
            if (BezierCurveManager.Instance == null)
            {
                return Array.Empty<Vertex>();
            }

            var bezier = BezierCurveManager.Instance.Profile2DCurve;
            if (bezier == null || bezier.ControlPoints.Count < 2)
            {
                return Array.Empty<Vertex>();
            }

            for (var i = 0; i < samples; i++)
            {
                var t = i / (samples - 1f);
                var op = bezier.GetOrientedPointAt(t);
                res.Add(new Vertex()
                {
                    multiplicity = 1,
                    point = op.position,
                    normal = op.up
                });
            }

            return res.ToArray();
        }

        public override int[] LineIndices => _lineIndices;

        private int[] ComputeLineIndices()
        {
            var l = Enumerable.Range(0, Vertices.Length).ToList();
            return l.ToArray();
        }
    }
}