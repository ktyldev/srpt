using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Ktyl.Rendering
{
    public class RelativisticRayTracing : ScriptableRendererFeature
    {
        [SerializeField] private RelativisticRayTracingRenderSettings _settings;

        [Serializable]
        private struct GlobalBuffer
        {
            public RenderTexture depth;
            public RenderTexture destination;
        }
        [SerializeField] private GlobalBuffer _gBuffer;
        
        // TODO: separate out more render passes, to do depth and shading separately
        private RelativisticDepth _depthPass;
        private RelativisticShading _shadingPass;

        public override void Create()
        {
            _depthPass = new RelativisticDepth(
                _settings.Relativity,
                _settings.Environment.Geometry,
                _settings.CameraProps,
                _settings.Environment,
                _settings.DepthShader,
                _gBuffer.depth)
            {
                renderPassEvent = RenderPassEvent.BeforeRendering
            };
            
            _shadingPass = new RelativisticShading(
                _settings.Relativity,
                _settings.CameraProps,
                _settings.Environment.Geometry,
                _settings.Sampling,
                _settings.Environment,
                _settings.ComputeShader,
                _gBuffer.depth,
                _gBuffer.destination)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_depthPass);
            renderer.EnqueuePass(_shadingPass);
        }

        private void OnDisable()
        {
            _depthPass?.Dispose();
            _shadingPass?.Dispose();
        }
    }
}