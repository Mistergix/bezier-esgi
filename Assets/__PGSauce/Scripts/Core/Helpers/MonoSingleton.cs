using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core.Utilities
{
    public abstract class MonoSingleton<T> : MonoSingletonBase where T : MonoSingletonBase
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            Init();
        }

        public virtual void Init()
        {

        }
    }
}
