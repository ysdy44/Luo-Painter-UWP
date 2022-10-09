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
                    this.BitmapLayer.Draw(this.BitmapLayer[BitmapType.Temp]);
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
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(opacity))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(opacity);
                    }
                    break;

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blend))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;

                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], opacity))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blend))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;

                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(blur);
                    }
                    break;

                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blur))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                    using (AlphaMaskEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(mosaic);
                    }
                    break;

                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    using (AlphaMaskEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(mosaic))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Erase_Dry:
                case InkType.Erase_WetComposite_Opacity:
                    using (ArithmeticCompositeEffect erase = this.InkPresenter.GetErase(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(erase);
                    }
                    break;

                case InkType.Liquefy:
                    break;

                default:
                    break;
            }
        }


        private void Paint(Stroke stroke, StrokeSegment segment)
        {
            if (this.InkType is InkType.Liquefy)
            {
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
                return;
            }

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                switch (this.InkType)
                {
                    case InkType.Brush_Dry:
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
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr, stroke, segment);
                        }
                        break;

                    case InkType.Brush_Wet_Pattern_Mix:
                    case InkType.Brush_Dry_Mix:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr, stroke, segment);
                        }
                        break;

                    case InkType.MaskBrush_Dry:
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
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr,
                            this.InkPresenter.Mask,
                            this.InkPresenter.Rotate, stroke, segment);
                        }
                        break;

                    case InkType.MaskBrush_Dry_Mix:
                    case InkType.MaskBrush_Wet_Pattern_Mix:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr,
                            this.InkPresenter.Mask,
                            this.InkPresenter.Rotate, stroke, segment);
                        }
                        break;

                    case InkType.Circle_Dry:
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
                        segment.IsometricFillCircle(ds, this.Color, stroke, segment);
                        break;

                    case InkType.Circle_Dry_Mix:
                    case InkType.Circle_Wet_Pattern_Mix:
                        segment.IsometricFillCircle(ds, this.InkMixer.Color, stroke, segment);
                        break;

                    case InkType.Line_Dry:
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
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                        break;

                    case InkType.Line_Dry_Mix:
                    case InkType.Line_Wet_Pattern_Mix:
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                        break;

                    case InkType.Erase_Dry:
                    case InkType.Erase_WetComposite_Opacity:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, Vector4.One, stroke, segment);
                        }
                        break;

                    case InkType.Liquefy:
                        break;

                    default:
                        break;
                }
            }
        }

    }
}