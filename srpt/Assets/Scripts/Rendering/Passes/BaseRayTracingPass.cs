using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ktyl.Rendering
{
    public abstract class BaseRayTracingPass : ScriptableRenderPass, IDisposable
    {
        protected CommandBuffer CommandBuffer => _commandBuffer;
        private CommandBuffer _commandBuffer;

        // rendering
        private RenderTexture _target;
        private RenderTexture _depth;
        private RenderTargetIdentifier _targetIdentifier;
        private RenderTargetIdentifier _rtSource;
        private RenderTargetIdentifier _depthIdentifier;
        private RenderTargetIdentifier _depthSource;
        private Material _sampleAccumulator;

        protected abstract BaseRayTracingRenderSettings Settings { get; }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (_target != null)
            {
                Dispose();
            }

            Initialise();

            _commandBuffer = CommandBufferPool.Get();
        }

        protected virtual void Initialise()
        {
            // _sampleAccumulator = Settings.SampleAccumulator;
            
            InitialiseRenderTextures();
        }

        private void InitialiseRenderTextures()
        {
            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true,
            };
            _target.Create();
            _targetIdentifier = new RenderTargetIdentifier(_target);

            // _depth = new RenderTexture(
            //     Screen.width,
            //     Screen.height,
            //     0,
            //     RenderTextureFormat.ARGBFloat,
            //     RenderTextureReadWrite.Linear)
            // {
            //     enableRandomWrite = true
            // };
            // _depth.Create();
            // _depthIdentifier = new RenderTargetIdentifier(_depth);
            // _depthSource = new RenderTargetIdentifier(Settings.Depth);

            _rtSource = new RenderTargetIdentifier(Settings.Destination);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SetShaderProperties(renderingData);

            // depth pass
            var groups = Settings.DepthShader.GetThreadGroupSizesForScreen();
            _commandBuffer.SetComputeTextureParam(Settings.DepthShader, 0, "Result", _depthIdentifier);
            _commandBuffer.DispatchCompute(Settings.DepthShader, 0, groups.x, groups.y, 1);
            _commandBuffer.Blit(_depthIdentifier, _depthSource);
            
            // dispatch compute shader
            groups = Settings.ComputeShader.GetThreadGroupSizesForScreen();
            _commandBuffer.DispatchCompute(Settings.ComputeShader, 0, groups.x, groups.y, 1);
            
            // blend frames
            _sampleAccumulator.SetFloat("_Sample", Settings.Sampling.Value.SampleWeight);
            _commandBuffer.Blit(_targetIdentifier, _rtSource, _sampleAccumulator);
            
            context.ExecuteCommandBuffer(_commandBuffer);
        }

        protected virtual void SetShaderProperties(RenderingData renderingData)
        {
            var shader = Settings.ComputeShader;
            
            // aspect
            var aspect = (float) Screen.width / Screen.height;
            _commandBuffer.SetComputeFloatParam(shader, "_Aspect", aspect);
            _commandBuffer.SetComputeFloatParam(Settings.DepthShader, "_Aspect", aspect);
            
            // random
            _commandBuffer.SetComputeFloatParam(shader, "_Seed", UnityEngine.Random.value);

            // sampling
            Settings.Sampling.SetShaderSamplingProperties(_commandBuffer, shader);

            // camera
            Settings.CameraProps.SetCameraShaderProperties(_commandBuffer, renderingData, _target, shader);
            Settings.CameraProps.SetCameraShaderProperties(_commandBuffer, renderingData, _depth, Settings.DepthShader);
            
            // environment
            Settings.Environment.SetShaderEnvironmentProperties(_commandBuffer, shader);

            // textures
            _commandBuffer.SetComputeTextureParam(shader, 0, "Result", _targetIdentifier);
            _commandBuffer.SetComputeTextureParam(shader, 0, "Depth", _depthIdentifier);
        }

        public virtual void Dispose()
        {
            if (_target) _target.Release();
            if (_depth) _depth.Release();

            // _commandBuffer?.Dispose();
        }
    }
}