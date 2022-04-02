using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public ImageSource Thumbnail => this.ThumbnailWriteableBitmap;
        readonly CanvasRenderTarget ThumbnailRenderTarget;
        readonly WriteableBitmap ThumbnailWriteableBitmap;

        public void RenderThumbnail()
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(new ScaleEffect
                {
                    Scale = new Vector2(50f / Math.Max(this.Width, this.Height)),
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = this.SourceRenderTarget,
                });
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