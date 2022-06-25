#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SAMPLE
#define D2D_INPUT1_SAMPLE
#define D2D_INPUT2_SAMPLE
#include "d2d1effecthelpers.hlsli"

D2D_PS_ENTRY(main) 
{
    float4 area = D2DGetInput(0);
    float a = area.a;

    if(a == 1)
        return float4(0, 0, 0, 0); 
    else if(a == 0)
        return D2DGetInput(1);
    else
    {
        float4 p = D2DGetInput(1);
        float ra = smoothstep(0, 1, (1 - a));
        return float4(p.r * ra, p.g * ra, p.b * ra, p.a * ra); 
    }
}