#ifndef __RAY__
#define __RAY__

#include "Assets/Shaders/camera.cginc"
#include "Assets/Shaders/const.cginc"

struct Ray
{
    float3 origin;
    float3 direction;

    // TODO: combine energy with wavelengths
    float energy;
    // lets get the most out of our memory shall we
    float4 wavelengths;
};

Ray create_ray(float3 origin, float3 direction)
{
    Ray ray;
    ray.origin = origin;
    ray.direction = direction;
    ray.wavelengths = 1;
    ray.energy = 1.0;
    
    return ray;
}

struct RayHit
{
    float3 position;
    float distance;
    float3 normal;

    // TODO: rgb colour oh no
    float3 albedo;
    float3 specular;
    float3 emission;
};

RayHit create_ray_hit()
{
    RayHit hit;
    hit.position = float3(0,0,0);
    hit.distance = BIG;
    hit.normal = float3(0,0,0);
    hit.albedo = float3(0,0,0);
    hit.specular = float3(0,0,0);
    hit.emission = float3(0,0,0);
    return hit;
}

Ray create_camera_ray(float2 uv)
{
    float4x4 c2w = _Camera2World;
    float4x4 cip = _CameraInverseProjection;
    float3 cll = _CameraLowerLeftCorner;
    float3 ch = _CameraHorizontal;
    float3 cv = _CameraVertical;
    
    // transform -1..1 -> 0..1
    uv = uv * 0.5 + 0.5;
    uv.x = 1-uv.x;
    
    // transform the camera origin to world space
    float3 origin = mul(c2w, float4(0,0,0,1)).xyz;
    // invert the perspective projection of the view-space position
    float3 direction = mul(cip, float4(uv, 0, 1)).xyz;

    // nudge the ray a touch
    float4 random_rot = float4(rand(),rand(),rand(),rand()) * 0.1;
    direction = random_rot*direction;

    // transform the direction from camera to world space and normalise
    direction = mul(c2w, float4(direction, 0)).xyz;
    direction = cll
        + uv.x * ch
        + uv.y * cv
        - origin;
    direction = normalize(direction);
    
    return create_ray(origin, direction);
}

#endif // __RAY__
