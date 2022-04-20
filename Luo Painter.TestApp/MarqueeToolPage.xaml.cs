using FanKit.Transformers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
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

        DottedLineBrush Brush;
        BitmapLayer BitmapLayer;
        CanvasRenderTarget Image;

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
                this.Baking();
            };
        }

        public void ConstructCanvas()
        {
            //Canvas
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                //@DPI
                this.Brush = new DottedLineBrush(sender);
                this.Image = new CanvasRenderTarget(sender, 512, 512, 96);
                this.BitmapLayer = new BitmapLayer(sender, 512, 512);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(this.Image);
                args.DrawingSession.Blend = CanvasBlend.Min;
                args.DrawingSession.FillRectangle(0, 0, (float)sender.Size.Width, (float)sender.Size.Height, this.Brush.Brush);
            };
            this.CanvasControl.Update += (sender, args) =>
            {
                this.Brush.Update();
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
                    ds.FillMarqueeMaskl(this.CanvasControl, MarqueeToolType.Rectangular, this.MarqueeTool, this.Image.Bounds, MarqueeCompositeMode.New);
                }
                this.Baking();

                this.MarqueeTool.Complete(this.StartingPosition, point, MarqueeToolType.Rectangular, false, false);

                this.CanvasControl.Paused = false;
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
        }

        public void Baking()
        {
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateTempDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = new Rect(2, 2, this.BitmapLayer.Width - 4, this.BitmapLayer.Height - 4),
                    Source = this.BitmapLayer.Source
                });
            }
            using (CanvasDrawingSession ds = this.Image.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(new InvertEffect
                {
                    Source = new LuminanceToAlphaEffect//Alpha
                    {
                        Source = new EdgeDetectionEffect//Edge
                        {
                            Amount = 1,
                            Source = this.BitmapLayer.Temp
                        }
                    }
                });
            }
        }

    }
}