using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MarqueeToolTransformPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        readonly MarqueeTool MarqueeTool = new MarqueeTool();

        BitmapLayer BitmapLayer;

        Vector2 StartingPosition;
        byte[] ShaderCodeBytes;

        public MarqueeToolTransformPage()
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
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
            };
        }

        public void ConstructCanvas()
        {
            //Canvas
            this.CanvasAnimatedControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasAnimatedControl.UseSharedDevice = true;
            this.CanvasAnimatedControl.CustomDevice = this.Device;
            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height);
                this.Transformer.Fit();
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawCard(new ColorSourceEffect
                {
                    Color = Colors.White
                }, this.Transformer, Colors.Black);

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source1 = this.BitmapLayer.Source,
                    Properties =
                    {
                        ["time"] = (float)args.Timing.UpdateCount,
                        ["lineWidth"] = sender.Dpi.ConvertDipsToPixels(2),
                        ["left"] = 0f,
                        ["top"] = 0f,
                        ["right"] = (float)this.Transformer.Width,
                        ["bottom"] = (float)this.Transformer.Height,
                        ["matrix3x2"] = this.Transformer.GetInverseMatrix(),
                    },
                });
            };
            //this.CanvasControl.Update += (sender, args) =>
            //{
            //};


            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                Matrix3x2 matrix = this.CanvasAnimatedControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawMarqueeTool(sender, MarqueeToolType.Elliptical, this.MarqueeTool, matrix);
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.DottedLineTransform.LoadAsync();
        }

        public void ConstructOperator()
        {
            //Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = Vector2.Transform(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
                this.MarqueeTool.Start(this.StartingPosition, MarqueeToolType.Elliptical, false, false);

                this.CanvasAnimatedControl.Paused = true;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = Vector2.Transform(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
                this.MarqueeTool.Delta(this.StartingPosition, position, MarqueeToolType.Elliptical, false, false);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                Vector2 position = Vector2.Transform(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
                bool redraw = this.MarqueeTool.Complete(this.StartingPosition, position, MarqueeToolType.Elliptical, false, false);
                if (redraw is false) return;

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    ds.FillMarqueeMaskl(this.CanvasAnimatedControl, MarqueeToolType.Elliptical, this.MarqueeTool, new Rect(0, 0, 512, 512), MarqueeCompositeMode.New);
                }

                this.CanvasAnimatedControl.Paused = false;
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
            {
                this.Transformer.Move(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(center), this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(center), this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasAnimatedControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasAnimatedControl.Invalidate(); // Invalidate
            };
        }

    }
}