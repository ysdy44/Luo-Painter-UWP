using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public ImageSource Thumbnail => this.ThumbnailWriteableBitmap;

        readonly CanvasRenderTarget ThumbnailRenderTarget;
        readonly WriteableBitmap ThumbnailWriteableBitmap;
        readonly ThumbnailType ThumbnailType;
        readonly Vector2 ThumbnailScale;

        public void RenderThumbnail()
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Blend = CanvasBlend.Copy;
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