using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.TestApp
{
    [ContentProperty(Name = nameof(Title))]
    public class InkTouchMode
    {
        public string Title { get; set; }
        public TouchMode TouchMode { get; set; }
        public override string ToString() => this.Title;
    }

    public sealed partial class InkTouchModePage : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        CanvasRenderTarget RenderTarget;
        Transformer Border;

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        public InkTouchModePage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            switch (this.Operator.TouchMode)
            {
                case TouchMode.Disable: this.ListView.SelectedIndex = 2; break;
                case TouchMode.SingleFinger: this.ListView.SelectedIndex = 0; break;
                case TouchMode.RightButton: this.ListView.SelectedIndex = 1; break;
                default: break;
            }
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is InkTouchMode item)
                {
                    switch (item.TouchMode)
                    {
                        case TouchMode.Disable: this.Operator.TouchMode = TouchMode.Disable; break;
                        case TouchMode.SingleFinger: this.Operator.TouchMode = TouchMode.SingleFinger; break;
                        case TouchMode.RightButton: this.Operator.TouchMode = TouchMode.RightButton; break;
                        default: break;
                    }
                }
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
                args.DrawingSession.DrawCircle(p, 12, Colors.White);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPressure = this.Pressure = properties.Pressure;

                this.TasksElapsed(new StrokeSegment(this.StartingPosition, this.StartingPosition, this.StartingPressure, this.StartingPressure, 12, 0.25f));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Pressure = properties.Pressure;

                this.TasksElapsed(new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, 12, 0.25f));

                this.StartingPosition = this.Position;
                this.StartingPressure = this.Pressure;
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);

                this.TasksElapsed(new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, 12, 0.25f));

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

        private void TasksElapsed(StrokeSegment segment)
        {
            using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
            {
                ds.DrawLine(segment.StartingPosition, segment.Position, Colors.DodgerBlue, segment.Size, this.CanvasStrokeStyle);
            }

            this.CanvasControl.Invalidate();
        }
        private void TasksFinished()
        {
        }

    }
}