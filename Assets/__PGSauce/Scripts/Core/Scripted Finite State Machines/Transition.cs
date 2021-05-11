using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachineScripted
{
    public class Transition<T> where T : StateMachineController<T>
    {
        public delegate bool Decision();

        private readonly Decision decision;

        public Transition(State<T> from, State<T> to, Decision decision)
        {
            From = from;
            To = to;
            this.decision = decision;
        }

        public bool Decide(T controller)
        {
            return decision();
        }

        public State<T> From { get; set; }
        public State<T> To { get; set; }

    }
}
