using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    [System.Serializable]
    public class Transition<T> : ITransition where T : StateController<T>
    {
        public Decision<T> decision;
        public State<T> state;
        public bool reverseValue;

        public override IDecision GetDecision()
        {
            return decision;
        }

        public override IState GetState()
        {
            return state;
        }

        public override bool ReverseValue()
        {
            return reverseValue;
        }
    }
}
