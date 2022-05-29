namespace Luo_Painter.Elements
{
    public enum ElementType
    {
        // None
        Brush,
        Edit,
        Layer,
        Option,
        MoreOption,

        Snap,
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
        ArrowTailTypesNone,
        ArrowTailTypesArrow,

        // PatternGridTypes
        Grid,
        Horizontal,
        Vertical,

        // BrushEdgeHardness
        BrushEdgeHardnessNone,
        BrushEdgeHardnessCosine,
        BrushEdgeHardnessQuadratic,
        BrushEdgeHardnessCube,
        BrushEdgeHardnessQuartic,
    }
}