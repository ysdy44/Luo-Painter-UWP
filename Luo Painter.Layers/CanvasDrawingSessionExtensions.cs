using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Blends
{
    public static class CanvasDrawingSessionExtensions
    {
        public static void Invalidate(this CanvasAnimatedControl canvasAnimatedControl, bool isPaused)
        {
            if (canvasAnimatedControl.Paused == isPaused) return;

            if (isPaused)
            {
                canvasAnimatedControl.Paused = true;
                canvasAnimatedControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                canvasAnimatedControl.Invalidate(); // Invalidate
                canvasAnimatedControl.Paused = false;
                canvasAnimatedControl.Visibility = Visibility.Visible;
            }
        }

        private static void DrawAnchor(this CanvasDrawingSession ds, Anchor anchor, Vector2 point)
        {
            if (anchor.IsSmooth)
            {
                if (anchor.IsChecked) ds.FillCircle(point, 4, Colors.White);
                ds.FillCircle(point, 3, Colors.DodgerBlue);
            }
            else
            {
                if (anchor.IsChecked) ds.FillRectangle(point.X - 3, point.Y - 3, 6, 6, Colors.White);
                ds.FillRectangle(point.X - 2, point.Y - 2, 4, 4, Colors.DodgerBlue);
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

    }
}