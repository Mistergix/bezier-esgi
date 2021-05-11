using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core
{
    [CreateAssetMenu(fileName = "new Global Bool", menuName = "PG/Global Variables/Global Bool")]
    public class GlobalBool : IGlobalValue<bool>
    {
    }
}