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


        private void Paint(Stroke stroke, StrokeSegment segment)
        {
            switch (this.InkType)
            {
                case InkType.Brush_Dry:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr, stroke, segment);
                    }
                    break;

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
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr, stroke, segment);
                    }
                    break;

                case InkType.Brush_Wet_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr, stroke, segment);
                    }
                    break;

                case InkType.Brush_Dry_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr, stroke, segment);
                    }
                    break;

                case InkType.MaskBrush_Dry:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate, stroke, segment);
                    }
                    break;

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
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate, stroke, segment);
                    }
                    break;

                case InkType.MaskBrush_Dry_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate, stroke, segment);
                    }
                    break;

                case InkType.MaskBrush_Wet_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    using (ds.CreateLayer(1f, RectExtensions.GetRect(stroke.StartingPosition, stroke.Position, this.InkPresenter.Size)))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, (int)this.InkPresenter.Hardness, this.InkMixer.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate, stroke, segment);
                    }
                    break;

                case InkType.Circle_Dry:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricFillCircle(ds, this.Color, stroke, segment);
                    }
                    break;

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
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricFillCircle(ds, this.Color, stroke, segment);
                    }
                    break;

                case InkType.Circle_Dry_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricFillCircle(ds, this.InkMixer.Color, stroke, segment);
                    }
                    break;

                case InkType.Circle_Wet_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricFillCircle(ds, this.InkMixer.Color, stroke, segment);
                    }
                    break;

                case InkType.Line_Dry:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                    }
                    break;

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
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                    }
                    break;

                case InkType.Line_Dry_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                    }
                    break;

                case InkType.Line_Wet_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.DrawLine(stroke.StartingPosition, stroke.Position, this.InkMixer.Color, this.InkPresenter.Size * stroke.Pressure * 2, StrokeSegment.CanvasStrokeStyle);
                    }
                    break;

                case InkType.Erase_Dry:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Source))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Blend = CanvasBlend.Copy;
                        segment.IsometricFillCircle(ds, Colors.Transparent, stroke, segment);
                    }
                    break;

                case InkType.Erase_WetComposite_Opacity:
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        segment.IsometricFillCircle(ds, Colors.White, stroke, segment);
                    }
                    break;

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
                    break;

                default:
                    break;
            }
        }

    }
}