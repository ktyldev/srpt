using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using Unity.Mathematics;
using UnityEngine;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Lattice Settings")]
    public class LatticeSettings : SerialVar<LatticeData>
    {
        public float4x4 Lattice2World => _lattice2World;
        [SerializeField] private SerialFloat4x4 _lattice2World;
    }

    [Serializable]
    public struct LatticeData
    {
        public int size;
        public float spacing;

        public float sphereRadius;
        public Color sphereColor;
        [ColorUsage(false, true)] public Color sphereEmission;

        public float cylinderRadius;
        public Color cylinderColor;
        [ColorUsage(false, true)] public Color cylinderEmission;
    }
}