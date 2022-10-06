using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Numerics;
using Windows.UI;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Graphics.Canvas.Effects;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage.Streams;

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


        //@Construct
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap) : this(resourceCreator, (int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height)
        {
            this.OriginRenderTarget.CopyPixelsFromBitmap(bitmap);
            this.SourceRenderTarget.CopyPixelsFromBitmap(bitmap);

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
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap, int width, int height, Vector2 offset) : this(resourceCreator, width, height)
        {
            int x = (int)offset.X;
            int y = (int)offset.Y;

            int w = System.Math.Min(width, (int)bitmap.SizeInPixels.Width);
            int h = System.Math.Min(height, (int)bitmap.SizeInPixels.Height);

            this.OriginRenderTarget.CopyPixelsFromBitmap(bitmap, x, y, 0, 0, w, h);
            this.SourceRenderTarget.CopyPixelsFromBitmap(bitmap, x, y, 0, 0, w, h);

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


            this.XDivisor = width / BitmapLayer.Unit;
            this.YDivisor = height / BitmapLayer.Unit;
            this.XRemainder = width % BitmapLayer.Unit;
            this.YRemainder = height % BitmapLayer.Unit;


            this.XLength = this.XDivisor;
            this.YLength = this.YDivisor;
            if (this.XRemainder is 0 is false)
            {
                this.RegionType |= RegionType.XRemainder;
                this.XLength += 1;
            }
            if (this.YRemainder is 0 is false)
            {
                this.RegionType |= RegionType.YRemainder;
                this.YLength += 1;
            }


            this.Hits = new bool[this.XLength * this.YLength];

            this.Bounds = new PixelBounds
            {
                Left = 0,
                Top = 0,
                Right = width,
                Bottom = height,
            };
            this.Interpolation = new CanvasRenderTarget(resourceCreator, this.XLength, this.YLength, 96);
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

            this.Interpolation.Dispose();
        }
    }
}