namespace Luo_Painter.Models
{
    public enum ElementType
    {
        Snap,
        SnapToTick,

        ResetRadian,
        ResetScale,

        ConvertToCurves,

        // Harmony
        Complementary,
        SplitComplementary,
        Analogous,
        Triadic,
        Tetradic,

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

        // SymmetryModes
        SymmetryX,
        SymmetryY,
        SymmetryRadial,
        MirrorRadial
    }
}