using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class State<T> : IState where T : StateController<T>
    {
        [SerializeField] [HideIf("IsNullState")] private Action<T> enterAction;
        [SerializeField] [HideIf("IsNullState")] private Action<T> inputAction;
        [SerializeField] [HideIf("IsNullState")] private Action<T> logicAction;
        [SerializeField] [HideIf("IsNullState")] private Action<T> physicsAction;
        [SerializeField] [HideIf("IsNullState")] private Action<T> exitAction;

        [SerializeField] [HideIf("IsNullState")] [ReorderableList] private Transition<T>[] transitions;

        public override List<ITransition> GetTransitions()
        {
            return transitions.Select(t => t as ITransition).ToList();
        }

        public void OnEnter(T controller) {
            if(enterAction == null) { return; }
            enterAction.Act(controller);
        }
        public void HandleInput(T controller) {
            if (inputAction == null) { return; }
            inputAction.Act(controller);
        }
        public void LogicUpdate(T controller) {
            if (logicAction == null) { return; }
            logicAction.Act(controller);
        }
        public void PhysicsUpdate(T controller) {
            if (physicsAction == null) { return; }
            physicsAction.Act(controller);
        }
        public void OnExit(T controller) {
            if (exitAction == null) { return; }
            exitAction.Act(controller);
        }

        public void CheckTransitions(T controller, StateMachine<T> fsm)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                bool decisionSucceeded = transitions[i].decision.Decide(controller);

                if (transitions[i].reverseValue)
                {
                    decisionSucceeded = !decisionSucceeded;
                }

                if (decisionSucceeded)
                {
                    fsm.ChangeState(transitions[i].state);
                }
            }
        }
    }
}
