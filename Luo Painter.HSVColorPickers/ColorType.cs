namespace Luo_Painter.HSVColorPickers
{
    public enum ColorType : byte
    {
        None = 0,
        Wheel = 4,
        Slider = 8,
        Box = 16,
        Ring = 32,

        Rectcolor = 1 | Wheel,
        Tricolor = 2 | Wheel,

        RGB = 1 | Slider,
        HSV = 2 | Slider,

        Hue = 1 | Box,
        Saturation = 2 | Box,
        Value = 3 | Box,

        Circle = 1 | Ring,
        Harmony = 2 | Ring,
    }
}