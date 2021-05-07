using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ktyl.Rendering
{
    public static class ComputeShaderExtensions
    {
        public static Vector3Int GetThreadGroupSizesForScreen(this ComputeShader shader)
        {
            shader.GetKernelThreadGroupSizes(0, out var x, out var y, out var z);

            return new Vector3Int
            {
                x = Mathf.CeilToInt(Screen.width / (float) x),
                y = Mathf.CeilToInt(Screen.height / (float) y),
                z = (int) z
            };
        }
    }
}