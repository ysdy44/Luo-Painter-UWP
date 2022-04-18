//声明有两张纹理
#define D2D_INPUT_COUNT 2
//声明主纹理为简单采样
#define D2D_INPUT0_SIMPLE
//第一第二张纹理为复杂采样
#define D2D_INPUT1_COMPLEX
//#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"



float getGray(in float3 color) {
	float gray = (color.r* 587 + color.g *144 +color.b*269)/1000;
	return gray;
}

D2D_PS_ENTRY(main) {

	//使用简单采样 采样第一张纹理的颜色
	float4 color = D2DGetInput(0).rgba;
    if (color.a == 0) return color;

	float gray = getGray(color);
	float2 xy = float2(gray, 0.5);
	float4 cc = D2DSampleInput(1, xy);
	return float4(cc.r, cc.g, cc.b, cc.a);
}

