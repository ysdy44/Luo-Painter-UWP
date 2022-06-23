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


        #region Property

        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }

        public int Width() => this.Right - this.Left;
        public int Height() => this.Bottom - this.Top;

        #endregion


        #region Math

        public PixelBounds Scale(int scale) => new PixelBounds
        {
            Left = this.Left * scale,
            Top = this.Top * scale,
            Right = this.Right * scale,
            Bottom = this.Bottom * scale,
        };
        public PixelBounds Scale(int scale, int maxRight, int maxBottom) => new PixelBounds
        {
            Left = this.Left * scale,
            Top = this.Top * scale,
            Right = System.Math.Min(maxRight, this.Right * scale),
            Bottom = System.Math.Min(maxBottom, this.Bottom * scale),
        };

        public PixelBounds Offset(int x, int y) => new PixelBounds
        {
            Left = this.Left + x,
            Top = this.Top + y,
            Right = this.Right + x,
            Bottom = this.Bottom + y
        };

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

        #endregion


        #region Colors

        public static PixelBounds CreateFromBitmap(CanvasBitmap source, int left, int top, int width, int height) => PixelBounds.CreateFromBytes(source.GetPixelColors(left, top, width, height), width, height);
        public static PixelBounds CreateFromBitmap(CanvasBitmap source, int left, int top, int width, int height, int maxRight, int maxBottom) => PixelBounds.CreateFromBitmap(source, left, top, (left + width > maxRight) ? (maxRight - left) : width, (top + height > maxBottom) ? (maxBottom - top) : height);
        public static PixelBounds CreateFromBytes(Color[] source, int width, int height)
        {
            PixelBounds rect = PixelBounds.Zero;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte a = source[y * width + x].A;
                    if (a is byte.MinValue) continue;

                    if (rect.Left > x) rect.Left = x;
                    if (rect.Top > y) rect.Top = y;
                    if (rect.Right < x + 1) rect.Right = x + 1;
                    if (rect.Bottom < y + 1) rect.Bottom = y + 1;
                }
            }

            return rect;
        }

        public static int CreateLeftFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height) => PixelBounds.CreateLeftFromBytes(bitmap.GetPixelColors(left, top, width, height), width, height);
        public static int CreateLeftFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height, int maxRight, int maxBottom) => PixelBounds.CreateLeftFromBitmap(bitmap, left, top, (left + width > maxRight) ? (maxRight - left) : width, (top + height > maxBottom) ? (maxBottom - top) : height);
        public static int CreateLeftFromBytes(Color[] source, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte a = source[x + y * width].A;
                    if (a is byte.MinValue) continue;
                    else return x;
                }
            }

            return int.MaxValue;
        }

        public static int CreateTopFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height) => PixelBounds.CreateTopFromBytes(bitmap.GetPixelColors(left, top, width, height), width, height);
        public static int CreateTopFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height, int maxRight, int maxBottom) => PixelBounds.CreateTopFromBitmap(bitmap, left, top, (left + width > maxRight) ? (maxRight - left) : width, (top + height > maxBottom) ? (maxBottom - top) : height);
        public static int CreateTopFromBytes(Color[] source, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte a = source[x + y * width].A;
                    if (a is byte.MinValue) continue;
                    else return y;
                }
            }

            return int.MaxValue;
        }

        public static int CreateRightFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height) => PixelBounds.CreateRightFromBytes(bitmap.GetPixelColors(left, top, width, height), width, height);
        public static int CreateRightFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height, int maxRight, int maxBottom) => PixelBounds.CreateRightFromBitmap(bitmap, left, top, (left + width > maxRight) ? (maxRight - left) : width, (top + height > maxBottom) ? (maxBottom - top) : height);
        public static int CreateRightFromBytes(Color[] source, int width, int height)
        {
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    byte a = source[x + y * width].A;
                    if (a is byte.MinValue) continue;
                    else return x;
                }
            }

            return int.MinValue;
        }

        public static int CreateBottomFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height) => PixelBounds.CreateBottomFromBytes(bitmap.GetPixelColors(left, top, width, height), width, height);
        public static int CreateBottomFromBitmap(CanvasBitmap bitmap, int left, int top, int width, int height, int maxRight, int maxBottom) => PixelBounds.CreateBottomFromBitmap(bitmap, left, top, (left + width > maxRight) ? (maxRight - left) : width, (top + height > maxBottom) ? (maxBottom - top) : height);
        public static int CreateBottomFromBytes(Color[] source, int width, int height)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    byte a = source[x + y * width].A;
                    if (a is byte.MinValue) continue;
                    else return y;
                }
            }

            return int.MinValue;
        }

        #endregion


        #region Operator

        public override string ToString() => $"{this.Left},{this.Top},{this.Right},{this.Bottom}";
        public static bool operator ==(PixelBounds left, PixelBounds right) => left.Equals(right);
        public static bool operator !=(PixelBounds left, PixelBounds right) => !left.Equals(right);

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) => base.Equals(obj);
        public bool Equals(PixelBounds other)
        {
            if (this.Left != other.Left) return false;
            if (this.Top != other.Top) return false;
            if (this.Right != other.Right) return false;
            if (this.Bottom != other.Bottom) return false;
            return true;
        }

        #endregion

    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public readonly PixelBounds Bounds; // 0, 0, 250, 250
        readonly CanvasRenderTarget Interpolation;

        public ScaleEffect GetInterpolationScaled() => new ScaleEffect
        {
            Scale = new Vector2(BitmapLayer.Unit),
            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            Source = this.Interpolation
        };


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

                for (int x = 0; x < this.XDivisor; x++)
                {
                    for (int y = 0; y < this.YDivisor; y++)
                    {
                        using (ds.CreateLayer(1, new Rect(x, y, 1, 1)))
                        {
                            ds.DrawImage(new ScaleEffect
                            {
                                Scale = new Vector2(1f / BitmapLayer.Unit),
                                InterpolationMode = CanvasImageInterpolation.Anisotropic,
                                Source = new CropEffect
                                {
                                    SourceRectangle = new Rect(x * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit),
                                    Source = source
                                }
                            });
                        }
                    }
                }

                if (this.RegionType.HasFlag(RegionType.YRemainder))
                {
                    for (int x = 0; x < this.XDivisor; x++)
                    {
                        using (ds.CreateLayer(1, new Rect(x, this.YDivisor, 1, 1)))
                        {
                            ds.DrawImage(new ScaleEffect
                            {
                                Scale = new Vector2(1f / BitmapLayer.Unit, 1f / this.YRemainder),
                                InterpolationMode = CanvasImageInterpolation.Anisotropic,
                                Source = new CropEffect
                                {
                                    SourceRectangle = new Rect(x * BitmapLayer.Unit, this.YDivisor * BitmapLayer.Unit, BitmapLayer.Unit, this.YRemainder),
                                    Source = source
                                }
                            });
                        }
                    }
                }

                if (this.RegionType.HasFlag(RegionType.XRemainder))
                {
                    for (int y = 0; y < this.YDivisor; y++)
                    {
                        using (ds.CreateLayer(1, new Rect(this.XDivisor, y, 1, 1)))
                        {
                            ds.DrawImage(new ScaleEffect
                            {
                                Scale = new Vector2(1f / this.XRemainder, 1f / BitmapLayer.Unit),
                                InterpolationMode = CanvasImageInterpolation.Anisotropic,
                                Source = new CropEffect
                                {
                                    SourceRectangle = new Rect(this.XDivisor * BitmapLayer.Unit, y * BitmapLayer.Unit, this.XRemainder, BitmapLayer.Unit),
                                    Source = source
                                }
                            });
                        }
                    }
                }

                if (this.RegionType.HasFlag(RegionType.XYRemainder))
                {
                    using (ds.CreateLayer(1, new Rect(this.XDivisor, this.YDivisor, 1, 1)))
                    {
                        ds.DrawImage(new ScaleEffect
                        {
                            Scale = new Vector2(1f / this.XRemainder, 1f / this.YRemainder),
                            InterpolationMode = CanvasImageInterpolation.Anisotropic,
                            Source = new CropEffect
                            {
                                SourceRectangle = new Rect(this.XDivisor * BitmapLayer.Unit, this.YDivisor * BitmapLayer.Unit, this.XRemainder, this.YRemainder),
                                Source = source
                            }
                        });
                    }
                }

                ds.Blend = CanvasBlend.SourceOver;

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


        public PixelBoundsMode GetInterpolationBoundsMode(Color[] interpolationColors)
        {
            bool isSolid = true;
            bool isTransarent = true;

            foreach (Color item in interpolationColors)
            {
                byte a = item.A;

                if (a is byte.MinValue) isSolid = false;
                else if (a is byte.MaxValue) isTransarent = false;
                else return PixelBoundsMode.None;
            }

            if (isSolid) return PixelBoundsMode.Solid;
            else if (isTransarent) return PixelBoundsMode.Transarent;
            else return PixelBoundsMode.None;
        }


        public PixelBounds CreateInterpolationBounds(Color[] interpolationColors)
        {
            return PixelBounds.CreateFromBytes(interpolationColors, this.XLength, this.YLength);
        }
        public PixelBounds CreateInterpolationBounds() => new PixelBounds
        {
            Left = 0,
            Top = 0,
            Right = this.XLength,
            Bottom = this.YLength
        };


        public PixelBounds CreatePixelBounds(PixelBounds interpolationBounds)
        {
            PixelBounds b2 = interpolationBounds.Scale(BitmapLayer.Unit, this.Width, this.Height);

            int width = b2.Width();
            int height = b2.Height();

            Color[] colorsLeft = this.SourceRenderTarget.GetPixelColors(b2.Left, b2.Top, BitmapLayer.Unit, height);
            int left = PixelBounds.CreateLeftFromBytes(colorsLeft, BitmapLayer.Unit, height);
            if (left is int.MaxValue) left = BitmapLayer.Unit;

            Color[] colorsTop = this.SourceRenderTarget.GetPixelColors(b2.Left, b2.Top, width, BitmapLayer.Unit);
            int top = PixelBounds.CreateTopFromBytes(colorsTop, width, BitmapLayer.Unit);
            if (top is int.MaxValue) top = BitmapLayer.Unit;

            Color[] colorsRight = this.SourceRenderTarget.GetPixelColors(b2.Right - BitmapLayer.Unit, b2.Top, BitmapLayer.Unit, height);
            int right = PixelBounds.CreateRightFromBytes(colorsRight, BitmapLayer.Unit, height);
            if (right is int.MinValue) right = 0;

            Color[] colorsBottom = this.SourceRenderTarget.GetPixelColors(b2.Left, b2.Bottom - BitmapLayer.Unit, width, BitmapLayer.Unit);
            int bottom = PixelBounds.CreateBottomFromBytes(colorsBottom, width, BitmapLayer.Unit);
            if (bottom is int.MinValue) bottom = 0;

            // ⬛ Rect 0,0,200,200
            return new PixelBounds
            {
                Left = left + b2.Left,
                Top = top + b2.Top,
                Right = b2.Right - BitmapLayer.Unit + right,
                Bottom = b2.Bottom - BitmapLayer.Unit + bottom
            };
        }

        public PixelBounds CreatePixelBounds(PixelBounds interpolationBounds, Color[] interpolationColors)
        {
            // Left
            int indexLeft = interpolationBounds.Left;
            int maxLeft = BitmapLayer.Unit;
            for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
            {
                byte a = interpolationColors[indexLeft + y * this.XLength].A;
                if (a is byte.MinValue) continue;

                //this.Hits[indexLeft + y * this.XLength] = true;
                int left = PixelBounds.CreateLeftFromBitmap(this.SourceRenderTarget, indexLeft * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (left is int.MaxValue) continue;

                if (maxLeft > left) maxLeft = left;
            }

            // Top
            int indexTop = interpolationBounds.Top;
            int maxTop = BitmapLayer.Unit;
            for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
            {
                byte a = interpolationColors[x + indexTop * this.XLength].A;
                if (a is byte.MinValue) continue;

                //this.Hits[x + indexTop * this.XLength] = true;
                int top = PixelBounds.CreateTopFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexTop * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (top is int.MaxValue) continue;

                if (maxTop > top) maxTop = top;
            }

            // Right
            int indexRight = interpolationBounds.Right - 1;
            int minRight = BitmapLayer.Unit;
            for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
            {
                byte a = interpolationColors[indexRight + y * this.XLength].A;
                if (a is byte.MinValue) continue;

                //this.Hits[indexRight + y * this.XLength] = true;
                int right = PixelBounds.CreateRightFromBitmap(this.SourceRenderTarget, indexRight * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (right is int.MinValue) continue;

                if (minRight < right) minRight = right;
            }

            // Bottom
            int indexBottom = interpolationBounds.Bottom - 1;
            int minBottom = BitmapLayer.Unit;
            for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
            {
                byte a = interpolationColors[x + indexBottom * this.XLength].A;
                if (a is byte.MinValue) continue;

                //this.Hits[x + indexBottom * this.XLength] = true;
                int bottom = PixelBounds.CreateBottomFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexBottom * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (bottom is int.MinValue) continue;

                if (minBottom < bottom) minBottom = bottom;
            }


            PixelBounds b2 = interpolationBounds.Scale(BitmapLayer.Unit, this.Width, this.Height);
            return new PixelBounds
            {
                Left = maxLeft + b2.Left,
                Top = maxTop + b2.Top,
                Right = b2.Right - BitmapLayer.Unit + minRight,
                Bottom = b2.Bottom - BitmapLayer.Unit + minBottom
            };
        }

    }
}