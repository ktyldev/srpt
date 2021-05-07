#ifndef __CYLINDER__
#define __CYLINDER__

#include "Assets/Shaders/ray.cginc"

struct Cylinder
{
    // spatial information
    float3 position;
    float3 axis;
    float height; // we could encode this as the axis' length
    float radius;
    
    // TODO: these will be affected by doppler shift
    float3 albedo;
    float3 specular;
    float3 emission;
};

void intersect_cylinder(Ray ray, inout RayHit best_hit, Cylinder cylinder)
{
    float3 ro = ray.origin;
    float3 rd = ray.direction;
    
    float3 pa = cylinder.position;
    // central axis
    float3 ca = cylinder.axis*cylinder.height;
    // base to eye
    float3 oc = ro - pa;
    float ra = cylinder.radius;

    // dot products
    float caca = dot(ca,ca);
    float card = dot(ca,rd);
    float caoc = dot(ca,oc);

    // find intersects
    float a = caca-card*card;
    float b = caca*dot(oc,rd)-caoc*card;
    float c = caca*dot(oc,oc)-caoc*caoc-ra*ra*caca;
    float h = b*b-a*c;

    if (h < 0.0) return;

    h = sqrt(h);
    float t = (-b-h)/a;

    // body
    float y = caoc+t*card;
    if (t > 0 && t < best_hit.distance && y > 0.0 && y < caca)
    {
        best_hit.distance = t;
        best_hit.position = ray.origin + t*ray.direction;
        best_hit.normal = (oc+t*rd - ca*y/caca)/ra;
        
        best_hit.albedo = cylinder.albedo;
        best_hit.specular = cylinder.specular;
        best_hit.emission = cylinder.emission;
        // return;
    }

    // caps
    // t = ((y < 0.0 ? 0.0 : caca) - caoc) / card;
    // if (t < best_hit.distance && abs(b+a*t)<h)
    // {
    //     best_hit.distance = t;
    //     best_hit.position = ray.origin + t*ray.direction;
    //     best_hit.normal = ca*sign(y)/caca;
    //     
    //     best_hit.albedo = cylinder.albedo;
    //     best_hit.specular = cylinder.specular;
    //     best_hit.emission = cylinder.emission;
    // }
}

#endif // __CYLINDER__
