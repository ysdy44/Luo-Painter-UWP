using FanKit.Transformers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Shaders;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        Rippler Rippler = Rippler.Zero;

        bool IsRipplerPoint;
        Vector2 RipplerCenter;
        Vector2 StartingRipplerCenter;
        Vector2 RipplerPoint;
        Vector2 StartingRipplerPoint;

        public void ConstructRippleEffect()
        {
            this.FrequencySlider.Click += (s, e) => this.NumberShowAt(this.FrequencySlider, NumberPickerMode.Case0);
            this.FrequencySlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Frequency = (float)(e.NewValue);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.PhaseSlider.Click += (s, e) => this.NumberShowAt(this.PhaseSlider, NumberPickerMode.Case1);
            this.PhaseSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Phase = (float)(e.NewValue);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.AmplitudeSlider.Click += (s, e) => this.NumberShowAt(this.AmplitudeSlider, NumberPickerMode.Case2);
            this.AmplitudeSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Amplitude = (float)(e.NewValue);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

        private void ResetRippleEffect(BitmapLayer bitmapLayer)
        {
            if (this.RipplerCenter == Vector2.Zero)
            {
                this.Rippler.Spread = System.Math.Min(bitmapLayer.Width, bitmapLayer.Height) / 4096f;

                this.RipplerCenter = bitmapLayer.Center;
                this.RipplerPoint.X = this.RipplerCenter.X + this.Rippler.Spread * 1.41421356f * 512;
                this.RipplerPoint.Y = this.RipplerCenter.Y;
            }
        }

        private void DrawRippleEffect(CanvasDrawingSession ds)
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

        private void RippleEffect_Start()
        {
            this.IsRipplerPoint = FanKit.Math.InNodeRadius(this.StartingPoint, this.ToPoint(this.RipplerPoint));
            this.StartingRipplerCenter = this.RipplerCenter;
            this.StartingRipplerPoint = this.RipplerPoint;
        }

        private void RippleEffect_Delta()
        {
            Vector2 move = this.Position - this.StartingPosition;
            this.RipplerPoint = move + this.StartingRipplerPoint;

            if (this.IsRipplerPoint)
            {
                this.Rippler.Spread = Vector2.Distance(this.RipplerCenter, this.RipplerPoint) / 512 / 1.41421356f;

                this.ToastTip.Tip(TipType.Spread.GetString(), $"{this.Rippler.Spread * 100:0.00}%");
            }
            else
            {
                this.RipplerCenter = move + this.StartingRipplerCenter;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

    }
}