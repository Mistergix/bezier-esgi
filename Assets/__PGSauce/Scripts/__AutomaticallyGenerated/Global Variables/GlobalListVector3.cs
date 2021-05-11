using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core
{
    [CreateAssetMenu(fileName = "new Global ListVector3", menuName = "PG/Global Variables/Global ListVector3")]
    public class GlobalListVector3 : IGlobalValue<List<Vector3>>
    {
    }
}