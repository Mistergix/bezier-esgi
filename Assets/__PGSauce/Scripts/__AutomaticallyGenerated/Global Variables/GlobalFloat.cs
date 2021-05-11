using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGSauce.Core
{
    [CreateAssetMenu(fileName = "new Global Float", menuName = "PG/Global Variables/Global Float")]
    public class GlobalFloat : IGlobalValue<float>
    {
    }
}