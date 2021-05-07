#ifndef __EINSTEIN__
#define __EINSTEIN__

#include "Assets/Shaders/random.cginc"
#include "Assets/Shaders/ray.cginc"
#include "Assets/Shaders/v3.cginc"

// relativity stuff
float _Velocity;        // speed of S' frame as a proportion of c
float _LorentzFactor;
float _Doppler;
float _Searchlight;

float aberrate_angle(float phi)
{
    float B = _Velocity;
    return acos((cos(phi)-B)/(1.0-cos(phi)*B));
}

// transform a ray from its origin reference frame to another reference frame using lorentz transformation
void transform_ray(inout Ray ray)
{
    // direction of travel
    float3 fwd = float3(0,0,1);
    
    // relativistic aberration

    // find axis of rotation
    float3 axis = cross(ray.direction, fwd);
    // find angle of original ray
    float phi = acos(dot(ray.direction, fwd));

    // aberrate phi
    float phi_a = aberrate_angle(phi);

    // rotate away from forward
    ray.direction = fwd;
    rotate_about_axis(ray.direction, axis, -phi_a);

    // modulate power of ray

    // doppler factor
    float y = _LorentzFactor;
    float B = _Velocity;
    
    float D = (1.0-B*cos(phi_a))/sqrt(1.0-B*B);
    
    // modulate wavelength in accordance with doppler factor
    ray.wavelengths *= lerp(1.0, D, _Doppler);
    ray.energy /= lerp(1.0, D*D*D*D, _Searchlight);
}

#endif // __EINSTEIN__
