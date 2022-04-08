#define D2D_INPUT_COUNT 0
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"


float4 color;
float size;
D2D_PS_ENTRY(main)
{
	float radius = size / 2;
	float2 center = float2(radius, radius);
	// 当前位置
	float2 cPoint = D2DGetScenePosition().xy;
	// 计算当前点到目标点距离
	float dist = distance(center, cPoint);
	if (dist < radius) {
		float m = 1- (dist / radius);
		float a = smoothstep(0, 0.5, m)*0.5;
		return float4(color.r*a,color.g*a,color.b*a, a);
	}
	else {
		return float4(0, 0, 0, 0);
	}

}