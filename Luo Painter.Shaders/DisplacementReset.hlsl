#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

float radius;
float2 targetPosition;

D2D_PS_ENTRY(main)
{
    float2 p = D2DGetScenePosition().xy;
    float4 color = D2DGetInput(0);

    // In-Radius
    float dist = distance(targetPosition, p);
	if (radius < dist) return color;

    float opacityR = dist / radius;
    float opacity = 1 - opacityR;
    float opacityHalf = opacity * 0.5f;

    // Red & Green
    color.r = opacityHalf + opacityR * color.r;
    color.g = opacityHalf + opacityR * color.g;

	return color;
}