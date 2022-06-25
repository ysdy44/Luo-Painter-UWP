#define D2D_INPUT_COUNT 3
#define D2D_INPUT0_SAMPLE
#define D2D_INPUT1_SAMPLE
#define D2D_INPUT2_SAMPLE
#include "d2d1effecthelpers.hlsli"

D2D_PS_ENTRY(main) 
{
    float4 area = D2DGetInput(0);
    float a = area.a;

    if(a == 0)
        return D2DGetInput(1);
    else if(a == 1)
    {
        float4 area1 = D2DGetInput(2);
        float a1 = area1.a;

        float4 p = D2DGetInput(1);
        float ra = smoothstep(0, 1, (1 - a1));
        return float4(p.r * ra, p.g * ra, p.b * ra, p.a * ra); 
    }
    else
    {
        float4 area2 = D2DGetInput(2);
        float a2 = area2.a;

        float4 p2 = D2DGetInput(1);
        float ra2 = smoothstep(0, 1, (1 - a2) * a);
        return float4(p2.r * ra2, p2.g * ra2, p2.b * ra2, p2.a * ra2); 
    }
}