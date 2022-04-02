using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public LayerType Type => LayerType.Bitmap;

        //@Construct
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer) : this(resourceCreator, bitmapLayer.SourceRenderTarget, bitmapLayer.Width, bitmapLayer.Height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer, int width, int height) : this(resourceCreator, bitmapLayer.SourceRenderTarget, width, height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap) : this(resourceCreator, bitmap, (int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height) { }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap, int width, int height) : this(resourceCreator, width, height)
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(bitmap);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(bitmap);
            }
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            //@DPI
            this.OriginRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.TempRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.ThumbnailWriteableBitmap = new WriteableBitmap(50, 50);
            this.ThumbnailRenderTarget = new CanvasRenderTarget(resourceCreator, 50, 50, 96);


            this.Center = new Vector2((float)width / 2, (float)height / 2);
            this.Width = width;
            this.Height = height;
            this.XDivisor = width / BitmapLayer.Unit;
            this.YDivisor = height / BitmapLayer.Unit;
            this.XRemainder = width % BitmapLayer.Unit;
            this.YRemainder = height % BitmapLayer.Unit;

            bool noXR = this.XRemainder == 0;
            bool noYR = this.YRemainder == 0;

            if (noXR && noYR)
            {
                this.RegionType = RegionType.None;
                this.XLength = this.XDivisor;
                this.YLength = this.YDivisor;
            }
            else if (noXR && noYR == false)
            {
                this.RegionType = RegionType.XYRemainder;
                this.XLength = this.XDivisor;
                this.YLength = this.YDivisor + 1;
            }
            else if (noXR == false && noYR)
            {
                this.RegionType = RegionType.YRemainder;
                this.XLength = this.XDivisor + 1;
                this.YLength = this.YDivisor;
            }
            else
            {
                this.RegionType = RegionType.XYRemainder;
                this.XLength = this.XDivisor + 1;
                this.YLength = this.YDivisor + 1;
            }

            this.Hits = new bool[this.XLength * this.YLength];
        }

    }
}