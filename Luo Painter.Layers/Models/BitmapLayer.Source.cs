using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public ICanvasImage Origin => this.OriginRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public ICanvasImage Temp => this.TempRenderTarget;

        readonly CanvasRenderTarget OriginRenderTarget;
        readonly CanvasRenderTarget SourceRenderTarget;
        readonly CanvasRenderTarget TempRenderTarget;

        public CanvasDrawingSession CreateSourceDrawingSession() => this.SourceRenderTarget.CreateDrawingSession();
        public CanvasDrawingSession CreateTempDrawingSession() => this.TempRenderTarget.CreateDrawingSession();

        public void Flush() => this.OriginRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);
        public void CopyPixels(BitmapLayer bitmapLayer) => this.SourceRenderTarget.CopyPixelsFromBitmap(bitmapLayer.TempRenderTarget);

        public void DrawSource(ICanvasImage image)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image);
            }
        }

        public void Clear(Color color)
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(color);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(color);
            }
        }

        public void ClearTemp()
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
            }
        }

    }
}