#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"

int hardness;
bool rotate;
float2 normalization;

float4 color;
float pressure;

float radius;
float2 targetPosition;

float Cosine(float value)
{
    return cos(value * 3.14159274) / 2 + 0.5;
}

float GetRadiusAlpha(float x)
{
    // Cosine
    if(hardness == 1) return Cosine(x);
    
    // Quadratic
    if(hardness == 2) return  Cosine(x*x);
    
    // Cube
    if(hardness == 3) return  Cosine(x*x*x);
    
    // Quartic
    if(hardness == 4) return  Cosine(x*x*x*x);

    // None
    return 1;
}

float GetTextureAlpha(float2 p)
{
    // Vector : -1~0~1
    float vectX = (p.x - targetPosition.x) / radius;
    float vectY = (p.y - targetPosition.y) / radius;
    
    if (rotate)
    {
        // Rotate: x <-> y
        float sin2 = normalization.x;
        float cos2 = normalization.y;

        // Vector : -1~0~1
        float vectX2 = vectX * cos2 - vectY * sin2;
        float vectY2 = vectX * sin2 + vectY * cos2;

        // Sample : 0~0.5~1
        float2 sample = float2(vectX2 / 2 + 0.5, vectY2 / 2 + 0.5);
        return D2DSampleInput(0, sample).a;
    }
    else
    {
        // Sample : 0~0.5~1
        float2 sample = float2(vectX / 2 + 0.5, vectY / 2 + 0.5);
        return D2DSampleInput(0, sample).a;
    }
}

D2D_PS_ENTRY(main)
{
    float2 p = D2DGetScenePosition().xy;

    // In-Radius
    float dist = distance(targetPosition, p);
	if (dist > radius) return float4(0, 0, 0, 0);

    // Alpha : 0~1
    float alpha = GetTextureAlpha(p);
    if(alpha == 0.0) return float4(0, 0, 0, 0);

    // None
    if(hardness == 0) return float4(color.r, color.g, color.b, alpha * pressure);

    // Alpha : 0~1
    float a = alpha * pressure * GetRadiusAlpha(dist / radius);
    return float4(color.r*a, color.g*a, color.b*a, a);
}