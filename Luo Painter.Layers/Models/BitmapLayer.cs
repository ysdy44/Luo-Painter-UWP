using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Layers.Models
{
    public enum BitmapType
    {
        Origin,
        Source,
        Temp,
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public float ConvertValueToOne(float value) => value / Math.Max(this.Width, this.Height);
        public float ConvertOneToValue(float one) => one * Math.Max(this.Width, this.Height);

        public Vector2 ConvertValueToOne(Vector2 value) => new Vector2(value.X / this.Width, value.Y / this.Height);
        public Vector2 ConvertOneToValue(Vector2 one) => new Vector2(one.X * this.Width, one.Y * this.Height);


        public LayerType Type => LayerType.Bitmap;


        public ICanvasImage Origin => this.OriginRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public ICanvasImage Temp => this.TempRenderTarget;

        private CanvasRenderTarget this[BitmapType type]
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


        //@Construct
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer) : this(resourceCreator, bitmapLayer.SourceRenderTarget, bitmapLayer.Width, bitmapLayer.Height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, BitmapLayer bitmapLayer, int width, int height) : this(resourceCreator, bitmapLayer.SourceRenderTarget, width, height) { }
        public BitmapLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap bitmap) : this(resourceCreator, bitmap, (int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height) { }

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

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            //@DPI
            this.OriginRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.TempRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.ThumbnailWriteableBitmap = new WriteableBitmap(50, 50);
            this.ThumbnailRenderTarget = new CanvasRenderTarget(resourceCreator, 50, 50, 96);

            int wh = Math.Max(width, height);
            if (wh <= 50) this.ThumbnailType = ThumbnailType.Origin;
            else if (wh >= 5000) this.ThumbnailType = ThumbnailType.Oversize;
            else
            {
                this.ThumbnailScale = new Vector2(50f / wh);
                this.ThumbnailType = ThumbnailType.None;
            }

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


        public void Flush() => this.OriginRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);

        public void CopyPixels(BitmapLayer source, BitmapType sourceType = BitmapType.Source, BitmapType destinationType = BitmapType.Source) => this[destinationType].CopyPixelsFromBitmap(source[sourceType]);

        public CanvasDrawingSession CreateDrawingSession(BitmapType type = BitmapType.Source) => this[type].CreateDrawingSession();

        public void DrawCopy(ICanvasImage image, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image);
            }
        }

        public void Clear(Color color, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                ds.Clear(color);
            }
        }

    }
}