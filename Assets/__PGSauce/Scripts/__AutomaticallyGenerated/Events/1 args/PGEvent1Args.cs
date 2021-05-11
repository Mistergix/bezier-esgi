using System;
using UnityEngine;

namespace PGSauce.Core.PGEvents
{
	public class PGEvent1Args<T0> : ScriptableObject
	{
		private event Action<T0> action;

		public void Raise(T0 value0)
		{
			action?.Invoke(value0);
		}

		public void Register(Action<T0> callback)
		{
			action += callback;
		}

		public void Unregister(Action<T0> callback)
		{
			action -= callback;
		}
	}
}
