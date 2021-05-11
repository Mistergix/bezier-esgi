using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace PGSauce.Core.PGEvents
{
    public class PGEventListenerNoArg<PGEventT, UnityEventT> : IPGEventListener where PGEventT : PGEventNoArg where UnityEventT : UnityEvent
    {
        [SerializeField] private PGEventT gameEvent;
        [SerializeField] private UnityEventT action;

        private void OnEnable()
        {
            gameEvent.Register(ActionCalled);
        }

        private void OnDisable()
        {
            gameEvent.Unregister(ActionCalled);
        }

        private void ActionCalled()
        {
            action.Invoke();
        }
    }
}
