using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Environment Settings")]
    public class EnvironmentSettings : ScriptableObject
    {
        public Color GroundColor => _groundColor;
        [SerializeField] private Color _groundColor;
        public float GroundHeight => (float)_groundHeight.Value;
        [SerializeField] private SerialDouble _groundHeight;
        public Color SkyColor => _skyColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _skyColor;

        public LatticeSettings Lattice => _lattice;
        [SerializeField] private LatticeSettings _lattice;

        public GeometryBuffers Geometry => _geometry;
        [SerializeField] private GeometryBuffers _geometry;

        public void SetShaderEnvironmentProperties(CommandBuffer commands, ComputeShader shader)
        {
            commands.SetComputeVectorParam(shader, "_SkyColor", SkyColor);
            commands.SetComputeVectorParam(shader, "_GroundColor", GroundColor);
            commands.SetComputeFloatParam(shader, "_GroundHeight", GroundHeight);
        }
    }
}
