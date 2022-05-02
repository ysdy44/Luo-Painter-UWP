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

        //float InkSize = 22f;
        //float InkOpacity = 1;
        //BlendEffectMode? InkBlendMode = null;

        //private void SetPaint()
        //{
        //    this.PaintSizeSlider.Value = this.PaintSizeSlider2.Value;
        //    this.PaintOpacitySlider.Value = this.PaintOpacitySlider2.Value;
        //}

        private void ConstructPaint()
        {
            //this.PaintSizeSlider2.ValueChanged += (s, e) =>
            //{
            //    this.Tip("Size", $"{e.NewValue}"); // Tip

            //    this.InkSize = (float)e.NewValue;
            //};
            //this.PaintOpacitySlider2.ValueChanged += (s, e) =>
            //{
            //    this.Tip("Opacity", $"{e.NewValue:0.00}%"); // Tip

            //    this.InkOpacity = (float)(e.NewValue / 100);
            //};
        }

        private void Paint_Start(Vector2 point, PointerPointProperties properties)
        {
            this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
            if (this.BitmapLayer == null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.Point = point;
            this.Position = this.ToPosition(point);
            this.Pressure = properties.Pressure;

            this.BitmapLayer.InkMode = this.PaintTool.GetInkMode(this.ToolType == ToolType.PaintEraseBrush, this.ToolType == ToolType.PaintLiquefaction);
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Paint_Delta(Vector2 point, PointerPointProperties properties)
        {
            if (this.BitmapLayer == null) return;

            Vector2 position = this.ToPosition(point);
            float pressure = properties.Pressure;

            Rect rect = this.Position.GetRect(this.PaintTool.InkSize);
            this.BitmapLayer.Hit(rect);

            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.Dry:
                    this.BitmapLayer.FillCircleDry(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize, this.ColorButton.Color);
                    break;
                case InkMode.WetWithOpacity:
                case InkMode.WetWithBlendMode:
                case InkMode.WetWithOpacityAndBlendMode:
                    this.BitmapLayer.FillCircleWet(this.Position, position, this.Pressure, pressure, this.PaintTool.InkSize, this.ColorButton.Color);
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

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasControl.Dpi.ConvertPixelsToDips(this.PaintTool.InkSize * this.Transformer.Scale));
            if (this.CanvasControl.Size.TryIntersect(ref region))
            {
                this.CanvasControl.Invalidate(region); // Invalidate
            }

            this.Point = point;
            this.Position = position;
            this.Pressure = pressure;
        }

        private void Paint_Complete(Vector2 point, PointerPointProperties properties)
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
                    this.BitmapLayer.ClearTemp();

                    // History
                    int removes3 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    this.BitmapLayer.Flush();
                    this.BitmapLayer.RenderThumbnail();
                    break;

                default:
                    this.BitmapLayer.DrawSource(this.PaintTool.GetInk(this.BitmapLayer));
                    this.BitmapLayer.ClearTemp();

                    // History
                    int removes2 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    this.BitmapLayer.Flush();
                    this.BitmapLayer.RenderThumbnail();
                    break;
            }

            this.BitmapLayer = null;
            this.CanvasControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

    }
}