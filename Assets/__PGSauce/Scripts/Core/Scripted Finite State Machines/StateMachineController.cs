using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public abstract class StateMachineController<T> : MonoBehaviour where T : StateMachineController<T>
    {
        private StateMachine<T> fsm;

        public StateMachine<T> Fsm { get => fsm; private set => fsm = value; }

        private void Awake()
        {
            Fsm = new StateMachine<T>(this as T);
            InitFSM();
        }

        protected abstract void InitFSM();

        private void Update()
        {
            Fsm.HandleInput();
            Fsm.LogicUpdate();
            Fsm.CheckTransitions();
        }

        private void FixedUpdate()
        {
            Fsm.PhysicsUpdate();
        }
    }
}
