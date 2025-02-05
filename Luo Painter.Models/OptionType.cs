using System;

namespace Luo_Painter.Models
{
    /// <summary>
    /// None: <para/>
    /// 0b_00000000_00000000_00000000_00000000 <para/>
    ///
    /// Root: <para/>
    /// 0b_00011111_00000000_00000000_00000000 <para/>
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
    public enum OptionType
    {
        // None
        None = 0,

        // Flag
        IsItemClickEnabled = 1,
        ExistIcon = 2,
        ExistThumbnail = 4,
        HasMenu = 8,
        HasPreview = 16,
        HasDifference = 32,
        WithState = 64,
        WithTransform = 128,

        // Root
        App = 1 << 24,
        Option = 2 << 24,
        Layer = 4 << 24,
        Effect = 8 << 24,
        Tool = 16 << 24,

        #region App

        // Category
        File = App | 1 << 16,
        Layout = App | 2 << 16,
        Format = App | 4 << 16,
        Menu = App | 8 << 16,

        // File
        Home = File | 1 << 8 | IsItemClickEnabled,
        Close = File | 2 << 8 | IsItemClickEnabled,
        Save = File | 3 << 8 | IsItemClickEnabled,

        Export = File | 4 << 8 | IsItemClickEnabled,
        ExportAll = File | 5 << 8 | IsItemClickEnabled,
        ExportCurrent = File | 6 << 8 | IsItemClickEnabled,

        Undo = File | 7 << 8 | IsItemClickEnabled,
        Redo = File | 8 << 8 | IsItemClickEnabled,

        // Layout
        FullScreen = Layout | 1 << 8 | IsItemClickEnabled,
        UnFullScreen = Layout | 2 << 8 | IsItemClickEnabled,

        DockLeft = Layout | 3 << 8 | IsItemClickEnabled,
        DockRight = Layout | 4 << 8 | IsItemClickEnabled,

        // Format
        JPEG = Format | 1 << 8 | ExistIcon | IsItemClickEnabled,
        PNG = Format | 2 << 8 | ExistIcon | IsItemClickEnabled,
        BMP = Format | 3 << 8 | ExistIcon | IsItemClickEnabled,
        GIF = Format | 4 << 8 | ExistIcon | IsItemClickEnabled,
        TIFF = Format | 5 << 8 | ExistIcon | IsItemClickEnabled,

        // Menu
        FileMenu = Menu | 1 << 8 | HasMenu | IsItemClickEnabled,
        ExportMenu = Menu | 2 << 8 | HasMenu | IsItemClickEnabled,
        HistogramMenu = Menu | 3 << 8 | HasMenu | IsItemClickEnabled,

        ColorMenu = Menu | 4 << 8 | HasMenu | IsItemClickEnabled,
        ColorHarmonyMenu = Menu | 5 << 8 | HasMenu | IsItemClickEnabled,

        EditMenu = Menu | 6 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        AdjustmentMenu = Menu | 7 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        OtherMenu = Menu | 8 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        PaintMenu = Menu | 9 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        BrushMenu = Menu | 10 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        SizeMenu = Menu | 11 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        EffectMenu = Menu | 12 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        HistoryMenu = Menu | 13 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        ToolMenu = Menu | 14 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        LayerMenu = Menu | 15 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        LayerNewMenu = Menu | 16 << 8 | HasMenu | IsItemClickEnabled,
        LayerPropertyMenu = Menu | 17 << 8 | HasMenu | IsItemClickEnabled,
        LayerRenameMenu = Menu | 18 << 8 | HasMenu | IsItemClickEnabled,

        #endregion

        #region Option

        // Category
        Edit = Option | 1 << 16,
        Select = Option | 2 << 16,
        Marquees = Option | 4 << 16,

        CropCanvas = Option | 8 << 16 | 1 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        ResizeCanvas = Option | 16 << 16 | ExistIcon | IsItemClickEnabled,
        RotateCanvas = Option | 32 << 16 | ExistIcon | IsItemClickEnabled,

        // Edit
        Cut = Edit | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Copy = Edit | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Paste = Edit | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Clear = Edit | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Select
        All = Select | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Deselect = Select | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        MarqueeInvert = Select | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Pixel = Select | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Marquees
        Feather = Marquees | 1 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        MarqueeTransform = Marquees | 2 << 8 | WithState | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        Grow = Marquees | 3 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        Shrink = Marquees | 4 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,

