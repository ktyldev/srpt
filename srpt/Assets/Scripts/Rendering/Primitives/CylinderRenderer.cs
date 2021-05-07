using System.Collections;
using System.Collections.Generic;
using Ktyl.Rendering;
using UnityEngine;

public class CylinderRenderer : PrimitiveRenderer<Cylinder>
{
    public override Cylinder Value
    {
        get
        {
            var c = _cylinder;
            c.position = transform.position;
            c.axis = transform.up;
            return c;
        }
        set
        {
            transform.position = value.position;
            transform.up = value.axis;
            
            _cylinder = value;
            _cylinder.position = default;
            _cylinder.axis = default;
        }
    }

    private Cylinder _cylinder;
}
