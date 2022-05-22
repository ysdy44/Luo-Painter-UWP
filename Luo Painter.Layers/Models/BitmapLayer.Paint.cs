using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public enum InkMode
    {
        None,

        Dry,
        WetWithOpacity,
        WetWithBlendMode,
        WetWithOpacityAndBlendMode,

        EraseDry,
        EraseWetWithOpacity,

        Liquefy,
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        //@Static
        public static readonly Vector4 DisplacementColor = new Vector4(0.5f, 0.5f, 1, 1);
        public static readonly Vector4 DodgerBlue = new Vector4(0.11764705882352941176470588235294f, 0.56470588235294117647058823529412f, 1, 1);
        private static readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };


        public InkMode InkMode { get; set; }


        public void ClearDisplacement()
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
        }


        public void Shade(IGraphicsEffectSource source, Windows.Foundation.Rect rect)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = source
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


        public CompositeEffect GetWeting(float opacity) => new CompositeEffect
        {
            Sources =
            {
                this.OriginRenderTarget,
                new OpacityEffect
                {
                    Source = this.TempRenderTarget,
                    Opacity = opacity
                }
            }
        };
        public BlendEffect GetWeting(BlendEffectMode blendMode) => new BlendEffect
        {
            Background = this.OriginRenderTarget,
            Foreground = this.TempRenderTarget,
            Mode = blendMode
        };
        public BlendEffect GetWeting(float opacity, BlendEffectMode blendMode) => new BlendEffect
        {
            Background = this.OriginRenderTarget,
            Foreground = new OpacityEffect
            {
                Source = this.TempRenderTarget,
                Opacity = opacity
            },
            Mode = blendMode
        };


        public void FillCircleDry(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillCircle(position, size * targetPressure, color);

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.FillCircle(p, size * e, color);
                    }
                }
            }
        }
        public void FillCircleWet(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillCircle(position, size * targetPressure, color);

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.FillCircle(p, size * e, color);
                    }
                }
            }
        }


        public ArithmeticCompositeEffect GetEraseWeting(float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = this.OriginRenderTarget,
            Source2 = this.TempRenderTarget,
        };

        public void ErasingDry(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Blend = CanvasBlend.Copy;

                ds.FillCircle(position, size * targetPressure, Colors.Transparent);

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.FillCircle(p, size * e, Colors.Transparent);
                    }
                }
            }
        }
        public void ErasingWet(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillCircle(position, size * targetPressure, Colors.White);

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.FillCircle(p, size * e, Colors.White);
                    }
                }
            }
        }


        public void DrawLine(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, Color color, float strokeWidth)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.DrawLine(position, targetPosition, color, strokeWidth, BitmapLayer.CanvasStrokeStyle);
        }


        public void FillCircleFlipX(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, p.Y, size, color);
                }
            }
        }

        public void FillCircleFlipY(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float height)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(position.X, height - position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(p.X, height - p.Y, size, color);
                }
            }
        }

        public void FillCircleTwo(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, height - position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, height - p.Y, size, color);
                }
            }
        }

        public void FillCircleFour(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height, Vector2 center)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, height - position.Y, size, color);

            Vector2 vector = position - center;
            ds.FillCircle(center.X - vector.Y, center.Y + vector.X, size, color);
            ds.FillCircle(center.X + vector.Y, center.Y - vector.X, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, height - p.Y, size, color);

                    Vector2 v = p - center;
                    ds.FillCircle(center.X - v.Y, center.Y + v.X, size, color);
                    ds.FillCircle(center.X + v.Y, center.Y - v.X, size, color);
                }
            }
        }

    }
}