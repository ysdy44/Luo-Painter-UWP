using FanKit.Transformers;
using Luo_Painter.Layers.Models;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        float LightDistance = 100;
        float LightAmbient = 0;
        float LightAngle = MathF.PI * 60 / 180;

        bool IsLightPoint;
        Vector2 LightCenter;
        Vector2 StartingLightCenter;
        Vector2 LightPoint;
        Vector2 StartingLightPoint;

        Vector3 LightTarget;
        Vector3 LightPosition;

        public void ConstructLighting()
        {
            this.LightDistanceSlider.Click += (s, e) => this.NumberShowAt(this.LightDistanceSlider, NumberPickerMode.Case0);
            this.LightDistanceSlider.ValueChanged += (s, e) =>
            {
                this.LightDistance = (float)e.NewValue;
                this.LightPosition = new Vector3(this.LightCenter, this.LightDistance);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.LightAngleSlider.Click += (s, e) => this.NumberShowAt(this.LightAngleSlider, NumberPickerMode.Case2);
            this.LightAngleSlider.ValueChanged += (s, e) =>
            {
                this.LightAngle = (float)(e.NewValue * MathF.PI / 180);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.LightAmbientSlider.Click += (s, e) => this.NumberShowAt(this.LightAmbientSlider, NumberPickerMode.Case1);
            this.LightAmbientSlider.ValueChanged += (s, e) =>
            {
                this.LightAmbient = (float)(e.NewValue / 100);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

        private void ResetLighting(BitmapLayer bitmapLayer)
        {
            if (this.LightCenter == Vector2.Zero)
            {
                float spread = System.Math.Min(bitmapLayer.Width, bitmapLayer.Height) / 4096f;

                this.LightCenter = bitmapLayer.Center;
                this.LightPoint.X = this.LightCenter.X + spread * 1.41421356f * 512;
                this.LightPoint.Y = this.LightCenter.Y;

                this.LightTarget = new Vector3(this.LightPoint, 0);
                this.LightPosition = new Vector3(this.LightCenter, this.LightDistance);
            }
        }

        private void DrawLighting(CanvasDrawingSession ds)
        {
            Vector2 center = this.ToPoint(this.LightCenter);
            Vector2 point = this.ToPoint(this.LightPoint);
            float radius = Vector2.Distance(center, point);

            ds.DrawCircle(center, radius, Windows.UI.Colors.Black, 3);
            ds.DrawLine(center, point, Windows.UI.Colors.Black, 3);

            ds.DrawCircle(center, radius, Windows.UI.Colors.White, 2);
            ds.DrawLine(center, point, Windows.UI.Colors.White, 2);

            ds.DrawNode2(center);
            ds.DrawNode2(point);
        }

        private void Lighting_Start()
        {
            this.IsLightPoint = FanKit.Math.InNodeRadius(this.StartingPoint, this.ToPoint(this.LightPoint));
            this.StartingLightCenter = this.LightCenter;
            this.StartingLightPoint = this.LightPoint;
        }

        private void Lighting_Delta()
        {
            Vector2 move = this.Position - this.StartingPosition;
            this.LightPoint = move + this.StartingLightPoint;
            this.LightTarget = new Vector3(this.LightPoint, 0);

            if (this.IsLightPoint is false)
            {
                this.LightCenter = move + this.StartingLightCenter;
                this.LightPosition = new Vector3(this.LightCenter, this.LightDistance);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Lighting_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

    }
}