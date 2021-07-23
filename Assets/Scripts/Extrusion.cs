using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PGSauce.Core.Utilities;
using Sirenix.OdinInspector;

namespace Esgi.Bezier
{
    public class Extrusion : MonoSingleton<Extrusion>
    {
        [Range(2, 200)]
        [SerializeField] private int edgeRingCount = 20;
        [SerializeField] private Mesh2D shape;
        public bool doNormals;
        private Mesh mesh;


        public override void Init()
        {
            base.Init();
            mesh = new Mesh {name = "Extrusion Généralisée de balayage"};
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        private void Update()
        {
            GenerateMesh();
        }

        private void OnDisable()
        {
            mesh.Clear();
        }

        void GenerateMesh()
        {
            mesh.Clear();
            
            if(Manager.Instance.currentMode != Manager.Mode.General){return;}

            var bezier = BezierCurveManager.Instance.CurrentCurve;
            
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            for (var ring = 0; ring < edgeRingCount; ring++)
            {
                var point = bezier.GetOrientedPointAt((float) ring / (edgeRingCount - 1));
                for (var i = 0; i < shape.VertexCount; i++)
                {
                    verts.Add(point.LocalToWorldPosition(shape.Vertices[i].point));
                    normals.Add(point.LocalToWorldDirection(shape.Vertices[i].normal));
                }
            }

            var tris = new List<int>();
            for (var ring = 0; ring < edgeRingCount - 1; ring++)
            {
                var rootIndex = ring * shape.VertexCount;
                var rootIndexNext = rootIndex + shape.VertexCount;

                for (var line = 0; line < shape.LineCount - 1; line+=2)
                {
                    var lineIndexA = shape.LineIndices[line];
                    var lineIndexB = shape.LineIndices[line + 1];

                    var currentA = rootIndex + lineIndexA;
                    var currentB = rootIndex + lineIndexB;
                    var nextA = rootIndexNext + lineIndexA;
                    var nextB = rootIndexNext + lineIndexB;
                    
                    tris.Add(currentA);
                    tris.Add(nextA);
                    tris.Add(nextB);
                    
                    tris.Add(currentA);
                    tris.Add(nextB);
                    tris.Add(currentB);
                }
            }
            
            mesh.SetVertices(verts);
            if (doNormals)
            {
                mesh.SetNormals(normals);
            }
            mesh.SetTriangles(tris, 0);
            
        }

    }
}
