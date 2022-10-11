using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType : uint
    {
        None = 0,

        // Mode
        Blend = 1,
        Blur = 2,
        Mosaic = 4,
        Mix = 8,

        // Property
        Mask = 16,
        Pattern = 32,
        Opacity = 64,

        // UI
        UISpacing = 128,
        UIFlow = 256,
        UIShape = 512,
        UIBlendMode = 1024,
        UIHardness = 2048,
        UIMask = 4096,
        UIPattern = 8192,

        Brush = 16384 | UISpacing | UIFlow | UIBlendMode | UIHardness | UIMask | UIPattern,
        #region Brush

        Brush_Pattern = Brush | Pattern,
        Brush_Opacity = Brush | Opacity,
        Brush_Pattern_Opacity = Brush | Pattern | Opacity,

        Brush_Blend = Brush | Blend,
        Brush_Pattern_Blend = Brush | Pattern | Blend,
        Brush_Opacity_Blend = Brush | Opacity | Blend,
        Brush_Pattern_Opacity_Blend = Brush | Pattern | Opacity | Blend,

        Brush_Blur = Brush | Blur,
        Brush_Pattern_Blur = Brush | Pattern | Blur,

        Brush_Mosaic = Brush | Mosaic,
        Brush_Pattern_Mosaic = Brush | Pattern | Mosaic,

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

        MaskBrush_Blur = Mask | Brush_Blur,
        MaskBrush_Pattern_Blur = Mask | Brush_Pattern_Blur,

        MaskBrush_Mosaic = Mask | Brush_Mosaic,
        MaskBrush_Pattern_Mosaic = Mask | Brush_Pattern_Mosaic,

        MaskBrush_Mix = Mask | Brush_Mix,
        MaskBrush_Pattern_Mix = Mask | Brush_Pattern_Mix,

        #endregion

        Circle = 32768 | UISpacing | UIShape | UIBlendMode | UIPattern,
        #region Circle

        Circle_Pattern = Circle | Pattern,
        Circle_Opacity = Circle | Opacity,
        Circle_Pattern_Opacity = Circle | Pattern | Opacity,

        Circle_Blend = Circle | Blend,
        Circle_Pattern_Blend = Circle | Pattern | Blend,
        Circle_Opacity_Blend = Circle | Opacity | Blend,
        Circle_Pattern_Opacity_Blend = Circle | Pattern | Opacity | Blend,

        Circle_Blur = Circle | Blur,
        Circle_Pattern_Blur = Circle | Pattern | Blur,

        Circle_Mosaic = Circle | Mosaic,
        Circle_Pattern_Mosaic = Circle | Pattern | Mosaic,

        Circle_Mix = Circle | Mix,
        Circle_Pattern_Mix = Circle | Pattern | Mix,

        #endregion

        Line = 65536 | UIBlendMode | UIPattern,
        #region Line

        Line_Pattern = Line | Pattern,
        Line_Opacity = Line | Opacity,
        Line_Pattern_Opacity = Line | Pattern | Opacity,

        Line_Blend = Line | Blend,
        Line_Pattern_Blend = Line | Pattern | Blend,
        Line_Opacity_Blend = Line | Opacity | Blend,
        Line_Pattern_Opacity_Blend = Line | Pattern | Opacity | Blend,

        Line_Blur = Line | Blur,
        Line_Pattern_Blur = Line | Pattern | Blur,

        Line_Mosaic = Line | Mosaic,
        Line_Pattern_Mosaic = Line | Pattern | Mosaic,

        Line_Mix = Line | Mix,
        Line_Pattern_Mix = Line | Pattern | Mix,

        #endregion

        Erase = 131072 | UISpacing | UIFlow | UIHardness,
        Erase_Opacity = Erase | Opacity,

        Liquefy = 262144,
    }
}