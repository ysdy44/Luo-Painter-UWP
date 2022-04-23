#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SIMPLE
#define D2D_INPUT1_COMPLEX
//#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"



float getGray(in float3 color) {
	float gray = (color.r* 587 + color.g *144 +color.b*269)/1000;
	return gray;
}

D2D_PS_ENTRY(main) {

	float4 color0 = D2DGetInput(0).rgba;
    float4 color1 = D2DGetInput(1).rgba;
    if (color0.a != color1.a) return float4(0, 0, 0, 1);
    if (color0.r != color1.r) return float4(0, 0, 0, 1);
    if (color0.g != color1.g) return float4(0, 0, 0, 1);
    if (color0.b != color1.b) return float4(0, 0, 0, 1);
    return float4(0, 0, 0, 0);
}

