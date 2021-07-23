using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        [SerializeField] private bool completeCasteljauLines = true;
        [SerializeField] private BezierCurve bezierCurvePrefab;
        [SerializeField] private Color handleColor = Color.red, curveColor = Color.blue, mainCurveColor = Color.magenta;
        [SerializeField] private CurveTransform curveTransform;
        [SerializeField] private bool manipulateCurrentCurve = true;
        [SerializeField] private bool showConvexHull;

        private List<BezierCurve> curves;
        public BezierCurve CurrentCurve => curves[_currentCurveIndex];

        [ShowInInspector, ReadOnly]
        private int _currentCurveIndex;

        private ControlPointMover controlPointMover;
        private Dictionary<ControlPoint, BezierCurve> pointToCurve;
        private List<ControlPoint> _allControlPoints;

        public int Steps => steps;

        public float ControlPointRadius => controlPointRadius;

        public float CurvePointRadius => curvePointRadius;

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

        private bool CurrentCurveEmpty => curves.Count > 0 && CurrentCurve.PointCount == 0;

        public bool ShowConvexHull => showConvexHull;

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

            var points = pointToCurve.Keys.Where(point => pointToCurve[point] == CurrentCurve);

            var controlPoints = points.ToList();
            foreach (var point in controlPoints)
            {
                pointToCurve.Remove(point);
            }

            PGDebug.SetCondition(true).Message($"{controlPoints.Count()} points removed").Log();
            
            Destroy(CurrentCurve.gameObject);
            curves.Remove(CurrentCurve);

            if (curves.Count == 0)
            {
                NewCurve();
            }
            else
            {
                NextCurve();
            }
        }

        [NotNull] private List<Vector2> originalPos;

        private delegate void TransformAction(Vector3 clickWorldPos, List<ControlPoint> points,
            List<Vector2> originalPositions);
        
        private void Update()
        {
            if(Manager.Instance.currentMode != Manager.Mode.Profile2D){return;}
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
            else if(Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.C))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    curveTransform.SetPoint(clickInWorldPos);
                    SetOriginalPositions();
                }
                else if(Input.GetMouseButton(0))
                {
                    TransformAction action;

                    if (Input.GetKey(KeyCode.S))
                    {
                        action = curveTransform.Scale;
                    }
                    else if(Input.GetKey(KeyCode.C))
                    {
                        action = curveTransform.Shear;
                    }
                    else if(Input.GetKey(KeyCode.R))
                    {
                        action = curveTransform.Rotate;
                    }
                    else if(Input.GetKey(KeyCode.T))
                    {
                        action = curveTransform.Translate;
                    }
                    else
                    {
                        throw new UnityException("IMPOSSIBLE");
                    }

                    action(clickInWorldPos, manipulateCurrentCurve ? CurrentCurve.ControlPoints : _allControlPoints,
                        originalPos);

                    curveTransform.ShowPoint = true;
                }
            }
            else
            {
                curveTransform.ShowPoint = false;
                if (controlPointMover.IsHoldingControlPoint)
                {
                    controlPointMover.Release();
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    var cp = CurrentCurve.AppendPoint(clickInWorldPos);
                    pointToCurve.Add(cp, CurrentCurve);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    TryDestroyPoint(closestPoints);
                }
            }

            foreach (var curve in curves)
            {
                curve.IsMainCurve = curve == CurrentCurve;
            }
        }

        private void SetOriginalPositions()
        {
            if (manipulateCurrentCurve)
            {
                originalPos = CurrentCurve.ControlPoints.Select(point => point.position).ToList();
            }
            else
            {
                originalPos = _allControlPoints.Select(point => point.position).ToList();
            }
        }

        private void TryDestroyPoint(List<ControlPoint> closestPoints)
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
        }

        private List<ControlPoint> AllControlPoints()
        {
            var allControlPoints = new List<ControlPoint>();
            foreach (var curve in curves)
            {
                allControlPoints.AddRange(curve.ControlPoints);
            }

            return allControlPoints;
        }
    }
}
