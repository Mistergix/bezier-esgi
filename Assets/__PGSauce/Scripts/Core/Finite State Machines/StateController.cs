using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class StateController<T> : MonoBehaviour where T : StateController<T>
    {
        [SerializeField] private State<T> initialState;

        private StateMachine<T> fsm;

        private State<T> currentState;

        private void Start()
        {
            fsm = new StateMachine<T>(initialState, this as T);
        }

        private void Update()
        {
            fsm.HandleInput();
            fsm.LogicUpdate();
            fsm.CheckTransitions();
            currentState = fsm.CurrentState;
        }

        private void FixedUpdate()
        {
            fsm.PhysicsUpdate();
        }
    }
}
