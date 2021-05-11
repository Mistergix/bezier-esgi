using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.PGDebugging;
using PGSauce.Core.Utilities;
using Sirenix.OdinInspector;

namespace Esgi.Bezier
{
    public class BezierCurveManager : MonoSingleton<BezierCurveManager>
    {
        [SerializeField, Range(2, 250)] private int steps = 2;
        [SerializeField, Min(0)] private float controlPointRadius = .5f;
        [SerializeField, Min(0)] private float curvePointRadius = .1f;
        [SerializeField] private bool loopControlPolygon = true, completeCasteljauLines = true;
        [SerializeField] private BezierCurve bezierCurvePrefab;
        [SerializeField] private Color handleColor = Color.red, curveColor = Color.blue, focusedHandleColor = Color.green, mainCurveColor = Color.magenta;

        private List<BezierCurve> curves;
        private BezierCurve _currentCurve => curves[_currentCurveIndex];

        [ShowInInspector, ReadOnly]
        private int _currentCurveIndex;
        
        public int Steps => steps;

        public float ControlPointRadius => controlPointRadius;

        public float CurvePointRadius => curvePointRadius;

        public bool LoopControlPolygon => loopControlPolygon;

        public bool CompleteCasteljauLines => completeCasteljauLines;

        public Color MainCurveColor => mainCurveColor;

        public Color FocusedHandleColor => focusedHandleColor;

        public Color CurveColor
        {
            get => curveColor;
            set => curveColor = value;
        }

        public Color HandleColor => handleColor;

        public override void Init()
        {
            base.Init();
            curves = new List<BezierCurve>();
            NewCurve();
        }

        [Button, DisableInEditorMode]
        public void NewCurve()
        {
            if (curves.Count > 0 && _currentCurve.PointCount == 0)
            {
                PGDebug.Message($"Pas de curve générée").LogWarning();
                return;
            }
            BezierCurve currentCurve = Instantiate(bezierCurvePrefab, transform);
            curves.Add(currentCurve);
            _currentCurveIndex = curves.Count - 1;
        }

        [Button, DisableInEditorMode]
        public void PrevCurve()
        {
            _currentCurveIndex--;
            if (_currentCurveIndex < 0)
            {
                _currentCurveIndex += curves.Count;
            }
        }

        [Button, DisableInEditorMode]
        public void NextCurve()
        {
            _currentCurveIndex++;
            _currentCurveIndex %= curves.Count;
        }

        [Button, DisableInEditorMode]
        public void DeleteCurrentCurve()
        {
            Destroy(_currentCurve.gameObject);
            curves.Remove(_currentCurve);

            if (curves.Count == 0)
            {
                NewCurve();
            }
            else
            {
                NextCurve();
            }
        }

        private void Update()
        {
            Vector3 clickInWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        
            if (Input.GetMouseButtonDown(0))
            {
                _currentCurve.AppendPoint(clickInWorldPos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _currentCurve.TryDestroyPoint(clickInWorldPos);
            }

            foreach (var curve in curves)
            {
                curve.IsMainCurve = curve == _currentCurve;
            }
        }
    }
}
