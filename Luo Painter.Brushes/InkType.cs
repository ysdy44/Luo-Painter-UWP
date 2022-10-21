using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType : uint
    {
        None = 0,

        // Property
        Blend = 1,
        Mix = 2,
        Mask = 4,
        Pattern = 8,
        Opacity = 16,

        // UI
        UIOpacity = 64,
        UISpacing = 128,
        UIFlow = 256,
        UITip = 512,
        UIBlendMode = 1024,
        UIHardness = 2048,
        UIMask = 4096,
        UIPattern = 8192,

        Brush = 16384 | UIOpacity | UISpacing | UIFlow | UIBlendMode | UIHardness | UIMask | UIPattern,
        #region Brush

        Brush_Pattern = Brush | Pattern,
        Brush_Opacity = Brush | Opacity,
        Brush_Pattern_Opacity = Brush | Pattern | Opacity,

        Brush_Blend = Brush | Blend,
        Brush_Pattern_Blend = Brush | Pattern | Blend,
        Brush_Opacity_Blend = Brush | Opacity | Blend,
        Brush_Pattern_Opacity_Blend = Brush | Pattern | Opacity | Blend,

        Brush_Mix = Brush | Mix,
        Brush_Pattern_Mix = Brush | Pattern | Mix,

        #endregion

        MaskBrush = Mask | Brush,
        #region MaskBrush

        MaskBrush_Pattern = Mask | Brush_Pattern,
        MaskBrush_Opacity = Mask | Brush_Opacity,
        MaskBrush_Pattern_Opacity = Mask | Brush_Pattern_Opacity,

        MaskBrush_Blend = Mask | Brush_Blend,
        MaskBrush_Pattern_Blend = Mask | Brush_Pattern_Blend,
        MaskBrush_Opacity_Blend = Mask | Brush_Opacity_Blend,
        MaskBrush_Pattern_Opacity_Blend = Mask | Brush_Pattern_Opacity_Blend,

        MaskBrush_Mix = Mask | Brush_Mix,
        MaskBrush_Pattern_Mix = Mask | Brush_Pattern_Mix,

        #endregion

        Tip = 32768 | UIOpacity | UISpacing | UITip | UIBlendMode | UIPattern,
        #region Tip

        Tip_Pattern = Tip | Pattern,
        Tip_Opacity = Tip | Opacity,
        Tip_Pattern_Opacity = Tip | Pattern | Opacity,

        Tip_Blend = Tip | Blend,
        Tip_Pattern_Blend = Tip | Pattern | Blend,
        Tip_Opacity_Blend = Tip | Opacity | Blend,
        Tip_Pattern_Opacity_Blend = Tip | Pattern | Opacity | Blend,

        Tip_Mix = Tip | Mix,
        Tip_Pattern_Mix = Tip | Pattern | Mix,

        #endregion

        Line = 65536 | UIOpacity | UIBlendMode | UIPattern,
        #region Line

        Line_Pattern = Line | Pattern,
        Line_Opacity = Line | Opacity,
        Line_Pattern_Opacity = Line | Pattern | Opacity,

        Line_Blend = Line | Blend,
        Line_Pattern_Blend = Line | Pattern | Blend,
        Line_Opacity_Blend = Line | Opacity | Blend,
        Line_Pattern_Opacity_Blend = Line | Pattern | Opacity | Blend,

        Line_Mix = Line | Mix,
        Line_Pattern_Mix = Line | Pattern | Mix,

        #endregion

        Blur = 131072 | UISpacing | UIFlow | UIHardness,

        Mosaic = 262144 | UISpacing | UIHardness,

        Erase = 524288 | UIOpacity | UISpacing | UIFlow | UIHardness,
        Erase_Opacity = Erase | Opacity,

        Liquefy = 1048576,

    }
}