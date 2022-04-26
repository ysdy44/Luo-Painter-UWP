#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_SIMPLE

#include "d2d1effecthelpers.hlsli"

float4 matchColor;
float threshold;

D2D_PS_ENTRY(main)
{
	float4 sColor = D2DGetInput(0);
	if (abs(sColor.r - matchColor.r) <= threshold && abs(sColor.g - matchColor.g) <= threshold && abs(sColor.b - matchColor.b) <= threshold && abs(sColor.a - matchColor.a) <= threshold)
	{
		return sColor;
	}
	else
	{
		return float4(0,0,0,0);
	}
}
