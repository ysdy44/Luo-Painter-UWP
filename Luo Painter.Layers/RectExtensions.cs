using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Models
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

            boundsLeft = System.Math.Clamp(boundsLeft, 0, intersectSize.Width);
            boundsTop = System.Math.Clamp(boundsTop, 0, intersectSize.Height);
            return new Rect
            (
                boundsLeft,
                boundsTop,
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

            boundsLeft = System.Math.Clamp(boundsLeft, 0, intersectSize.Width);
            boundsTop = System.Math.Clamp(boundsTop, 0, intersectSize.Height);
            return new Rect
            (
                boundsLeft,
                boundsTop,
                System.Math.Max(System.Math.Min(boundsRight, intersectSize.Width) - boundsLeft, 0),
                System.Math.Max(System.Math.Min(boundsBottom, intersectSize.Height) - boundsTop, 0)
            );
        }

    }
}