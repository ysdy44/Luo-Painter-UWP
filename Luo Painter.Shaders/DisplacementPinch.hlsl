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

    float opacity = 1 - dist / radius;

    // Unit Vector
    float2 vect = targetPosition - p;
    float lenght = distance(targetPosition, p);
    float2 vectorUnit = mul(vect, 1 / lenght);

    // Red & Green
    color.r -= 0.01f * opacity * vectorUnit.x;
    color.g -= 0.01f * opacity * vectorUnit.y;

	return color;
}