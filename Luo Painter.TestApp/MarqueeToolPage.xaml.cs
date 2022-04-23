using FanKit.Transformers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MarqueeToolPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        readonly MarqueeTool MarqueeTool = new MarqueeTool();

        BitmapLayer BitmapLayer;
        CanvasLinearGradientBrush Brush;
        Matrix3x2 BrushTransform = Matrix3x2.Identity;

        Vector2 StartingPosition;

        public MarqueeToolPage()
        {
            this.InitializeComponent();
            this.ConstructMarqueeTool();
            this.ConstructOperator();
            this.ConstructCanvas();
        }

        public void ConstructMarqueeTool()
        {
            this.ResetButton.Tapped += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.Transparent);
            };
        }

        public void ConstructCanvas()
        {
            //Canvas
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(sender, 512, 512);
                this.Brush = new CanvasLinearGradientBrush(sender, new CanvasGradientStop[]
                {
                    new CanvasGradientStop
                    {
                        Color = Windows.UI.Colors.White,
                        Position = 0
                    },
                    new CanvasGradientStop
                    {
                        Color = Windows.UI.Colors.White,
                        Position = 0.49f
                    },
                    new CanvasGradientStop
                    {
                        Color = Windows.UI.Colors.Black,
                        Position = 0.51f
                    },
                    new CanvasGradientStop
                    {
                        Color = Windows.UI.Colors.Black,
                        Position = 1
                    }
                }, CanvasEdgeBehavior.Wrap, CanvasAlphaMode.Premultiplied)
                {
                    StartPoint = Vector2.Zero,
                    EndPoint = new Vector2(12)
                };
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(new InvertEffect
                {
                    Source = new LuminanceToAlphaEffect//Alpha
                    {
                        Source = new EdgeDetectionEffect//Edge
                        {
                            Amount = 1,
                            Source = this.BitmapLayer.Source
                        }
                    }
                });
                args.DrawingSession.Blend = CanvasBlend.Min;
                args.DrawingSession.FillRectangle(0, 0, (float)sender.Size.Width, (float)sender.Size.Height, this.Brush);
            };
            this.CanvasControl.Update += (sender, args) =>
            {
                this.BrushTransform.M31--;
                this.BrushTransform.M32--;
                this.Brush.Transform = this.BrushTransform;
            };


            this.ToolCanvasControl.UseSharedDevice = true;
            this.ToolCanvasControl.CustomDevice = this.Device;
            this.ToolCanvasControl.CreateResources += (sender, args) =>
            {
            };
            this.ToolCanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawMarqueeTool(sender, MarqueeToolType.Rectangular, this.MarqueeTool);
            };
        }

        public void ConstructOperator()
        {
            //Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = point;
                this.MarqueeTool.Start(point, MarqueeToolType.Rectangular, false, false);

                this.CanvasControl.Paused = true;
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.MarqueeTool.Delta(this.StartingPosition, point, MarqueeToolType.Rectangular, false, false);
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateSourceDrawingSession())
                {
                    ds.FillMarqueeMaskl(this.CanvasControl, MarqueeToolType.Rectangular, this.MarqueeTool, new Rect(0, 0, 512, 512), MarqueeCompositeMode.New);
                }

                this.MarqueeTool.Complete(this.StartingPosition, point, MarqueeToolType.Rectangular, false, false);

                this.CanvasControl.Paused = false;
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}