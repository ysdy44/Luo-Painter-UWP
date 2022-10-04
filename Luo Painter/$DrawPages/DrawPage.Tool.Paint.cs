using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        int MixX = -1;
        int MixY = -1;


        private void Paint_Start()
        {
            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.Position);

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Paint_Delta(Vector2 position, Vector2 point, float pressure)
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            Rect rect = position.GetRect(this.InkPresenter.Size);
            this.BitmapLayer.Hit(rect);

            if (this.Paint(this.BitmapLayer, position, pressure) is false) return;
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(position, this.InkPresenter.Opacity);

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.Position = position;
            this.Point = point;
            this.Pressure = pressure;
        }

        private void Paint_Complete()
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            if (this.Paint() is false) return;
            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private bool Paint()
        {
            if (this.InkType.HasFlag(InkType.Dry)) return true;
            else if (this.InkType.HasFlag(InkType.Wet)) { this.BitmapLayer.Draw(this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetBlur)) { this.BitmapLayer.Draw(this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetMosaic)) { this.BitmapLayer.Draw(this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetComposite)) { this.BitmapLayer.DrawCopy(this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Origin], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp]))); return true; }
            else return false;
        }

        private bool Paint(BitmapLayer bitmapLayer, Vector2 position, float pressure)
        {
            switch (this.InkType)
            {
                case InkType.Brush_Dry:
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
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
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.Brush_Wet_Pattern_Mix:
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardness(
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
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardness(
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
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
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
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size),
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        BitmapType.Temp);

                case InkType.MaskBrush_Dry_Mix:
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
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
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
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
                    return bitmapLayer.IsometricFillCircle(
                        this.ColorMenu.Color,
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
                    return bitmapLayer.IsometricFillCircle(
                        this.ColorMenu.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Circle_Dry_Mix:
                    return bitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Source);

                case InkType.Circle_Wet_Pattern_Mix:
                    return bitmapLayer.IsometricFillCircle(
                        this.InkMixer.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        BitmapType.Temp);

                case InkType.Line_Dry:
                    bitmapLayer.DrawLine(this.Position, position, this.ColorMenu.Color, this.InkPresenter.Size, BitmapType.Source);
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
                    bitmapLayer.DrawLine(this.Position, position, this.ColorMenu.Color, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.Line_Dry_Mix:
                    bitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Source);
                    return true;

                case InkType.Line_Wet_Pattern_Mix:
                    bitmapLayer.DrawLine(this.Position, position, this.InkMixer.Color, this.InkPresenter.Size, BitmapType.Temp);
                    return true;

                case InkType.Erase_Dry:
                    return bitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Erase_WetComposite_Opacity:
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    return bitmapLayer.IsometricErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

                case InkType.Liquefy:
                    bitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = bitmapLayer[BitmapType.Source],
                        Properties =
                        {
                            ["radius"] = bitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                            ["position"] = bitmapLayer .ConvertValueToOne(this.Position),
                            ["targetPosition"] = bitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size));
                    return true;

                default:
                    return false;
            }
        }

        private bool CacheMix(Vector2 position)
        {
            this.MixX = -1;
            this.MixY = -1;

            // 1. Get Position and Target
            int px = (int)position.X;
            int py = (int)position.Y;
            if (this.BitmapLayer.Contains(px, py) is false) return false;

            this.MixX = px;
            this.MixY = py;

            Color target = this.BitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

            // 2. Cache TargetColor with Color
            return this.InkMixer.Cache(target);
        }
        private bool CacheMixAll(Vector2 position)
        {
            this.MixX = -1;
            this.MixY = -1;

            foreach (ILayer item in this.ObservableCollection)
            {
                if (item.Type is LayerType.Bitmap is false) continue;

                if (item is BitmapLayer bitmapLayer)
                {
                    // 1. Get Position and Target
                    int px = (int)position.X;
                    int py = (int)position.Y;
                    if (bitmapLayer.Contains(px, py) is false) continue;

                    this.MixX = px;
                    this.MixY = py;

                    Color target = bitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

                    // 2. Cache TargetColor with Color
                    if (target.A is byte.MaxValue) return this.InkMixer.Cache(target);
                }
            }

            return false;
        }

        private bool Mix(Vector2 position, float opacity)
        {
            // 1. Get Position and Target
            int px = (int)position.X;
            int py = (int)position.Y;
            if (this.BitmapLayer.Contains(px, py) is false) return false;

            if (this.MixX == px && this.MixY == py) return false;
            this.MixX = px;
            this.MixY = py;

            Color target = this.BitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

            // 2. Blend TargetColor with Color
            return this.InkMixer.Mix(target, opacity);
        }

        private bool MixAll(Vector2 position, float opacity)
        {
            foreach (ILayer item in this.ObservableCollection)
            {
                if (item.Type is LayerType.Bitmap is false) continue;

                if (item is BitmapLayer bitmapLayer)
                {
                    // 1. Get Position and Target
                    int px = (int)position.X;
                    int py = (int)position.Y;
                    if (bitmapLayer.Contains(px, py) is false) return false;

                    if (this.MixX == px && this.MixY == py) return false;
                    this.MixX = px;
                    this.MixY = py;

                    Color target = bitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

                    // 2. Blend TargetColor with Color
                    if (target.A is byte.MaxValue) return this.InkMixer.Mix(target, opacity);
                }
            }

            return false;
        }

    }
}