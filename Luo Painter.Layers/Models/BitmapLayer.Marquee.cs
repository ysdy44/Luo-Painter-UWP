﻿using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

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
                Source = this.Source,
                AlphaMask = marquee.SourceRenderTarget
            };
        }


        public IHistory Clear(BitmapLayer marquee, Color[] interpolationColors)
        {
            PixelBounds bounds = marquee.CreateInterpolationBounds(interpolationColors);
            Rect rect = bounds.ToRect(BitmapLayer.Unit);

            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.DrawImage(marquee.SourceRenderTarget, (float)rect.Left, (float)rect.Top, rect, 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationOut);
            }
            this.Hit(interpolationColors);

            return this.GetBitmapHistory();
        }

        public IHistory Invert(Color color)
        {
            this.TempRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);
            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.Clear(color);
                ds.DrawImage(this.Temp, 0, 0, new Rect(0, 0, this.Width, this.Height), 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationOut);
            }

            return this.GetBitmapResetHistory();
        }

        public IHistory Pixel(BitmapLayer bitmapLayer, Color color)
        {
            this.TempRenderTarget.CopyPixelsFromBitmap(bitmapLayer.SourceRenderTarget);
            using (CanvasDrawingSession ds = this.CreateDrawingSession())
            {
                ds.Clear(color);
                ds.DrawImage(this.Temp, 0, 0, new Rect(0, 0, this.Width, this.Height), 1, CanvasImageInterpolation.NearestNeighbor, CanvasComposite.DestinationIn);
            }

            return this.GetBitmapResetHistory();
        }

    }
}