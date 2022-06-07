using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType
    {
        None = 0,
        Pattern = 1,
        Opacity = 2,
        BlendMode = 4,

        BrushDry = 16,
        BrushWetPattern = BrushDry | Pattern,
        BrushWetOpacity = BrushDry | Opacity,
        BrushWetPatternOpacity = BrushDry | Pattern | Opacity,
        BrushWetBlendMode = BrushDry | BlendMode,
        BrushWetPatternBlendMode = BrushDry | Pattern | BlendMode,
        BrushWetOpacityBlendMode = BrushDry | Opacity | BlendMode,
        BrushWetPatternOpacityBlendMode = BrushDry | Pattern | Opacity | BlendMode,

        MaskBrushDry = 32,
        MaskBrushWetPattern = MaskBrushDry | Pattern,
        MaskBrushWetOpacity = MaskBrushDry | Opacity,
        MaskBrushWetPatternOpacity = MaskBrushDry | Pattern | Opacity,
        MaskBrushWetBlendMode = MaskBrushDry | BlendMode,
        MaskBrushWetPatternBlendMode = MaskBrushDry | Pattern | BlendMode,
        MaskBrushWetOpacityBlendMode = MaskBrushDry | Opacity | BlendMode,
        MaskBrushWetPatternOpacityBlendMode = MaskBrushDry | Pattern | Opacity | BlendMode,

        CircleDry = 64,
        CircleWetPattern = CircleDry | Pattern,
        CircleWetOpacity = CircleDry | Opacity,
        CircleWetPatternOpacity = CircleDry | Pattern | Opacity,
        CircleWetBlendMode = CircleDry | BlendMode,
        CircleWetPatternBlendMode = CircleDry | Pattern | BlendMode,
        CircleWetOpacityBlendMode = CircleDry | Opacity | BlendMode,
        CircleWetPatternOpacityBlendMode = CircleDry | Pattern | Opacity | BlendMode,

        LineDry = 128,
        LineWetPattern = LineDry | Pattern,
        LineWetOpacity = LineDry | Opacity,
        LineWetPatternOpacity = LineDry | Pattern | Opacity,
        LineWetBlendMode = LineDry | BlendMode,
        LineWetPatternBlendMode = LineDry | Pattern | BlendMode,
        LineWetOpacityBlendMode = LineDry | Opacity | BlendMode,
        LineWetPatternOpacityBlendMode = LineDry | Pattern | Opacity | BlendMode,

        EraseDry = 256,
        EraseWetPattern = EraseDry | Pattern,
        EraseWetOpacity = EraseDry | Opacity,
        EraseWetPatternOpacity = EraseDry | Pattern | Opacity,

        Liquefy = 512,
    }
}