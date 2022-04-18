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
    internal sealed class InkRender
    {
        readonly BitmapLayer PaintLayer;
        public ICanvasImage Souce => this.PaintLayer.Source;
        public InkRender(CanvasControl sender) => this.PaintLayer = new BitmapLayer(sender, (int)sender.ActualWidth, (int)sender.ActualHeight);
        public void Render(float size, Color color)
        {
            this.PaintLayer.Clear(Colors.Transparent);

            float width = this.PaintLayer.Width;
            float height = this.PaintLayer.Height;
            float space = System.Math.Max(2, size / height * 2);

            Vector2 position = new Vector2(10, height / 2 + 3.90181f);
            float pressure = 0.001f;

            for (float x = 10; x < width - 10; x += space)
            {
                // 0 ~ Π
                float radian = x / width * FanKit.Math.Pi;

                // Sin 0 ~ Π ︵
                float targetPressure = (float)System.Math.Sin(radian);
                // Sin 0 ~ 2Π ~
                float offsetY = 20 * (float)System.Math.Sin(radian + radian);
                Vector2 targetPosition = new Vector2(x, height / 2 + offsetY);

                this.PaintLayer.FillCircleDry(position, targetPosition, pressure, targetPressure, space, color);
                position = targetPosition;
                pressure = targetPressure;
            }
        }
    }

    public sealed partial class DrawPage : Page
    {

        Vector2 Point;
        Vector2 Position;
        float Pressure;

        float InkSize = 22f;
        float InkOpacity = 1;
        BlendEffectMode? InkBlendMode = null;

        InkRender InkRender;

        private void SetPaint()
        {
            this.PaintSizeSlider.Value = this.PaintSizeSlider2.Value;
            this.PaintOpacitySlider.Value = this.PaintOpacitySlider2.Value;
        }

        private void ConstructPaint()
        {
            this.PaintCanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new InkRender(sender);
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.InkRender.Render(this.InkSize, Colors.Black);
                        break;
                    case ElementTheme.Dark:
                        this.InkRender.Render(this.InkSize, Colors.White);
                        break;
                }
            };
            this.PaintCanvasControl.Draw += (sender, args) =>
            {
                if (this.InkRender == null) return;
                args.DrawingSession.DrawImage(this.InkRender.Souce);
            };

            this.PaintSizeSlider.ValueChanged += (s, e) =>
            {
                this.InkSize = (float)e.NewValue;
                this.PaintSizeSlider2.Value = e.NewValue;

                if (this.InkRender == null) return; 
                
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.InkRender.Render(this.InkSize, Colors.Black);
                        break;
                    case ElementTheme.Dark:
                        this.InkRender.Render(this.InkSize, Colors.White);
                        break;
                }
                this.PaintCanvasControl.Invalidate(); // Invalidate
            };
            this.PaintOpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)(e.NewValue / 100);
                this.PaintCanvasControl.Opacity = this.InkOpacity;

                this.PaintOpacitySlider2.Value = e.NewValue;
            };
            this.PaintBlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    bool isNone = item.IsNone();

                    if (isNone) this.InkBlendMode = null;
                    else this.InkBlendMode = item;
                }
            };

            this.PaintSizeSlider2.ValueChanged += (s, e) =>
            {
                this.Tip("Size", $"{e.NewValue}"); // Tip

                this.InkSize = (float)e.NewValue;
            };
            this.PaintOpacitySlider2.ValueChanged += (s, e) =>
            {
                this.Tip("Opacity", $"{e.NewValue:0.00}%"); // Tip

                this.InkOpacity = (float)(e.NewValue / 100);
            };
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

            this.BitmapLayer.InkMode = this.GetInkMode(this.ToolType == ToolType.PaintEraseBrush, this.ToolType == ToolType.PaintLiquefaction);
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Paint_Delta(Vector2 point, PointerPointProperties properties)
        {
            if (this.BitmapLayer == null) return;

            Vector2 position = this.ToPosition(point);
            float pressure = properties.Pressure;

            Rect rect = this.Position.GetRect(this.InkSize);
            this.BitmapLayer.Hit(rect);

            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.Dry:
                    this.BitmapLayer.FillCircleDry(this.Position, position, this.Pressure, pressure, this.InkSize, this.ColorPicker.Color);
                    break;
                case InkMode.WetWithOpacity:
                case InkMode.WetWithBlendMode:
                case InkMode.WetWithOpacityAndBlendMode:
                    this.BitmapLayer.FillCircleWet(this.Position, position, this.Pressure, pressure, this.InkSize, this.ColorPicker.Color);
                    break;

                case InkMode.EraseDry:
                    this.BitmapLayer.ErasingDry(this.Position, position, this.Pressure, pressure, this.InkSize);
                    break;
                case InkMode.EraseWetWithOpacity:
                    this.BitmapLayer.ErasingWet(this.Position, position, this.Pressure, pressure, this.InkSize);
                    break;

                case InkMode.Liquefy:
                    this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = this.BitmapLayer.Source,
                        Properties =
                        {
                            ["radius"] = this.BitmapLayer.ConvertValueToOne(this.InkSize),
                            ["position"] = this.BitmapLayer .ConvertValueToOne(this.Position),
                            ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(this.Position, position, this.InkSize));
                    break;

                default:
                    break;
            }

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasControl.Dpi.ConvertPixelsToDips(this.InkSize * this.Transformer.Scale));
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
                    this.BitmapLayer.DrawSource(this.GetInk(this.BitmapLayer));
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


        private ICanvasImage GetInk(BitmapLayer bitmapLayer)
        {
            switch (bitmapLayer.InkMode)
            {
                case InkMode.WetWithOpacity:
                    return bitmapLayer.GetWeting(this.InkOpacity);
                case InkMode.WetWithBlendMode:
                    return bitmapLayer.GetWeting(this.InkBlendMode.Value);
                case InkMode.WetWithOpacityAndBlendMode:
                    return bitmapLayer.GetWeting(this.InkOpacity, this.InkBlendMode.Value);

                case InkMode.EraseWetWithOpacity:
                    return bitmapLayer.GetEraseWeting(this.InkOpacity);

                default:
                    return bitmapLayer.Source;
            }
        }

        private InkMode GetInkMode(bool isErase, bool isLiquefaction)
        {
            if (isLiquefaction) return InkMode.Liquefy;

            if (isErase)
            {
                if (this.InkOpacity == 1f) return InkMode.EraseDry;
                else return InkMode.EraseWetWithOpacity;
            }

            if (this.InkBlendMode.HasValue == false)
            {
                if (this.InkOpacity == 1f) return InkMode.Dry;
                else return InkMode.WetWithOpacity;
            }

            if (this.InkOpacity == 1f) return InkMode.WetWithBlendMode;
            else return InkMode.WetWithOpacityAndBlendMode;
        }

    }
}