using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class BrushEdgeHardnessWithTexturePage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;
        float Pressure;

        byte[] ShaderCodeBytes;
        CanvasBitmap Texture;

        public BrushEdgeHardnessWithTexturePage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                args.TrackAsyncAction(this.CreateResourcesAsync(sender).AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, 512, 512, Colors.White);
                args.DrawingSession.FillRectangle(512, 0, 512, 512, Colors.Black);

                args.DrawingSession.DrawImage(this.BitmapLayer.Source);
                args.DrawingSession.DrawImage(this.Texture, 512, 0);
            };
        }

        private async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            this.ShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();
            this.Texture = await CanvasBitmap.LoadAsync(resourceCreator, this.Brush.Source, 96);
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Pressure = properties.Pressure;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                float pressure = properties.Pressure;

                this.BitmapLayer.IsometricShape(this.Position, position, this.Pressure, pressure, (float)this.Brush.Size,
                    this.ShaderCodeBytes, this.Texture, (int)this.Brush.Hardness, BitmapLayer.DodgerBlue,
                    BitmapType.Source);

                this.Position = position;
                this.Pressure = pressure;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.BitmapLayer.Flush();
                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}