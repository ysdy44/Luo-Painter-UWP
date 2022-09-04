using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal struct PenHitter
    {
        float StrokeWidth;

        public bool IsContains;
        public bool IsHit;
        public Vector2 Target;

        public void Hit(CanvasGeometry geometry, Vector2 position, float strokeWidth = 32)
        {
            this.StrokeWidth = strokeWidth;

            this.IsContains = false;
            this.IsHit = false;
            this.Target = Vector2.Zero;

            for (int i = 0; i < strokeWidth; i++)
            {
                this.IsContains = geometry.StrokeContainsPoint(position, i + i);
                if (this.IsContains)
                {
                    this.StrokeWidth = i;
                    this.Hit2(geometry, position);
                    return;
                }
            }
        }

        // angle = 12*30 = 360°
        private void Hit2(CanvasGeometry geometry, Vector2 position)
        {
            for (int i = 0; i < 12; i++)
            {
                // It's like a clock.
                int angle = i * 30;

                float radians = angle / 180f * FanKit.Math.Pi;
                this.Target.Y = position.Y - this.StrokeWidth * (float)System.Math.Sin(radians);
                this.Target.X = position.X + this.StrokeWidth * (float)System.Math.Cos(radians);

                this.IsHit = geometry.StrokeContainsPoint(this.Target, 2);
                if (this.IsHit)
                {
                    this.Hit3(geometry, position, angle);
                    return;
                }
            }
        }

        // angle-30° ~ angle+30°
        private void Hit3(CanvasGeometry geometry, Vector2 position, int angle)
        {
            // Zero 
            if (this.Hit4(geometry, position, angle)) return;
            for (int i = 1; i < 30; i++)
            {
                // Positive 
                if (this.Hit4(geometry, position, angle + i)) return;
                // Negative
                if (this.Hit4(geometry, position, angle - i)) return;
            }
        }

        private bool Hit4(CanvasGeometry geometry, Vector2 position, int angle)
        {
            float radians = angle / 180f * FanKit.Math.Pi;
            this.Target.Y = position.Y - this.StrokeWidth * (float)System.Math.Sin(radians);
            this.Target.X = position.X + this.StrokeWidth * (float)System.Math.Cos(radians);

            return geometry.StrokeContainsPoint(this.Target, 1);
        }
    }

    public sealed partial class PenHitterPage : Page
    {

        CanvasGeometry Geometry;
        PenHitter Hitter;
        Vector2 Position;

        public PenHitterPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                CanvasGeometry a = CanvasGeometry.CreateEllipse(sender, 222, 222, 100, 100);
                CanvasGeometry b = CanvasGeometry.CreateEllipse(sender, 333, 333, 100, 100);
                this.Geometry = a.CombineWith(b, Matrix3x2.Identity, CanvasGeometryCombine.Union);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawGeometry(this.Geometry, this.Hitter.IsContains ? Colors.Orange : Colors.DodgerBlue);

                if (this.Hitter.IsContains)
                {
                    args.DrawingSession.DrawLine(this.Hitter.Target, this.Position, this.Hitter.IsHit ? Colors.Orange : Colors.DodgerBlue, 2);

                    args.DrawingSession.FillCircle(this.Hitter.Target, 4, Colors.White);
                    args.DrawingSession.FillCircle(this.Hitter.Target, 3, Colors.DodgerBlue);
                }

                args.DrawingSession.FillCircle(this.Position, 4, Colors.White);
                args.DrawingSession.FillCircle(this.Position, 3, Colors.DodgerBlue);
            };
        }

        private void ConstructOperator()
        {
            this.CanvasControl.PointerMoved += (s, e) =>
            {
                this.Position = e.GetCurrentPoint(this.CanvasControl).Position.ToVector2();
                this.Hitter.Hit(this.Geometry, this.Position);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}