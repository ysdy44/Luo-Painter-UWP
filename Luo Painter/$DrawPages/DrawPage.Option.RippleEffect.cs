using FanKit.Transformers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        Rippler Rippler = Rippler.Zero;

        bool IsRipplerPoint;
        Vector2 RipplerCenter;
        Vector2 StaringRipplerCenter;
        Vector2 RipplerPoint;
        Vector2 StaringRipplerPoint;

        private void SetRippleEffect(BitmapLayer bitmapLayer)
        {
            if (this.RipplerCenter == Vector2.Zero)
            {
                this.Rippler.Spread = System.Math.Min(bitmapLayer.Width, bitmapLayer.Height) / 4096f;

                this.RipplerCenter = bitmapLayer.Center;
                this.RipplerPoint.X = this.RipplerCenter.X + this.Rippler.Spread * 1.41421356f * 512;
                this.RipplerPoint.Y = this.RipplerCenter.Y;
            }
        }


        private void ConstructRippleEffect()
        {
            this.FrequencySlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Frequency = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.PhaseSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Phase = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.AmplitudeSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Amplitude = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void DrawRippleEffect(CanvasVirtualControl sender, CanvasDrawingSession ds)
        {
            Vector2 center = this.ToPoint(this.RipplerCenter);
            Vector2 point = this.ToPoint(this.RipplerPoint);
            float radius = Vector2.Distance(center, point);

            ds.DrawCircle(center, radius, Windows.UI.Colors.Black, 3);
            ds.DrawLine(center, point, Windows.UI.Colors.Black, 3);

            ds.DrawCircle(center, radius, Windows.UI.Colors.White, 2);
            ds.DrawLine(center, point, Windows.UI.Colors.White, 2);

            ds.DrawNode2(center);
            ds.DrawNode2(point);
        }

        private ICanvasImage GetRippleEffectPreview(ICanvasImage image)
        {
            return new PixelShaderEffect(this.RippleEffectShaderCodeBytes)
            {
                Source2BorderMode = EffectBorderMode.Hard,
                Source1 = image,
                Properties =
                {
                    ["frequency"] = this.Rippler.Frequency,
                    ["phase"] = this.Rippler.Phase,
                    ["amplitude"] = this.Rippler.Amplitude,
                    ["spread"] = this.Rippler.Spread,
                    ["center"] = this.RipplerCenter,
                    ["dpi"] = 96.0f, // Default value 96f,
                },
            };
        }


        private void RippleEffect_Start(Vector2 point, PointerPointProperties properties)
        {
            this.IsRipplerPoint = FanKit.Math.InNodeRadius(point, this.ToPoint(this.RipplerPoint));
            this.StaringPosition = this.ToPosition(point);
            this.StaringRipplerCenter = this.RipplerCenter;
            this.StaringRipplerPoint = this.RipplerPoint;
        }

        private void RippleEffect_Delta(Vector2 point, PointerPointProperties properties)
        {
            Vector2 position = this.ToPosition(point);
            Vector2 move = position - this.StaringPosition;
            this.RipplerPoint = move + this.StaringRipplerPoint;

            if (this.IsRipplerPoint)
            {
                this.Rippler.Spread = Vector2.Distance(this.RipplerCenter, this.RipplerPoint) / 512 / 1.41421356f;

                this.Tip("Spread", $"{this.Rippler.Spread * 100:0.00}%"); // Tip
            }
            else
            {
                this.RipplerCenter = move + this.StaringRipplerCenter;
            }

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void RippleEffect_Complete(Vector2 point, PointerPointProperties properties)
        {
        }

    }
}