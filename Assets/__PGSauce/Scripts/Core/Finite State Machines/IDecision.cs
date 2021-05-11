using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGFiniteStateMachine
{
    public abstract class IDecision : ScriptableObject
    {
        [SerializeField] private string decisionName;

        public string DecisionName { get => decisionName; }
    }
}
