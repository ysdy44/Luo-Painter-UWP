using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        private void Paint(CanvasDrawingSession ds)
        {
            switch (this.InkType)
            {
                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Pattern_Mix:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Pattern_Mix:
                    using (AlphaMaskEffect wet = this.InkPresenter.GetPattern(this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(wet);
                    }
                    break;
                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    using (AlphaMaskEffect wet = this.InkPresenter.GetPattern(this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Source], wet))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(mosaic);
                    }
                    break;
                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(composite);
                    }
                    break;

                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    using (AlphaMaskEffect wet = this.InkPresenter.GetPattern(this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Source], wet))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(blur);
                    }
                    break;
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    using (AlphaMaskEffect wet = this.InkPresenter.GetPattern(this.BitmapLayer[BitmapType.Temp]))
                    using (ArithmeticCompositeEffect composite = this.InkPresenter.GetErase(this.BitmapLayer[BitmapType.Source], wet))
                    {
                        ds.DrawImage(composite);
                    }
                    break;

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(wet);
                    }
                    break;

                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(this.BitmapLayer[BitmapType.Temp]))
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(pattern))
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Source], wet))
                    {
                        ds.DrawImage(composite);
                    }
                    break;

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(composite);
                    }
                    break;
                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(blur);
                    }
                    break;
                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                    using (AlphaMaskEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        ds.DrawImage(mosaic);
                    }
                    break;
                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Source], wet))
                    {
                        ds.DrawImage(composite);
                    }
                    break;
                case InkType.Erase_WetComposite_Opacity:
                    using (ArithmeticCompositeEffect composite = this.InkPresenter.GetErase(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]))
                    {
                        ds.DrawImage(composite);
                    }
                    break;

                default:
                    ds.DrawImage(this.BitmapLayer[BitmapType.Source]);
                    break;
            }
        }


        private void Paint()
        {
            switch (this.InkType)
            {
                case InkType.Brush_Dry:
                case InkType.Brush_Dry_Mix:
                case InkType.MaskBrush_Dry:
                case InkType.MaskBrush_Dry_Mix:
                case InkType.Circle_Dry:
                case InkType.Circle_Dry_Mix:
                case InkType.Line_Dry:
                case InkType.Line_Dry_Mix:
                case InkType.Erase_Dry:
                    break;

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(wet);
                    }
                    break;
                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Circle_Wet_Pattern_Mix:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Line_Wet_Pattern_Mix:
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(wet))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                    using (ICanvasImage blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(blur);
                    }
                    break;
                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    using (ICanvasImage blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(blur))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                    using (ICanvasImage mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(mosaic);
                    }
                    break;
                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    using (ICanvasImage mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(mosaic))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(composite);
                    }
                    break;
                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(composite);
                    }
                    break;
                case InkType.Erase_WetComposite_Opacity:
                    using (ICanvasImage composite = this.InkPresenter.GetErase(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(composite);
                    }
                    break;

                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(composite))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    using (ICanvasImage composite = this.InkPresenter.GetErase(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(composite))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    using (OpacityEffect wet = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (BlendEffect composite = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], wet))
                    using (ICanvasImage pattern = this.InkPresenter.GetPattern(composite))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;

                default:
                    break;
            }
        }


        private bool Paint(Stroke stroke)
        {
            switch (this.InkType)
            {
                case InkType.Brush_Dry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorHdr,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
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
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorHdr,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.Brush_Wet_Pattern_Mix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.Brush_Dry_Mix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrush_Dry:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
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
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.MaskBrush_Dry_Mix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Source);

                case InkType.MaskBrush_Wet_Pattern_Mix:
                    return this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.Circle_Dry:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.Color,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
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
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Circle_Dry_Mix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.Circle_Wet_Pattern_Mix:
                    return this.BitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        stroke.StartingPosition, stroke.Position,
                        stroke.StartingPressure, stroke.Pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Line_Dry:
                    this.BitmapLayer.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure, BitmapType.Source);
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
                    this.BitmapLayer.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure, BitmapType.Temp);
                    return true;

                case InkType.Line_Dry_Mix:
                    this.BitmapLayer.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure, BitmapType.Source);
                    return true;

                case InkType.Line_Wet_Pattern_Mix:
                    this.BitmapLayer.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure, BitmapType.Temp);
                    return true;

                case InkType.Erase_Dry:
                    return this.BitmapLayer.IsometricErasingDry(stroke.StartingPosition, stroke.Position, stroke.StartingPressure, stroke.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Erase_WetComposite_Opacity:
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    return this.BitmapLayer.IsometricErasingWet(stroke.StartingPosition, stroke.Position, stroke.StartingPressure, stroke.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Liquefy:
                    this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = this.BitmapLayer[BitmapType.Source],
                        Properties =
                        {
                            ["radius"] = this.BitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                            ["position"] = this.BitmapLayer .ConvertValueToOne(stroke.StartingPosition),
                            ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(stroke.Position),
                            ["pressure"] = stroke.Pressure,
                        }
                    }, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size));
                    return true;

                default:
                    return false;
            }
        }

    }
}