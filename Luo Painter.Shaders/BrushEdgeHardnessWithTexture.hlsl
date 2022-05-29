#define D2D_INPUT_COUNT 1
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

    // Texture : 0~1
    float textureX = (p.x - targetPosition.x) / radius / 2 + 0.5;
    float textureY = (p.y - targetPosition.y) / radius / 2 + 0.5;
    float alpha = D2DSampleInput(0, float2(textureX, textureY)).a;
    if(alpha == 0.0) return float4(0, 0, 0, 0);

    // None
    if(hardness == 0) return float4(color.r, color.g, color.b, alpha * pressure);

    float x = dist / radius;
    
    // Cosine
    if(hardness == 1) return float4(color.r*(alpha * pressure * Cosine(x)), color.g*(alpha * pressure * Cosine(x)), color.b*(alpha * pressure * Cosine(x)), alpha * pressure * Cosine(x));
    
    // Quadratic
    if(hardness == 2) return float4(color.r*(alpha * pressure *  Cosine(x*x)), color.g*(alpha * pressure *  Cosine(x*x)), color.b*(alpha * pressure *  Cosine(x*x)), alpha * pressure * Cosine(x*x));
    
    // Cube
    if(hardness == 3) return float4(color.r*(alpha * pressure *  Cosine(x*x*x)), color.g*(alpha * pressure *  Cosine(x*x*x)), color.b*(alpha * pressure *  Cosine(x*x*x)), alpha * pressure * Cosine(x*x*x));
    
    // Quartic
    if(hardness == 4) return float4(color.r*(alpha * pressure *  Cosine(x*x*x*x)), color.g*(alpha * pressure *  Cosine(x*x*x*x)), color.b*(alpha * pressure *  Cosine(x*x*x*x)), alpha * pressure * Cosine(x*x*x*x));

    // None
    return float4(color.r*(alpha * pressure ), color.g*(alpha * pressure ), color.b*(alpha * pressure ), alpha * pressure);
}