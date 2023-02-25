namespace Luo_Painter.Models
{
    public enum UIType : int
    {
        Name,

        Version,

        InputText,
        Untitled,
        UntitledFolder,

        GithubLink,
        FeedbackLink,

        Github,
        Feedback,

        Apply,
        Back,
        OK,
        Cancel,

        X,
        Y,
        Width,
        Height,



        // Layer
        Layer_Property,
        Layer_Property_Opacity,
        Layer_Property_BlendMode,
        Layer_Property_TagType,

        // Paint
        Paint_Scratchpad,

        Paint_Property,
        Paint_Property_Size,
        Paint_Property_Opacity,
        Paint_Property_Spacing,
        Paint_Property_Flow,
        Paint_Property_MinSize,
        Paint_Property_MinFlow,
        Paint_Property_Tip,
        Paint_Property_Hardness,

        Paint_Texture,
        Paint_Texture_Shape,
        Paint_Texture_Rotate,
        Paint_Texture_Grain,
        Paint_Texture_Scale,
        Paint_Texture_Import,
        Paint_Texture_Recolor,
        Paint_Texture_Remove,
        Paint_Texture_BlendMode,

        Paint_Mix,
        Paint_Mix_Mix,
        Paint_Mix_Wet,
        Paint_Mix_Persistence,

        // Effect
        Effect_Apply,

        // Color
        Color_Straw,

        Color_Wheel,
        Color_Box,
        Color_Palette,
        Color_Parameter,

        Color_Alpha,
        Color_Red,
        Color_Green,
        Color_Blue,

        Color_Hue,
        Color_Saturation,
        Color_Value,

        Color_Hex,



        // Export
        Export, // OptionType.Export
        Export_DPI,

        Export_Object,
        Export_Object_Image,
        Export_Object_Layers,
        Export_Object_Layer,

        // Extend
        Extend, // OptionType.Extend
        Extend_Indicator,

        // Offset
        Offset, // OptionType.Offset

        // Stretch
        Stretch, // OptionType.Stretch
        Stretch_Interpolation,

        // Texture
        Texture,



        // NewProject
        NewProject,

        // NewImage
        NewImage,

        // NewFolder
        NewFolder,

        // DupliateProject
        DupliateProject,
        DupliateProject_Subtitle,

        // DeleteProject
        DeleteProject,
        DeleteProject_Subtitle,

        // SelectProject
        SelectProject,
        SelectProject_Subtitle,

        // MoveProject
        MoveProject,
        MoveProject_Subtitle,

        // RenameProject
        RenameProject,



        // Main
        Main_Documentation,
        Main_Setting,

        Main_Home,
        Main_Back,

        Main_Type,
        Main_Date,
        Main_Name,



        // Setup
        CropCanvas_Radians,

        // Marquees
        Feather, // OptionType.Feather
        Grow, // OptionType.Grow
        Shrink, // OptionType.Shrink

        // Other
        Transform_Snap,
        Transform_Ratio,
        Transform_Center,
        Transform_X,
        Transform_Y,
        Transform_W,
        Transform_H,

        DisplacementLiquefaction_Tool,
        DisplacementLiquefaction_Push,
        DisplacementLiquefaction_Pinch,
        DisplacementLiquefaction_Expand,
        DisplacementLiquefaction_TwirlLeft,
        DisplacementLiquefaction_TwirlRight,
        DisplacementLiquefaction_Reset,
        DisplacementLiquefaction_Size,
        DisplacementLiquefaction_Pressure,
        GradientMapping_DropRemove,
        RippleEffect_Frequency,
        RippleEffect_Phase,
        RippleEffect_Amplitude,
        Threshold, // OptionType.Threshold
        Threshold_Reverse,
        HSB_Hue,
        HSB_Saturation,
        HSB_Brightness,

        // Adjustment1
        Exposure, // OptionType.Exposure
        Saturation, // OptionType.Saturation
        HueRotation_Angle,
        Contrast, // OptionType.Contrast
        Temperature, // OptionType.Temperature
        Temperature_Tint,
        HighlightsAndShadows_Shadows,
        HighlightsAndShadows_Highlights,
        HighlightsAndShadows_Clarity,
        HighlightsAndShadows_Blur,

        // Adjustment2
        GammaTransfer_ChannelSelect,
        GammaTransfer_Alpha,
        GammaTransfer_Red,
        GammaTransfer_Green,
        GammaTransfer_Blue,
        GammaTransfer_Amplitude,
        GammaTransfer_Exponent,
        GammaTransfer_Offset,
        Vignette_Amount,

        // Effect1
        GaussianBlur_BlurAmount,
        DirectionalBlur_BlurAmount,
        DirectionalBlur_Angle,
        Sharpen_Amount,
        Shadow_Amount,
        Shadow_Opacity,
        Shadow_X,
        Shadow_Y,
        EdgeDetection_Amount,
        EdgeDetection_BlurAmount,
        EdgeDetection_OverlayEdges,
        Morphology_Size,
        Emboss_Amount,
        Emboss_Angle,
        Straighten_Angle,

        // Effect2
        Posterize_Count,
        LuminanceToAlpha_Background,
        LuminanceToAlpha_Normal,
        LuminanceToAlpha_WhitePaper,
        LuminanceToAlpha_BlackPaper,
        ChromaKey_Tolerance,
        ChromaKey_Invert,
        ChromaKey_Feather,
        Border_EdgeBehaviorX,
        Border_EdgeBehaviorY,
        Border_Clamp,
        Border_Wrap,
        Border_Mirror,
        Colouring_Angle,

        // Effect3
        Lighting_Distance,
        Lighting_Angle,
        Lighting_Ambient,
        Fog_ScaleFactor,
        Fog_Amount,
        Glass_ScaleFactor,
        Glass_Amount,

        // Tool
        SelectionFlood_Tolerance,
        SelectionFlood_Contiguous,
        SelectionFlood_Feather,
        SelectionBrush_Size,
        Paint_Size,
        Paint_Opacity,
        PaintBrushMulti_Sectors,
        View_Radian,
        View_Scale,
        Straw_Object,
        Straw_Layers,
        Straw_Layer,
        Brush_GradientType,
        Brush_Linear,
        Brush_Radial,
        Brush_Elliptical,
        Brush_Opaque,
        Brush_Reverse,
        Transparency_TransparencyType,
        Transparency_Reverse,
        Pen_SegmentType,
        Pen_Curve,
        Pen_Line,
        Pen_Close,

        // Geometry
        GeometryRoundRect_Corner,
        GeometryTriangle_CenterPoint,
        GeometryDiamond_MidPoint,
        GeometryPentagon_Points,
        GeometryStar_Points,
        GeometryStar_InnerRadius,
        GeometryDonut_HoleRadius,
        GeometryCog_Count,
        GeometryCog_InnerRadius,
        GeometryCog_Tooth,
        GeometryCog_Notch,
        GeometryPie_SweepAngle,
        GeometryCookie_InnerRadius,
        GeometryCookie_SweepAngle,
        GeometryArrow_Width,
        GeometryHeart_Spread,

        // Pattern
        PatternGrid_StrokeWidth,
        PatternGrid_ColumnSpan,
        PatternGrid_RowSpan,
        PatternDiagonal_Frequency,
        PatternDiagonal_Phase,
        PatternDiagonal_Amplitude,
        PatternSpotted_Radius,
        PatternSpotted_Span,
        PatternSpotted_Fade,
    }
}