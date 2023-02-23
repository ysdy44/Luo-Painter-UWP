namespace Luo_Painter.Models
{
    public enum ElementType
    {
        Snap,
        SnapToTick,

        ResetRadian,
        ResetScale,

        ConvertToCurves,

        // Ink
        InkErase,
        InkGeneral,
        InkTip,
        InkLine,
        InkBlur,
        InkMosaic,
        InkLiquefy,

        // Harmony
        HarmonyComplementary,
        HarmonySplitComplementary,
        HarmonyAnalogous,
        HarmonyTriadic,
        HarmonyTetradic,

        // CompositeModes
        CompositeAdd,
        CompositeSubtract,

        // Combine
        CombineNew,
        CombineUnion,
        CombineExclude,
        CombineIntersect,

        // Nodes
        NodeInsert,
        NodeRemove,
        NodeSharp,
        NodeSmooth,

        // NodeModes
        NodeDisconnected,
        NodeMirrored,
        NodeAsymmetric,

        // ArrowTailTypes
        ArrowNone,
        ArrowArrow,

        // PatternGridTypes
        GridGrid,
        GridColumn,
        GridRow,

        // BrushEdgeHardness
        HardnessNone,
        HardnessCosine,
        HardnessQuadratic,
        HardnessCube,
        HardnessQuartic,

        // SymmetryModes
        SymmetryX,
        SymmetryY,
        SymmetryRadial,
        MirrorRadial
    }
}