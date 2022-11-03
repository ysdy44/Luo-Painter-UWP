#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_SIMPLE

#include "d2d1effecthelpers.hlsli"

float threshold; // 0~3
float4 color0; 
float4 color1;

D2D_PS_ENTRY(main) 
{
	float3 color = D2DGetInput(0).rgb;

	float gray= color.r+color.g+color.b;

	if(gray<=threshold)
		return color1;
	
	return color0;
}