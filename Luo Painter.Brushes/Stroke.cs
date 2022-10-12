using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Brushes
{
    public struct Stroke
    {
        public readonly bool HasIntersect;
        public readonly Rect Intersect;

        public Stroke(Vector2 startingPoint, Vector2 point, Size screenSize, float size = 22f)
        {
            double boundsLeft = System.Math.Min(startingPoint.X, point.X) - size;
            double boundsTop = System.Math.Min(startingPoint.Y, point.Y) - size;
            double boundsRight = boundsLeft + System.Math.Abs(startingPoint.X - point.X) + size + size;
            double boundsBottom = boundsTop + System.Math.Abs(startingPoint.Y - point.Y) + size + size;

            this.HasIntersect = false;
            if (boundsLeft > screenSize.Width - 1) return;
            if (boundsTop > screenSize.Height - 1) return;
            if (boundsRight < 0) return;
            if (boundsBottom < 0) return;

            this.HasIntersect = true;
            this.Intersect.X = System.Math.Max(0, boundsLeft);
            this.Intersect.Y = System.Math.Max(0, boundsTop);
            this.Intersect.Width = System.Math.Min(screenSize.Width - 1, boundsRight) - boundsLeft;
            this.Intersect.Height = System.Math.Min(screenSize.Height - 1, boundsBottom) - boundsTop;
        }
    }
}