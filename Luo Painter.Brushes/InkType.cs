using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType
    {
        None = 0,
        Pattern = 1,
        Opacity = 2,
        Blend = 4,

        BrushDry = 16,
        BrushWetPattern = BrushDry | Pattern,
        BrushWetOpacity = BrushDry | Opacity,
        BrushWetPatternOpacity = BrushDry | Pattern | Opacity,
        BrushWetBlend = BrushDry | Blend,
        BrushWetPatternBlend = BrushDry | Pattern | Blend,
        BrushWetOpacityBlend = BrushDry | Opacity | Blend,
        BrushWetPatternOpacityBlend = BrushDry | Pattern | Opacity | Blend,

        MaskBrushDry = 32,
        MaskBrushWetPattern = MaskBrushDry | Pattern,
        MaskBrushWetOpacity = MaskBrushDry | Opacity,
        MaskBrushWetPatternOpacity = MaskBrushDry | Pattern | Opacity,
        MaskBrushWetBlend = MaskBrushDry | Blend,
        MaskBrushWetPatternBlend = MaskBrushDry | Pattern | Blend,
        MaskBrushWetOpacityBlend = MaskBrushDry | Opacity | Blend,
        MaskBrushWetPatternOpacityBlend = MaskBrushDry | Pattern | Opacity | Blend,

        CircleDry = 64,
        CircleWetPattern = CircleDry | Pattern,
        CircleWetOpacity = CircleDry | Opacity,
        CircleWetPatternOpacity = CircleDry | Pattern | Opacity,
        CircleWetBlend = CircleDry | Blend,
        CircleWetPatternBlend = CircleDry | Pattern | Blend,
        CircleWetOpacityBlend = CircleDry | Opacity | Blend,
        CircleWetPatternOpacityBlend = CircleDry | Pattern | Opacity | Blend,

        LineDry = 128,
        LineWetPattern = LineDry | Pattern,
        LineWetOpacity = LineDry | Opacity,
        LineWetPatternOpacity = LineDry | Pattern | Opacity,
        LineWetBlend = LineDry | Blend,
        LineWetPatternBlend = LineDry | Pattern | Blend,
        LineWetOpacityBlend = LineDry | Opacity | Blend,
        LineWetPatternOpacityBlend = LineDry | Pattern | Opacity | Blend,

        EraseDry = 256,
        EraseWetOpacity = EraseDry | Opacity,
        EraseWetPatternOpacity = EraseDry | Pattern | Opacity,

        Liquefy = 512,
    }
}