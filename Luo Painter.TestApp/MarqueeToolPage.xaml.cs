using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MarqueeToolPage : Page
    {
        DottedLineImage DottedLineImage;
        DottedLineBrush DottedLineBrush;

        Vector2 StartingPosition = new Vector2();
        TransformerRect Rect;

        public MarqueeToolPage()
        {
            this.InitializeComponent();
            this.ConstructLiquefaction();
            this.ConstructOperator();
            this.ConstructCanvas();
        }

        public void ConstructLiquefaction()
        {
            this.ResetButton.Tapped += (s, e) =>
            {
                using (CanvasDrawingSession ds = this.DottedLineImage.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                }
                this.DottedLineImage.Baking(this.CanvasControl);
            };
        }
       
        public void ConstructCanvas()
        {
            //Canvas
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(sender, 512, 512);
                this.DottedLineImage = new DottedLineImage(canvasRenderTarget);
                this.DottedLineBrush = new DottedLineBrush(sender);

                this.DottedLineImage.Baking(sender);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawRectangle(0, 0, 512, 512, Colors.Red);
                args.DrawingSession.DrawDottedLine(sender, this.DottedLineBrush, this.DottedLineImage, 512, 512);

                Rect rect = this.Rect.ToRect();
                args.DrawingSession.DrawThickRectangle(rect);
            };
            this.CanvasControl.Update += (sender, args) =>
            {
                this.DottedLineBrush.Update();
            };
        }

        public void ConstructOperator()
        {
            //Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = point;
                this.Rect = new TransformerRect(point, point);
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Rect = new TransformerRect(StartingPosition, point);
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                using (var ds = this.DottedLineImage.CreateDrawingSession())
                {
                    ds.FillRectangle(this.Rect.ToRect(), Windows.UI.Colors.Gray);
                }
                this.DottedLineImage.Baking(this.CanvasControl);

                this.Rect = new TransformerRect(Vector2.Zero, Vector2.Zero);
            };
        }

    } 
}