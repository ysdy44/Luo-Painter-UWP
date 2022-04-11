using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal sealed class InkRender
    {
        readonly BitmapLayer PaintLayer;
        public ICanvasImage Souce => this.PaintLayer.Source;
        public InkRender(CanvasControl sender) => this.PaintLayer = new BitmapLayer(sender, (int)sender.ActualWidth, (int)sender.ActualHeight);
        public void Render(float size)
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

                this.PaintLayer.FillCircleDry(position, targetPosition, pressure, targetPressure, space, Colors.Black);
                position = targetPosition;
                pressure = targetPressure;
            }
        }
    }

    public sealed partial class DrawPage : Page
    {

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
                this.InkRender.Render(this.InkSize);
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
                this.InkRender.Render(this.InkSize);
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

        private void Paint_Delta(BitmapLayer bitmapLayer, Vector2 staringPosition, Vector2 position, float staringPressure, float pressure, Color color)
        {
            Rect rect = staringPosition.GetRect(this.InkSize);
            bitmapLayer.Hit(rect);

            switch (bitmapLayer.InkMode)
            {
                case InkMode.Dry:
                    bitmapLayer.FillCircleDry(staringPosition, position, staringPressure, pressure, this.InkSize, color);
                    break;
                case InkMode.WetWithOpacity:
                case InkMode.WetWithBlendMode:
                case InkMode.WetWithOpacityAndBlendMode:
                    bitmapLayer.FillCircleWet(staringPosition, position, staringPressure, pressure, this.InkSize, color);
                    break;

                case InkMode.EraseDry:
                    bitmapLayer.ErasingDry(staringPosition, position, staringPressure, pressure, this.InkSize);
                    break;
                case InkMode.EraseWetWithOpacity:
                    bitmapLayer.ErasingWet(staringPosition, position, staringPressure, pressure, this.InkSize);
                    break;

                case InkMode.Liquefy:
                    bitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = bitmapLayer.Source,
                        Properties =
                        {
                            ["radius"] = bitmapLayer.ConvertValueToOne(this.InkSize),
                            ["position"] = bitmapLayer.ConvertValueToOne(staringPosition),
                            ["targetPosition"] = bitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(staringPosition, position, this.InkSize));
                    break;

                default:
                    break;
            }

            this.CanvasControl.Invalidate(); // Invalidate
            Rect region = RectExtensions.GetRect
            (
                this.ToPoint(staringPosition),
                this.ToPoint(position),
                this.CanvasControl.Dpi.ConvertPixelsToDips(this.InkSize * this.Transformer.Scale)
            );

            if (this.CanvasControl.Size.TryIntersect(ref region))
            {
                this.CanvasControl.Invalidate(region); // Invalidate
            }
        }

        private bool Paint_Complete(BitmapLayer bitmapLayer)
        {
            switch (bitmapLayer.InkMode)
            {
                case InkMode.None:
                    return false;

                case InkMode.Dry:
                case InkMode.EraseDry:
                    // History
                    int removes = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    return true;

                case InkMode.Liquefy:
                    bitmapLayer.ClearTemp();

                    // History
                    int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    return true;

                default:
                    bitmapLayer.DrawSource(this.GetInk(bitmapLayer));
                    bitmapLayer.ClearTemp();

                    // History
                    int removes2 = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    return true;
            }
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