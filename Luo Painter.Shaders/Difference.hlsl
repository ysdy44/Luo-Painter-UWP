#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SIMPLE
#define D2D_INPUT1_COMPLEX
//#define D2D_REQUIRES_SCENE_POSITION
#include "d2d1effecthelpers.hlsli"


// Windows.UI.Colors.DodgerBlue
// R 30/255
// G 144/255
// B 255/255
// A 255/255
float4 color = float4(0.11764705882352941176470588235294f, 0.56470588235294117647058823529412f, 1, 1);

float getGray(in float3 color2) {
    float gray = (color2.r * 587 + color2.g * 144 + color2.b * 269) / 1000;
	return gray;
}

D2D_PS_ENTRY(main) {

	float4 color0 = D2DGetInput(0).rgba;
    float4 color1 = D2DGetInput(1).rgba;
    if (color0.a != color1.a) return color;
    if (color0.r != color1.r) return color;
    if (color0.g != color1.g) return color;
    if (color0.b != color1.b) return color;
    return float4(0, 0, 0, 0);
}

