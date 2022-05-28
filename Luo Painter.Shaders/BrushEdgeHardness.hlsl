#define D2D_INPUT_COUNT 0
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

int hardness;

float4 color;
float pressure;

float radius;
float2 targetPosition;

float Cosine(float value)
{
    return cos(value * 3.14159274) / 2 + 0.5;
}

D2D_PS_ENTRY(main)
{
    float2 p = D2DGetScenePosition().xy;

    // In-Radius
    float dist = distance(targetPosition, p);
	if (dist > radius) return float4(0, 0, 0, 0);

    // None
    if(hardness == 0) return float4(color.r, color.g, color.b, pressure);

    float x = dist / radius;
    
    // Cosine
    if(hardness == 1) return float4(color.r, color.g, color.b, pressure * Cosine(x));
    
    // Quadratic
    if(hardness == 2) return float4(color.r, color.g, color.b, pressure * Cosine(x*x));
    
    // Cube
    if(hardness == 3) return float4(color.r, color.g, color.b, pressure * Cosine(x*x*x));
    
    // Quartic
    if(hardness == 4) return float4(color.r, color.g, color.b, pressure * Cosine(x*x*x*x));

    // None
    return float4(color.r, color.g, color.b, pressure);
}