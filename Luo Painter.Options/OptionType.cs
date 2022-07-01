using System;

namespace Luo_Painter.Options
{
    /// <summary>
    /// Root4___Category12_________Item4___Flag12                                <para/>
    /// 0000____0000_0000_0000____0000____0000_0000_0000
    /// </summary>
    [Flags]
    public enum OptionType : uint
    {
        // None
        None = 0,

        // Flag
        IsItemClickEnabled = 1,
        ExistIcon = 2,
        ExistThumbnail = 4,
        AllowDrag = 8,
        HasPreview = 16,
        HasDifference = 32,
        TempOverlay = 64,
        HasState = 128,

        // Foot
        Edit = 1 << 28,
        Option = 2 << 28,
        Tool = 4 << 28,


        // Edit
        Layering = Edit | 1 << 16,
        Editing = Edit | 2 << 16,
        Grouping = Edit | 4 << 16,
        Select = Edit | 8 << 16,
        Combine = Edit | 16 << 16,
        Setup = Edit | 32 << 16,

        #region Edit


        // Layer
        Remove = Layering | 1 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        AddLayer = Layering | 2 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        AddImageLayer = Layering | 3 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        CutLayer = Layering | 4 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        CopyLayer = Layering | 5 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        PasteLayer = Layering | 6 << 8 | HasState | ExistIcon | IsItemClickEnabled,


        // Editing
        Cut = Editing | 1 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Duplicate = Editing | 2 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Copy = Editing | 3 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Paste = Editing | 4 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        Clear = Editing | 5 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        Extract = Editing | 6 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Merge = Editing | 7 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Flatten = Editing | 8 << 8 | HasState | ExistIcon | IsItemClickEnabled,


        // Grouping
        Group = Grouping | 1 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Ungroup = Grouping | 2 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        Release = Grouping | 3 << 8 | HasState | ExistIcon | IsItemClickEnabled,


        // Select
        All = Select | 1 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Deselect = Select | 2 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        MarqueeInvert = Select | 3 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Pixel = Select | 4 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        Feather = Select | 5 << 8 | HasState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        MarqueeTransform = 6 << 8 | Select | HasState | HasDifference | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        Grow = Select | 7 << 8 | HasState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        Shrink = Select | 8 << 8 | HasState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,


        // Combine
        Union = Combine | 1 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Exclude = Combine | 2 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Xor = Combine | 3 << 8 | HasState | ExistIcon | IsItemClickEnabled,
        Intersect = Combine | 4 << 8 | HasState | ExistIcon | IsItemClickEnabled,

        ExpandStroke = Combine | 5 << 8 | HasState | ExistIcon | IsItemClickEnabled,


        // Setup
        CropCanvas = Setup | 1 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,

        Stretch = Setup | 2 << 8 | ExistIcon | IsItemClickEnabled,

        FlipHorizontal = Setup | 3 << 8 | ExistIcon | IsItemClickEnabled,
        FlipVertical = Setup | 4 << 8 | ExistIcon | IsItemClickEnabled,

        LeftTurn = Setup | 5 << 8 | ExistIcon | IsItemClickEnabled,
        RightTurn = Setup | 6 << 8 | ExistIcon | IsItemClickEnabled,
        OverTurn = Setup | 7 << 8 | ExistIcon | IsItemClickEnabled,


        #endregion


        // Option
        More = Option | 1 << 16,
        Adjustment = Option | 2 << 16,
        Effect1 = Option | 4 << 16,
        Effect2 = Option | 8 << 16,
        Effect3 = Option | 16 << 16,

        #region Option


        // More
        Transform = More | 1 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        DisplacementLiquefaction = More | 2 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        GradientMapping = More | 3 << 8 | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        RippleEffect = More | 4 << 8 | HasPreview | AllowDrag | ExistIcon | HasDifference | IsItemClickEnabled,
        Fill = More | 5 << 8 | TempOverlay | HasPreview | ExistIcon | IsItemClickEnabled,


        // Adjustment
        Gray = Adjustment | 1 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Invert = Adjustment | 2 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Exposure = Adjustment | 3 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Brightness = Adjustment | 4 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Saturation = Adjustment | 5 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        HueRotation = Adjustment | 6 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Contrast = Adjustment | 7 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Temperature = Adjustment | 8 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        HighlightsAndShadows = Adjustment | 9 << 8 | HasPreview | AllowDrag | ExistThumbnail | ExistIcon | IsItemClickEnabled,


