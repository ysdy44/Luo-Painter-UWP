using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
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

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
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

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
                args.DrawingSession.DrawImage(this.Texture, 512, 0);
            };
        }

        private async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            this.ShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();
            this.Texture = await CanvasBitmap.LoadAsync(resourceCreator, this.Brush.Mask.Source, 96);
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.StartingPressure = properties.Pressure;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Pressure = properties.Pressure;

                StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, (float)this.Brush.Size, (float)this.Brush.Spacing);
                if (segment.InRadius) return;

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                using (ds.CreateLayer(1f, segment.Bounds))
                {
                    segment.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.ShaderCodeBytes, BitmapLayer.DodgerBlue, this.Texture, true, (int)this.Brush.Hardness, (float)this.Brush.Flow);
                }

                this.CanvasControl.Invalidate(); // Invalidate

                this.StartingPosition = this.Position;
                this.StartingPressure = this.Pressure;
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