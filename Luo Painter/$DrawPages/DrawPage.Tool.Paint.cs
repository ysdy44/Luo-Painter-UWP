using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        Vector2 Point;
        Vector2 Position;
        float Pressure;


        private InkType GetInkToolType(ToolType type)
        {
            switch (type)
            {
                case ToolType.PaintBrush: return (this.InkPresenter.AllowMask) ? InkType.MaskBrushDry : InkType.BrushDry;
                case ToolType.PaintWatercolorPen: return InkType.CircleDry;
                case ToolType.PaintPencil: return InkType.LineDry;
                case ToolType.PaintEraseBrush: return InkType.EraseDry;
                case ToolType.PaintLiquefaction: return InkType.Liquefy;
                default: return InkType.None;
            }
        }
        private BitmapType GetBitmapType(InkType type)
        {
            if (type.HasFlag(InkType.Pattern))
                return BitmapType.Temp;
            else if (type.HasFlag(InkType.Opacity))
                return BitmapType.Temp;
            else if (type.HasFlag(InkType.BlendMode))
                return BitmapType.Temp;
            else
                return BitmapType.Source;
        }



        private void ConstructPaint()
        {
            this.PaintBrushTool.ItemClick += async (s, brush) =>
            {
                if (brush.Mask is PaintTexture mask)
                {
                    this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, mask.Source));
                    this.PaintMenu.SetMaskTexture(mask.Texture);
                }
                else this.InkPresenter.SetMask(false);

                if (brush.Pattern is PaintTexture pattern)
                {
                    this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, pattern.Source));
                    this.PaintMenu.SetPatternTexture(pattern.Texture);
                    this.PaintMenu.SetStep(pattern.Step);
                }
                else this.InkPresenter.SetPattern(false);

                this.InkPresenter.SetBrush(brush);
                this.PaintMenu.Construct(brush);
            };


            this.PaintMenu.SelectMask += async (s, e) => await this.SelectMask();
            this.PaintMenu.SelectPattern += async (s, e) => await this.SelectPattern();

            this.PaintMenu.MaskClosed += (s, e) => this.InkPresenter.SetMask(false);
            this.PaintMenu.PatternClosed += (s, e) => this.InkPresenter.SetPattern(false);

            this.PaintMenu.MaskOpened += async (s, e) =>
            {
                if (this.InkPresenter.Mask is null)
                {
                    bool result = await this.SelectMask();
                    if (result is false) this.PaintMenu.CloseMask();
                }
                else this.InkPresenter.SetMask(true);
            };
            this.PaintMenu.PatternOpened += async (s, e) =>
            {
                if (this.InkPresenter.Pattern is null)
                {
                    bool result = await this.SelectPattern();
                    if (result is false) this.PaintMenu.ClosePattern();
                }
                else this.InkPresenter.SetPattern(true);
            };
        }

        private async Task<bool> SelectMask()
        {
            this.TextureDialog.Construct(this.PaintMenu.MaskTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintMenu.SetMaskTexture(item.Texture);
                        return true;
                    }
                    else return false;
                default: return false;
            }
        }

        private async Task<bool> SelectPattern()
        {
            this.TextureDialog.Construct(this.PaintMenu.PatternTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintMenu.SetPatternTexture(item.Texture);
                        this.PaintMenu.SetStep(item.Step);
                        return true;
                    }
                    else return false;
                default: return false;
            }
        }


        private void Paint_Start(Vector2 point, float pressure)
        {
            this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.Point = point;
            this.Position = this.ToPosition(point);
            this.Pressure = pressure;

            this.InkType = this.InkPresenter.GetType(this.GetInkToolType(this.ToolType));
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Paint_Delta(Vector2 point, float pressure)
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            Vector2 position = this.ToPosition(point);

            Rect rect = this.Position.GetRect(this.InkPresenter.Size);
            this.BitmapLayer.Hit(rect);

            if (this.Paint(position, pressure) is false) return;

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.Point = point;
            this.Position = position;
            this.Pressure = pressure;
        }

        private void Paint_Complete(Vector2 point, float pressure)
        {
            this.Paint_Delta(point, pressure);
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            if (this.InkType.HasFlag(InkType.BlendMode))
                this.BitmapLayer.DrawCopy(this.InkPresenter.GetWetPreview(this.InkType, this.BitmapLayer.Temp, this.BitmapLayer.Origin));
            else if (this.InkType.HasFlag(InkType.Opacity) || this.InkType.HasFlag(InkType.Pattern))
                this.BitmapLayer.Draw(this.InkPresenter.GetWetPreview(this.InkType, this.BitmapLayer.Temp));

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.InkType = default;
            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private bool Paint(Vector2 position, float pressure)
        {
            if (this.InkType.HasFlag(InkType.BrushDry))
            {
                return this.BitmapLayer.IsometricShapeBrushEdgeHardness(
                    this.BrushEdgeHardnessShaderCodeBytes,
                    this.ColorMenu.ColorHdr,
                    this.Position, position,
                    this.Pressure, pressure,
                    this.InkPresenter.Size,
                    this.InkPresenter.Spacing,
                    (int)this.InkPresenter.Hardness,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.MaskBrushDry))
            {
                return this.BitmapLayer.IsometricShapeBrushEdgeHardnessWithTexture(
                    this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                    this.ColorMenu.ColorHdr,
                    this.InkPresenter.Mask,
                    this.InkPresenter.Rotate,
                    this.Position, position,
                    this.Pressure, pressure,
                    this.InkPresenter.Size,
                    this.InkPresenter.Spacing,
                    (int)this.InkPresenter.Hardness,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.CircleDry))
            {
                return this.BitmapLayer.IsometricFillCircle(
                    this.ColorMenu.Color,
                    this.Position, position,
                    this.Pressure, pressure,
                    this.InkPresenter.Size,
                    this.InkPresenter.Spacing,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.LineDry))
            {
                this.BitmapLayer.DrawLine(this.Position, position, this.ColorMenu.Color, this.InkPresenter.Size, this.GetBitmapType(this.InkType));
                return true;
            }
            else if (this.InkType.HasFlag(InkType.EraseDry))
            {
                if (this.InkType.HasFlag(InkType.Pattern) || this.InkType.HasFlag(InkType.Opacity))
                    return this.BitmapLayer.IsometricErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
                else
                    return this.BitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            }
            else if (this.InkType.HasFlag(InkType.Liquefy))
            {
                this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                {
                    Source1BorderMode = EffectBorderMode.Hard,
                    Source1 = this.BitmapLayer.Source,
                    Properties =
                    {
                        ["radius"] = this.BitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                        ["position"] = this.BitmapLayer .ConvertValueToOne(this.Position),
                        ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(position),
                        ["pressure"] = pressure,
                    }
                }, RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size));
                return true;
            }
            else return false;
        }

    }
}