using System;

namespace Luo_Painter.Options
{
    /// <summary>
    /// None: <para/>
    /// 0b_00000000_00000000_00000000_00000000 <para/>
    /// 
    /// Root: <para/>
    /// 0b_11111111_00000000_00000000_00000000 <para/>
    /// 
    /// Category: <para/>
    /// 0b_00000000_11111111_00000000_00000000 <para/>
    /// 
    /// Item: <para/>
    /// 0b_00000000_00000000_11111111_00000000 <para/>
    /// 
    /// Flag: <para/>
    /// 0b_00000000_00000000_00000000_11111111 <para/>
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
        WithState = 64,
        WithTransform = 128,

        // Root
        File = 1 << 24,
        Edit = 2 << 24,
        Menu = 4 << 24,
        Setup = 8 << 24,
        Layer = 16 << 24,
        Select = 32 << 24,
        Effect = 64 << 24,
        Tool = (uint)128 << 24,

        #region File

        Close = File | 1 << 8 | IsItemClickEnabled,
        Save = File | 2 << 8 | IsItemClickEnabled,

        Export = File | 3 << 8 | IsItemClickEnabled,
        ExportAll = File | 4 << 8 | IsItemClickEnabled,
        ExportCurrent = File | 5 << 8 | IsItemClickEnabled,

        Undo = File | 6 << 8 | IsItemClickEnabled,
        Redo = File | 7 << 8 | IsItemClickEnabled,

        FullScreen = File | 8 << 8 | IsItemClickEnabled,
        UnFullScreen = File | 9 << 8 | IsItemClickEnabled,

        #endregion

        #region Edit

        Cut = Edit | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Copy = Edit | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Paste = Edit | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Clear = Edit | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Menu

        DockLeft = Menu | 1 << 8 | IsItemClickEnabled,
        DockRight = Menu | 2 << 8 | IsItemClickEnabled,

        DockLeftMenu = Menu | 3 << 8 | IsItemClickEnabled,
        DockRightMenu = Menu | 4 << 8 | IsItemClickEnabled,

        ExportMenu = Menu | 5 << 8 | IsItemClickEnabled,
        ColorMenu = Menu | 6 << 8 | IsItemClickEnabled,
        PaletteMenu = Menu | 7 << 8 | IsItemClickEnabled,

        EditMenu = Menu | 8 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        AdjustmentMenu = Menu | 9 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        OtherMenu = Menu | 10 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,

        PaintMenu = Menu | 11 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        BrushMenu = Menu | 12 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        SizeMenu = Menu | 13 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,

        ToolMenu = Menu | 14 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,

        HistoryMenu = Menu | 15 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        EffectMenu = Menu | 16 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,
        LayerMenu = Menu | 17 << 8 | IsItemClickEnabled | ExistIcon | IsItemClickEnabled,

        AddMenu = Menu | 18 << 8 | IsItemClickEnabled,
        PropertyMenu = Menu | 19 << 8 | IsItemClickEnabled,
        PropertyMenuWithRename = Menu | 20 << 8,

        #endregion

        #region Setup

        CropCanvas = Setup | 1 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,

        Stretch = Setup | 2 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        Extend = Setup | 3 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        Offset = Setup | 4 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,

        FlipHorizontal = Setup | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        FlipVertical = Setup | 6 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        LeftTurn = Setup | 7 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        RightTurn = Setup | 8 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        OverTurn = Setup | 9 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Layer

        // Category
        Add = Layer | 1 << 16,
        Clipboard = Layer | 2 << 16,
        Layering = Layer | 4 << 16,
        Grouping = Layer | 8 << 16,
        Combine = Layer | 16 << 16,

        // Add
        AddLayer = Add | 2 << 8 | ExistIcon | IsItemClickEnabled,
        AddImageLayer = Add | 3 << 8 | ExistIcon | IsItemClickEnabled,
        AddCurveLayer = Add | 4 << 8 | ExistIcon | IsItemClickEnabled,

