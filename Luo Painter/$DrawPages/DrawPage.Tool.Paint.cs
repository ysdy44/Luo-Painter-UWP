using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructPaint()
        {
            this.SizeSlider.ValueChanged += (s, e) =>
            {
                this.Tip("Size", $"{e.NewValue}"); // Tip

                this.InkSize = (float)e.NewValue;
            };
            this.BlendButton.Click += (s, e) => this.BlendFlyout.ShowAt(this.BlendButton);
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.Tip("Opacity", $"{e.NewValue:0.00}%"); // Tip

                this.InkOpacity = (float)(e.NewValue / 100);
            };
        }

        private void Paint_Delta(BitmapLayer bitmapLayer, Vector2 staringPosition, Vector2 position, float pressure, Color color)
        {
            Rect rect = staringPosition.GetRect(this.InkSize);
            bitmapLayer.Hit(rect);

            switch (bitmapLayer.InkMode)
            {
                case InkMode.Dry:
                    bitmapLayer.FillCircleDry(staringPosition, position, this.InkSize * pressure, color);
                    break;
                case InkMode.WetWithOpacity:
                case InkMode.WetWithBlendMode:
                case InkMode.WetWithOpacityAndBlendMode:
                    bitmapLayer.FillCircleWet(staringPosition, position, this.InkSize * pressure, color);
                    break;

                case InkMode.EraseDry:
                    bitmapLayer.ErasingDry(staringPosition, position, this.InkSize * pressure);
                    break;
                case InkMode.EraseWetWithOpacity:
                    bitmapLayer.ErasingWet(staringPosition, position, this.InkSize * pressure);
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
                    return bitmapLayer.GetWeting(this.BlendType);
                case InkMode.WetWithOpacityAndBlendMode:
                    return bitmapLayer.GetWeting(this.InkOpacity, this.BlendType);

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

            if (this.BlendType.IsNone())
            {
                if (this.InkOpacity == 1f) return InkMode.Dry;
                else return InkMode.WetWithOpacity;
            }

            if (this.InkOpacity == 1f) return InkMode.WetWithBlendMode;
            else return InkMode.WetWithOpacityAndBlendMode;
        }

    }
}