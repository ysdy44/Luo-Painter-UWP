using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using System.Timers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class InkBrownianPage : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };
        readonly Timer Timer = new Timer
        {
            Interval = 20
        };

        CanvasRenderTarget RenderTarget;
        Transformer Border;

        Vector2 Velocity;

        float Inertia = 100;
        Vector2 PositionInertia;

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        public InkBrownianPage()
        {
            this.InitializeComponent();
            this.ConstructInkStabilizer();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructInkStabilizer()
        {
            this.Slider.ValueChanged += (s, e) =>
            {
                this.Inertia = (float)e.NewValue;
            };
            this.ClearButton.Click += (s, e) =>
            {
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Timer.Elapsed += (s, e) =>
            {
                float strokeWidth = 12 * 2 * this.Pressure;
                if (strokeWidth < 1) return;

                for (int i = 0; i < this.Inertia; i++)
                {
                    Vector2 d = this.Position - this.PositionInertia;
                    if (d.LengthSquared() < 12 * 12) return;

                    this.Velocity += d * 0.01666f * 0.01666f;
                    this.PositionInertia += this.Velocity * 0.01666f * d.Length();
                }

                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.DrawLine(this.StartingPosition, this.PositionInertia, Colors.DodgerBlue, strokeWidth, this.CanvasStrokeStyle);
                }
                this.StartingPosition = this.PositionInertia;

                this.CanvasControl.Invalidate();
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Transformer.Fit();
                //@DPI
                this.RenderTarget = new CanvasRenderTarget(this.CanvasControl, this.Transformer.Width, this.Transformer.Height, 96);
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.RenderTarget,
                    TransformMatrix = this.Transformer.GetMatrix(),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                });

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                Vector2 p = Vector2.Transform(this.Position, matrix);
                Vector2 sp = Vector2.Transform(this.StartingPosition, matrix);
                args.DrawingSession.DrawLine(sp, p, Colors.White);
                args.DrawingSession.DrawCircle(p, 12, Colors.White);
                args.DrawingSession.DrawCircle(sp, 12, Colors.White);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPressure = this.Pressure = properties.Pressure;

                this.Velocity = Vector2.Zero;
                this.PositionInertia = this.Position;

                this.Timer.Start();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Pressure = properties.Pressure;

                // goto Timer.Elapsed
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);

                this.Timer.Stop();
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }
    }
}