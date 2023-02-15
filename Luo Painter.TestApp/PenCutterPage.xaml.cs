using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal struct PenCutter
    {
        float StrokeWidth;
        float Length;

        public bool IsContains;
        public bool IsHit;
        public Vector2 Target;

        public void Hit(CanvasGeometry geometry, Vector2 startingPosition, Vector2 position, float strokeWidth = 32)
        {
            this.StrokeWidth = strokeWidth;
            this.Length = Vector2.Distance(position, startingPosition);

            this.IsContains = false;
            this.IsHit = false;

            // position ~ position+32
            this.Target = position;
            this.IsContains = geometry.StrokeContainsPoint(this.Target, this.StrokeWidth);
            if (this.IsContains)
            {
                this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                if (this.IsHit) return;

                for (float i = 1; i < this.StrokeWidth; i++)
                {
                    this.Target = Vector2.Lerp(position, startingPosition, i / this.Length);
                    this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                    if (this.IsHit) return;
                }
                return;
            }

            // lerp-32 ~ lerp+32
            for (float lerp = this.StrokeWidth; lerp < this.Length; lerp += this.StrokeWidth)
            {
                this.Target = Vector2.Lerp(position, startingPosition, lerp / this.Length);
                this.IsContains = geometry.StrokeContainsPoint(this.Target, this.StrokeWidth);
                if (this.IsContains)
                {
                    // Zero 
                    this.Target = Vector2.Lerp(position, startingPosition, lerp / this.Length);
                    this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                    if (this.IsHit) return;

                    for (float i = 1; i < this.StrokeWidth; i++)
                    {
                        // Positive 
                        this.Target = Vector2.Lerp(position, startingPosition, (lerp + i) / this.Length);
                        this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                        if (this.IsHit) return;

                        // Negative
                        this.Target = Vector2.Lerp(position, startingPosition, (lerp - i) / this.Length);
                        this.IsHit = geometry.StrokeContainsPoint(this.Target, 1);
                        if (this.IsHit) return;
                    }

                    return;
                }
            }
        }


    }

    public sealed partial class PenCutterPage : Page
    {

        CanvasGeometry Geometry;
        PenCutter Cutter;

        Vector2 StartingPosition;
        Vector2 Position;

        public PenCutterPage()
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
                args.DrawingSession.DrawGeometry(this.Geometry, this.Cutter.IsContains ? Colors.Orange : Colors.DodgerBlue);

                args.DrawingSession.DrawLine(this.StartingPosition, this.Position, Colors.DodgerBlue, 2);

                args.DrawingSession.FillCircle(this.StartingPosition, 4, Colors.White);
                args.DrawingSession.FillCircle(this.StartingPosition, 3, Colors.DodgerBlue);

                args.DrawingSession.FillCircle(this.Position, 4, Colors.White);
                args.DrawingSession.FillCircle(this.Position, 3, Colors.DodgerBlue);

                if (this.Cutter.IsHit)
                {
                    args.DrawingSession.FillCircle(this.Cutter.Target, 4, Colors.White);
                    args.DrawingSession.FillCircle(this.Cutter.Target, 3, Colors.DodgerBlue);
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = point;
                this.Position = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = point;

                this.Cutter.Hit(this.Geometry, this.StartingPosition, this.Position);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Position = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}