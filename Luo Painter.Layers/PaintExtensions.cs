using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public static class PaintExtensions
    {

        public static void ErasingDry(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
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
        public static void ErasingWet(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
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


        public static readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };
        public static void DrawLine(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, Color color)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.DrawLine(position, targetPosition, color, 12, PaintExtensions.CanvasStrokeStyle);
        }


        public static void FillCircle(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
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

        public static void FillCircleFlipX(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width)
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

        public static void FillCircleFlipY(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float height)
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

        public static void FillCircleTwo(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height)
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

        public static void FillCircleFour(this CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height, Vector2 center)
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