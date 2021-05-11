using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class Action<T> : IAction where T : StateController<T>
    {
        public abstract void Act(T controller);
    }
}
