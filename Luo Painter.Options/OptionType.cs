using System;

namespace Luo_Painter.Options
{
    [Flags]
    public enum OptionType
    {
        // None
        None = 0,

        IsItemClickEnabled = 1,
        ExistIcon = 2,
        ExistThumbnail = 4,
        AllowDrag = 8,
        HasPreview = 16,
        HasDifference = 32,
        TempOverlay = 64,
        HasState = 128,

        // Type
        Edit = 1 << 20,
        Option = 2 << 20,
        Tool = 4 << 20,


        // Edit
        Editing = 1 << 12 | Edit,
        Grouping = 2 << 12 | Edit,
        Select = 4 << 12 | Edit,
        Combine = 8 << 12 | Edit,
        Setup = 16 << 12 | Edit,

        #region Edit


        // Editing
        Cut = 1 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Duplicate = 2 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Copy = 3 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Paste = 4 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,

        Clear = 5 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Remove = 6 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,

        Extract = 7 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Merge = 8 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,
        Flatten = 9 << 8 | Editing | IsItemClickEnabled | ExistIcon | HasState,


        // Grouping
        Group = 1 << 8 | Grouping | IsItemClickEnabled | ExistIcon | HasState,
        Ungroup = 2 << 8 | Grouping | IsItemClickEnabled | ExistIcon | HasState,

        Release = 3 << 8 | Grouping | IsItemClickEnabled | ExistIcon | HasState,


        // Select
        All = 1 << 8 | Select | IsItemClickEnabled | ExistIcon | HasState,
        Deselect = 2 << 8 | Select | IsItemClickEnabled | ExistIcon | HasState,
        MarqueeInvert = 3 << 8 | Select | IsItemClickEnabled | ExistIcon | HasState,
        Pixel = 4 << 8 | Select | IsItemClickEnabled | ExistIcon | HasState,

        Feather = 5 << 8 | Select | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview | HasState,
        MarqueeTransform = 6 << 8 | Select | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview | HasPreview | HasDifference | HasState,
        Grow = 7 << 8 | Select | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview | HasState,
        Shrink = 8 << 8 | Select | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview | HasState,


        // Combine
        Union = 1 << 8 | Combine | IsItemClickEnabled | ExistIcon | HasState,
        Exclude = 2 << 8 | Combine | IsItemClickEnabled | ExistIcon | HasState,
        Xor = 3 << 8 | Combine | IsItemClickEnabled | ExistIcon | HasState,
        Intersect = 4 << 8 | Combine | IsItemClickEnabled | ExistIcon | HasState,

        ExpandStroke = 5 << 8 | Combine | IsItemClickEnabled | ExistIcon | HasState,


        // Setup
        CropCanvas = 1 << 8 | Setup | IsItemClickEnabled | ExistIcon | HasPreview,

        Stretch = 2 << 8 | Setup | IsItemClickEnabled | ExistIcon,

        FlipHorizontal = 3 << 8 | Setup | IsItemClickEnabled | ExistIcon,
        FlipVertical = 4 << 8 | Setup | IsItemClickEnabled | ExistIcon,

        LeftTurn = 5 << 8 | Setup | IsItemClickEnabled | ExistIcon,
        RightTurn = 6 << 8 | Setup | IsItemClickEnabled | ExistIcon,
        OverTurn = 7 << 8 | Setup | IsItemClickEnabled | ExistIcon,


        #endregion


        // Option
        More = 1 << 12 | Option,
        Adjustment = 2 << 12 | Option,
        Effect1 = 4 << 12 | Option,
        Effect2 = 8 << 12 | Option,
        Effect3 = 16 << 12 | Option,

        #region Option


        // More
        Transform = 1 << 8 | More | IsItemClickEnabled | ExistIcon | HasPreview | HasDifference,
        DisplacementLiquefaction = 2 << 8 | More | IsItemClickEnabled | ExistIcon | HasPreview | HasDifference,
        GradientMapping = 3 << 8 | More | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview,
        RippleEffect = 4 << 8 | More | IsItemClickEnabled | ExistIcon | AllowDrag | HasPreview | HasDifference,
        Fill = 5 << 8 | More | IsItemClickEnabled | ExistIcon | HasPreview | TempOverlay,


        // Adjustment
        Gray = 1 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Invert = 2 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Exposure = 3 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        Brightness = 4 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        Saturation = 5 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        HueRotation = 6 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        Contrast = 7 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        Temperature = 8 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,
        HighlightsAndShadows = 9 << 8 | Adjustment | IsItemClickEnabled | ExistIcon | ExistThumbnail | AllowDrag | HasPreview,


