using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core.Utilities
{
    public abstract class MonoSingleton<T> : MonoSingletonBase where T : MonoSingletonBase
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

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
