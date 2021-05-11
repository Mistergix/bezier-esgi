using UnityEngine;
using System;

namespace PGSauce.Core.PGEvents
{
    public class PGEventNoArg : ScriptableObject
    {
		private event Action action;

		public void Raise()
		{
			action?.Invoke();
		}

		public void Register(Action callback)
		{
			action += callback;
		}

		public void Unregister(Action callback)
		{
			action -= callback;
		}
	}
}
