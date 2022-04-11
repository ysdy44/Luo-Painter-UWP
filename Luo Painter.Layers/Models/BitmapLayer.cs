using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Layers.Models
{
    public enum ThumbnailType
    {
        None,
        Origin,
        Oversize,
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public float ConvertValueToOne(float value) => value / Math.Max(this.Width, this.Height);
        public float ConvertOneToValue(float one) => one * Math.Max(this.Width, this.Height);

        public Vector2 ConvertValueToOne(Vector2 value) => new Vector2(value.X / this.Width, value.Y / this.Height);
        public Vector2 ConvertOneToValue(Vector2 one) => new Vector2(one.X * this.Width, one.Y * this.Height);


        public LayerType Type => LayerType.Bitmap;


        public ImageSource Thumbnail => this.ThumbnailWriteableBitmap;
        readonly CanvasRenderTarget ThumbnailRenderTarget;
        readonly WriteableBitmap ThumbnailWriteableBitmap;
        readonly ThumbnailType ThumbnailType;
        readonly Vector2 ThumbnailScale;


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


        public void Shade(PixelShaderEffect shader, Rect rect)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = shader
                });
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = this.TempRenderTarget
                });
            }
        }


        public void RenderThumbnail()
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                switch (this.ThumbnailType)
                {
                    case ThumbnailType.Origin:
                        ds.DrawImage(this.SourceRenderTarget);
                        break;
                    case ThumbnailType.Oversize:
                        ds.DrawImage(new ScaleEffect
                        {
                            Scale = new Vector2(0.01f),
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            Source = this.SourceRenderTarget,
                        });
                        break;
                    default:
                        ds.DrawImage(new ScaleEffect
                        {
                            Scale = this.ThumbnailScale,
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            Source = this.SourceRenderTarget,
                        });
                        break;
                }
            }

            byte[] bytes = this.ThumbnailRenderTarget.GetPixelBytes();
            using (Stream stream = this.ThumbnailWriteableBitmap.PixelBuffer.AsStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.ThumbnailWriteableBitmap.Invalidate();
        }
        public void ClearThumbnail(Color color)
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Clear(color);
            }

            byte[] bytes = this.ThumbnailRenderTarget.GetPixelBytes();
            using (Stream stream = this.ThumbnailWriteableBitmap.PixelBuffer.AsStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.ThumbnailWriteableBitmap.Invalidate();
        }

    }
}