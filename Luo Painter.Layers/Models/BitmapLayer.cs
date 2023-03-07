using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {
        public LayerType Type => LayerType.Bitmap;
        public ICanvasImage this[BitmapType type]
        {
            get
            {
                switch (type)
                {
                    case BitmapType.Origin: return this.OriginRenderTarget;
                    case BitmapType.Temp: return this.TempRenderTarget;
                    default: return this.SourceRenderTarget;
                }
            }
        }

        readonly CanvasRenderTarget OriginRenderTarget;
        readonly CanvasRenderTarget SourceRenderTarget;
        readonly CanvasRenderTarget TempRenderTarget;

        readonly byte[] Pixels;
        readonly IBuffer Buffer;

        public IAsyncAction SaveAsync(IRandomAccessStream stream) => this.SourceRenderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
        public IBuffer GetPixelBytes()
        {
            this.SourceRenderTarget.GetPixelBytes(this.Buffer);
            return this.Buffer;
        }


        public Color[] GetInterpolationColorsBySource() => this.GetInterpolationColors(this.SourceRenderTarget);
        public void SetPixelBytes(IDictionary<int, IBuffer> colors) => base.SetPixelBytes(SourceRenderTarget, colors);
        public PixelBounds CreatePixelBounds(PixelBounds interpolationBounds, Color[] interpolationColors) => base.CreatePixelBounds(SourceRenderTarget, interpolationBounds, interpolationColors);


        //@Construct
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap) : this(resourceCreator, (int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height)
        {
            this.OriginRenderTarget.CopyPixelsFromBitmap(bitmap);
            this.SourceRenderTarget.CopyPixelsFromBitmap(bitmap);

            this.RenderThumbnail();
        }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap, Vector2 offset) : this(resourceCreator, (int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height)
        {
            int width = base.Width;
            int height = base.Height;

            int x = (int)offset.X;
            int y = (int)offset.Y;

            int w = System.Math.Min(width, -x + (int)bitmap.SizeInPixels.Width);
            int h = System.Math.Min(height, -y + (int)bitmap.SizeInPixels.Height);

            if (w > 0 && h > 0)
            {
                this.OriginRenderTarget.CopyPixelsFromBitmap(bitmap, x, y, 0, 0, w, h);
                this.SourceRenderTarget.CopyPixelsFromBitmap(bitmap, x, y, 0, 0, w, h);
            }

            this.RenderThumbnail();
        }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap, int width, int height) : this(resourceCreator, width, height)
        {
            int w = System.Math.Min(width, (int)bitmap.SizeInPixels.Width);
            int h = System.Math.Min(height, (int)bitmap.SizeInPixels.Height);

            this.OriginRenderTarget.CopyPixelsFromBitmap(bitmap, 0, 0, 0, 0, w, h);
            this.SourceRenderTarget.CopyPixelsFromBitmap(bitmap, 0, 0, 0, 0, w, h);

            this.RenderThumbnail();
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer) : this(resourceCreator, bitmapLayer.SourceRenderTarget, bitmapLayer.Width, bitmapLayer.Height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer, int width, int height) : this(resourceCreator, bitmapLayer.SourceRenderTarget, width, height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer, Vector2 offset) : this(resourceCreator, bitmapLayer.SourceRenderTarget, bitmapLayer.Width, bitmapLayer.Height, offset) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer, int width, int height, Vector2 offset) : this(resourceCreator, bitmapLayer.SourceRenderTarget, width, height, offset) { }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, ICanvasImage image, int width, int height) : this(resourceCreator, width, height)
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(image);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(image);
            }

            this.RenderThumbnail();
        }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, ICanvasImage image, int width, int height, Vector2 offset) : this(resourceCreator, width, height)
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(image, offset);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.DrawImage(image, offset);
            }

            this.RenderThumbnail();
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, IBuffer bytes, int width, int height) : this(null, null, resourceCreator, bytes, width, height) { }
        public BitmapLayer(string id, XElement element, ICanvasResourceCreator resourceCreator, IBuffer bytes, int width, int height) : this(id, element, resourceCreator, width, height)
        {
            this.OriginRenderTarget.SetPixelBytes(bytes);
            this.SourceRenderTarget.SetPixelBytes(bytes);

            this.RenderThumbnail();
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height) : this(null, null, resourceCreator, width, height) { }
        public BitmapLayer(string id, XElement element, ICanvasResourceCreator resourceCreator, int width, int height) : base(id, element, resourceCreator, width, height)
        {
            //@DPI
            this.OriginRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.TempRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.Pixels = new byte[width * height * 4];
            this.Buffer = this.Pixels.AsBuffer();
        }

        public XElement Save() => base.Save(this.Type);


        public void RenderThumbnail() => base.RenderThumbnail(this.SourceRenderTarget);

        public void Flush() => this.OriginRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);

        public void CopyPixels(BitmapLayer source) => this.SourceRenderTarget.CopyPixelsFromBitmap(source.SourceRenderTarget);

        public CanvasDrawingSession CreateDrawingSession(BitmapType type = BitmapType.Source)
        {
            switch (type)
            {
                case BitmapType.Origin: return this.OriginRenderTarget.CreateDrawingSession();
                case BitmapType.Source: return this.SourceRenderTarget.CreateDrawingSession();
                case BitmapType.Temp: return this.TempRenderTarget.CreateDrawingSession();
                default: return this.SourceRenderTarget.CreateDrawingSession();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OriginRenderTarget.Dispose();
            this.SourceRenderTarget.Dispose();
            this.TempRenderTarget.Dispose();
        }
    }
}