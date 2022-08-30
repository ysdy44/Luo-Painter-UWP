using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal struct PenHitter
    {
        int Distance;

        public bool IsContains;
        public bool IsHit;
        public Vector2 Target;

        public void Hit(CanvasGeometry geometry, Vector2 position, int distance = 32)
        {
            this.Distance = distance;

            this.IsContains = false;
            this.IsHit = false;
            this.Target = Vector2.Zero;

            for (int i = 0; i < distance; i++)
            {
                this.IsContains = geometry.StrokeContainsPoint(position, i + i);
                if (this.IsContains)
                {
                    this.Distance = i;
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
                this.Target.Y = position.Y - this.Distance * (float)System.Math.Sin(radians);
                this.Target.X = position.X + this.Distance * (float)System.Math.Cos(radians);

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
            for (int i = 0; i < 30; i++)
            {
                // Positive 
                float radians1 = (angle + i) / 180f * FanKit.Math.Pi;
                this.Target.Y = position.Y - this.Distance * (float)System.Math.Sin(radians1);
                this.Target.X = position.X + this.Distance * (float)System.Math.Cos(radians1);

                this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                if (this.IsHit) return;


                // Negative
                float radians2 = (angle - i) / 180f * FanKit.Math.Pi;
                this.Target.Y = position.Y - this.Distance * (float)System.Math.Sin(radians2);
                this.Target.X = position.X + this.Distance * (float)System.Math.Cos(radians2);

                this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                if (this.IsHit) return;
            }
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
                args.DrawingSession.DrawGeometry(Geometry, Hitter.IsContains ? Colors.Orange : Colors.DodgerBlue);

                if (Hitter.IsContains)
                {
                    args.DrawingSession.DrawLine(Hitter.Target, Position, Hitter.IsHit ? Colors.Orange : Colors.DodgerBlue, 2);

                    args.DrawingSession.FillCircle(Hitter.Target, 4, Colors.White);
                    args.DrawingSession.FillCircle(Hitter.Target, 3, Colors.DodgerBlue);
                }

                args.DrawingSession.FillCircle(Position, 4, Colors.White);
                args.DrawingSession.FillCircle(Position, 3, Colors.DodgerBlue);
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