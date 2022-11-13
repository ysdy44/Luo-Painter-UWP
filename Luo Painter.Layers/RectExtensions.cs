using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Layers
{
    public static class RectExtensions
    {

        public static Rect GetRect(this Vector2 point, float radius)
        {
            float diameter = radius + radius;
            return new Rect
            (
                point.X - radius,
                point.Y - radius,
                diameter,
                diameter
            );
        }

        public static Rect GetRect(Vector2 point0, Vector2 point1, float radius)
        {
            float diameter = radius + radius;
            return new Rect
            (
                System.Math.Min(point0.X, point1.X) - radius,
                System.Math.Min(point0.Y, point1.Y) - radius,
                System.Math.Abs(point0.X - point1.X) + diameter,
                System.Math.Abs(point0.Y - point1.Y) + diameter
            );
        }

        public static Rect? TryGetRect(Vector2 point, Size intersectSize, float radius)
        {
            double boundsRight = point.X + radius;
            if (boundsRight <= 0) return null;

            double boundsBottom = point.Y + radius;
            if (boundsBottom < 0) return null;

            double boundsLeft = point.X - radius;
            if (boundsLeft >= intersectSize.Width) return null;

            double boundsTop = point.Y - radius;
            if (boundsTop >= intersectSize.Height) return null;

            return new Rect
            (
                System.Math.Clamp(boundsLeft, 0, intersectSize.Width),
                System.Math.Clamp(boundsTop, 0, intersectSize.Height),
                System.Math.Max(System.Math.Min(boundsRight, intersectSize.Width) - boundsLeft, 0),
                System.Math.Max(System.Math.Min(boundsBottom, intersectSize.Height) - boundsTop, 0)
            );
        }

        public static Rect? TryGetRect(Vector2 point0, Vector2 point1, Size intersectSize, float radius)
        {
            double boundsRight = System.Math.Max(point0.X, point1.X) + radius;
            if (boundsRight <= 0) return null;

            double boundsBottom = System.Math.Max(point0.Y, point1.Y) + radius;
            if (boundsBottom < 0) return null;

            double boundsLeft = System.Math.Min(point0.X, point1.X) - radius;
            if (boundsLeft >= intersectSize.Width) return null;

            double boundsTop = System.Math.Min(point0.Y, point1.Y) - radius;
            if (boundsTop >= intersectSize.Height) return null;

            return new Rect
            (
                System.Math.Clamp(boundsLeft, 0, intersectSize.Width),
                System.Math.Clamp(boundsTop, 0, intersectSize.Height),
                System.Math.Max(System.Math.Min(boundsRight, intersectSize.Width) - boundsLeft, 0),
                System.Math.Max(System.Math.Min(boundsBottom, intersectSize.Height) - boundsTop, 0)
            );
        }

        public static bool TryIntersect(this Size size, ref Rect rect)
        {
            if (rect.Left > size.Width - 1) return false;
            if (rect.Top > size.Height - 1) return false;
            if (rect.Right < 0) return false;
            if (rect.Bottom < 0) return false;

            rect.X = System.Math.Max(0, rect.Left);
            rect.Y = System.Math.Max(0, rect.Top);
            rect.Width = System.Math.Min(size.Width - 1, rect.Right) - rect.X;
            rect.Height = System.Math.Min(size.Height - 1, rect.Bottom) - rect.Y;
            return true;
        }
        public static bool TryIntersect(this Rect region, ref Rect rect)
        {
            if (rect.Left > region.Right) return false;
            if (rect.Top > region.Bottom) return false;
            if (rect.Right < region.Left) return false;
            if (rect.Bottom < region.Top) return false;

            rect.Intersect(region);
            return true;
        }

    }
}