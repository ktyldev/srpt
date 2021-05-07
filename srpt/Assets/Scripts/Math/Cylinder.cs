using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ktyl.Rendering
{
    public struct Cylinder
    {
        public Vector3 position;
        public Vector3 axis;
        public float height;
        public float radius;

        public Vector3 albedo;
        public Vector3 specular;
        public Vector3 emission;
    }
}