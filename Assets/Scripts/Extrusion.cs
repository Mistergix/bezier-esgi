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
    public class Extrusion : ExtrusionBase
    {
        public override string ExtrusionMeshName => "Extrusion Généralisée de balayage";
        public override Manager.Mode ExtrusionMode => Manager.Mode.General;

        protected override void GenerateMesh()
        {
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
