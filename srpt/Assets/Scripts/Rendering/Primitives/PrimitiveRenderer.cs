using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ktyl.Rendering
{
    public abstract class PrimitiveRenderer<T> : MonoBehaviour where T : struct
    {
        public abstract T Value { get; set; }
    }
}