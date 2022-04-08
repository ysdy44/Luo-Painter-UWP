#define D2D_INPUT_COUNT 1
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
	////0.5减去距离，如果小于0则丢弃当前像素，不绘制任何颜色；
	//clip(radius - dist);
	if (dist < radius) {
		return float4(color.r, color.g, color.b, 0.5);
	}
	return float4(0, 0, 0, 0);
}