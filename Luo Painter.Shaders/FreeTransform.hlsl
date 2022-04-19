#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float4x4 matrix4;
float2 distce;
float left;
float top;
float right;
float bottom;

float2 transform(float2 p, float4x4 mat)
{
    float x = mat[0][0] * p.x + mat[1][0] * p.y + mat[3][0];
    float y = mat[0][1] * p.x + mat[1][1] * p.y + mat[3][1];
    return float2(x, y);
}

D2D_PS_ENTRY(main)
{

	// 当前位置
float2 p = D2DGetScenePosition().xy;

float2 up = transform(p, matrix4) / (1 + dot(p, distce));
	if (up.x >= left && up.x <= right && up.y >=top && up.y <= bottom)
	{
float4 color = D2DSampleInputAtPosition(0, up).rgba;
		return
color;
	}
	return float4(0, 0, 0, 0);
}