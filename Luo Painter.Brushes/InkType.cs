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
        WetBlur = 1 << 2,
        // WetComposite
        WetComposite = 1 << 3,
        WetCompositeBlend = 1 << 4 | WetComposite,
        WetCompositeEraseOpacity = 1 << 5 | WetComposite,

        // Mode
        Blend = 1 << 6,
        Blur = 1 << 7,
        Mix = 1 << 8,

        // Property
        Mask = 1 << 9,
        Pattern = 1 << 10,
        Opacity = 1 << 11,


        Brush = 1 << 12,
        #region Brush


        BrushDry = Brush | Dry,

        BrushWetPattern = Brush | Wet | Pattern,
        BrushWetOpacity = Brush | Wet | Opacity,
        BrushWetPatternOpacity = Brush | Wet | Pattern | Opacity,

        BrushWetBlend = Brush | WetCompositeBlend | Blend,
        BrushWetPatternBlend = Brush | WetCompositeBlend | Pattern | Blend,
        BrushWetOpacityBlend = Brush | WetCompositeBlend | Opacity | Blend,
        BrushWetPatternOpacityBlend = Brush | WetCompositeBlend | Pattern | Opacity | Blend,

        BrushWetBlur = Brush | WetBlur | Blur,
        BrushWetPatternBlur = Brush | WetBlur | Pattern | Blur,

        BrushDryMix = Brush | Dry | Mix,
        BrushWetPatternMix = Brush | Wet | Pattern | Mix,


        #endregion


        #region MaskBrush


        MaskBrushDry = Brush | Dry | Mask,

        MaskBrushWetPattern = Brush | Wet | Mask | Pattern,
        MaskBrushWetOpacity = Brush | Wet | Mask | Opacity,
        MaskBrushWetPatternOpacity = Brush | Wet | Mask | Pattern | Opacity,

        MaskBrushWetBlend = Brush | WetCompositeBlend | Mask | Blend,
        MaskBrushWetPatternBlend = Brush | WetCompositeBlend | Mask | Pattern | Blend,
        MaskBrushWetOpacityBlend = Brush | WetCompositeBlend | Mask | Opacity | Blend,
        MaskBrushWetPatternOpacityBlend = Brush | WetCompositeBlend | Mask | Pattern | Opacity | Blend,

        MaskBrushWetBlur = Brush | WetBlur | Mask | Blur,
        MaskBrushWetPatternBlur = Brush | WetBlur | Mask | Pattern | Blur,

        MaskBrushDryMix = Brush | Dry | Mask | Mix,
        MaskBrushWetPatternMix = Brush | Wet | Mask | Pattern | Mix,


        #endregion


        Circle = 1 << 14,
        #region Circle


        CircleDry = Circle | Dry,

        CircleWetPattern = Circle | Wet | Pattern,
        CircleWetOpacity = Circle | Wet | Opacity,
        CircleWetPatternOpacity = Circle | Wet | Pattern | Opacity,

        CircleWetBlend = Circle | WetCompositeBlend | Blend,
        CircleWetPatternBlend = Circle | WetCompositeBlend | Pattern | Blend,
        CircleWetOpacityBlend = Circle | WetCompositeBlend | Opacity | Blend,
        CircleWetPatternOpacityBlend = Circle | WetCompositeBlend | Pattern | Opacity | Blend,

        CircleWetBlur = Circle | WetBlur | Blur,
        CircleWetPatternBlur = Circle | WetBlur | Pattern | Blur,

        CircleDryMix = Circle | Dry | Mix,
        CircleWetPatternMix = Circle | Wet | Pattern | Mix,


        #endregion


        Line = 1 << 15,
        #region Line


        LineDry = Line | Dry,

        LineWetPattern = Line | Wet | Pattern,
        LineWetOpacity = Line | Wet | Opacity,
        LineWetPatternOpacity = Line | Wet | Pattern | Opacity,

        LineWetBlend = Line | WetCompositeBlend | Blend,
        LineWetPatternBlend = Line | WetCompositeBlend | Pattern | Blend,
        LineWetOpacityBlend = Line | WetCompositeBlend | Opacity | Blend,
        LineWetPatternOpacityBlend = Line | WetCompositeBlend | Pattern | Opacity | Blend,

        LineWetBlur = Line | WetBlur | Blur,
        LineWetPatternBlur = Line | WetBlur | Pattern | Blur,

        LineDryMix = Line | Dry | Mix,
        LineWetPatternMix = Line | Wet | Pattern | Mix,


        #endregion


        Erase = 1 << 16,
        #region Erase

        EraseDry = Erase | Dry,

        EraseWetOpacity = Erase | WetCompositeEraseOpacity | Opacity,
        EraseWetPatternOpacity = Erase | WetCompositeEraseOpacity | Pattern | Opacity,


        #endregion


        Liquefy = 1 << 17 | Dry,
    }
}