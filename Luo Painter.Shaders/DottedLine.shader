#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float time;

D2D_PS_ENTRY(main)
{
	////获取当前颜色
	float4 col = D2DGetInput(0);

	if (col.a == 0) {
		return float4(0, 0, 0, 0);
	}

	//上面
	float ta = D2DSampleInputAtOffset(0, float2(0, -1)).a;
	//下面
	float ba = D2DSampleInputAtOffset(0, float2(0, 1)).a;
	//左边
	float la = D2DSampleInputAtOffset(0, float2(-1, 0)).a;
	float ra = D2DSampleInputAtOffset(0, float2(1, 0)).a;

	if (ta == 0 || ba == 0 || la == 0 || ra == 0) {
		//绘制蚂蚁线
		float2 p = D2DGetInputCoordinate(0).xy;
		float scale = time * 100;
		float3 dottedLine = lerp(float3(0, 0, 0), float3(1, 1, 1), fmod(ceil(scale + p.x * 100) + ceil(scale + p.y * 100), 2));
		return float4(dottedLine, col.a);
	}

	return col;
}