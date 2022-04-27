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
        float Time;
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
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source1 = this.BitmapLayer.Source,
                    Properties =
                    {
                        ["time"] = this.Time,
                        ["left"] = 0f,
                        ["top"] = 0f,
                        ["right"] = 512f,
                        ["bottom"] = 512f,
                    },
                });
            };
            this.CanvasControl.Update += (sender, args) =>
            {
                this.Time++;
            };


            this.ToolCanvasControl.UseSharedDevice = true;
            this.ToolCanvasControl.CustomDevice = this.Device;
            this.ToolCanvasControl.CreateResources += (sender, args) =>
            {
            };
            this.ToolCanvasControl.Draw += (sender, args) =>
            {
                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
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
                this.StartingPosition = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.MarqueeTool.Start(point, MarqueeToolType.Elliptical, false, false);

                this.CanvasControl.Paused = true;
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.MarqueeTool.Delta(this.StartingPosition, position, MarqueeToolType.Elliptical, false, false);
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateSourceDrawingSession())
                {
                    ds.FillMarqueeMaskl(this.CanvasControl, MarqueeToolType.Elliptical, this.MarqueeTool, new Rect(0, 0, 512, 512), MarqueeCompositeMode.New);
                }

                this.MarqueeTool.Complete(this.StartingPosition, point, MarqueeToolType.Elliptical, false, false);

                this.CanvasControl.Paused = false;
                this.ToolCanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}