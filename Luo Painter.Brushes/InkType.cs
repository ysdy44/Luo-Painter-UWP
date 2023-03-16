using System;

namespace Luo_Painter.Brushes
{
    [Flags]
    public enum InkType : uint
    {
        None = 0,


        // Property
        Opacity = 1,

        Shape = 2,
        Grain = 4,

        Blend = 8,
        Mix = 16,


        // UI
        UIOpacity = 32,
        UISpacing = 64,
        UIFlow = 128,

        UIHardness = 256,
        UITip = 512,
        UIShape = 1024,

        UIBlendMode = 2048,
        UIGrain = 4096,

        UIMix = 8192,


        General = 16384 | UIOpacity | UISpacing | UIFlow | UIHardness | UIShape | UIGrain | UIBlendMode | UIMix,
        #region General

        General_Opacity = General | Opacity,

        General_Grain = General | Grain,
        General_Opacity_Grain = General | Opacity | Grain,

        General_Blend = General | Blend,

        General_Opacity_Blend = General | Opacity | Blend,
        General_Grain_Blend = General | Grain | Blend,
        General_Opacity_Grain_Blend = General | Opacity | Grain | Blend,

        General_Mix = General | Mix,

        General_Opacity_Mix = General | Opacity | Mix,
        General_Grain_Mix = General | Grain | Mix,
        General_Opacity_Grain_Mix = General | Opacity | Grain | Mix,
        General_Blend_Mix = General | Blend | Mix,
        General_Opacity_Blend_Mix = General | Opacity | Blend | Mix,
        General_Grain_Blend_Mix = General | Grain | Blend | Mix,
        General_Opacity_Grain_Blend_Mix = General | Opacity | Grain | Blend | Mix,

        #endregion

        ShapeGeneral = Shape | General,
        #region ShapeGeneral

        ShapeGeneral_Opacity = Shape | General_Opacity,
        ShapeGeneral_Grain = Shape | General_Grain,
        ShapeGeneral_Opacity_Grain = Shape | General_Opacity_Grain,

        ShapeGeneral_Blend = Shape | General_Blend,
        ShapeGeneral_Opacity_Blend = Shape | General_Opacity_Blend,
        ShapeGeneral_Grain_Blend = Shape | General_Grain_Blend,
        ShapeGeneral_Opacity_Grain_Blend = Shape | General_Opacity_Grain_Blend,

        ShapeGeneral_Mix = Shape | General_Mix,
        ShapeGeneral_Opacity_Mix = Shape | General_Opacity_Mix,
        ShapeGeneral_Grain_Mix = Shape | General_Grain_Mix,
        ShapeGeneral_Opacity_Grain_Mix = Shape | General_Opacity_Grain_Mix,
        ShapeGeneral_Blend_Mix = Shape | General_Blend_Mix,
        ShapeGeneral_Opacity_Blend_Mix = Shape | General_Opacity_Blend_Mix,
        ShapeGeneral_Grain_Blend_Mix = Shape | General_Grain_Blend_Mix,
        ShapeGeneral_Opacity_Grain_Blend_Mix = Shape | General_Opacity_Grain_Blend_Mix,

        #endregion

        Tip = 32768 | UIOpacity | UISpacing | UITip | UIGrain | UIBlendMode,
        #region Tip

        Tip_Opacity = Tip | Opacity,
        Tip_Grain = Tip | Grain,
        Tip_Opacity_Grain = Tip | Opacity | Grain,

        Tip_Blend = Tip | Blend,
        Tip_Opacity_Blend = Tip | Opacity | Blend,
        Tip_Grain_Blend = Tip | Grain | Blend,
        Tip_Opacity_Grain_Blend = Tip | Opacity | Grain | Blend,

        #endregion

        Line = 65536 | UIOpacity | UIGrain | UIBlendMode,
        #region Line

        Line_Opacity = Line | Opacity,
        Line_Grain = Line | Grain,
        Line_Opacity_Grain = Line | Opacity | Grain,

        Line_Blend = Line | Blend,
        Line_Opacity_Blend = Line | Opacity | Blend,
        Line_Grain_Blend = Line | Grain | Blend,
        Line_Opacity_Grain_Blend = Line | Opacity | Grain | Blend,

        #endregion

        Blur = 131072 | UISpacing | UIFlow | UIHardness,

        Mosaic = 262144,

        Erase = 524288 | UIOpacity | UISpacing | UIFlow | UIHardness,
        Erase_Opacity = Erase | Opacity,

        Liquefy = 1048576,

    }
}