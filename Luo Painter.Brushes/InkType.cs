using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType
    {
        None = 0,

        // Dry
        Dry = 1,
        // Wet
        Wet = 1 << 1,
        // Wet
        WetBlur = 1 << 2,
        // Wet
        WetMosaic = 1 << 3,
        // WetComposite
        WetComposite = 1 << 4,
        WetComposite_Blend = 1 << 5 | WetComposite,
        WetComposite_Erase_Opacity = 1 << 6 | WetComposite,

        // Mode
        Blend = 1 << 7,
        Blur = 1 << 8,
        Mosaic = 1 << 9,
        Mix = 1 << 10,

        // Property
        Mask = 1 << 11,
        Pattern = 1 << 12,
        Opacity = 1 << 13,


        Brush = 1 << 14,
        #region Brush


        Brush_Dry = Brush | Dry,

        Brush_Wet_Pattern = Brush | Wet | Pattern,
        Brush_Wet_Opacity = Brush | Wet | Opacity,
        Brush_Wet_Pattern_Opacity = Brush | Wet | Pattern | Opacity,

        Brush_WetComposite_Blend = Brush | WetComposite_Blend | Blend,
        Brush_WetComposite_Pattern_Blend = Brush | WetComposite_Blend | Pattern | Blend,
        Brush_WetComposite_Opacity_Blend = Brush | WetComposite_Blend | Opacity | Blend,
        Brush_WetComposite_Pattern_Opacity_Blend = Brush | WetComposite_Blend | Pattern | Opacity | Blend,

        Brush_WetBlur_Blur = Brush | WetBlur | Blur,
        Brush_WetBlur_Pattern_Blur = Brush | WetBlur | Pattern | Blur,

        Brush_WetMosaic_Mosaic = Brush | WetMosaic | Mosaic,
        Brush_WetMosaic_Pattern_Mosaic = Brush | WetMosaic | Pattern | Mosaic,

        Brush_Dry_Mix = Brush | Dry | Mix,
        Brush_Wet_Pattern_Mix = Brush | Wet | Pattern | Mix,


        #endregion


        #region MaskBrush


        MaskBrush_Dry = Brush | Dry | Mask,

        MaskBrush_Wet_Pattern = Brush | Wet | Mask | Pattern,
        MaskBrush_Wet_Opacity = Brush | Wet | Mask | Opacity,
        MaskBrush_Wet_Pattern_Opacity = Brush | Wet | Mask | Pattern | Opacity,

        MaskBrush_WetComposite_Blend = Brush | WetComposite_Blend | Mask | Blend,
        MaskBrush_WetComposite_Pattern_Blend = Brush | WetComposite_Blend | Mask | Pattern | Blend,
        MaskBrush_WetComposite_Opacity_Blend = Brush | WetComposite_Blend | Mask | Opacity | Blend,
        MaskBrush_WetComposite_Pattern_Opacity_Blend = Brush | WetComposite_Blend | Mask | Pattern | Opacity | Blend,

        MaskBrush_WetBlur_Blur = Brush | WetBlur | Mask | Blur,
        MaskBrush_WetBlur_Pattern_Blur = Brush | WetBlur | Mask | Pattern | Blur,

        MaskBrush_WetMosaic_Mosaic = Brush | WetMosaic | Mask | Mosaic,
        MaskBrush_WetMosaic_Pattern_Mosaic = Brush | WetMosaic | Mask | Pattern | Mosaic,

        MaskBrush_Dry_Mix = Brush | Dry | Mask | Mix,
        MaskBrush_Wet_Pattern_Mix = Brush | Wet | Mask | Pattern | Mix,


        #endregion


        Circle = 1 << 15,
        #region Circle


        Circle_Dry = Circle | Dry,

        Circle_Wet_Pattern = Circle | Wet | Pattern,
        Circle_Wet_Opacity = Circle | Wet | Opacity,
        Circle_Wet_Pattern_Opacity = Circle | Wet | Pattern | Opacity,

        Circle_WetComposite_Blend = Circle | WetComposite_Blend | Blend,
        Circle_WetComposite_Pattern_Blend = Circle | WetComposite_Blend | Pattern | Blend,
        Circle_WetComposite_Opacity_Blend = Circle | WetComposite_Blend | Opacity | Blend,
        Circle_WetComposite_Pattern_Opacity_Blend = Circle | WetComposite_Blend | Pattern | Opacity | Blend,

        Circle_WetBlur_Blur = Circle | WetBlur | Blur,
        Circle_WetBlur_Pattern_Blur = Circle | WetBlur | Pattern | Blur,

        Circle_WetMosaic_Mosaic = Circle | WetMosaic | Mosaic,
        Circle_WetMosaic_Pattern_Mosaic = Circle | WetMosaic | Pattern | Mosaic,

        Circle_Dry_Mix = Circle | Dry | Mix,
        Circle_Wet_Pattern_Mix = Circle | Wet | Pattern | Mix,


        #endregion


        Line = 1 << 16,
        #region Line


        Line_Dry = Line | Dry,

        Line_Wet_Pattern = Line | Wet | Pattern,
        Line_Wet_Opacity = Line | Wet | Opacity,
        Line_Wet_Pattern_Opacity = Line | Wet | Pattern | Opacity,

        Line_WetComposite_Blend = Line | WetComposite_Blend | Blend,
        Line_WetComposite_Pattern_Blend = Line | WetComposite_Blend | Pattern | Blend,
        Line_WetComposite_Opacity_Blend = Line | WetComposite_Blend | Opacity | Blend,
        Line_WetComposite_Pattern_Opacity_Blend = Line | WetComposite_Blend | Pattern | Opacity | Blend,

        Line_WetBlur_Blur = Line | WetBlur | Blur,
        Line_WetBlur_Pattern_Blur = Line | WetBlur | Pattern | Blur,

        Line_WetMosaic_Mosaic = Line | WetMosaic | Mosaic,
        Line_WetMosaic_Pattern_Mosaic = Line | WetMosaic | Pattern | Mosaic,

        Line_Dry_Mix = Line | Dry | Mix,
        Line_Wet_Pattern_Mix = Line | Wet | Pattern | Mix,


        #endregion


        Erase = 1 << 17,
        #region Erase

        Erase_Dry = Erase | Dry,

        Erase_WetComposite_Opacity = Erase | WetComposite_Erase_Opacity | Opacity,
        Erase_WetComposite_Pattern_Opacity = Erase | WetComposite_Erase_Opacity | Pattern | Opacity,


        #endregion


        Liquefy = 1 << 18 | Dry,
    }
}