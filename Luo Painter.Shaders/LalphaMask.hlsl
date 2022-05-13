//声明需要三张纹理图
#define D2D_INPUT_COUNT 3
//原图纹理采样为简单模式
#define D2D_INPUT0_SAMPLE
//选区纹理采样为简单模式
#define D2D_INPUT1_SAMPLE
//效果图纹理采样为简单模式
#define D2D_INPUT2_SAMPLE
#include "d2d1effecthelpers.hlsli"


D2D_PS_ENTRY(main) {
//获取选区当前像素的颜色
float4 area = D2DGetInput(0);
//判断选区是否有颜色，如果有则返回效果图像素，否则返回原图像素
if(area.a>0){
return D2DGetInput(1);
}
return D2DGetInput(2);

}