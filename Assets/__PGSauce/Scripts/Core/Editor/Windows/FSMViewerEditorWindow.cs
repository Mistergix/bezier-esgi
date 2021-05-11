using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PGSauce.Core.PGFiniteStateMachine;
using System.IO;
using System;

namespace PGSauce.Core.PGEditor
{
    public class FSMViewerEditorWindow : EditorWindow
    {
        [MenuItem("PG/View FSM")]
        static void ShowEditor()
        {
            FSMViewerEditorWindow editor = EditorWindow.GetWindow<FSMViewerEditorWindow>();
        }

        IState s;

        HashSet<IState> visited;
        private HashSet<Connection> connections;

        private void OnGUI()
        {
            string path = "";
            s = null;
            foreach (IState state in Selection.GetFiltered(typeof(IState), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(state);
                if(! string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    s = state;
                    break;
                }
            }

            if(s == null) { return; }

            visited = new HashSet<IState>();
            connections = new HashSet<Connection>();
            allStates = new HashSet<IState>();

            DrawFSM();
        }
        Dictionary<IState, Rect> rects;
        private void DrawFSM()
        {
            FindAllStates(s);

            float y = (allStates.Count - 1) * (width + 10) / 2;

            rects = new Dictionary<IState, Rect>();
            int i = 0;
            foreach (var state in allStates)
            {
                Rect stateRect = new Rect(10 + i * (width + 10), y, width, height);
                i++;
                rects.Add(state, stateRect);
            }

            DrawStates();
        }

        HashSet<IState> allStates;

        private void FindAllStates(IState state)
        {
            if (state.IsNullState) { return; }
            if (visited.Contains(state)) { return; }

            visited.Add(state);
            allStates.Add(state);

            foreach (var transition in state.GetTransitions())
            {
                IState t = transition.GetState();

                if (!t.IsNullState)
                {
                    allStates.Add(t);
                }

                FindAllStates(t);
            }
        }

        float width = 150, height = 40;
        

        private void DrawStates()
        {
            int i = 0;
            foreach (var state in allStates)
            {
                foreach (var transition in state.GetTransitions())
                {
                    IState t = transition.GetState();

                    if (!t.IsNullState)
                    {
                        DrawNodeCurve(rects[state], rects[t], transition.ReverseValue(), ! transition.ReverseValue() ? Color.green : Color.red);
                    }
                }
                BeginWindows();
                rects[state] = GUI.Window(i, rects[state], DrawNodeWindow, string.IsNullOrEmpty(state.StateName) ? $"{state.name}" : state.StateName);
                EndWindows();
                i++;
            }
        }

        private void DrawNodeWindow(int id)
        {
            GUI.DragWindow();
        }

        public float angle = 180;

        void DrawNodeCurve(Rect start, Rect end, bool up, Color color)
        {
            float radius = Mathf.Abs(end.center.x - start.center.x) / 2;
            Vector3 from = (end.center.x > start.center.x) ? start.center : end.center;
            Vector3 center = (end.center + start.center) / 2;
            Vector3 fromDir = from - center;
            Handles.color = color;
            Handles.DrawWireArc(center, Vector3.forward, fromDir ,(up ? -1 : 1) * angle, radius);

            Vector3 pos = center;
            pos.y += (radius) * (up ? 1 : -1);

            DrawArrow(pos, (end.center.x < start.center.x), color);

            return;
            /*
            Vector3 pos = end.center;
            float cubeSize = 20;
            pos.y = pos.y + (up ? 1 : -1) * (height / 2 + cubeSize / 2);
            Handles.DrawWireCube(pos, cubeSize * Vector3.one);
            */
        }

        void DrawArrow(Vector3 position, bool pointingLeft, Color color)
        {
            float hauteur = 20;
            float sign = (pointingLeft ? 1 : -1);
            float cote = 2 / Mathf.Sqrt(3) * hauteur;
            Vector3 pointA = position + Vector3.right * hauteur * sign;
            Vector3 pointTMP = pointA;
            pointA.x += hauteur * -sign;
            Vector3 pointB = pointTMP;
            Vector3 pointC = pointTMP;

            pointB.y += cote / 2;
            pointC.y -= cote / 2;

            Handles.color = color;
            Handles.DrawLine(pointA, pointB);
            Handles.DrawLine(pointA, pointC);
            Handles.DrawLine(pointB, pointC);
        }

        private struct Connection
        {
            public IState stateA, stateB;

            public Connection(IState stateA, IState stateB)
            {
                this.stateA = stateA;
                this.stateB = stateB;
            }
        }
    }
}
