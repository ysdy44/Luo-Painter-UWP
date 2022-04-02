using System;
using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Elements
{
    public static class RectExtensions
    {

        public static Rect GetRect(this Vector2 center, float radius)
        {
            float diameter = radius + radius;
            return new Rect(center.X - radius, center.Y - radius, diameter, diameter);
        }
        public static Rect GetRect(Vector2 a, Vector2 b, float radius)
        {
            float diameter = radius + radius;
            return new Rect(Math.Min(a.X, b.X) - radius, Math.Min(a.Y, b.Y) - radius, Math.Abs(a.X - b.X) + diameter, Math.Abs(a.Y - b.Y) + diameter);
        }

        public static bool TryIntersect(this Size size, ref Rect rect)
        {
            if (rect.Left > size.Width - 1) return false;
            if (rect.Top > size.Height - 1) return false;
            if (rect.Right < 0) return false;
            if (rect.Bottom < 0) return false;

            rect.X = Math.Max(0, rect.Left);
            rect.Y = Math.Max(0, rect.Top);
            rect.Width = Math.Min(size.Width - 1, rect.Right) - rect.X;
            rect.Height = Math.Min(size.Height - 1, rect.Bottom) - rect.Y;
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