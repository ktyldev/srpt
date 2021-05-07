using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ktyl.Util
{
    public static class BuildMenuItem
    {
        [MenuItem("ktyl/Build")]
        private static void Build()
        {
            BuildCommand.Run();
        }
    }
}