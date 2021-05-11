using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public abstract class State<T> where T : StateMachineController<T>
    {
        private StateMachine<T> stateMachine;
        private T reference;

        public T Reference { get => reference; private set => reference = value; }
        public StateMachine<T> StateMachine { get => stateMachine; private set => stateMachine = value; }

        public void Init(T reference, StateMachine<T> stateMachine)
        {
            this.Reference = reference;
            this.StateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void HandleInput() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }

        public void CheckTransitions(StateMachine<T> fsm)
        {
            List<Transition<T>> transitions = fsm.GetTransitions(this);
            for (int i = 0; i < transitions.Count; i++)
            {
                bool decisionSucceeded = transitions[i].Decide(reference);

                if (decisionSucceeded)
                {
                    fsm.ChangeState(transitions[i].To);
                }
            }
        }
    }
}
