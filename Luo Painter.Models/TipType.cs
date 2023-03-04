namespace Luo_Painter.Models
{
    public enum TipType
    {
        NoLayer,
        NotBitmapLayer,
        NotCurveLayer,

        NoPixel,
        NoPixelForMarquee,
        NoPixelForBitmapLayer,

        NoPaintTool,

        Spread, // No Subtitle
        Zoom, // No Subtitle
        Undo, // No Subtitle
        Redo, // No Subtitle

        Saving, // No Subtitle
        SaveSuccess, // No Subtitle
        SaveFailed,

        NoFile, // No Subtitle
        NoFolder, // No Subtitle
        NoSupport, // No Subtitle
        NoCompatible,
    }
}