#ifndef __V3__
#define __V3__

// https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Rotate-About-Axis-Node.html
void rotate_about_axis(inout float3 v, float3 axis, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    float one_minus_c = 1.0 - c;

    float3 a = normalize(axis);
    float3x3 rot_mat =
    {
        one_minus_c*a.x*a.x+c,      one_minus_c*a.x*a.y-a.z*s,  one_minus_c*a.z*a.x+a.y*s,
        one_minus_c*a.x*a.y+a.z*s,  one_minus_c*a.y*a.y+c,      one_minus_c*a.y*a.z-a.x*s,
        one_minus_c*a.z*a.x-a.y*s,  one_minus_c*a.y*a.z+a.x*s,  one_minus_c*a.z*a.z*s+c
    };

   v = mul(rot_mat, v);
}

float3x3 get_tangent_space(float3 normal)
{
    // helper vector for the cross product
    float3 helper = float3(1, 0, 0);
    if (abs(normal.x) > 0.99f)
    {
        helper = float3(0, 0, 1);
    }

    // generate vectors
    float3 tangent = normalize(cross(normal, helper));
    float3 binormal = normalize(cross(normal, tangent));
    return float3x3(tangent, binormal, normal);
}

#endif // __V3__