using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ktyl.Rendering
{
    public class SphereRenderer : PrimitiveRenderer<Sphere>
    {
        public override Sphere Value
        {
            get
            {
                var s = _sphere;
                s.position = transform.position;
                return s;
            }
            set
            {
                transform.position = value.position;
                _sphere = value;
                _sphere.position = default;
            }
        }
        private Sphere _sphere;
    }
}