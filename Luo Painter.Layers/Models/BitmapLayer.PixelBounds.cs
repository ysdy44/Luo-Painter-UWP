using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public enum PixelBoundsMode
    {
        Transarent,
        Solid,
        None,
    }

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

        public FanKit.Transformers.TransformerBorder ToBorder() => new FanKit.Transformers.TransformerBorder(this.Left, this.Top, this.Right, this.Bottom);
        public FanKit.Transformers.TransformerBorder ToBorder(float scale) => new FanKit.Transformers.TransformerBorder(this.Left * scale, this.Top * scale, this.Right * scale, this.Bottom * scale);

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
        public static bool operator ==(PixelBounds left, PixelBounds right) => left.Equals(right);
        public static bool operator !=(PixelBounds left, PixelBounds right) => !left.Equals(right);
        public bool Equals(PixelBounds other)
        {
            if (this.Left != other.Left) return false;
            if (this.Top != other.Top) return false;
            if (this.Right != other.Right) return false;
            if (this.Bottom != other.Bottom) return false;
            return true;
        }
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public readonly PixelBounds Bounds; // 0, 0, 250, 250
        readonly CanvasRenderTarget Interpolation;

        private bool IsTransparent(Color color) => color.A == byte.MinValue;
        private bool IsSolid(Color color) => color.A == byte.MaxValue;

        public Color[] GetInterpolationColorsBySource() => this.GetInterpolationColors(this.SourceRenderTarget);
        public Color[] GetInterpolationColorsByDifference() => this.GetInterpolationColors(new LuminanceToAlphaEffect
        {
            Source = new BlendEffect
            {
                Mode = BlendEffectMode.Difference,
                Foreground = this.SourceRenderTarget,
                Background = this.OriginRenderTarget
            }
        });
        public Color[] GetInterpolationColors(IGraphicsEffectSource source)
        {
            using (CanvasDrawingSession ds = this.Interpolation.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(new ScaleEffect
                {
                    Scale = new Vector2(1f / BitmapLayer.Unit),
                    InterpolationMode = CanvasImageInterpolation.Anisotropic,
                    Source = new BorderEffect
                    {
                        ExtendX = CanvasEdgeBehavior.Mirror,
                        ExtendY = CanvasEdgeBehavior.Mirror,
                        Source = new CropEffect
                        {
                            SourceRectangle = new Rect(0, 0, this.Width, this.Height),
                            BorderMode = EffectBorderMode.Hard,
                            Source = source
                        }
                    }
                });
            }

            return this.Interpolation.GetPixelColors();
        }

        public PixelBoundsMode GetInterpolationBoundsMode(Color[] InterpolationColors) =>
            InterpolationColors.All(this.IsTransparent) ?
            PixelBoundsMode.Transarent :
            InterpolationColors.All(this.IsSolid) ?
            PixelBoundsMode.Solid :
            PixelBoundsMode.None;

        public PixelBounds CreateInterpolationBounds(Color[] InterpolationColors)
        {
            return PixelBounds.CreateFromBytes(InterpolationColors, this.XLength, this.YLength);
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

            // ⬛ Rect 0,0,200,200
            PixelBounds bounds = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, width, height), width, height);
            return new PixelBounds
            {
                Left = bounds.Left + interpolationBounds.Left,
                Top = bounds.Top + interpolationBounds.Top,
                Right = bounds.Right + interpolationBounds.Left,
                Bottom = bounds.Bottom + interpolationBounds.Top
            };

            //if (width < BitmapLayer.Unit + BitmapLayer.Unit || height < BitmapLayer.Unit + BitmapLayer.Unit)
            //{
            //    // ⬛ Rect 0,0,200,200
            //    PixelBounds bounds = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, width, height), width, height);
            //    return new PixelBounds
            //    {
            //        Left = bounds.Left + interpolationBounds.Left,
            //        Top = bounds.Top + interpolationBounds.Top,
            //        Right = bounds.Right + interpolationBounds.Left,
            //        Bottom = bounds.Bottom + interpolationBounds.Top
            //    };
            //}
            //else
            //{
            //    // ◧ Rect 0,0,100,1024
            //    PixelBounds leftBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, BitmapLayer.Unit, height), BitmapLayer.Unit, height);
            //    // ⬓ Rect 0,0,1024,100
            //    PixelBounds topBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Top, width, BitmapLayer.Unit), width, BitmapLayer.Unit);
            //    // ◨ Rect 924,0,100,1024 
            //    PixelBounds rightBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Right - BitmapLayer.Unit, interpolationBounds.Top, BitmapLayer.Unit, height), BitmapLayer.Unit, height);
            //    // ⬒ Rect 0,924,1024,100
            //    PixelBounds bottomBar = PixelBounds.CreateFromBytes(this.SourceRenderTarget.GetPixelColors(interpolationBounds.Left, interpolationBounds.Bottom - BitmapLayer.Unit, width, BitmapLayer.Unit), width, BitmapLayer.Unit);

            //    interpolationBounds.Left += (leftBar == PixelBounds.Zero) ? BitmapLayer.Unit : leftBar.Left;
            //    interpolationBounds.Top += (topBar == PixelBounds.Zero) ? BitmapLayer.Unit : topBar.Top;
            //    interpolationBounds.Right -= (rightBar == PixelBounds.Zero) ? BitmapLayer.Unit : (BitmapLayer.Unit - rightBar.Right);
            //    interpolationBounds.Bottom -= (bottomBar == PixelBounds.Zero) ? BitmapLayer.Unit : (BitmapLayer.Unit - bottomBar.Bottom);
            //    return interpolationBounds;
            //}
        }

    }
}