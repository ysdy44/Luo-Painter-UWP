using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
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
                case InkType.ShapeBrush:
                case InkType.ShapeBrush_Mix:
                case InkType.Tip:
                case InkType.Tip_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    this.BitmapLayer.Draw(this.BitmapLayer[BitmapType.Temp]);
                    break;

                case InkType.Brush_Grain:
                case InkType.Brush_Grain_Opacity:
                case InkType.Brush_Grain_Mix:
                case InkType.ShapeBrush_Grain:
                case InkType.ShapeBrush_Grain_Opacity:
                case InkType.ShapeBrush_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Tip_Grain_Opacity:
                case InkType.Tip_Grain_Mix:
                case InkType.Line_Grain:
                case InkType.Line_Grain_Opacity:
                case InkType.Line_Grain_Mix:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect grain = this.InkPresenter.GetGrain(opacity))
                    {
                        this.BitmapLayer.Draw(grain);
                    }
                    break;

                case InkType.Brush_Opacity:
                case InkType.ShapeBrush_Opacity:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(opacity);
                    }
                    break;

                case InkType.Brush_Blend:
                case InkType.ShapeBrush_Blend:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_Grain_Blend:
                case InkType.ShapeBrush_Grain_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    using (AlphaMaskEffect grain = this.InkPresenter.GetGrain(blend))
                    {
                        this.BitmapLayer.DrawCopy(grain);
                    }
                    break;

                case InkType.Brush_Opacity_Blend:
                case InkType.ShapeBrush_Opacity_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.DrawCopy(blend);
                    }
                    break;

                case InkType.Brush_Grain_Opacity_Blend:
                case InkType.ShapeBrush_Grain_Opacity_Blend:
                case InkType.Tip_Grain_Opacity_Blend:
                case InkType.Line_Grain_Opacity_Blend:
                    using (OpacityEffect opacity = this.InkPresenter.GetOpacity(this.BitmapLayer[BitmapType.Temp]))
                    using (BlendEffect blend = this.InkPresenter.GetBlend(this.BitmapLayer[BitmapType.Origin], opacity))
                    using (AlphaMaskEffect grain = this.InkPresenter.GetGrain(blend))
                    {
                        this.BitmapLayer.DrawCopy(grain);
                    }
                    break;

                case InkType.Blur:
                    using (AlphaMaskEffect blur = this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(blur);
                    }
                    break;

                case InkType.Mosaic:
                    using (ScaleEffect mosaic = this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]))
                    {
                        this.BitmapLayer.Draw(mosaic);
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


        private void Paint(StrokeSegment segment)
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
                        ["position"] = this.BitmapLayer .ConvertValueToOne(segment.StartingPosition),
                        ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(segment.Position),
                        ["pressure"] = segment.Pressure,
                    }
                }, segment.Bounds);
                return;
            }

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                switch (this.InkType)
                {
                    case InkType.Brush:
                    case InkType.Brush_Grain:
                    case InkType.Brush_Opacity:
                    case InkType.Brush_Grain_Opacity:
                    case InkType.Brush_Blend:
                    case InkType.Brush_Grain_Blend:
                    case InkType.Brush_Opacity_Blend:
                    case InkType.Brush_Grain_Opacity_Blend:
                    case InkType.Blur:
                        using (ds.CreateLayer(1f, segment.Bounds))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                        }
                        break;

                    case InkType.Brush_Grain_Mix:
                    case InkType.Brush_Mix:
                        using (ds.CreateLayer(1f, segment.Bounds))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, this.InkMixer.ColorHdr, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                        }
                        break;

                    case InkType.ShapeBrush:
                    case InkType.ShapeBrush_Grain:
                    case InkType.ShapeBrush_Opacity:
                    case InkType.ShapeBrush_Grain_Opacity:
                    case InkType.ShapeBrush_Blend:
                    case InkType.ShapeBrush_Grain_Blend:
                    case InkType.ShapeBrush_Opacity_Blend:
                    case InkType.ShapeBrush_Grain_Opacity_Blend:
                        using (ds.CreateLayer(1f, segment.Bounds))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkPresenter.ShapeSource, this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                        }
                        break;

                    case InkType.ShapeBrush_Mix:
                    case InkType.ShapeBrush_Grain_Mix:
                        using (ds.CreateLayer(1f, segment.Bounds))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.InkMixer.ColorHdr, this.InkPresenter.ShapeSource, this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                        }
                        break;

                    case InkType.Tip:
                    case InkType.Tip_Grain:
                    case InkType.Tip_Opacity:
                    case InkType.Tip_Grain_Opacity:
                    case InkType.Tip_Blend:
                    case InkType.Tip_Grain_Blend:
                    case InkType.Tip_Opacity_Blend:
                    case InkType.Tip_Grain_Opacity_Blend:
                        segment.IsometricTip(ds, this.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke);
                        break;

                    case InkType.Tip_Mix:
                    case InkType.Tip_Grain_Mix:
                        segment.IsometricTip(ds, this.InkMixer.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke);
                        break;

                    case InkType.Line:
                    case InkType.Line_Grain:
                    case InkType.Line_Opacity:
                    case InkType.Line_Grain_Opacity:
                    case InkType.Line_Blend:
                    case InkType.Line_Grain_Blend:
                    case InkType.Line_Opacity_Blend:
                    case InkType.Line_Grain_Opacity_Blend:
                    case InkType.Mosaic:
                        segment.DrawLine(ds, this.Color, this.InkPresenter.IgnoreSizePressure);
                        break;

                    case InkType.Line_Mix:
                    case InkType.Line_Grain_Mix:
                        segment.DrawLine(ds, this.InkMixer.Color, this.InkPresenter.IgnoreSizePressure);
                        break;

                    case InkType.Erase:
                    case InkType.Erase_Opacity:
                        using (ds.CreateLayer(1f, segment.Bounds))
                        {
                            segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, Vector4.One, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
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