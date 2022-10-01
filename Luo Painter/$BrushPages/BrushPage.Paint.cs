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
                case InkType.Brush_Dry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Opacity:
                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Blend:
                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Brush_WetBlur_Blur:
                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.Brush_Wet_Pattern_Mix:
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

                case InkType.Brush_Dry_Mix:
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

                case InkType.MaskBrush_Dry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.MaskBrush_Dry_Mix:
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

                case InkType.MaskBrush_Wet_Pattern_Mix:
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

                case InkType.Circle_Dry:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Opacity:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Circle_Dry_Mix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.Circle_Wet_Pattern_Mix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Line_Dry:
                    this.BitmapLayer.DrawLine(this.Position, position, this.Color, this.InkPresenter.Size, BitmapType.Source);
                    return true;

                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Line_WetComposite_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetBlur_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                case InkType.Line_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    this.BitmapLayer.DrawLine(this.Position, position, this.Color, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.Line_Dry_Mix:
                    this.BitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Source);
                    return true;

                case InkType.Line_Wet_Pattern_Mix:
                    this.BitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.Erase_Dry:
                    return this.BitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Erase_WetComposite_Opacity:
                case InkType.Erase_WetComposite_Pattern_Opacity:
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