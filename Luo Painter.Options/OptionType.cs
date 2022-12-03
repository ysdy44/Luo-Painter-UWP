﻿using System;

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
        HasMenu = 8,
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

        Home = File | 1 << 8 | IsItemClickEnabled,
        Close = File | 2 << 8 | IsItemClickEnabled,
        Save = File | 3 << 8 | IsItemClickEnabled,

        Export = File | 5 << 8 | IsItemClickEnabled,
        ExportAll = File | 6 << 8 | IsItemClickEnabled,
        ExportCurrent = File | 7 << 8 | IsItemClickEnabled,

        Undo = File | 9 << 8 | IsItemClickEnabled,
        Redo = File | 10 << 8 | IsItemClickEnabled,

        FullScreen = File | 11 << 8 | IsItemClickEnabled,
        UnFullScreen = File | 12 << 8 | IsItemClickEnabled,

        #endregion

        #region Edit

        Cut = Edit | 1 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Copy = Edit | 3 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        Paste = Edit | 4 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        Clear = Edit | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        #endregion

        #region Menu

        FileMenu = Menu | 1 << 8 | HasMenu | IsItemClickEnabled,
        ExportMenu = Menu | 2 << 8 | HasMenu | IsItemClickEnabled,

        ColorMenu = Menu | 6 << 8 | HasMenu | IsItemClickEnabled,
        PaletteMenu = Menu | 7 << 8 | HasMenu | IsItemClickEnabled,

        EditMenu = Menu | 8 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        AdjustmentMenu = Menu | 9 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        OtherMenu = Menu | 10 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        PaintMenu = Menu | 11 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        BrushMenu = Menu | 12 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        SizeMenu = Menu | 13 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        EffectMenu = Menu | 14 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        HistoryMenu = Menu | 15 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        ToolMenu = Menu | 16 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,
        LayerMenu = Menu | 17 << 8 | HasMenu | ExistIcon | IsItemClickEnabled,

        AddMenu = Menu | 18 << 8 | HasMenu | IsItemClickEnabled,
        PropertyMenu = Menu | 19 << 8 | HasMenu | IsItemClickEnabled,
        PropertyMenuWithRename = Menu | 20 << 8,

        #endregion

        #region Setup

        CropCanvas = Setup | 1 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,

        Stretch = Setup | 2 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,
        Extend = Setup | 3 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,
        Offset = Setup | 4 << 8 | WithState | HasMenu | ExistIcon | IsItemClickEnabled,

        FlipHorizontal = Setup | 5 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        FlipVertical = Setup | 6 << 8 | WithState | ExistIcon | IsItemClickEnabled,

        LeftTurn = Setup | 7 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        RightTurn = Setup | 8 << 8 | WithState | ExistIcon | IsItemClickEnabled,
        OverTurn = Setup | 9 << 8 | WithState | ExistIcon | IsItemClickEnabled,

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
        AddLayer = New | 0 << 8 | ExistIcon | IsItemClickEnabled,
        AddBitmapLayer = New | 1 << 8 | ExistIcon | IsItemClickEnabled,
        AddImageLayer = New | 2 << 8 | ExistIcon | IsItemClickEnabled,
        AddCurveLayer = New | 3 << 8 | ExistIcon | IsItemClickEnabled,

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
        Feather = Marquees | 1 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        MarqueeTransform = Marquees | 2 << 8 | WithState | HasDifference | HasPreview | ExistIcon | IsItemClickEnabled,
        Grow = Marquees | 3 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,
        Shrink = Marquees | 4 << 8 | WithState | HasPreview | ExistIcon | IsItemClickEnabled,

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
        RippleEffect = Other | 6 << 8 | HasPreview | ExistIcon | HasDifference | IsItemClickEnabled,
        Fill = Other | 7 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,
        Threshold = Other | 8 << 8 | HasPreview | ExistIcon | IsItemClickEnabled,

        // Adjustment1
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
        Posterize = Effect2 | 2 << 8 | ExistThumbnail | IsItemClickEnabled,
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
        Crop = Vector | 4 << 8 | ExistIcon | IsItemClickEnabled,

        Brush = Vector | 5 << 8 | ExistIcon | IsItemClickEnabled,
        Transparency = Vector | 6 << 8 | ExistIcon | IsItemClickEnabled,

        Image = Vector | 7 << 8 | ExistIcon | IsItemClickEnabled,

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
        GeometryRectangleTransform = GeometryRectangle | WithTransform | HasPreview,
        GeometryEllipseTransform = GeometryEllipse | WithTransform | HasPreview,
        // Geometry1
        GeometryRoundRectTransform = GeometryRoundRect | WithTransform | HasPreview,
        GeometryTriangleTransform = GeometryTriangle | WithTransform | HasPreview,
        GeometryDiamondTransform = GeometryDiamond | WithTransform | HasPreview,
        // Geometry2
        GeometryPentagonTransform = GeometryPentagon | WithTransform | HasPreview,
        GeometryStarTransform = GeometryStar | WithTransform | HasPreview,
        GeometryCogTransform = GeometryCog | WithTransform | HasPreview,
        // Geometry3
        GeometryDountTransform = GeometryDount | WithTransform | HasPreview,
        GeometryPieTransform = GeometryPie | WithTransform | HasPreview,
        GeometryCookieTransform = GeometryCookie | WithTransform | HasPreview,
        // Geometry4
        GeometryArrowTransform = GeometryArrow | WithTransform | HasPreview,
        GeometryCapsuleTransform = GeometryCapsule | WithTransform | HasPreview,
        GeometryHeartTransform = GeometryHeart | WithTransform | HasPreview,

    }
}