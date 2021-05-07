using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ktyl.Rendering
{
    [CreateAssetMenu(menuName = "ktyl/Rendering/Camera Properties")]
    public class CameraProperties : ScriptableObject
    {
        private struct CamProps
        {
            public Vector3 w, u, v;
            public Vector3 horizontal;
            public Vector3 vertical;
            public Vector3 lowerLeftCorner;
        }

        private CamProps GetCameraProperties(RenderTexture target, Camera cam)
        {
            CamProps props = default;

            var theta = math.radians(cam.fieldOfView);
            var h = math.tan(theta * 0.5f);
            var aspectRatio = target.width / target.height;
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;

            props.w = -cam.transform.forward;
            props.u = math.normalize(math.cross(cam.transform.up, props.w));
            props.v = math.cross(props.w, props.u);

            var focusDistance = 1;
            props.horizontal = focusDistance * viewportWidth * props.u;
            props.vertical = focusDistance * viewportHeight * props.v;
            props.lowerLeftCorner = cam.transform.position
                                    - props.horizontal * 0.5f
                                    - props.vertical * 0.5f
                                    - focusDistance * props.w;

            return props;
        }

        public void SetCameraShaderProperties(CommandBuffer commands, RenderingData renderingData, RenderTexture target, ComputeShader shader)
        {
            var cam = renderingData.cameraData.camera;
            var props = GetCameraProperties(target, cam);
            commands.SetComputeMatrixParam(shader, "_Camera2World", cam.cameraToWorldMatrix);
            commands.SetComputeMatrixParam(shader, "_CameraInverseProjection", cam.projectionMatrix.inverse);
            commands.SetComputeVectorParam(shader, "_CameraHorizontal", props.horizontal);
            commands.SetComputeVectorParam(shader, "_CameraVertical", props.vertical);
            commands.SetComputeVectorParam(shader, "_CameraLowerLeftCorner", props.lowerLeftCorner);
            commands.SetComputeFloatParam(shader, "_CameraNearClip", cam.nearClipPlane);
            commands.SetComputeFloatParam(shader, "_CameraFarClip", cam.farClipPlane);
        }
    }
}