#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float time;

float3 color1 = float3(0, 0, 0);
float3 color2 = float3(1, 1, 1);
float lineWidth = 1.5;
float lineGap = 100;
float lineSpeed = 200;
D2D_PS_ENTRY(main)
{
	////获取当前颜色
	float4 col = D2DGetInput(0);

	if (col.a != 0) {
		return col;
	}
	float2 p = D2DGetInputCoordinate(0).xy;
	//获取像素和像素直接的距离并乘以一个系数来控制线的宽度
	float w = fwidth(p.x) * lineWidth;
	float h = fwidth(p.y) * lineWidth;

	//上面
	float ta = D2DSampleInput(0, p + float2(0, -h)).a;
	//下面
	float ba = D2DSampleInput(0, p + float2(0, h)).a;
	//左边
	float la = D2DSampleInput(0, p + float2(-w, 0)).a;
	float ra = D2DSampleInput(0, p + float2(w, 0)).a;
	float scale = time * lineSpeed;
	if (ta != 0 || ba != 0 || la != 0 || ra != 0) {
		//绘制蚂蚁线
		float2 p = D2DGetInputCoordinate(0).xy;
		float3 dottedLine = lerp(color1, color2, fmod(ceil(scale + p.x * lineGap) + ceil(scale + p.y * lineGap), 2));
		return float4(dottedLine, 1);
	}
	return float4(0, 0, 0, 0);
}