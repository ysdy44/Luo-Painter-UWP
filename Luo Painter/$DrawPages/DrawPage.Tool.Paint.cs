using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
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

        private void Paint_Start(Vector2 point, float pressure)
        {
            this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
            if (this.BitmapLayer == null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.Point = point;
            this.Position = this.ToPosition(point);
            this.Pressure = pressure;

            this.BitmapLayer.InkMode = this.PaintTool.GetInkMode(this.ToolType == ToolType.PaintEraseBrush, this.ToolType == ToolType.PaintLiquefaction);
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void Paint_Delta(Vector2 point, float pressure)
        {
            if (this.BitmapLayer == null) return;

            Vector2 position = this.ToPosition(point);

            Rect rect = this.Position.GetRect(this.PaintTool.InkSize);
            this.BitmapLayer.Hit(rect);

            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.Dry:
                    this.BitmapLayer.IsometricFillCircle(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize, this.ColorMenu.Color, BitmapType.Source);
                    break;
                case InkMode.WetWithOpacity:
                case InkMode.WetWithBlendMode:
                case InkMode.WetWithOpacityAndBlendMode:
                    this.BitmapLayer.IsometricFillCircle(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize, this.ColorMenu.Color, BitmapType.Temp);
                    break;

                case InkMode.EraseDry:
                    this.BitmapLayer.ErasingDry(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize);
                    break;
                case InkMode.EraseWetWithOpacity:
                    this.BitmapLayer.ErasingWet(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize);
                    break;

                case InkMode.Liquefy:
                    this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = this.BitmapLayer.Source,
                        Properties =
                        {
                            ["radius"] = this.BitmapLayer.ConvertValueToOne(this.PaintTool.InkSize),
                            ["position"] = this.BitmapLayer .ConvertValueToOne(this.Position),
                            ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(this.Position, position, this.PaintTool.InkSize));
                    break;

                default:
                    break;
            }

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.PaintTool.InkSize * this.Transformer.Scale));
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
            if (this.BitmapLayer == null) return;

            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.None:
                    return;

                case InkMode.Dry:
                case InkMode.EraseDry:
                    // History
                    int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    this.BitmapLayer.Flush();
                    this.BitmapLayer.RenderThumbnail();
                    break;

                case InkMode.Liquefy:
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                    // History
                    int removes3 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    this.BitmapLayer.Flush();
                    this.BitmapLayer.RenderThumbnail();
                    break;

                default:
                    this.BitmapLayer.DrawCopy(this.InkPresenter.GetWetPreview(this.InkType, this.BitmapLayer.Temp));
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                    // History
                    int removes2 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    this.BitmapLayer.Flush();
                    this.BitmapLayer.RenderThumbnail();
                    break;
            }

            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

    }
}