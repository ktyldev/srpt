#ifndef __PLANE__
#define __PLANE__

#include "Assets/Shaders/PhotonRay.cginc"

struct Plane
{
    // spatial information
    float3 normal;
    float3 origin;
    
    // TODO: these will be affected by doppler shift
    float3 albedo;
    float3 specular;
    float3 emission;
};

Plane create_ground_plane()
{
    Plane ground;
    ground.origin = float3(0, -1, 0);
    ground.normal = float3(0,1,0);
    ground.albedo = float3(1,1,1);
    ground.specular = float3(0,0,0);
    ground.emission = float3(0,0,0);

    return ground;
}

void intersect_plane(PhotonRay ray, inout RayHit best_hit, Plane plane)
{
    float3 ro = ray.origin;
    float3 rd = ray.direction;
    float3 po = plane.origin;

    float denom = dot(rd,plane.normal);
    float3 poro = po - ro;
    
    float t = dot(poro, plane.normal) / denom; 
    
    if (t > 0 && t < best_hit.distance)
    {
        best_hit.distance = t;
        best_hit.position = ray.origin + t*ray.direction;
        best_hit.normal = plane.normal;
        best_hit.albedo = plane.albedo;
        best_hit.specular = plane.specular;
        best_hit.emission = plane.emission;
    }
}

#endif // __PLANE__
