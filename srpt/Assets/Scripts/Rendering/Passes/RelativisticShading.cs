using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ktyl.Rendering
{
    public class RelativisticShading : ScriptableRenderPass, IDisposable
    {
        private readonly GeometryBuffers _geometry;
        private readonly SpecialRelativitySettings _relativity;
        private readonly SamplingSettings _sampling;
        private readonly CameraProperties _cameraProperties;
        private readonly EnvironmentSettings _environment;
        private readonly ComputeShader _shader;
        
        private CommandBuffer _cb;
        
        private RenderTexture _target;
        private RenderTargetIdentifier _targetId;
        private RenderTargetIdentifier _targetSource;
        private readonly RenderTexture _destination;

        private RenderTexture _depth;
        private RenderTargetIdentifier _depthId;
        private RenderTexture _depthSource;
        private RenderTargetIdentifier _depthSourceId;

        public RelativisticShading(
            SpecialRelativitySettings relativity, 
            CameraProperties cameraProperties,
            GeometryBuffers geometry, 
            SamplingSettings sampling,
            EnvironmentSettings environment,
            ComputeShader shader,
            RenderTexture depth,
            RenderTexture destination)
        {
            _relativity = relativity;
            _geometry = geometry;
            _sampling = sampling;
            _destination = destination;
            _cameraProperties = cameraProperties;
            _shader = shader;
            _environment = environment;
            _depthSource = depth;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (_target != null)
            {
                Dispose();
            }

            // initialise rendertexture
            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true,
            };
            _target.Create();
            _targetId = new RenderTargetIdentifier(_target);
            _targetSource = new RenderTargetIdentifier(_destination);

            // TODO: we dont need random write??
            // use a global depth texture if possible??
            _depth = new RenderTexture(
                Screen.width, 
                Screen.height, 
                0, 
                RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            _depth.Create();
            _depthId = new RenderTargetIdentifier(_depth);
            _depthSourceId = new RenderTargetIdentifier(_depthSource);
            
            _cb = CommandBufferPool.Get();
        }

        private const string BUFFER_NAME = "Relativistic Shading";
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            _cb.Blit(_depthSourceId, _depthId);
            
            SetShaderProperties(ref renderingData);
            
            // dispatch shader
            _cb.BeginSample(BUFFER_NAME);
            var groups = _shader.GetThreadGroupSizesForScreen();
            _cb.DispatchCompute(_shader, 0, groups.x, groups.y, groups.z);
            _cb.EndSample(BUFFER_NAME);
            
            // blend frames
            var accumulator = _sampling.SampleAccumulator;
            accumulator.SetFloat("_Sample", _sampling.Value.SampleWeight);
            _cb.Blit(_targetId, _targetSource, accumulator);
            
            context.ExecuteCommandBuffer(_cb);
        }

        private void SetShaderProperties(ref RenderingData renderingData)
        {
            // aspect
            _cb.SetComputeFloatParam(_shader, "_Aspect", (float)Screen.width/Screen.height);
            
            // textures
            _cb.SetComputeTextureParam(_shader, 0, "Result", _targetId);
            _cb.SetComputeTextureParam(_shader, 0, "Depth", _depthId);
            
            // random
            _cb.SetComputeFloatParam(_shader, "_Seed", UnityEngine.Random.value);
            
            // camera
            _cameraProperties.SetCameraShaderProperties(_cb, renderingData, _target, _shader);
            
            // sampling
            _sampling.SetShaderSamplingProperties(_cb, _shader);
            
            // environment + geometry
            _environment.SetShaderEnvironmentProperties(_cb, _shader);
            _geometry.SetData(_cb, _shader);
            _cb.SetComputeMatrixParam(_shader, "_Lattice2World", _environment.Lattice.Lattice2World);
            
            // relativity
            _relativity.SetShaderRelativityParameters(_cb, _shader);
        }

        public void Dispose()
        {
            if (!Application.isPlaying) return;
            
            if (_target) _target.Release();
            if (_depth) _depth.Release();
        }
    }
}