using System.Numerics;

namespace Luo_Painter.Elements
{
    public static class DPIExtensions
    {
        public static float ConvertDipsToPixels(this float dpi, float vector) => vector * dpi / 96;
        public static Vector2 ConvertDipsToPixels(this float dpi, Vector2 vector) => vector * dpi / 96;
        public static Matrix3x2 ConvertDipsToPixels(this float dpi) => Matrix3x2.CreateScale(dpi / 96);
        
        public static float ConvertPixelsToDips(this float dpi, float vector) => vector / dpi * 96;
        public static Vector2 ConvertPixelsToDips(this float dpi, Vector2 vector) => vector / dpi * 96;
        public static Matrix3x2 ConvertPixelsToDips(this float dpi) => Matrix3x2.CreateScale(1 / dpi * 96);
    }
}