        // Effect1
        GaussianBlur = Effect1 | 1 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        DirectionalBlur = Effect1 | 2 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Sharpen = Effect1 | 3 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Shadow = Effect1 | 4 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        ChromaKey = Effect1 | 5 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        EdgeDetection = Effect1 | 6 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Border = Effect1 | 7 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Emboss = Effect1 | 8 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Lighting = Effect1 | 9 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,


        // Effect2
        LuminanceToAlpha = Effect2 | 1 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Fog = Effect2 | 2 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Sepia = Effect2 | 3 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Posterize = Effect2 | 4 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Colouring = Effect2 | 5 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Tint = Effect2 | 6 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        DiscreteTransfer = Effect2 | 7 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Vignette = Effect2 | 8 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        GammaTransfer = Effect2 | 9 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,


        // Effect3
        Glass = Effect3 | 1 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        PinchPunch = Effect3 | 2 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Morphology = Effect3 | 3 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,


        #endregion


        // Tool
        Marquee = Tool | 1 << 16,
        Selection = Tool | 2 << 16,
        Paint = Tool | 4 << 16,
        Vector = Tool | 8 << 16,
        Curve = Tool | 16 << 16,
        Text = Tool | 32 << 16,
        Geometry = Tool | 64 << 16,
        Pattern = Tool | 128 << 16,

        #region Tool


        // Marquee
        MarqueeRectangular = Marquee | 1 << 8 | ExistIcon | IsItemClickEnabled,
        MarqueeElliptical = Marquee | 2 << 8 | ExistIcon | IsItemClickEnabled,
        MarqueePolygon = Marquee | 3 << 8 | ExistIcon | IsItemClickEnabled,
        MarqueeFreeHand = Marquee | 4 << 8 | ExistIcon | IsItemClickEnabled,


        // Selection
        SelectionFlood = Selection | 1 << 8 | ExistIcon | IsItemClickEnabled,
        SelectionBrush = Selection | 2 << 8 | ExistIcon | IsItemClickEnabled,


        // Paint
        PaintBrush = Paint | 1 << 8 | ExistIcon | IsItemClickEnabled,
        PaintWatercolorPen = Paint | 2 << 8 | ExistIcon | IsItemClickEnabled,
        PaintPencil = Paint | 3 << 8 | ExistIcon | IsItemClickEnabled,
        PaintEraseBrush = Paint | 4 << 8 | ExistIcon | IsItemClickEnabled,
        PaintLiquefaction = Paint | 5 << 8 | ExistIcon | IsItemClickEnabled,


        // Vector
        Cursor = Vector | 1 << 8 | ExistIcon | IsItemClickEnabled,
        View = Vector | 2 << 8 | ExistIcon | IsItemClickEnabled,
        Crop = Vector | 3 << 8 | ExistIcon | IsItemClickEnabled,

        Brush = Vector | 4 << 8 | ExistIcon | IsItemClickEnabled,
        Transparency = Vector | 5 << 8 | ExistIcon | IsItemClickEnabled,

        Image = Vector | 6 << 8 | ExistIcon | IsItemClickEnabled,


        // Curve
        Node = Curve | 1 << 8 | ExistIcon | IsItemClickEnabled,
        Pen = Curve | 2 << 8 | ExistIcon | IsItemClickEnabled,


        // Text
        TextArtistic = Text | 1 << 8 | ExistIcon | IsItemClickEnabled,
        TextFrame = Text | 2 << 8 | ExistIcon | IsItemClickEnabled,


        // Geometry
        // Geometry0
        GeometryRectangle = Geometry | 1 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryEllipse = Geometry | 2 << 8 | ExistIcon | IsItemClickEnabled,
        // Geometry1
        GeometryRoundRect = Geometry | 3 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryTriangle = Geometry | 4 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryDiamond = Geometry | 5 << 8 | ExistIcon | IsItemClickEnabled,
        // Geometry2
        GeometryPentagon = Geometry | 6 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryStar = Geometry | 7 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCog = Geometry | 8 << 8 | ExistIcon | IsItemClickEnabled,
        // Geometry3
        GeometryDount = Geometry | 9 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryPie = Geometry | 10 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCookie = Geometry | 11 << 8 | ExistIcon | IsItemClickEnabled,
        // Geometry4
        GeometryArrow = Geometry | 12 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCapsule = Geometry | 13 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryHeart = Geometry | 14 << 8 | ExistIcon | IsItemClickEnabled,


        // Pattern
        PatternGrid = Pattern | 1 << 8 | ExistIcon | IsItemClickEnabled,
        PatternDiagonal = Pattern | 2 << 8 | ExistIcon | IsItemClickEnabled,
        PatternSpotted = Pattern | 3 << 8 | ExistIcon | IsItemClickEnabled,


        #endregion

    }
}