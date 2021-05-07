using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Ktyl.Util
{
    public static class MathExtensions
    {
        public static Vector3 v3(this double3 v) => new Vector3((float)v.x, (float)v.y, (float)v.z); 
    }
}