        // CropCanvas

        // ResizeCanvas
        Stretch = ResizeCanvas | 1 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,
        Extend = ResizeCanvas | 2 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,
        Offset = ResizeCanvas | 3 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,

        // RotateCanvas
        FlipHorizontal = RotateCanvas | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        FlipVertical = RotateCanvas | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        LeftTurn = RotateCanvas | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        RightTurn = RotateCanvas | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        OverTurn = RotateCanvas | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Layer

        // Category
        New = Layer | 1 << 16,

        Clipboard = Layer | 2 << 16,
        Layering = Layer | 4 << 16,
        Grouping = Layer | 8 << 16,
        Combine = Layer | 16 << 16,

        Transforms = Layer | 32 << 16,
        Arrange = Layer | 64 << 16,
        Align = Layer | 128 << 16,

        // New
        AddLayer = New | 1 << 8 | ExistIcon | IsItemClickEnabled,
        AddBitmapLayer = New | 2 << 8 | ExistIcon | IsItemClickEnabled,
        AddImageLayer = New | 3 << 8 | ExistIcon | IsItemClickEnabled,
        AddCurveLayer = New | 4 << 8 | ExistIcon | IsItemClickEnabled,
        AddFillLayer = New | 5 << 8 | ExistIcon | IsItemClickEnabled,

        // Clipboard
        CutLayer = Clipboard | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        CopyLayer = Clipboard | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        PasteLayer = Clipboard | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Layering
        Remove = Layering | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Duplicate = Layering | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Extract = Layering | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Merge = Layering | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Flatten = Layering | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Grouping
        Group = Grouping | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Ungroup = Grouping | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Release = Grouping | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Combine
        Union = Combine | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Exclude = Combine | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Xor = Combine | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Intersect = Combine | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        ExpandStroke = Combine | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Transforms
        MirrorHorizontally = Transforms | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        MirrorVertically = Transforms | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        RotateLeft = Transforms | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        RotateRight = Transforms | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Arrange
        BackOne = Arrange | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        ForwardOne = Arrange | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        MoveBack = Arrange | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        MoveFront = Arrange | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        // Align
        AlignLeft = Align | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        AlignCenter = Align | 2 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        AlignRight = Align | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        SpaceHorizontally = Align | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        AlignTop = Align | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        AlignMiddle = Align | 6 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        AlignBottom = Align | 7 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        SpaceVertically = Align | 8 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Effect

        // Category
        Other = Effect | 1 << 16,
        Adjustment = Effect | 2 << 16,
        Adjustment2 = Effect | 4 << 16,
        Effect1 = Effect | 8 << 16,
        Effect2 = Effect | 16 << 16,
        Effect3 = Effect | 32 << 16,

        // Other
        Move = Other | 1 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        Transform = Other | 2 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        FreeTransform = Other | 3 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,

        DisplacementLiquefaction = Other | 4 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        GradientMapping = Other | 5 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,
        RippleEffect = Other | 6 << 8 | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        Threshold = Other | 7 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,

        HSB = Other | 8 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,

        // Adjustment
        Gray = Adjustment | 1 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Invert = Adjustment | 2 << 8 | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Exposure = Adjustment | 3 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Brightness = Adjustment | 4 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Saturation = Adjustment | 5 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        HueRotation = Adjustment | 6 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Contrast = Adjustment | 7 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Temperature = Adjustment | 8 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        HighlightsAndShadows = Adjustment | 9 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,

        // Adjustment2
        GammaTransfer = Adjustment2 | 1 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Vignette = Adjustment2 | 2 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        ColorMatrix = Adjustment2 | 3 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        ColorMatch = Adjustment2 | 4 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,

        // Effect1
        GaussianBlur = Effect1 | 1 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        DirectionalBlur = Effect1 | 2 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Sharpen = Effect1 | 3 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Shadow = Effect1 | 4 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        EdgeDetection = Effect1 | 5 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Morphology = Effect1 | 6 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Emboss = Effect1 | 7 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,
        Straighten = Effect1 | 8 << 8 | HasPreview | ExistThumbnail | ExistIcon | IsItemClickEnabled,

        // Effect2
        Sepia = Effect2 | 1 << 8 | ExistThumbnail | IsItemClickEnabled,
        Posterize = Effect2 | 2 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        LuminanceToAlpha = Effect2 | 3 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        ChromaKey = Effect2 | 4 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        Border = Effect2 | 5 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        Colouring = Effect2 | 6 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        Tint = Effect2 | 7 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        DiscreteTransfer = Effect2 | 8 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,

