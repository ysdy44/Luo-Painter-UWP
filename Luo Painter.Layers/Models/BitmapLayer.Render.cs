using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    partial class BitmapLayer
    {
        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.SourceRenderTarget);
        public ICanvasImage Render(ICanvasImage background, string id, ICanvasImage mezzanine) => base.Render(background,
            (base.Id == id) ? mezzanine : this.SourceRenderTarget);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.SourceRenderTarget
        });
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.SourceRenderTarget
        });

        public void Merge(ILayer neighbor)
        {
            if (neighbor.Opacity == 0.0) return;
            else if (neighbor.Opacity == 1.0)
            {
                ICanvasImage image = neighbor[BitmapType.Source];
                if (base.Opacity == 0.0)
                    this.DrawCopy(image);
                else
                    this.DrawCopy(this.Render(image, this.OriginRenderTarget));
            }
            else
            {
                using (ICanvasImage image = new OpacityEffect
                {
                    Source = neighbor[BitmapType.Source],
                    Opacity = neighbor.Opacity
                })
                {
                    if (base.Opacity == 0.0)
                        this.DrawCopy(image);
                    else
                        this.DrawCopy(this.Render(image, this.OriginRenderTarget));
                }
            }
        }

        public void Draw(ICanvasImage image, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.DrawImage(image);
            }
        }
        public void Draw(ICanvasImage image, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            using (ds.CreateLayer(1, clipRectangle))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.DrawImage(image);
            }
        }

        public void DrawCopy(ICanvasImage image, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image);
            }
        }
        public void DrawCopy(ICanvasImage image, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            using (ds.CreateLayer(1, clipRectangle))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image);
            }
        }
        public void DrawCopy(ICanvasImage image1, ICanvasImage image2, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image1);
                ds.Blend = CanvasBlend.SourceOver;
                ds.DrawImage(image2);
            }
        }
        public void DrawCopy(ICanvasImage image1, ICanvasImage image2, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            using (ds.CreateLayer(1, clipRectangle))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image1);
                ds.Blend = CanvasBlend.SourceOver;
                ds.DrawImage(image2);
            }
        }

        public void Fill(Color color, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.FillRectangle(0, 0, base.Width, base.Height, color);
            }
        }
        public void Fill(Color color, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.FillRectangle(clipRectangle, color);
            }
        }

        public void Fill(ICanvasBrush brush, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.FillRectangle(0, 0, base.Width, base.Height, brush);
            }
        }
        public void Fill(ICanvasBrush brush, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.FillRectangle(clipRectangle, brush);
            }
        }

        public void Clear(ICanvasBrush brush, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.FillRectangle(0, 0, base.Width, base.Height, brush);
            }
        }
        public void Clear(ICanvasBrush brush, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.FillRectangle(clipRectangle, brush);
            }
        }

        public void Clear(Color color, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                ds.Clear(color);
            }
        }
        public void Clear(Color color, Rect clipRectangle, BitmapType type = BitmapType.Source)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            using (ds.CreateLayer(1, clipRectangle))
            {
                ds.Clear(color);
            }
        }
    }
}