using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Elements
{
    public static class DPIExtensions
    {
        public static float ConvertPixels(this float dpi) => dpi / 96f;
        public static float ConvertDipsToPixels(this float dpi, float vector) => vector * dpi / 96f;
        public static Vector2 ConvertDipsToPixels(this float dpi, Vector2 vector) => vector * dpi / 96f;
        public static Matrix3x2 ConvertDipsToPixels(this float dpi) => Matrix3x2.CreateScale(dpi / 96f);
        public static Matrix3x2 ConvertDipsToPixels(this float dpi, Matrix3x2 matrix) => matrix * dpi.ConvertDipsToPixels();
        public static Rect ConvertDipsToPixels(this float dpi, Rect rect) => new Rect(rect.X * dpi / 96f, rect.Y * dpi / 96f, rect.Width * dpi / 96f, rect.Height * dpi / 96f);

        public static float ConvertDips(this float dpi) => 1f / dpi * 96f;
        public static float ConvertPixelsToDips(this float dpi, float vector) => vector / dpi * 96f;
        public static Vector2 ConvertPixelsToDips(this float dpi, Vector2 vector) => vector / dpi * 96f;
        public static Matrix3x2 ConvertPixelsToDips(this float dpi) => Matrix3x2.CreateScale(1 / dpi * 96f);
        public static Matrix3x2 ConvertPixelsToDips(this float dpi, Matrix3x2 matrix) => matrix * dpi.ConvertPixelsToDips();
        public static Rect ConvertPixelsToDips(this float dpi, Rect rect) => new Rect(rect.X / dpi * 96f, rect.Y / dpi * 96f, rect.Width / dpi * 96f, rect.Height / dpi * 96f);

    }
}