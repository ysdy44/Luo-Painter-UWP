using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public float GetRadius(float rangeSize) => rangeSize / 2f / Math.Max(this.Width, this.Height);

        public void Shade(PixelShaderEffect shader, Rect rect)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = shader
                });
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = this.TempRenderTarget
                });
            }
        }


        private void ErasingDryCore(Vector2 position, Vector2 targetPosition, float size)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.Blend = CanvasBlend.Copy;

                    ds.FillCircle(position, size, Colors.Transparent);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.Blend = CanvasBlend.Copy;

                    ds.FillCircle(position, size, Colors.Transparent);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        var p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, Colors.Transparent);
                    }
                }
            }
        }
        private void ErasingWetCore(Vector2 position, Vector2 targetPosition, float size)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, Colors.White);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, Colors.Transparent);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        var p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, Colors.White);
                    }
                }
            }
        }


        static readonly CanvasStrokeStyle canvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };
        public void DrawLine(Vector2 position, Vector2 targetPosition, Color color)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawLine(position, targetPosition, color, 12, canvasStrokeStyle);
            }
        }


        private void FillCircle(Vector2 position, Vector2 targetPosition, float size, Color color, CanvasRenderTarget renderTarget)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        var p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, color);
                    }
                }
            }
        }

        public void FillCircleFlipX(Vector2 position, Vector2 targetPosition, float size, Color color)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);

                    ds.FillCircle(this.Width - position.X, position.Y, size, color);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, color);

                        ds.FillCircle(this.Width - p.X, p.Y, size, color);
                    }
                }
            }
        }

        public void FillCircleFlipY(Vector2 position, Vector2 targetPosition, float size, Color color)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);

                    ds.FillCircle(position.X, this.Height - position.Y, size, color);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, color);

                        ds.FillCircle(p.X, this.Height - p.Y, size, color);
                    }
                }
            }
        }

        public void FillCircleTwo(Vector2 position, Vector2 targetPosition, float size, Color color)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);

                    ds.FillCircle(this.Width - position.X, this.Height - position.Y, size, color);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, color);

                        ds.FillCircle(this.Width - p.X, this.Height - p.Y, size, color);
                    }
                }
            }
        }

        public void FillCircleFour(Vector2 position, Vector2 targetPosition, float size, Color color)
        {
            float length = Vector2.Distance(position, targetPosition);
            if (length < size / 2)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);

                    ds.FillCircle(this.Width - position.X, this.Height - position.Y, size, color);

                    Vector2 v = position - this.Center;
                    ds.FillCircle(this.Center.X - v.Y, this.Center.Y + v.X, size, color);
                    ds.FillCircle(this.Center.X + v.Y, this.Center.Y - v.X, size, color);
                }
            }
            else
            {
                float spane = size / 2 / length;
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.FillCircle(position, size, color);
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        ds.FillCircle(p, size, color);

                        ds.FillCircle(this.Width - p.X, this.Height - p.Y, size, color);

                        Vector2 v = p - this.Center;
                        ds.FillCircle(this.Center.X - v.Y, this.Center.Y + v.X, size, color);
                        ds.FillCircle(this.Center.X + v.Y, this.Center.Y - v.X, size, color);
                    }
                }
            }
        }

    }
}