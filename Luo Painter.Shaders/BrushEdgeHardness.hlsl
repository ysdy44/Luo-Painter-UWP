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

D2D_PS_ENTRY(main)
{
    float2 p = D2DGetScenePosition().xy;
	
    // In-Radius
    float dist = distance(targetPosition, p);
	if (dist > radius) return float4(0, 0, 0, 0);

    // None
    if(hardness == 0) 
    {
        // Alpha : 0~1
        float alpha1 = smoothstep(0, 1, color.a * pressure);
        return float4(color.r * alpha1, color.g * alpha1, color.b * alpha1, alpha1);
    }
    else
    {
        // Alpha : 0~1
        float alpha2 = smoothstep(0, 1, color.a * pressure * GetRadiusAlpha(dist / radius));
        return float4(color.r * alpha2, color.g * alpha2, color.b * alpha2, alpha2);
    }    
}