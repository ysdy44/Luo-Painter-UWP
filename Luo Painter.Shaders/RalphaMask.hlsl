#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SAMPLE
#define D2D_INPUT1_SAMPLE
#define D2D_INPUT2_SAMPLE
#include "d2d1effecthelpers.hlsli"

D2D_PS_ENTRY(main) 
{
    float4 area = D2DGetInput(0);
    if(area.a > 0)
        return float4(0, 0, 0, 0); 
    else
        return D2DGetInput(1);
}