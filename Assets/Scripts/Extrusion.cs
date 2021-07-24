using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PGSauce.Core.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Esgi.Bezier
{
    public class Extrusion : MonoSingleton<Extrusion>
    {
        [Range(2, 200)]
        [SerializeField] private int edgeRingCount = 20;

        [SerializeField] private float starScale = 1;
        [SerializeField] private float finalScale = 0.2f;
        [SerializeField] private Mesh2D shape;
        public bool doNormals;
        private Mesh mesh;

        [SerializeField] private bool debugMesh2D;

        [SerializeField, Range(0, 1f), ShowIf("@debugMesh2D")]
        private float tTest;

        private void OnDrawGizmos()
        {
            if(!debugMesh2D) {return;}
            OrientatedPoint op;

            if (BezierCurveManager.Instance)
            {
                if (BezierCurveManager.Instance.CurrentCurve.ControlPointsCount <= 1)
                {
                    return;
                }
                op = BezierCurveManager.Instance.CurrentCurve.GetOrientedPointAt(tTest);
            }
            else
            {
                op = new OrientatedPoint()
                {
                    position = Vector2.zero,
                    rotation = Quaternion.identity
                };
            }
            
            var scale =  Mathf.Lerp(starScale, finalScale, tTest);
            
            for (var i = 0; i < shape.VertexCount; i++)
            {
                var pos = op.LocalToWorldPosition(shape.Vertices[i].point * scale);
                Gizmos.DrawSphere(pos, 0.15f);
                Gizmos.DrawRay(pos, op.LocalToWorldDirection(shape.Vertices[i].normal));
            }

            for (var i = 0; i < shape.LineCount; i++)
            {
                var index = shape.LineIndices[i];
                var vert = shape.Vertices[index];
                
                var pos = op.LocalToWorldPosition(vert.point * scale) + Vector3.up * .2f;
                Handles.Label(pos, $"{index}");
            }
        }


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
                var t = (float) ring / (edgeRingCount - 1);
                var scale = Mathf.Lerp(starScale, finalScale, t);
                var point = bezier.GetOrientedPointAt(t);
                for (var i = 0; i < shape.VertexCount; i++)
                {
                    verts.Add(point.LocalToWorldPosition(shape.Vertices[i].point * scale));
                    normals.Add(point.LocalToWorldDirection(shape.Vertices[i].normal));
                }
            }

            var tris = new List<int>();
            for (var ring = 0; ring < edgeRingCount - 1; ring++)
            {
                var rootIndex = ring * shape.VertexCount;
                var rootIndexNext = rootIndex + shape.VertexCount;

                var line = 0;
                while (line < shape.LineCount - 1)
                {
                    var lineIndexA = shape.LineIndices[line];
                    var lineIndexB = shape.LineIndices[line + 1];
                    
                    if(lineIndexA == -1 || lineIndexB == -1){continue;}

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


                    line += shape.Vertices[shape.LineIndices[line]].multiplicity;
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
