﻿namespace Luo_Painter.Blends
{
    public enum TipType
    {
        NoLayer,
        NotBitmapLayer,
        NotCurveLayer,

        NoPixel,
        NoPixelForMarquee,
        NoPixelForBitmapLayer,

        Spread,
        Zoom,
        Undo,
        Redo,

        Saving,
        SaveSuccess,
        SaveFailed,
    }
}