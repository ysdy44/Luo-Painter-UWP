using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public struct PixelBounds
    {
        public static readonly PixelBounds Zero = new PixelBounds
        {
            Left = int.MaxValue,
            Top = int.MaxValue,
            Right = int.MinValue,
            Bottom = int.MinValue
        };

        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }

        public int Width() => this.Right - this.Left;
        public int Height() => this.Bottom - this.Top;

        public Rect ToRect() => new Rect
        {
            X = this.Left,
            Y = this.Top,
            Width = this.Width(),
            Height = this.Height()
        };

        public Rect ToRect(double scale) => new Rect
        {
            X = this.Left * scale,
            Y = this.Top * scale,
            Width = this.Width() * scale,
            Height = this.Height() * scale
        };

        public static PixelBounds CreateFromBytes(Color[] source, int w, int h)
        {
            PixelBounds rect = PixelBounds.Zero;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color color = source[index];

                    if (color.A != byte.MinValue) // 0
                    {
                        if (rect.Left > x) rect.Left = x;
                        if (rect.Top > y) rect.Top = y;
                        if (rect.Right < x + 1) rect.Right = x + 1;
                        if (rect.Bottom < y + 1) rect.Bottom = y + 1;
                    }
                }
            }

            return rect;
        }

        public override string ToString() => $"{this.Left},{this.Top},{this.Right},{this.Bottom}";
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public PixelBounds CreateInterpolationBounds()
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
                ds.DrawImage(new ScaleEffect
                {
                    Scale = new Vector2(1f / BitmapLayer.Unit),
                    InterpolationMode = CanvasImageInterpolation.HighQualityCubic,
                    Source = this.SourceRenderTarget,
                });
            }

            return PixelBounds.CreateFromBytes(this.TempRenderTarget.GetPixelColors(0, 0, this.XLength, this.YLength), this.XLength, this.YLength);
        }

        public PixelBounds CreatePixelBounds(PixelBounds interpolationBounds)
        {
            interpolationBounds.Left *= BitmapLayer.Unit;
            interpolationBounds.Top *= BitmapLayer.Unit;
            interpolationBounds.Right *= BitmapLayer.Unit;
            interpolationBounds.Bottom *= BitmapLayer.Unit;
            interpolationBounds.Right = System.Math.Min(this.Width, interpolationBounds.Right);
            interpolationBounds.Bottom = System.Math.Min(this.Height, interpolationBounds.Bottom);

            int width = interpolationBounds.Width();
            int height = interpolationBounds.Height();

            if (this.Width < BitmapLayer.Unit + BitmapLayer.Unit || this.Height < BitmapLayer.Unit + BitmapLayer.Unit)
            {
                // ⬛ Rect 0,0,200,200
                PixelBounds bounds = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, width, height), width, height);
                return new PixelBounds
                {
                    Left = bounds.Left + interpolationBounds.Left,
                    Top = bounds.Top + interpolationBounds.Top,
                    Right = bounds.Right + interpolationBounds.Left,
                    Bottom = bounds.Bottom + interpolationBounds.Top
                };
            }
            else
            {
                // ◧ Rect 0,0,100,1024
                PixelBounds leftBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, BitmapLayer.Unit, height), BitmapLayer.Unit, height);
                // ⬓ Rect 0,0,1024,100
                PixelBounds topBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, width, BitmapLayer.Unit), width, BitmapLayer.Unit);
                // ◨ Rect 924,0,100,1024 
                PixelBounds rightBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Right - BitmapLayer.Unit, interpolationBounds.Top, BitmapLayer.Unit, height), BitmapLayer.Unit, height);
                // ⬒ Rect 0,924,1024,100
                PixelBounds bottomBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Bottom - BitmapLayer.Unit, width, BitmapLayer.Unit), width, BitmapLayer.Unit);

                return new PixelBounds
                {
                    Left = leftBar.Left + interpolationBounds.Left,
                    Top = topBar.Top + interpolationBounds.Top,
                    Right = rightBar.Right - BitmapLayer.Unit + interpolationBounds.Right,
                    Bottom = bottomBar.Bottom - BitmapLayer.Unit + interpolationBounds.Bottom,
                };
            }
        }

    }
}