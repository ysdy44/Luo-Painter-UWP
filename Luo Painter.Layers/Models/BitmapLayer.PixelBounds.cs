using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
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
        public PixelBounds CreateInterpolationBoundsScaled(Color[] interpolationColors)
        {
            return PixelBounds.CreateFromBytes(interpolationColors, this.XLength, this.YLength).Scale(BitmapLayer.Unit, this.Width, this.Height);
        }
        public PixelBounds CreateInterpolationBounds() => new PixelBounds
        {
            Left = 0,
            Top = 0,
            Right = this.XLength,
            Bottom = this.YLength
        };


        public PixelBounds CreatePixelBounds(PixelBounds interpolationBounds, Color[] interpolationColors)
        {
            // Left
            bool hasMaxLeft = false;
            int indexLeft = interpolationBounds.Left;
            int maxLeft = BitmapLayer.Unit;
            for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
            {
                byte a = interpolationColors[indexLeft + y * this.XLength].A;
                if (a is byte.MinValue) continue;

                int left = PixelBounds.CreateLeftFromBitmap(this.SourceRenderTarget, indexLeft * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (left is int.MaxValue) continue;

                if (maxLeft > left)
                {
                    hasMaxLeft = true;
                    maxLeft = left;
                }
            }

            if (hasMaxLeft is false)
            {
                indexLeft = interpolationBounds.Left + 1;
                maxLeft = BitmapLayer.Unit;
                for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
                {
                    byte a = interpolationColors[indexLeft + y * this.XLength].A;
                    if (a is byte.MinValue) continue;

                    int left = PixelBounds.CreateLeftFromBitmap(this.SourceRenderTarget, indexLeft * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                    if (left is int.MaxValue) continue;

                    if (maxLeft > left)
                    {
                        maxLeft = left;
                    }
                }
            }


            // Top
            int indexTop = interpolationBounds.Top;
            int maxTop = BitmapLayer.Unit;
            bool hasMaxTop = false;
            for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
            {
                byte a = interpolationColors[x + indexTop * this.XLength].A;
                if (a is byte.MinValue) continue;

                int top = PixelBounds.CreateTopFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexTop * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (top is int.MaxValue) continue;

                if (maxTop > top)
                {
                    hasMaxTop = true;
                    maxTop = top;
                }
            }

            if (hasMaxTop is false)
            {
                indexTop = interpolationBounds.Top + 1;
                maxTop = BitmapLayer.Unit;
                for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
                {
                    byte a = interpolationColors[x + indexTop * this.XLength].A;
                    if (a is byte.MinValue) continue;

                    int top = PixelBounds.CreateTopFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexTop * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                    if (top is int.MaxValue) continue;

                    if (maxTop > top)
                    {
                        maxTop = top;
                    }
                }
            }


            // Right
            int indexRight = interpolationBounds.Right - 1;
            int minRight = 0;
            bool hasMinRight = false;
            for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
            {
                byte a = interpolationColors[indexRight + y * this.XLength].A;
                if (a is byte.MinValue) continue;

                int right = PixelBounds.CreateRightFromBitmap(this.SourceRenderTarget, indexRight * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (right is int.MinValue) continue;

                if (minRight < right)
                {
                    hasMinRight = true;
                    minRight = right;
                }
            }

            if (hasMinRight is false)
            {
                indexRight = interpolationBounds.Right - 2;
                minRight = 0;
                for (int y = interpolationBounds.Top; y < interpolationBounds.Bottom; y++)
                {
                    byte a = interpolationColors[indexRight + y * this.XLength].A;
                    if (a is byte.MinValue) continue;

                    int right = PixelBounds.CreateRightFromBitmap(this.SourceRenderTarget, indexRight * BitmapLayer.Unit, y * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                    if (right is int.MinValue) continue;

                    if (minRight < right)
                    {
                        minRight = right;
                    }
                }
            }


            // Bottom
            int indexBottom = interpolationBounds.Bottom - 1;
            int minBottom = 0;
            bool hasMinBottom = false;
            for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
            {
                byte a = interpolationColors[x + indexBottom * this.XLength].A;
                if (a is byte.MinValue) continue;

                int bottom = PixelBounds.CreateBottomFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexBottom * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                if (bottom is int.MinValue) continue;

                if (minBottom < bottom)
                {
                    hasMinBottom = true;
                    minBottom = bottom;
                }
            }

            if (hasMinBottom is false)
            {
                indexBottom = interpolationBounds.Bottom - 2;
                minBottom = 0;
                for (int x = interpolationBounds.Left; x < interpolationBounds.Right; x++)
                {
                    byte a = interpolationColors[x + indexBottom * this.XLength].A;
                    if (a is byte.MinValue) continue;

                    int bottom = PixelBounds.CreateBottomFromBitmap(this.SourceRenderTarget, x * BitmapLayer.Unit, indexBottom * BitmapLayer.Unit, BitmapLayer.Unit, BitmapLayer.Unit, this.Width, this.Height);
                    if (bottom is int.MinValue) continue;

                    if (minBottom < bottom)
                    {
                        minBottom = bottom;
                    }
                }
            }

            return new PixelBounds
            {
                Left = indexLeft * BitmapLayer.Unit + maxLeft,
                Top = indexTop * BitmapLayer.Unit + maxTop,
                Right = System.Math.Min(this.Width, indexRight * BitmapLayer.Unit + minRight),
                Bottom = System.Math.Min(this.Height, indexBottom * BitmapLayer.Unit + minBottom)
            };
        }

    }
}