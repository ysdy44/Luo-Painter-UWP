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

        General = 16384 | UIOpacity | UISpacing | UIFlow | UIBlendMode | UIHardness | UIShape | UIGrain,
        #region General

        General_Grain = General | Grain,
        General_Opacity = General | Opacity,
        General_Grain_Opacity = General | Grain | Opacity,

        General_Blend = General | Blend,
        General_Grain_Blend = General | Grain | Blend,
        General_Opacity_Blend = General | Opacity | Blend,
        General_Grain_Opacity_Blend = General | Grain | Opacity | Blend,

        General_Mix = General | Mix,
        General_Grain_Mix = General | Grain | Mix,

        #endregion

        ShapeGeneral = Shape | General,
        #region ShapeGeneral

        ShapeGeneral_Grain = Shape | General_Grain,
        ShapeGeneral_Opacity = Shape | General_Opacity,
        ShapeGeneral_Grain_Opacity = Shape | General_Grain_Opacity,

        ShapeGeneral_Blend = Shape | General_Blend,
        ShapeGeneral_Grain_Blend = Shape | General_Grain_Blend,
        ShapeGeneral_Opacity_Blend = Shape | General_Opacity_Blend,
        ShapeGeneral_Grain_Opacity_Blend = Shape | General_Grain_Opacity_Blend,

        ShapeGeneral_Mix = Shape | General_Mix,
        ShapeGeneral_Grain_Mix = Shape | General_Grain_Mix,

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