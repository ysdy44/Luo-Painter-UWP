namespace Luo_Painter.Elements
{
    public enum ElementType
    {
        // None
        SnapToTick,
        ResetRadian,
        ResetScale,
        ConvertToCurves,

        // CompositeModes
        New,
        Add,
        Subtract,
        Intersect,

        // Combine
        Union,
        Exclude,
        Simultaneously,

        // Nodes
        Insert,
        Remove,
        Sharp,
        Smooth,

        // NodeModes
        Disconnected,
        Mirrored,
        Asymmetric,

        // ArrowTailTypes
        None,
        Arrow,

        // PatternGridTypes
        Grid,
        Horizontal,
        Vertical,
    }
}