using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Esgi.Bezier
{
    public abstract class ExtrusionBase : MonoBehaviour
    {
        public int edgeRingCount => Manager.Instance.edgeRingCount;
        [SerializeField] public Mesh2D shape;
        public bool doNormals;
        public Mesh mesh;

        [SerializeField] public bool debugMesh2D;

        public float tTest => Manager.Instance.tTest;

        private void OnDrawGizmos()
        {
            if(!debugMesh2D) {return;}
            if(ExtrusionMode != Manager.Instance.currentMode){return;}
            DrawGizmos();
        }

        protected virtual void DrawGizmos()
        {
            
        }

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
            GetComponent<MeshFilter>().sharedMesh = mesh;
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