        // Effect3
        Lighting = Effect3 | 1 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        Fog = Effect3 | 2 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        Glass = Effect3 | 3 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,
        PinchPunch = Effect3 | 4 << 8 | HasPreview | ExistThumbnail | IsItemClickEnabled,

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

        // Vector
        Cursor = Vector | 1 << 8 | ExistIcon | IsItemClickEnabled,
        View = Vector | 2 << 8 | ExistIcon | IsItemClickEnabled,
        Straw = Vector | 3 << 8 | ExistIcon | IsItemClickEnabled,

        Fill = Vector | 4 << 8 | ExistIcon | IsItemClickEnabled,
        Brush = Vector | 5 << 8 | ExistIcon | IsItemClickEnabled,
        Transparency = Vector | 6 << 8 | ExistIcon | IsItemClickEnabled,

        Image = Vector | 7 << 8 | ExistIcon | IsItemClickEnabled,
        Crop = Vector | 8 << 8 | ExistIcon | IsItemClickEnabled,

        // Curve
        Node = Curve | 1 << 8 | ExistIcon | IsItemClickEnabled,
        Pen = Curve | 2 << 8 | ExistIcon | IsItemClickEnabled,

        // Text
        TextArtistic = Text | 1 << 8 | ExistIcon | IsItemClickEnabled,
        TextFrame = Text | 2 << 8 | ExistIcon | IsItemClickEnabled,

        // Geometry

        GeometryRectangle = Geometry | 1 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryEllipse = Geometry | 2 << 8 | ExistIcon | IsItemClickEnabled,

        GeometryRoundRect = Geometry | 3 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryTriangle = Geometry | 4 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryDiamond = Geometry | 5 << 8 | ExistIcon | IsItemClickEnabled,

        GeometryPentagon = Geometry | 6 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryStar = Geometry | 7 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCog = Geometry | 8 << 8 | ExistIcon | IsItemClickEnabled,

        GeometryDonut = Geometry | 9 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryPie = Geometry | 10 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCookie = Geometry | 11 << 8 | ExistIcon | IsItemClickEnabled,

        GeometryArrow = Geometry | 12 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryCapsule = Geometry | 13 << 8 | ExistIcon | IsItemClickEnabled,
        GeometryHeart = Geometry | 14 << 8 | ExistIcon | IsItemClickEnabled,

        // Pattern
        PatternGrid = Pattern | 1 << 8 | ExistIcon | IsItemClickEnabled,
        PatternDiagonal = Pattern | 2 << 8 | ExistIcon | IsItemClickEnabled,
        PatternSpotted = Pattern | 3 << 8 | ExistIcon | IsItemClickEnabled,

        #endregion

        // GeometryTransform
        GeometryRectangleTransform = GeometryRectangle | WithTransform | HasPreview,
        GeometryEllipseTransform = GeometryEllipse | WithTransform | HasPreview,

        GeometryRoundRectTransform = GeometryRoundRect | WithTransform | HasPreview,
        GeometryTriangleTransform = GeometryTriangle | WithTransform | HasPreview,
        GeometryDiamondTransform = GeometryDiamond | WithTransform | HasPreview,

        GeometryPentagonTransform = GeometryPentagon | WithTransform | HasPreview,
        GeometryStarTransform = GeometryStar | WithTransform | HasPreview,
        GeometryCogTransform = GeometryCog | WithTransform | HasPreview,

        GeometryDonutTransform = GeometryDonut | WithTransform | HasPreview,
        GeometryPieTransform = GeometryPie | WithTransform | HasPreview,
        GeometryCookieTransform = GeometryCookie | WithTransform | HasPreview,

        GeometryArrowTransform = GeometryArrow | WithTransform | HasPreview,
        GeometryCapsuleTransform = GeometryCapsule | WithTransform | HasPreview,
        GeometryHeartTransform = GeometryHeart | WithTransform | HasPreview,

        // PatternTransform
        PatternGridTransform = PatternGrid | WithTransform | HasPreview,
        PatternDiagonalTransform = PatternDiagonal | WithTransform | HasPreview,
        PatternSpottedTransform = PatternSpotted | WithTransform | HasPreview,

    }
}