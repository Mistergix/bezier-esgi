using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public class StateMachine<T> where T : StateMachineController<T>
    {
        
        public State<T> CurrentState { get; private set; }
        protected T Reference { get; private set; }
        private Dictionary<State<T>, List<Transition<T>>> allowedTransitions = new Dictionary<State<T>, List<Transition<T>>>();

        internal void CheckTransitions()
        {
            CurrentState.CheckTransitions(this);
        }

        public StateMachine(T reference)
        {
            Reference = reference;
        }

        public void AddNewTransition(State<T> from, State<T> to, Transition<T>.Decision decision)
        {
            Transition<T> stateTransition = new Transition<T>(from, to, decision);

            if (allowedTransitions.ContainsKey(from))
            {
                allowedTransitions.Add(from, new List<Transition<T>>());
            }

            allowedTransitions[from].Add(stateTransition);
        }

        public List<Transition<T>> GetTransitions(State<T> from)
        {
            return allowedTransitions[from];
        }

        public void Initialize(State<T> initialState)
        {
            CurrentState = initialState;
            Enter();
        }

        public virtual void ChangeState(State<T> newState)
        {
                CurrentState.Exit();

                CurrentState = newState;

                CurrentState.Enter();
        }

        public virtual void Enter()
        {
            CurrentState.Enter();
        }

        public virtual void HandleInput()
        {
            CurrentState.HandleInput();
        }

        public virtual void LogicUpdate()
        {
            CurrentState.LogicUpdate();
        }

        public virtual void PhysicsUpdate()
        {
            CurrentState.PhysicsUpdate();
        }

        public virtual void Exit()
        {
            CurrentState.Exit();
        }
    }

}
