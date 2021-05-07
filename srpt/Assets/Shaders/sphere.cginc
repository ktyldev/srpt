#ifndef __SPHERE__
#define __SPHERE__

#include "Assets/Shaders/ray.cginc"

struct Sphere
{
    // spatial information
    float3 position;
    float radius;
    
    // TODO: these will be affected by doppler shift
    float3 albedo;
    float3 specular;
    float3 emission;
};

void intersect_sphere(Ray ray, inout RayHit best_hit, Sphere sphere)
{
    // calculate distance along the ray where the sphere is intersected
    float3 d = ray.origin - sphere.position;
    float p1 = -dot(ray.direction, d);
    float p2sqr = p1*p1 - dot(d,d) + sphere.radius*sphere.radius;

    if (p2sqr < 0) return;

    float p2 = sqrt(p2sqr);
    float t = p1-p2>0 ? p1-p2 : p1+p2;
    if (t > 0 && t < best_hit.distance)
    {
        best_hit.distance = t;
        best_hit.position = ray.origin + t*ray.direction;
        best_hit.normal = normalize(best_hit.position - sphere.position);
        best_hit.albedo = sphere.albedo;
        best_hit.specular = sphere.specular;
        best_hit.emission = sphere.emission;
    }
}

#endif // __SPHERE__
