using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public class StateMachine<T> where T : StateController<T>
    {
        private State<T> currentState;
        private T controller;

        public State<T> CurrentState { get => currentState; private set => currentState = value; }

        public StateMachine(State<T> initialState, T controller)
        {
            this.CurrentState = initialState;
            this.controller = controller;
            CurrentState.OnEnter(controller);
        }

        public void ChangeState(State<T> newState)
        {
            if(! newState.IsNullState)
            {
                CurrentState.OnExit(controller);
                CurrentState = newState;
                CurrentState.OnEnter(controller);
            }
        }

        public void HandleInput()
        {
            CurrentState.HandleInput(controller);
        }
        public void LogicUpdate()
        {
            CurrentState.LogicUpdate(controller);
        }
        public void PhysicsUpdate()
        {
            CurrentState.PhysicsUpdate(controller);
        }

        public void CheckTransitions()
        {
            CurrentState.CheckTransitions(controller, this);
        }
    }
}
