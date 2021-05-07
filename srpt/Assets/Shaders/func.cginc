#ifndef __FUNC__
#define __FUNC__

float sdot(float3 x, float3 y, float f = 1.0f)
{
    return saturate(dot(x, y) * f);
}

#endif // __FUNC__
