using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core.PGEvents
{
    public class IPGEvent<T> : ScriptableObject where T : IPGEventListener
    {
        [SerializeField] [TextArea] private string description;

        public HashSet<T> Listeners { get; } = new HashSet<T>();

        public void RegisterListener(T listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(T listener)
        {
            Listeners.Remove(listener);
        }
    }
}