        // Effect1
        GaussianBlur = 1 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        DirectionalBlur = 2 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Sharpen = 3 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Shadow = 4 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        ChromaKey = 5 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        EdgeDetection = 6 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Border = 7 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Emboss = 8 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Lighting = 9 << 8 | Effect1 | IsItemClickEnabled | ExistIcon | ExistThumbnail,


        // Effect2
        LuminanceToAlpha = 1 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail | HasPreview,
        Fog = 2 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Sepia = 3 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Posterize = 4 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Colouring = 5 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Tint = 6 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        DiscreteTransfer = 7 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Vignette = 8 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        GammaTransfer = 9 << 8 | Effect2 | IsItemClickEnabled | ExistIcon | ExistThumbnail,


        // Effect3
        Glass = 1 << 8 | Effect3 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        PinchPunch = 2 << 8 | Effect3 | IsItemClickEnabled | ExistIcon | ExistThumbnail,
        Morphology = 3 << 8 | Effect3 | IsItemClickEnabled | ExistIcon | ExistThumbnail,


        #endregion


        // Tool
        Marquee = 1 << 12 | Tool,
        Selection = 2 << 12 | Tool,
        Paint = 4 << 12 | Tool,
        Vector = 8 << 12 | Tool,
        Curve = 16 << 12 | Tool,
        Text = 32 << 12 | Tool,
        Geometry = 64 << 12 | Tool,
        Pattern = 128 << 12 | Tool,

        #region Tool


        // Marquee
        MarqueeRectangular = 1 << 8 | Marquee | IsItemClickEnabled | ExistIcon,
        MarqueeElliptical = 2 << 8 | Marquee | IsItemClickEnabled | ExistIcon,
        MarqueePolygon = 3 << 8 | Marquee | IsItemClickEnabled | ExistIcon,
        MarqueeFreeHand = 4 << 8 | Marquee | IsItemClickEnabled | ExistIcon,


        // Selection
        SelectionFlood = 1 << 8 | Selection | IsItemClickEnabled | ExistIcon,
        SelectionBrush = 2 << 8 | Selection | IsItemClickEnabled | ExistIcon,


        // Paint
        PaintBrush = 1 << 8 | Paint | IsItemClickEnabled | ExistIcon,
        PaintWatercolorPen = 2 << 8 | Paint | IsItemClickEnabled | ExistIcon,
        PaintPencil = 3 << 8 | Paint | IsItemClickEnabled | ExistIcon,
        PaintEraseBrush = 4 << 8 | Paint | IsItemClickEnabled | ExistIcon,
        PaintLiquefaction = 5 << 8 | Paint | IsItemClickEnabled | ExistIcon,


        // Vector
        Cursor = 1 << 8 | Vector | IsItemClickEnabled | ExistIcon,
        View = 2 << 8 | Vector | IsItemClickEnabled | ExistIcon,
        Crop = 3 << 8 | Vector | IsItemClickEnabled | ExistIcon,

        Brush = 4 << 8 | Vector | IsItemClickEnabled | ExistIcon,
        Transparency = 5 << 8 | Vector | IsItemClickEnabled | ExistIcon,

        Image = 6 << 8 | Vector | IsItemClickEnabled | ExistIcon,


        // Curve
        Node = 1 << 8 | Curve | IsItemClickEnabled | ExistIcon,
        Pen = 2 << 8 | Curve | IsItemClickEnabled | ExistIcon,


        // Text
        TextArtistic = 1 << 8 | Text | IsItemClickEnabled | ExistIcon,
        TextFrame = 2 << 8 | Text | IsItemClickEnabled | ExistIcon,


        // Geometry
        // Geometry0
        GeometryRectangle = 1 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryEllipse = 2 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        // Geometry1
        GeometryRoundRect = 3 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryTriangle = 4 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryDiamond = 5 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        // Geometry2
        GeometryPentagon = 6 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryStar = 7 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryCog = 8 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        // Geometry3
        GeometryDount = 9 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryPie = 10 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryCookie = 11 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        // Geometry4
        GeometryArrow = 12 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryCapsule = 13 << 8 | Geometry | IsItemClickEnabled | ExistIcon,
        GeometryHeart = 14 << 8 | Geometry | IsItemClickEnabled | ExistIcon,


        // Pattern
        PatternGrid = 1 << 8 | Pattern | IsItemClickEnabled | ExistIcon,
        PatternDiagonal = 2 << 8 | Pattern | IsItemClickEnabled | ExistIcon,
        PatternSpotted = 3 << 8 | Pattern | IsItemClickEnabled | ExistIcon,


        #endregion

    }
}