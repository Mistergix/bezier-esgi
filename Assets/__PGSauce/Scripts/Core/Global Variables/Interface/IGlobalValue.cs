using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core
{
    public class IGlobalValue<T> : IGlobalValueScriptableObject
    {
        [SerializeField] private T value;

        public T Value { get => value; set => this.value = value; }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}

