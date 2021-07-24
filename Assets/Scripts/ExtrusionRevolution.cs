using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace Esgi.Bezier
{
    public class ExtrusionRevolution : ExtrusionBase
    {
        public Transform tmp, child;
        public override string ExtrusionMeshName => "Extrusion par révolution";
        public override Manager.Mode ExtrusionMode => Manager.Mode.Revolution;

        public Vector3 Axis => Manager.Instance.revolutionAxis;

        protected override void DrawGizmos()
        {
            base.DrawGizmos();
            DrawTranche(0);
            DrawTranche(tTest);
        }

        private void DrawTranche(float t)
        {
            var cos = Mathf.Cos(360 * t * Mathf.Deg2Rad);
            var sin = Mathf.Sin(360 * t * Mathf.Deg2Rad);

            tmp.position = Vector3.zero;
            child.SetParent(tmp);

            for (var i = 0; i < shape.VertexCount; i++)
            {
                Gizmos.color = Color.white;
                var point = shape.Vertices[i].point;
                //var dir = new Vector3(cos, 0, sin);
                var localPos = new Vector3(point.x * cos, point.y, point.x * sin);

                tmp.up = Axis;
                child.localPosition = localPos;

                var rot = Quaternion.AngleAxis(360 * (1 - t), tmp.up);
                var dir = rot * tmp.right;

                //dir = tmp.InverseTransformDirection(dir);

                //child.right = dir;
                //var normal = child.TransformDirection(shape.Vertices[i].normal);

                Gizmos.DrawSphere(child.position, 0.15f);
                Gizmos.DrawRay(child.position, dir);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(Vector3.zero, dir * 10);
            }
        }

        protected override void GenerateMesh()
        {
            base.GenerateMesh();
            
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            tmp.position = Vector3.zero;
            child.SetParent(tmp);
            for (var ring = 0; ring < edgeRingCount; ring++)
            {
                var t = (float) ring / (edgeRingCount - 1);
                var angle = 360f * t;
                var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
                var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
                
                for (var i = 0; i < shape.VertexCount; i++)
                {
                    var point = shape.Vertices[i].point;
                    var localPos = new Vector3(point.x * cos, point.y, point.x * sin);

                    tmp.up = Axis;
                    child.localPosition = localPos;
                    
                    var rot = Quaternion.AngleAxis(360 * (1 - t), tmp.up);
                    var dir = rot * tmp.right;

                    verts.Add(child.position);
                    normals.Add(dir.normalized);
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