        // Clipboard
        CutLayer = Clipboard | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        CopyLayer = Clipboard | 6 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        PasteLayer = Clipboard | 7 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Layering
        Remove = Layering | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Duplicate = Layering | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Extract = Layering | 6 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Merge = Layering | 7 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Flatten = Layering | 8 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Grouping
        Group = Grouping | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Ungroup = Grouping | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Release = Grouping | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Combine
        Union = Combine | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Exclude = Combine | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Xor = Combine | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Intersect = Combine | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        ExpandStroke = Layer | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Select

        // Category
        Selecting = Select | 1 << 16,
        Marquees = Select | 2 << 16,

        // Selecting
        All = Selecting | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Deselect = Selecting | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        MarqueeInvert = Selecting | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Pixel = Selecting | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Marquees
        Feather = Marquees | 5 << 8 | WithState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        MarqueeTransform = 6 << 8 | Marquees | WithTransform | WithState | HasDifference | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        Grow = Marquees | 7 << 8 | WithState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        Shrink = Marquees | 8 << 8 | WithState | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Effect

        // Category
        Other = Effect | 1 << 16,
        Adjustment = Effect | 2 << 16,
        Effect1 = Effect | 4 << 16,
        Effect2 = Effect | 8 << 16,
        Effect3 = Effect | 16 << 16,

        // Other
        Transform = Other | 1 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        DisplacementLiquefaction = Other | 2 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        GradientMapping = Other | 3 << 8 | HasPreview | AllowDrag | ExistIcon | IsItemClickEnabled,
        RippleEffect = Other | 4 << 8 | HasPreview | AllowDrag | ExistIcon | HasDifference | IsItemClickEnabled,
        Fill = Other | 5 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,
        Threshold = Other | 6 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,

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
        Straighten = Effect3 | 4 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Edge = Effect3 | 5 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Tool

        // Category
        Marquee = Tool | 1 << 16,
        Selection = Tool | 2 << 16,
        Paint = Tool | 4 << 16,
        Vector = Tool | 8 << 16,
        Curve = Tool | 16 << 16,
        Text = Tool | 32 << 16,
        Geometry = Tool | 64 << 16,
        Pattern = Tool | 128 << 16,

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
        PaintLine = Paint | 2 << 8 | ExistIcon | IsItemClickEnabled,
        PaintBrushForce = Paint | 3 << 8 | ExistIcon | IsItemClickEnabled,
        PaintBrushMulti = Paint | 4 << 8 | ExistIcon | IsItemClickEnabled,
        PaintStraw = Paint | 5 << 8 | ExistIcon | IsItemClickEnabled,

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

        // GeometryTransform
        // Geometry0
        GeometryRectangleTransform = GeometryRectangle | WithTransform | HasPreview | AllowDrag,
        GeometryEllipseTransform = GeometryEllipse | WithTransform | HasPreview | AllowDrag,
        // Geometry1
        GeometryRoundRectTransform = GeometryRoundRect | WithTransform | HasPreview | AllowDrag,
        GeometryTriangleTransform = GeometryTriangle | WithTransform | HasPreview | AllowDrag,
        GeometryDiamondTransform = GeometryDiamond | WithTransform | HasPreview | AllowDrag,
        // Geometry2
        GeometryPentagonTransform = GeometryPentagon | WithTransform | HasPreview | AllowDrag,
        GeometryStarTransform = GeometryStar | WithTransform | HasPreview | AllowDrag,
        GeometryCogTransform = GeometryCog | WithTransform | HasPreview | AllowDrag,
        // Geometry3
        GeometryDountTransform = GeometryDount | WithTransform | HasPreview | AllowDrag,
        GeometryPieTransform = GeometryPie | WithTransform | HasPreview | AllowDrag,
        GeometryCookieTransform = GeometryCookie | WithTransform | HasPreview | AllowDrag,
        // Geometry4
        GeometryArrowTransform = GeometryArrow | WithTransform | HasPreview | AllowDrag,
        GeometryCapsuleTransform = GeometryCapsule | WithTransform | HasPreview | AllowDrag,
        GeometryHeartTransform = GeometryHeart | WithTransform | HasPreview | AllowDrag,

    }
}