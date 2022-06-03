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
            this.PaintTool.SelectMask += async (s, e) => await this.SelectMask();
            this.PaintTool.SelectPattern += async (s, e) => await this.SelectPattern();

            this.PaintTool.MaskClosed += (s, e) => this.InkPresenter.SetMask(false);
            this.PaintTool.PatternClosed += (s, e) => this.InkPresenter.SetPattern(false);

            this.PaintTool.MaskOpened += async (s, e) =>
            {
                if (this.InkPresenter.Mask is null)
                {
                    bool result = await this.SelectMask();
                    if (result is false) this.PaintTool.CloseMask();
                }
                else this.InkPresenter.SetMask(true);
            };
            this.PaintTool.PatternOpened += async (s, e) =>
            {
                if (this.InkPresenter.Pattern is null)
                {
                    bool result = await this.SelectPattern();
                    if (result is false) this.PaintTool.ClosePattern();
                }
                else this.InkPresenter.SetPattern(true);
            };
        }

        private async Task<bool> SelectMask()
        {
            this.TextureDialog.Construct(this.PaintTool.MaskTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintTool.SetMaskTexture(item.Texture);
                        return true;
                    }
                    else return false;
                default: return false;
            }
        }

        private async Task<bool> SelectPattern()
        {
            this.TextureDialog.Construct(this.PaintTool.PatternTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintTool.SetPatternTexture(item.Texture);
                        this.PaintTool.SetStep(item.Step);
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

            if (this.InkType.HasFlag(InkType.BrushDry))
            {
                this.BitmapLayer.IsometricShape(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size,
                    this.BrushEdgeHardnessShaderCodeBytes, (int)this.InkPresenter.Hardness, this.ColorMenu.ColorHdr,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.MaskBrushDry))
            {
                this.BitmapLayer.IsometricShape(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size,
                    this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.InkPresenter.Mask, (int)this.InkPresenter.Hardness, this.InkPresenter.Rotate, this.ColorMenu.ColorHdr,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.CircleDry))
            {
                this.BitmapLayer.IsometricFillCircle(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size,
                    this.ColorMenu.Color, this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.LineDry))
            {
                this.BitmapLayer.DrawLine(this.Position, position, this.ColorMenu.Color, this.InkPresenter.Size,
                    this.GetBitmapType(this.InkType));
            }
            else if (this.InkType.HasFlag(InkType.EraseDry))
            {
                if (this.InkType.HasFlag(InkType.Pattern) || this.InkType.HasFlag(InkType.Opacity))
                    this.BitmapLayer.ErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size);
                else
                    this.BitmapLayer.ErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size);
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
            }

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

    }
}