using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ktyl.Rendering
{
    public class RelativisticDepth : ScriptableRenderPass, IDisposable
    {
        private CommandBuffer _cb;
        
        private RenderTexture _depth;
        private RenderTargetIdentifier _depthId;
        private readonly RenderTexture _depthSource;
        private RenderTargetIdentifier _depthSourceId;

        private readonly SpecialRelativitySettings _relativity;
        private readonly GeometryBuffers _geometry;
        private readonly CameraProperties _cameraProperties;
        private readonly ComputeShader _depthShader;
        private readonly EnvironmentSettings _environment;

        // TODO: miss texture

        public RelativisticDepth(
            SpecialRelativitySettings relativity,
            GeometryBuffers geometry, 
            CameraProperties cameraProperties, 
            EnvironmentSettings environment,
            ComputeShader depthShader, 
            RenderTexture depthSource)
        {
            _relativity = relativity;
            _geometry = geometry;
            _cameraProperties = cameraProperties;
            _depthSource = depthSource;
            _depthShader = depthShader;
            _environment = environment;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (_depth != null)
            {
                Dispose();
            }
            
            // depth buffer
            _depth = new RenderTexture(
                Screen.width,
                Screen.height,
                0,
                RenderTextureFormat.ARGBFloat,  // TODO: this could maybe be better
                RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            _depth.Create();
            _depthId = new RenderTargetIdentifier(_depth);
            _depthSourceId = new RenderTargetIdentifier(_depthSource);
            
            _cb = CommandBufferPool.Get();
        }

        private const string BUFFER_NAME = "Relativistic Depth";
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            _cb.BeginSample(BUFFER_NAME);
            
            SetShaderProperties(ref renderingData);

            var groups = _depthShader.GetThreadGroupSizesForScreen();
            _cb.DispatchCompute(_depthShader, 0, groups.x, groups.y, groups.z);
            _cb.EndSample(BUFFER_NAME);
            
            _cb.Blit(_depthId, _depthSourceId);
            context.ExecuteCommandBuffer(_cb);
        }

        private void SetShaderProperties(ref RenderingData renderingData)
        {
            // aspect ratio
            _cb.SetComputeFloatParam(_depthShader, "_Aspect", (float)Screen.width/Screen.height);
            
            //textures
            _cb.SetComputeTextureParam(_depthShader, 0, "Result", _depthId);
            
            // camera
            _cameraProperties.SetCameraShaderProperties(_cb, renderingData, _depth, _depthShader);
            
            // geometry
            _geometry.SetData(_cb, _depthShader);
            _cb.SetComputeMatrixParam(_depthShader, "_Lattice2World", _environment.Lattice.Lattice2World);
            
            // relativity
            _relativity.SetShaderRelativityParameters(_cb, _depthShader);
        }

        public void Dispose()
        {
            if (_depth) _depth.Release();
        }
    }
}