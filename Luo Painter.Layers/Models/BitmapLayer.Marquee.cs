using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public enum SelectionType
    {
        None,
        All,
        PixelBounds,
        MarqueePixelBounds
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public SelectionType GetDrawSelection(bool isOpaque, BitmapLayer marquee, out Color[] InterpolationColors, out PixelBoundsMode mode)
        {
            if (isOpaque)
            {
                InterpolationColors = this.GetInterpolationColorsBySource();
                mode = this.GetInterpolationBoundsMode(InterpolationColors);

                switch (mode)
                {
                    case PixelBoundsMode.Transarent: return SelectionType.None;
                    case PixelBoundsMode.Solid: return SelectionType.All;
                    default: return SelectionType.PixelBounds;
                }
            }
            else
            {
                InterpolationColors = marquee.GetInterpolationColorsBySource();
                mode = marquee.GetInterpolationBoundsMode(InterpolationColors);

                switch (mode)
                {
                    case PixelBoundsMode.Transarent: return SelectionType.All;
                    case PixelBoundsMode.Solid: return SelectionType.All;
                    default: return SelectionType.MarqueePixelBounds;
                }
            }
        }

        public SelectionType GetSelection(BitmapLayer marquee, out Color[] InterpolationColors, out PixelBoundsMode mode)
        {
            InterpolationColors = marquee.GetInterpolationColorsBySource();
            mode = marquee.GetInterpolationBoundsMode(InterpolationColors);

            switch (mode)
            {
                case PixelBoundsMode.Solid: return SelectionType.All;
                case PixelBoundsMode.None: return SelectionType.MarqueePixelBounds;
                default:
                    InterpolationColors = this.GetInterpolationColorsBySource();
                    mode = this.GetInterpolationBoundsMode(InterpolationColors);
                    switch (mode)
                    {
                        case PixelBoundsMode.Solid: return SelectionType.All;
                        case PixelBoundsMode.None: return SelectionType.PixelBounds;
                        default: return SelectionType.None;
                    }
            }
        }

        public void Marquee(Vector2 position, Vector2 targetPosition, float strokeWidth, bool isErasing)
        {
            if (isErasing)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    ds.Blend = CanvasBlend.Copy;

                    ds.DrawLine(position, targetPosition, Colors.Transparent, strokeWidth, BitmapLayer.CanvasStrokeStyle);
                }
            }
            else
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    ds.DrawLine(position, targetPosition, Colors.DodgerBlue, strokeWidth, BitmapLayer.CanvasStrokeStyle);
                }
            }
        }


        public AlphaMaskEffect GetMask(BitmapLayer marquee)
        {
            return new AlphaMaskEffect
            {
                Source = this.SourceRenderTarget,
                AlphaMask = marquee.SourceRenderTarget
            };
        }

        public IHistory Add(BitmapLayer marquee, Color[] interpolationColors, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.DrawImage(marquee[type]);
            }
            this.Hit(interpolationColors);

            return this.GetBitmapHistory();
        }

        public IHistory Clear(BitmapLayer marquee, Color[] interpolationColors, BitmapType type = BitmapType.Source)
        {
            PixelBounds bounds = marquee.CreateInterpolationBounds(interpolationColors);
            Rect rect = bounds.ToRect(BitmapLayer.Unit);

            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.DrawImage(marquee[type], (float)rect.Left, (float)rect.Top, rect, 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationOut);
            }
            this.Hit(interpolationColors);

            return this.GetBitmapHistory();
        }

        public IHistory Invert(Color color)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.Clear(color);
                ds.DrawImage(this.OriginRenderTarget, 0, 0, new Rect(0, 0, this.Width, this.Height), 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationOut);
            }

            return this.GetBitmapResetHistory();
        }

        public IHistory Pixel(BitmapLayer bitmapLayer, Color color, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.Clear(color);
                ds.DrawImage(bitmapLayer[type], 0, 0, new Rect(0, 0, this.Width, this.Height), 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationIn);
            }

            return this.GetBitmapResetHistory();
        }

    }
}