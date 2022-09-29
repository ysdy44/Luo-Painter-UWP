using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        private bool Paint(Vector2 position, float pressure)
        {
            switch (this.InkType)
            {
                case InkType.BrushDry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        Vector4.One,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.BrushWetPattern:
                case InkType.BrushWetOpacity:
                case InkType.BrushWetPatternOpacity:
                case InkType.BrushWetBlend:
                case InkType.BrushWetPatternBlend:
                case InkType.BrushWetOpacityBlend:
                case InkType.BrushWetPatternOpacityBlend:
                case InkType.BrushWetBlur:
                case InkType.BrushWetPatternBlur:
                case InkType.BrushWetMosaic:
                case InkType.BrushWetPatternMosaic:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        Vector4.One,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.BrushWetPatternMix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.BrushDryMix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrushDry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        Vector4.One,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrushWetPattern:
                case InkType.MaskBrushWetOpacity:
                case InkType.MaskBrushWetPatternOpacity:
                case InkType.MaskBrushWetBlend:
                case InkType.MaskBrushWetPatternBlend:
                case InkType.MaskBrushWetOpacityBlend:
                case InkType.MaskBrushWetPatternOpacityBlend:
                case InkType.MaskBrushWetBlur:
                case InkType.MaskBrushWetPatternBlur:
                case InkType.MaskBrushWetMosaic:
                case InkType.MaskBrushWetPatternMosaic:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        Vector4.One,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.MaskBrushDryMix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrushWetPatternMix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.CircleDry:
                    return this.BitmapLayer.IsometricFillCircle(
                        Colors.White,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.CircleWetPattern:
                case InkType.CircleWetOpacity:
                case InkType.CircleWetPatternOpacity:
                case InkType.CircleWetBlend:
                case InkType.CircleWetPatternBlend:
                case InkType.CircleWetOpacityBlend:
                case InkType.CircleWetPatternOpacityBlend:
                case InkType.CircleWetBlur:
                case InkType.CircleWetPatternBlur:
                case InkType.CircleWetMosaic:
                case InkType.CircleWetPatternMosaic:
                    return this.BitmapLayer.IsometricFillCircle(
                        Colors.White,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.CircleDryMix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.CircleWetPatternMix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.LineDry:
                    this.BitmapLayer.DrawLine(this.Position, position, Colors.White, this.InkPresenter.Size, BitmapType.Source);
                    return true;

                case InkType.LineWetPattern:
                case InkType.LineWetOpacity:
                case InkType.LineWetPatternOpacity:
                case InkType.LineWetBlend:
                case InkType.LineWetPatternBlend:
                case InkType.LineWetOpacityBlend:
                case InkType.LineWetPatternOpacityBlend:
                case InkType.LineWetBlur:
                case InkType.LineWetPatternBlur:
                case InkType.LineWetMosaic:
                case InkType.LineWetPatternMosaic:
                    this.BitmapLayer.DrawLine(this.Position, position, Colors.White, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.LineDryMix:
                    this.BitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Source);
                    return true;

                case InkType.LineWetPatternMix:
                    this.BitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.EraseDry:
                    return this.BitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.EraseWetOpacity:
                case InkType.EraseWetPatternOpacity:
                    return this.BitmapLayer.IsometricErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Liquefy:
                    this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = this.BitmapLayer[BitmapType.Source],
                        Properties =
                        {
                            ["radius"] = this.BitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                            ["position"] = this.BitmapLayer .ConvertValueToOne(this.Position),
                            ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size));
                    return true;

                default:
                    return false;
            }
        }

    }
}