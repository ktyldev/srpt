#ifndef __CAMERA__
#define __CAMERA__

// camera data
float4x4 _Camera2World;
float4x4 _CameraInverseProjection;
float3 _CameraHorizontal;
float3 _CameraVertical;
float3 _CameraLowerLeftCorner;
float _CameraNearClip;
float _CameraFarClip;

#endif // CAMERA
