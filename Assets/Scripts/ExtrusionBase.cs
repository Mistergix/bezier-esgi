using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Esgi.Bezier
{
    public abstract class ExtrusionBase : MonoBehaviour
    {
        [Range(2, 200)]
        [SerializeField] public int edgeRingCount = 20;
        [SerializeField] public Mesh2D shape;
        public bool doNormals;
        public Mesh mesh;

        [SerializeField] public bool debugMesh2D;

        [SerializeField, Range(0, 1f), ShowIf("@debugMesh2D")]
        public float tTest;

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

        public float finalScale => Manager.Instance.finalScale;

        public float starScale => Manager.Instance.starScale;

        protected void Awake()
        {
            mesh = new Mesh {name = ExtrusionMeshName};
            GetComponent<MeshFilter>().sharedMesh = mesh;
            Init();
        }

        public abstract string ExtrusionMeshName { get; }

        protected void OnDisable()
        {
            mesh.Clear();
        }
        
        private void Update()
        {
            mesh.Clear();
            if(Manager.Instance.currentMode != ExtrusionMode){return;}
            GenerateMesh();
        }

        public abstract Manager.Mode ExtrusionMode { get; }

        protected virtual void GenerateMesh()
        {
            
        }

        protected virtual void Init()
        {
        }
    }
}