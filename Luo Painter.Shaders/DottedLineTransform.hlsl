#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float time;
float lineWidth = 1.5;
float left;
float top;
float right;
float bottom;
float3x2 matrix3x2;
float4 color0 = float4(0, 0, 0, 0); // Transparent
float4 color1 = float4(0, 0, 0, 1); // Black
float4 color2 = float4(1, 1, 1, 1); // White

bool neighbor(float x, float y)
{
    // transform
    float ux = matrix3x2[0][0] * x + matrix3x2[1][0] * y + matrix3x2[2][0];
    if (ux < left) return false;
    if (ux > right) return false;
     
    // transform
    float uy = matrix3x2[0][1] * x + matrix3x2[1][1] * y + matrix3x2[2][1];
    if (uy < top) return false;
    if (uy > bottom) return false;
    
    float4 color = D2DSampleInputAtPosition(0, float2(ux, uy)).rgba;
    return color.a != 0;
}

float4 mesh(float d)
{
    if ((d + time) % 24 < 12)
        return color1;
    else
        return color2;
}

D2D_PS_ENTRY(main)
{
float2 p = D2DGetScenePosition().xy;
float x = p.x;
float y = p.y;
float d = x + y;

if (neighbor(x, y)) 
return color0;

if (neighbor(x-lineWidth, y)||
neighbor(x, y-lineWidth)||
neighbor(x+lineWidth, y)||
neighbor(x, y+lineWidth))
return mesh(d);

return color0;
}