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
        Shape = 4,
        Grain = 8,
        Opacity = 16,

        // UI
        UIOpacity = 64,
        UISpacing = 128,
        UIFlow = 256,
        UITip = 512,
        UIBlendMode = 1024,
        UIHardness = 2048,
        UIShape = 4096,
        UIGrain = 8192,

        Brush = 16384 | UIOpacity | UISpacing | UIFlow | UIBlendMode | UIHardness | UIShape | UIGrain,
        #region Brush

        Brush_Grain = Brush | Grain,
        Brush_Opacity = Brush | Opacity,
        Brush_Grain_Opacity = Brush | Grain | Opacity,

        Brush_Blend = Brush | Blend,
        Brush_Grain_Blend = Brush | Grain | Blend,
        Brush_Opacity_Blend = Brush | Opacity | Blend,
        Brush_Grain_Opacity_Blend = Brush | Grain | Opacity | Blend,

        Brush_Mix = Brush | Mix,
        Brush_Grain_Mix = Brush | Grain | Mix,

        #endregion

        ShapeBrush = Shape | Brush,
        #region MaskBrush

        ShapeBrush_Grain = Shape | Brush_Grain,
        ShapeBrush_Opacity = Shape | Brush_Opacity,
        ShapeBrush_Grain_Opacity = Shape | Brush_Grain_Opacity,

        ShapeBrush_Blend = Shape | Brush_Blend,
        ShapeBrush_Grain_Blend = Shape | Brush_Grain_Blend,
        ShapeBrush_Opacity_Blend = Shape | Brush_Opacity_Blend,
        ShapeBrush_Grain_Opacity_Blend = Shape | Brush_Grain_Opacity_Blend,

        ShapeBrush_Mix = Shape | Brush_Mix,
        ShapeBrush_Grain_Mix = Shape | Brush_Grain_Mix,

        #endregion

        Tip = 32768 | UIOpacity | UISpacing | UITip | UIBlendMode | UIGrain,
        #region Tip

        Tip_Grain = Tip | Grain,
        Tip_Opacity = Tip | Opacity,
        Tip_Grain_Opacity = Tip | Grain | Opacity,

        Tip_Blend = Tip | Blend,
        Tip_Grain_Blend = Tip | Grain | Blend,
        Tip_Opacity_Blend = Tip | Opacity | Blend,
        Tip_Grain_Opacity_Blend = Tip | Grain | Opacity | Blend,

        Tip_Mix = Tip | Mix,
        Tip_Grain_Mix = Tip | Grain | Mix,

        #endregion

        Line = 65536 | UIOpacity | UIBlendMode | UIGrain,
        #region Line

        Line_Grain = Line | Grain,
        Line_Opacity = Line | Opacity,
        Line_Grain_Opacity = Line | Grain | Opacity,

        Line_Blend = Line | Blend,
        Line_Grain_Blend = Line | Grain | Blend,
        Line_Opacity_Blend = Line | Opacity | Blend,
        Line_Grain_Opacity_Blend = Line | Grain | Opacity | Blend,

        Line_Mix = Line | Mix,
        Line_Grain_Mix = Line | Grain | Mix,

        #endregion

        Blur = 131072 | UISpacing | UIFlow | UIHardness,

        Mosaic = 262144 | UISpacing | UIHardness,

        Erase = 524288 | UIOpacity | UISpacing | UIFlow | UIHardness,
        Erase_Opacity = Erase | Opacity,

        Liquefy = 1048576,

    }
}