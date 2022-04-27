#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float3x2 matrix3x2;
float2 zdistance;
float left;
float top;
float right;
float bottom;

D2D_PS_ENTRY(main)
{
    float2 p = D2DGetScenePosition().xy;

    float x = matrix3x2[0][0] * p.x + matrix3x2[1][0] * p.y + matrix3x2[2][0];
    float y = matrix3x2[0][1] * p.x + matrix3x2[1][1] * p.y + matrix3x2[2][1];

    float dotted = 1 + dot(p, zdistance);

    float ux = x / dotted;
    float uy = y / dotted;
     
    if (ux < left) return float4(0, 0, 0, 0);
    if (ux > right) return float4(0, 0, 0, 0);
    if (uy < top) return float4(0, 0, 0, 0);
    if (uy > bottom) return float4(0, 0, 0, 0);

    return D2DSampleInputAtPosition(0, float2(ux, uy));
}