using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public static class CanvasDrawingSessionExtensions
    {

        private static void DrawAnchor(this CanvasDrawingSession ds, Anchor anchor, Vector2 position)
        {
            if (anchor.IsSmooth)
            {
                if (anchor.IsChecked) ds.FillCircle(position, 4, Colors.White);
                ds.FillCircle(position, 3, Colors.DodgerBlue);
            }
            else
            {
                if (anchor.IsChecked) ds.FillRectangle(position.X - 3, position.Y - 3, 6, 6, Colors.White);
                ds.FillRectangle(position.X - 2, position.Y - 2, 4, 4, Colors.DodgerBlue);
            }
        }

        public static void DrawAnchorCollection(this CanvasDrawingSession ds, AnchorCollection anchors)
        {
            foreach (Anchor item in anchors)
            {
                ds.DrawAnchor(item, item.Point);
            }
        }
        public static void DrawAnchorCollection(this CanvasDrawingSession ds, AnchorCollection anchors, Matrix3x2 matrix)
        {
            foreach (Anchor item in anchors)
            {
                ds.DrawAnchor(item, Vector2.Transform(item.Point, matrix));
            }
        }


        private static void FillAnchor(this CanvasDrawingSession ds, Anchor anchor, Color color, float strokeWidth)
        {
            float sw = anchor.Pressure * strokeWidth;

            // 1.Head
            ds.FillCircle(anchor.Point, sw * 4, color);

            // 2.Body
            for (float i = sw; i < anchor.ComputePathLength; i += sw)
            {
                ds.FillCircle(anchor.Geometry.ComputePointOnPath(i), sw * 4, color);
            }
        }
        private static void FillAnchor(this CanvasDrawingSession ds, Anchor anchor, float pressure, Color color, float strokeWidth)
        {
            float i;

            // 1.Head
            if (anchor.Pressure < strokeWidth / 8)
            {
                i = strokeWidth / 4;
            }
            else
            {
                ds.FillCircle(anchor.Point, anchor.Pressure * 4, color);
                i = anchor.Pressure;
            }

            // 2.Body
            do
            {
                float pect = i / anchor.ComputePathLength;
                float sw = (pect * pressure + (1 - pect) * anchor.Pressure) * strokeWidth;

                if (sw < strokeWidth / 8)
                {
                    i += strokeWidth / 4;
                }
                else
                {
                    ds.FillCircle(anchor.Geometry.ComputePointOnPath(i), sw * 4, color);
                    i += sw;
                }
            } while (i < anchor.ComputePathLength);
        }

        /// <summary>
        /// (Head+Body + Head+Body + Head+Body + ... + Foot)
        /// </summary>
        public static void FillAnchorCollection(this CanvasDrawingSession ds, AnchorCollection anchors, Color color, float strokeWidth)
        {
            Anchor previous = null;

            // 1.Head
            // 2.Body
            foreach (Anchor item in anchors)
            {
                if (previous is null)
                {
                    previous = item;
                    continue;
                }

                if (previous.Geometry is null is false)
                {
                    if (previous.Pressure == item.Pressure)
                        ds.FillAnchor(previous, color, strokeWidth);
                    else
                        ds.FillAnchor(previous, item.Pressure, color, strokeWidth);
                }
                previous = item;
            }

            // 3.Foot
            float sw = previous.Pressure * strokeWidth;

            ds.FillCircle(previous.Point, sw * 4, color);
        }
        /// <summary>
        /// (Head+Body + Head+Body + Head+Body + ... + Head+Body+Foot)
        /// </summary>
        public static void FillAnchorCollection(this CanvasDrawingSession ds, AnchorCollection anchors, Vector2 point, Color color, float strokeWidth)
        {
            Anchor previous = null;

            // 1.Head
            // 2.Body
            foreach (Anchor item in anchors)
            {
                if (previous is null)
                {
                    previous = item;
                    continue;
                }

                if (previous.Geometry is null is false)
                {
                    if (previous.Pressure == item.Pressure)
                        ds.FillAnchor(previous, color, strokeWidth);
                    else
                        ds.FillAnchor(previous, item.Pressure, color, strokeWidth);
                }
                previous = item;
            }

            // 1.Head
            // 2.Body
            if (previous.Geometry is null is false)
            {
                if (previous.Pressure == 1)
                    ds.FillAnchor(previous, color, strokeWidth);
                else
                    ds.FillAnchor(previous, 1, color, strokeWidth);
            }

            // 3.Foot
            {
                float sw = strokeWidth;

                ds.FillCircle(point, sw * 4, color);
            }
        }

    }
}