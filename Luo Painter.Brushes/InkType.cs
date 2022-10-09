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
        UIBlendMode = 256,
        UIHardness = 512,
        UIMask = 1024,
        UIPattern = 2048,

        Brush = 4096 | UISpacing | UIBlendMode | UIHardness | UIMask | UIPattern,
        #region Brush

        Brush_Dry = Brush,

        Brush_Wet_Pattern = Brush | Pattern,
        Brush_Wet_Opacity = Brush | Opacity,
        Brush_Wet_Pattern_Opacity = Brush | Pattern | Opacity,

        Brush_WetComposite_Blend = Brush | Blend,
        Brush_WetComposite_Pattern_Blend = Brush | Pattern | Blend,
        Brush_WetComposite_Opacity_Blend = Brush | Opacity | Blend,
        Brush_WetComposite_Pattern_Opacity_Blend = Brush | Pattern | Opacity | Blend,

        Brush_WetBlur_Blur = Brush | Blur,
        Brush_WetBlur_Pattern_Blur = Brush | Pattern | Blur,

        Brush_WetMosaic_Mosaic = Brush | Mosaic,
        Brush_WetMosaic_Pattern_Mosaic = Brush | Pattern | Mosaic,

        Brush_Dry_Mix = Brush | Mix,
        Brush_Wet_Pattern_Mix = Brush | Pattern | Mix,

        #endregion

        MaskBrush = Mask | Brush,
        #region MaskBrush

        MaskBrush_Dry = MaskBrush,

        MaskBrush_Wet_Pattern = Mask | Brush_Wet_Pattern,
        MaskBrush_Wet_Opacity = Mask | Brush_Wet_Opacity,
        MaskBrush_Wet_Pattern_Opacity = Mask | Brush_Wet_Pattern_Opacity,

        MaskBrush_WetComposite_Blend = Mask | Brush_WetComposite_Blend,
        MaskBrush_WetComposite_Pattern_Blend = Mask | Brush_WetComposite_Pattern_Blend,
        MaskBrush_WetComposite_Opacity_Blend = Mask | Brush_WetComposite_Opacity_Blend,
        MaskBrush_WetComposite_Pattern_Opacity_Blend = Mask | Brush_WetComposite_Pattern_Opacity_Blend,

        MaskBrush_WetBlur_Blur = Mask | Brush_WetBlur_Blur,
        MaskBrush_WetBlur_Pattern_Blur = Mask | Brush_WetBlur_Pattern_Blur,

        MaskBrush_WetMosaic_Mosaic = Mask | Brush_WetMosaic_Mosaic,
        MaskBrush_WetMosaic_Pattern_Mosaic = Mask | Brush_WetMosaic_Pattern_Mosaic,

        MaskBrush_Dry_Mix = Mask | Brush_Dry_Mix,
        MaskBrush_Wet_Pattern_Mix = Mask | Brush_Wet_Pattern_Mix,

        #endregion

        Circle = 8192 | UISpacing | UIBlendMode | UIPattern,
        #region Circle

        Circle_Dry = Circle,

        Circle_Wet_Pattern = Circle | Pattern,
        Circle_Wet_Opacity = Circle | Opacity,
        Circle_Wet_Pattern_Opacity = Circle | Pattern | Opacity,

        Circle_WetComposite_Blend = Circle | Blend,
        Circle_WetComposite_Pattern_Blend = Circle | Pattern | Blend,
        Circle_WetComposite_Opacity_Blend = Circle | Opacity | Blend,
        Circle_WetComposite_Pattern_Opacity_Blend = Circle | Pattern | Opacity | Blend,

        Circle_WetBlur_Blur = Circle | Blur,
        Circle_WetBlur_Pattern_Blur = Circle | Pattern | Blur,

        Circle_WetMosaic_Mosaic = Circle | Mosaic,
        Circle_WetMosaic_Pattern_Mosaic = Circle | Pattern | Mosaic,

        Circle_Dry_Mix = Circle | Mix,
        Circle_Wet_Pattern_Mix = Circle | Pattern | Mix,

        #endregion

        Line = 16384 | UIBlendMode | UIPattern,
        #region Line

        Line_Dry = Line,

        Line_Wet_Pattern = Line | Pattern,
        Line_Wet_Opacity = Line | Opacity,
        Line_Wet_Pattern_Opacity = Line | Pattern | Opacity,

        Line_WetComposite_Blend = Line | Blend,
        Line_WetComposite_Pattern_Blend = Line | Pattern | Blend,
        Line_WetComposite_Opacity_Blend = Line | Opacity | Blend,
        Line_WetComposite_Pattern_Opacity_Blend = Line | Pattern | Opacity | Blend,

        Line_WetBlur_Blur = Line | Blur,
        Line_WetBlur_Pattern_Blur = Line | Pattern | Blur,

        Line_WetMosaic_Mosaic = Line | Mosaic,
        Line_WetMosaic_Pattern_Mosaic = Line | Pattern | Mosaic,

        Line_Dry_Mix = Line | Mix,
        Line_Wet_Pattern_Mix = Line | Pattern | Mix,

        #endregion

        Erase = 32768 | UISpacing | UIHardness,

        Erase_Dry = Erase,
        Erase_WetComposite_Opacity = Erase | Opacity,

        Liquefy = 65536,
    }
}