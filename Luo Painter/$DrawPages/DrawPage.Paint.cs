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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void Paint()
        {
            switch (this.InkType)
            {
                case InkType.Brush:
                case InkType.Brush_Mix:
                case InkType.MaskBrush:
                case InkType.MaskBrush_Mix:
                case InkType.Circle:
                case InkType.Circle_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    this.BitmapLayer.Draw(this.BitmapLayer[BitmapType.Temp]);
                    break;

                case InkType.Brush_Pattern:
                case InkType.Brush_Pattern_Opacity:
                case InkType.Brush_Pattern_Mix:
                case InkType.MaskBrush_Pattern:
                case InkType.MaskBrush_Pattern_Opacity:
                case InkType.MaskBrush_Pattern_Mix:
                case InkType.Circle_Pattern:
                case InkType.Circle_Pattern_Opacity:
                case InkType.Circle_Pattern_Mix:
                case InkType.Line_Pattern:
                case InkType.Line_Pattern_Opacity:
                case InkType.Line_Pattern_Mix:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(opacity))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_Opacity:
                case InkType.MaskBrush_Opacity:
                case InkType.Circle_Opacity:
                case InkType.Line_Opacity:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(opacity);
                    }
                    break;

                case InkType.Brush_Blend:
                case InkType.MaskBrush_Blend:
                case InkType.Circle_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_Pattern_Blend:
                case InkType.MaskBrush_Pattern_Blend:
                case InkType.Circle_Pattern_Blend:
                case InkType.Line_Pattern_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blend))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;

                case InkType.Brush_Opacity_Blend:
                case InkType.MaskBrush_Opacity_Blend:
                case InkType.Circle_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_Pattern_Opacity_Blend:
                case InkType.MaskBrush_Pattern_Opacity_Blend:
                case InkType.Circle_Pattern_Opacity_Blend:
                case InkType.Line_Pattern_Opacity_Blend:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], opacity))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blend))
                    {
                        this.BitmapLayer.DrawCopy(pattern);
                    }
                    break;

                case InkType.Brush_Blur:
                case InkType.MaskBrush_Blur:
                case InkType.Circle_Blur:
                case InkType.Line_Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(blur);
                    }
                    break;

                case InkType.Brush_Pattern_Blur:
                case InkType.MaskBrush_Pattern_Blur:
                case InkType.Circle_Pattern_Blur:
                case InkType.Line_Pattern_Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(blur))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Brush_Mosaic:
                case InkType.MaskBrush_Mosaic:
                case InkType.Circle_Mosaic:
                case InkType.Line_Mosaic:
                    using (ScaleEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(mosaic);
                    }
                    break;

                case InkType.Brush_Pattern_Mosaic:
                case InkType.MaskBrush_Pattern_Mosaic:
                case InkType.Circle_Pattern_Mosaic:
                case InkType.Line_Pattern_Mosaic:
                    using (ScaleEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect pattern = this.InkPresenter.GetPattern(mosaic))
                    {
                        this.BitmapLayer.Draw(pattern);
                    }
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
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
                    case InkType.Brush:
                    case InkType.Brush_Pattern:
                    case InkType.Brush_Opacity:
                    case InkType.Brush_Pattern_Opacity:
                    case InkType.Brush_Blend:
                    case InkType.Brush_Pattern_Blend:
                    case InkType.Brush_Opacity_Blend:
                    case InkType.Brush_Pattern_Opacity_Blend:
                    case InkType.Brush_Blur:
                    case InkType.Brush_Pattern_Blur:
                    case InkType.Brush_Mosaic:
                    case InkType.Brush_Pattern_Mosaic:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr, stroke, segment);
                        }
                        break;

                    case InkType.Brush_Pattern_Mix:
                    case InkType.Brush_Mix:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr, stroke, segment);
                        }
                        break;

                    case InkType.MaskBrush:
                    case InkType.MaskBrush_Pattern:
                    case InkType.MaskBrush_Opacity:
                    case InkType.MaskBrush_Pattern_Opacity:
                    case InkType.MaskBrush_Blend:
                    case InkType.MaskBrush_Pattern_Blend:
                    case InkType.MaskBrush_Opacity_Blend:
                    case InkType.MaskBrush_Pattern_Opacity_Blend:
                    case InkType.MaskBrush_Blur:
                    case InkType.MaskBrush_Pattern_Blur:
                    case InkType.MaskBrush_Mosaic:
                    case InkType.MaskBrush_Pattern_Mosaic:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr,
                            this.InkPresenter.Mask,
                            this.InkPresenter.Rotate, stroke, segment);
                        }
                        break;

                    case InkType.MaskBrush_Mix:
                    case InkType.MaskBrush_Pattern_Mix:
                        using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr,
                            this.InkPresenter.Mask,
                            this.InkPresenter.Rotate, stroke, segment);
                        }
                        break;

                    case InkType.Circle:
                    case InkType.Circle_Pattern:
                    case InkType.Circle_Opacity:
                    case InkType.Circle_Pattern_Opacity:
                    case InkType.Circle_Blend:
                    case InkType.Circle_Pattern_Blend:
                    case InkType.Circle_Opacity_Blend:
                    case InkType.Circle_Pattern_Opacity_Blend:
                    case InkType.Circle_Blur:
                    case InkType.Circle_Pattern_Blur:
                    case InkType.Circle_Mosaic:
                    case InkType.Circle_Pattern_Mosaic:
                        segment.IsometricFillCircle(ds, this.Color, stroke, segment);
                        break;

                    case InkType.Circle_Mix:
                    case InkType.Circle_Pattern_Mix:
                        segment.IsometricFillCircle(ds, this.InkMixer.Color, stroke, segment);
                        break;

                    case InkType.Line:
                    case InkType.Line_Pattern:
                    case InkType.Line_Opacity:
                    case InkType.Line_Pattern_Opacity:
                    case InkType.Line_Blend:
                    case InkType.Line_Pattern_Blend:
                    case InkType.Line_Opacity_Blend:
                    case InkType.Line_Pattern_Opacity_Blend:
                    case InkType.Line_Blur:
                    case InkType.Line_Pattern_Blur:
                    case InkType.Line_Mosaic:
                    case InkType.Line_Pattern_Mosaic:
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                        break;

                    case InkType.Line_Mix:
                    case InkType.Line_Pattern_Mix:
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                        break;

                    case InkType.Erase:
                    case InkType.Erase_Opacity:
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