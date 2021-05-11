using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class Decision<T> : IDecision where T : StateController<T>
    {
        public abstract bool Decide(T controller);
    }
}
