using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
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

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height) : base(resourceCreator, width, height)
        {
            //@DPI
            this.OriginRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.TempRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);


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

            this.Bounds = new PixelBounds
            {
                Left = 0,
                Top = 0,
                Right = width,
                Bottom = height,
            };
            this.Interpolation = new CanvasRenderTarget(resourceCreator, this.XLength, this.YLength, 96);
        }

        public void RenderThumbnail() => base.RenderThumbnail(this.Source);

        public void Flush() => this.OriginRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);

        public void CopyPixels(BitmapLayer source, BitmapType sourceType = BitmapType.Source, BitmapType destinationType = BitmapType.Source) => this[destinationType].CopyPixelsFromBitmap(source[sourceType]);

        public CanvasDrawingSession CreateDrawingSession(BitmapType type = BitmapType.Source) => this[type].CreateDrawingSession();

        public void Draw(ICanvasImage image, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.DrawImage(image);
            }
        }

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
        public void DrawCopy(ICanvasImage image1, ICanvasImage image2, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image1);
                ds.Blend = CanvasBlend.SourceOver;
                ds.DrawImage(image2);
            }
        }

        public void Clear(ICanvasBrush brush, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.FillRectangle(0, 0, this.Width, this.Height, brush);
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