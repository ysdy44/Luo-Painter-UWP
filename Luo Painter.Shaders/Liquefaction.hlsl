#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"


// 半径
float radius;
// 开始位置
float2 position;
// 目标位置
float2 targetPosition;

float pressure;

D2D_PS_ENTRY(main)
{
	// 当前位置
float4 cPoint = D2DGetInputCoordinate(0);

	// 计算当前点到目标点距离
float dist = distance(targetPosition, cPoint);

	if (radius < dist) return D2DGetInput(0);

	//比例因子
float scaleFactor = 1 - (dist / radius);
	scaleFactor *=
pressure;

float2 dPos = targetPosition - position;
float2 pos = cPoint - mul(scaleFactor, dPos);
	return D2DSampleInput(0, pos);
}