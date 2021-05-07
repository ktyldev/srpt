using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using Unity.Mathematics;
using UnityEngine;

namespace Ktyl.Rendering
{
    public abstract class BaseRayTracingRenderSettings : ScriptableObject
    {
        public SamplingSettings Sampling => _samplingSettings;
        [SerializeField] private SamplingSettings _samplingSettings;

        public EnvironmentSettings Environment => _environmentSettings;
        [SerializeField] private EnvironmentSettings _environmentSettings;

        public ComputeShader ComputeShader => _shader;
        [Header("Rendering")] [SerializeField] private ComputeShader _shader;

        public ComputeShader DepthShader => _depthShader;
        [SerializeField] private ComputeShader _depthShader;

        public RenderTexture Depth => _depth;
        [SerializeField] private RenderTexture _depth;

        public RenderTexture Destination => _destination;
        [SerializeField] private RenderTexture _destination;

        public CameraProperties CameraProps => _cameraProperties;
        [SerializeField] private CameraProperties _cameraProperties;

        public SpecialRelativitySettings Relativity => _relativitySettings;
        [SerializeField] private SpecialRelativitySettings _relativitySettings;
    }
}