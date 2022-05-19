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

    // Red & Green
    p.x += (color.r-0.5) * 1000;
    p.y += (color.g-0.5) * 1000;
    dist = distance(targetPosition, p);

    float opacity = 1 - dist / radius;

    // Unit Vector
    float vectX = p.x - targetPosition.x;
    float vectY = p.y - targetPosition.y;

    float angle = 3.141592654f / 12 * opacity;
    float sin2 = sin(angle);
    float cos2 = cos(angle);

    float vectXR = vectX * cos2 - vectY * sin2;
    float vectYR = vectX * sin2 + vectY * cos2;

    float vectorUnitX = vectXR + targetPosition.x - p.x;
    float vectorUnitY = vectYR + targetPosition.y - p.y;

    // Red & Green
    color.r -= 0.001f * vectorUnitX;
    color.g -= 0.001f * vectorUnitY;

	return color;
}