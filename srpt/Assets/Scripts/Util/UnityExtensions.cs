using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Ktyl.Util
{
    public static class UnityExtensions
    {
        public static Vector4 v4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);
    }
}