#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_SIMPLE

#include "d2d1effecthelpers.hlsli"

float threshold;


float getGray(in float3 color){
	float gray = (color.r+color.g+color.b)/3;
	return gray;
}

D2D_PS_ENTRY(main) {

	float3 color = D2DGetInput(0).rgb;
	float gray= getGray(color);
	if(gray<=threshold){
		return float4(0,0,0,1);
	}
	
	return float4(1, 1, 1, 1);
}




