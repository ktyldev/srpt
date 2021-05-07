using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Rendering/Sampling Settings")]
    public class SamplingSettings : SerialVar<SamplingData>
    {
        public Material SampleAccumulator => _sampleAccumulator;
        [SerializeField] private Material _sampleAccumulator;
        
        public void SetShaderSamplingProperties(CommandBuffer commands, ComputeShader shader)
        {
            commands.SetComputeIntParam(shader, "_SamplesPerPixel", Value.samplesPerPixel);
            commands.SetComputeIntParam(shader, "_Bounces", Value.depth);
        }
    }

    [Serializable]
    public struct SamplingData
    {
        [Range(2, 8)] public int depth;
        public int samplesPerPixel;

        public float SampleWeight => Application.isPlaying ? _sampleWeight : _editorSampleWeight;
        [SerializeField] private SerialFloat _sampleWeight;
        [SerializeField] private float _editorSampleWeight;
    }
}