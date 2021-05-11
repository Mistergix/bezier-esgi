using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class IAction : ScriptableObject
    {
        [SerializeField] private string actionName;

        public string ActionName { get => actionName; }
    }
}
