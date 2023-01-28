using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class InkForcePage : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        private readonly PaintTaskCollection Tasks = new PaintTaskCollection();

        CanvasRenderTarget RenderTarget;
        Transformer Border;

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        public InkForcePage()
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
                this.Tasks.Speed = (float)e.NewValue * 2;
            };
            this.ClearButton.Click += (s, e) =>
            {
                using (CanvasDrawingSession ds = this.RenderTarget.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                }

                this.CanvasControl.Invalidate(); // Invalidate
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
                Vector2 sp = Vector2.Transform(this.Tasks.StartingPositionStabilizer, matrix);
                args.DrawingSession.DrawLine(sp, p, Colors.White);
                args.DrawingSession.DrawCircle(p, 12, Colors.White);
                args.DrawingSession.DrawCircle(sp, 12, Colors.White);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += async (point, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPressure = this.Pressure = properties.Pressure * properties.Pressure;

                //@Paint
                this.Tasks.StartForce(this.StartingPosition, this.StartingPressure, 12, 0.25f);
                this.Tasks.State = PaintTaskState.Painting;
                await Task.Run(this.TasksAction);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Pressure = properties.Pressure * properties.Pressure;

                // goto Timer.Elapsed
                this.Tasks.Position = this.Position;
                this.Tasks.Pressure = this.Pressure;
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                //@Paint
                this.Tasks.StopForce();
                this.Tasks.State = PaintTaskState.Painted;

                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
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

        private void TasksAction()
        {
            while (true)
            {
                switch (this.Tasks.GetBehavior())
                {
                    case PaintTaskBehavior.WaitingWork:
                        continue;
                    case PaintTaskBehavior.Working:
                    case PaintTaskBehavior.WorkingBeforeDead:
                        StrokeSegment segment = this.Tasks[0];
                        this.Tasks.Remove(segment);
                        this.TasksElapsed(segment);
                        break;
                    case PaintTaskBehavior.Dead:
                        this.Tasks.State = PaintTaskState.Finished;
                        this.Tasks.Clear();
                        this.TasksFinished();
                        return;
                    default:
                        return;
                }
            }
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