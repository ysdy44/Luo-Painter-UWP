using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Elements
{
    public static class DPIExtensions
    {
        public static float ConvertDipsToPixels(this float dpi, float vector) => vector * dpi / 96;
        public static Vector2 ConvertDipsToPixels(this float dpi, Vector2 vector) => vector * dpi / 96;
        public static Matrix3x2 ConvertDipsToPixels(this float dpi) => Matrix3x2.CreateScale(dpi / 96);
        public static Matrix3x2 ConvertDipsToPixels(this float dpi, Matrix3x2 matrix) => matrix * dpi.ConvertDipsToPixels();
        public static Rect ConvertDipsToPixels(this float dpi, Rect rect)
        {
            float s = 1 * dpi / 96;
            return new Rect(rect.X * s, rect.Y * s, rect.Width * s, rect.Height * s);
        }

        public static float ConvertPixelsToDips(this float dpi, float vector) => vector / dpi * 96;
        public static Vector2 ConvertPixelsToDips(this float dpi, Vector2 vector) => vector / dpi * 96;
        public static Matrix3x2 ConvertPixelsToDips(this float dpi) => Matrix3x2.CreateScale(1 / dpi * 96);
        public static Matrix3x2 ConvertPixelsToDips(this float dpi, Matrix3x2 matrix) => matrix * dpi.ConvertPixelsToDips();
        public static Rect ConvertPixelsToDips(this float dpi, Rect rect)
        {
            float s = 1 / dpi * 96;
            return new Rect(rect.X * s, rect.Y * s, rect.Width * s, rect.Height * s);
        }
    }
}