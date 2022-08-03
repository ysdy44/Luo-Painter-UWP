using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers
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
}