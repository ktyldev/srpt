using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ktyl.Rendering
{
    public struct Strides
    {
        // data structure sizes
        public const int SIZE_INT = 4;
        public const int SIZE_FLOAT = 4;
        public const int SIZE_V3 = SIZE_FLOAT * 3;
        public const int SIZE_V4 = SIZE_FLOAT * 4;
        
        public const int SIZE_SPHERE = SIZE_V3 * 4 + SIZE_FLOAT;
        public const int SIZE_CYLINDER = SIZE_V3 * 5 + SIZE_FLOAT * 2;
    }
}