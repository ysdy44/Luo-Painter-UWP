namespace Luo_Painter.Tools
{
    public enum ToolType
    {
        None,

        Cursor,
        View,
        Crop,

        Brush,
        Transparency,

        Image,

        Node,
        Pen,

        TextArtistic,
        TextFrame,

        MarqueeRectangular,
        MarqueeElliptical,
        MarqueePolygon,
        MarqueeFreeHand,
        MarqueeSelectionBrush,
        MarqueeFloodSelect,

        SelectionFlood,
        SelectionBrush,

        // Geometry0
        GeometryRectangle,
        GeometryEllipse,
        // Geometry1
        GeometryRoundRect,
        GeometryTriangle,
        GeometryDiamond,
        // Geometry2
        GeometryPentagon,
        GeometryStar,
        GeometryCog,
        // Geometry3
        GeometryDount,
        GeometryPie,
        GeometryCookie,
        // Geometry4
        GeometryArrow,
        GeometryCapsule,
        GeometryHeart,

        PatternGrid,
        PatternDiagonal,
        PatternSpotted,

        PaintBrush,
        PaintWatercolorPen,
        PaintPencil,
        PaintEraseBrush,
        PaintLiquefaction,
    }
}