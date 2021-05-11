using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core
{
    [CreateAssetMenu(fileName = "new Global Color", menuName = "PG/Global Variables/Global Color")]
    public class GlobalColor : IGlobalValue<Color>
    {
    }
}