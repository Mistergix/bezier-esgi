using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PGSauce.Core.Utilities;
using Sirenix.OdinInspector;

namespace Esgi.Bezier
{
    public class Manager : MonoSingleton<Manager>
    {
        [Range(2, 50)]
        [SerializeField] public int edgeRingCount = 20;
        [SerializeField] private bool showConvexHull;
        public bool closeSweepCurve;
        public bool closeProfile2DCurve;
        [SerializeField] public float starScale = 1; 
        [SerializeField] public float finalScale = 0.2f;
        [SerializeField] public Vector3 revolutionAxis = Vector3.up;
        [Range(0, 1f)] public float tTest;

        [SerializeField] private Extrusion extrusion;
        [SerializeField] private ExtrusionRevolution extrusionRevolution;

        private float currentOrtho;

        public ExtrusionBase CurrentExtrusion => currentMode == Mode.Revolution ? (ExtrusionBase)extrusionRevolution : (ExtrusionBase)extrusion;

        public override void Init()
        {
            base.Init();
            currentOrtho = Mathf.Lerp(orthoMin, orthoMax, 0.5f);
        }

        private void Update()
        {
            cam2D.Priority = Is2DEditing ? 50 : 0;
            cam3D.Priority = Is2DEditing ? 0 : 50;

            
            
            var diff = Input.mouseScrollDelta.y * scrollSpeed;
            currentOrtho -= diff;

            currentOrtho = Mathf.Clamp(currentOrtho, orthoMin, orthoMax);
            ((CinemachineFreeLook) cam3D).m_CommonLens = true;
            ((CinemachineFreeLook) cam3D).m_Lens.OrthographicSize = currentOrtho;

        }

        public bool ShowConvexHull => showConvexHull;
        public bool Is2DEditing => Instance.currentMode == Mode.SweepPath || Instance.currentMode == Mode.Profile2D;

        public enum Profile2D
        {
            Polygon,
            Bezier
        }
        
        public enum Mode
        {
            SweepPath,
            Profile2D,
            Revolution,
            General
        }

        public Profile2D sweepProfile2D;
        public Profile2D profile2DProfile2D;
        public Mode currentMode;

        [Title("NOT IN UI")]
        [SerializeField] public CinemachineVirtualCameraBase cam2D;
        [SerializeField] public CinemachineVirtualCameraBase cam3D;
        public float orthoMin = 2;
        public float orthoMax = 20;
        public float scrollSpeed = 5;
    }
}
