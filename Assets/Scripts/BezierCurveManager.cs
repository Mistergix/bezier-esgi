using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Color handleColor = Color.red, curveColor = Color.blue, mainCurveColor = Color.magenta;

        private List<BezierCurve> curves;
        private BezierCurve _currentCurve => curves[_currentCurveIndex];

        [ShowInInspector, ReadOnly]
        private int _currentCurveIndex;

        private ControlPointMover controlPointMover;
        private Dictionary<ControlPoint, BezierCurve> pointToCurve;
        private List<ControlPoint> _allControlPoints;

        public int Steps => steps;

        public float ControlPointRadius => controlPointRadius;

        public float CurvePointRadius => curvePointRadius;

        public bool LoopControlPolygon => loopControlPolygon;

        public bool CompleteCasteljauLines => completeCasteljauLines;

        public Color MainCurveColor => mainCurveColor;

        public Color CurveColor
        {
            get => curveColor;
            set => curveColor = value;
        }

        public Color HandleColor => handleColor;

        public override void Init()
        {
            base.Init();
            pointToCurve = new Dictionary<ControlPoint, BezierCurve>();
            curves = new List<BezierCurve>();
            NewCurve();
            controlPointMover = new ControlPointMover();
        }

        [Button, DisableInEditorMode]
        public void NewCurve()
        {
            if (CurrentCurveEmpty)
            {
                PGDebug.Message($"Pas de curve générée").LogWarning();
                return;
            }
            BezierCurve currentCurve = Instantiate(bezierCurvePrefab, transform);
            curves.Add(currentCurve);
            _currentCurveIndex = curves.Count - 1;
        }

        private bool CurrentCurveEmpty => curves.Count > 0 && _currentCurve.PointCount == 0;

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
            if (CurrentCurveEmpty)
            {
                PGDebug.Message($"La courbe n'a pas été détruite").LogWarning();
                return;
            }

            var points = pointToCurve.Keys.Where(point => pointToCurve[point] == _currentCurve);

            var controlPoints = points.ToList();
            foreach (var point in controlPoints)
            {
                pointToCurve.Remove(point);
            }

            PGDebug.SetCondition(true).Message($"{controlPoints.Count()} points removed").Log();
            
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
            _allControlPoints = AllControlPoints();
            var closestPoints = BezierCurve.ControlPointsInRadius(_allControlPoints, clickInWorldPos);

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetMouseButton(0))
                {
                    if (controlPointMover.IsHoldingControlPoint)
                    {
                        controlPointMover.MoveControlPoint(clickInWorldPos);
                    }
                    else
                    {
                        if (closestPoints.Count > 0)
                        {
                            controlPointMover.HoldPoint(closestPoints[0]);
                        }
                    }
                }
                else
                {
                    if (controlPointMover.IsHoldingControlPoint)
                    {
                        controlPointMover.Release();
                    }
                }
            }
            else
            {
                if (controlPointMover.IsHoldingControlPoint)
                {
                    controlPointMover.Release();
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    var cp = _currentCurve.AppendPoint(clickInWorldPos);
                    pointToCurve.Add(cp, _currentCurve);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (closestPoints.Count > 0)
                    {
                        if (pointToCurve.ContainsKey(closestPoints[0]))
                        {
                            var curve = pointToCurve[closestPoints[0]];
                            curve.DestroyPoint(closestPoints[0]);
                            pointToCurve.Remove(closestPoints[0]);
                            closestPoints.RemoveAt(0);
                        }
                    }
                    
                    //_currentCurve.TryDestroyPoint(clickInWorldPos);
                }
            }

            foreach (var curve in curves)
            {
                curve.IsMainCurve = curve == _currentCurve;
            }
        }

        private List<ControlPoint> AllControlPoints()
        {
            List<ControlPoint> allControlPoints = new List<ControlPoint>();
            foreach (var curve in curves)
            {
                allControlPoints.AddRange(curve.ControlPoints);
            }

            return allControlPoints;
        }
    }
}
