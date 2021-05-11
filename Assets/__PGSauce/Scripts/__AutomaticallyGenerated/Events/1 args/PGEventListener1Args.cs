using UnityEngine;
using UnityEngine.Events;

namespace PGSauce.Core.PGEvents
{
    public class PGEventListener1Args<T0, PGEventT, UnityEventT> : IPGEventListener where PGEventT : PGEvent1Args<T0> where UnityEventT : UnityEvent<T0>
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

        private void ActionCalled(T0 value0)
        {
            action.Invoke(value0);
        }
    }
}