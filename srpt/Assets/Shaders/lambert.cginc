#ifndef __LAMBERT__
#define __LAMBERT__

#include "Assets/Shaders/random.cginc"
#include "Assets/Shaders/const.cginc"
#include "Assets/Shaders/v3.cginc"
#include "Assets/Shaders/ray.cginc"

float3 sample_hemisphere(float3 normal)
{
    // uniformly sample hemisphere direction
    float cos_theta = rand();
    float sin_theta = sqrt(max(0.0f, 1.0f - cos_theta * cos_theta));
    float phi = 2 * PI * rand();
    float3 tangent_space_dir = float3(cos(phi) * sin_theta, sin(phi) * sin_theta, cos_theta);

    // transform direction to world space
    return mul(tangent_space_dir, get_tangent_space(normal));
}

float3 scatter_lambert(inout Ray ray, RayHit hit)
{
    ray.origin = hit.position + hit.normal * 0.001f;
    ray.direction = sample_hemisphere(hit.normal);

    // artificial reflectance factor
    float G = 0.3*hit.albedo.r+0.59*hit.albedo.g*0.11*hit.albedo.b;
    float far = G;
    ray.energy *= far;
    
    return 0.0f;
}

#endif // __LAMBERT__
