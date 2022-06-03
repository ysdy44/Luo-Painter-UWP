using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType
    {
        None = 0,
        Pattern = 1,
        Opacity = 2,
        PatternOpacity = 3,
        BlendMode = 4,
        PatternBlendMode = 5,
        OpacityBlendMode = 6,
        PatternOpacityBlendMode = 7,

        BrushDry = 16,
        BrushWetPattern = 17,
        BrushWetOpacity = 18,
        BrushWetPatternOpacity = 19,
        BrushWetBlendMode = 20,
        BrushWetPatternBlendMode = 21,
        BrushWetOpacityBlendMode = 22,
        BrushWetPatternOpacityBlendMode = 23,

        MaskBrushDry = 32,
        MaskBrushWetPattern = 33,
        MaskBrushWetOpacity = 34,
        MaskBrushWetPatternOpacity = 35,
        MaskBrushWetBlendMode = 36,
        MaskBrushWetPatternBlendMode = 37,
        MaskBrushWetOpacityBlendMode = 38,
        MaskBrushWetPatternOpacityBlendMode = 39,

        CircleDry = 64,
        CircleWetPattern = 65,
        CircleWetOpacity = 66,
        CircleWetPatternOpacity = 67,
        CircleWetBlendMode = 68,
        CircleWetPatternBlendMode = 69,
        CircleWetOpacityBlendMode = 70,
        CircleWetPatternOpacityBlendMode = 71,

        LineDry = 128,
        LineWetPattern = 129,
        LineWetOpacity = 130,
        LineWetPatternOpacity = 131,
        LineWetBlendMode = 132,
        LineWetPatternBlendMode = 133,
        LineWetOpacityBlendMode = 134,
        LineWetPatternOpacityBlendMode = 135,

        EraseDry = 256,
        EraseWetPattern = 257,
        EraseWetOpacity = 258,
        EraseWetPatternOpacity = 259,

        Liquefy = 512,
    }
}