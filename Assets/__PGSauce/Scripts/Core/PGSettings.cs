using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.Utilities;
using UnityEngine.EventSystems;

namespace PGSauce.Core
{
    [CreateAssetMenu(menuName = "PG/PGSettings", fileName = "_____________PGSettings")]
    public class PGSettings : SOSingleton<PGSettings>
    {
        [SerializeField] private string gameName;
        [SerializeField] private string gameNameInNamespace;

        public string GameNameInNamespace => gameNameInNamespace;
        public string GameName => gameName;
    }
}
