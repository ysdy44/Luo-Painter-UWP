#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float time;
float left;
float top;
float right;
float bottom;
float4 color0 = float4(0, 0, 0, 0); // Transparent
float4 color1 = float4(0, 0, 0, 1); // Black
float4 color2 = float4(1, 1, 1, 1); // White

float2 transform(float2 p)
{
    return p;
}

bool neighbor(float2 up)
{
    if (up.x >= left && up.x <= right && up.y >= top && up.y <= bottom)
    {
        float4 color = D2DSampleInputAtPosition(0, up).rgba;
        return (color.a == 0);
    }
      
    return true;
}

float4 mesh(float2 p, float time)
{
    if ((p.x+time) % 24 < 12)
    {
        if ((p.y + time) % 24 < 12)
            return color1;
        else
            return color2;
    }
    else
    {
        if ((p.y + time) % 24 < 12)
            return color2;
        else
            return color1;
    }
}

D2D_PS_ENTRY(main)
{
float2 p = D2DGetScenePosition().xy;

float2 up = transform(p);

float4 color = D2DSampleInputAtPosition(0, up).rgba;

if (color.a != 0)
{
if (neighbor(float2(up.x-1, up.y))) return mesh(p, time);
if (neighbor(float2(up.x, up.y-1))) return mesh(p, time);
if (neighbor(float2(up.x+1, up.y))) return mesh(p, time);
if (neighbor(float2(up.x, up.y+1))) return mesh(p, time);
}

return color0;
}