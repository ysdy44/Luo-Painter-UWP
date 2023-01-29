namespace Luo_Painter.Brushes
{
    /// <summary>
    /// Pressure of the brush easing.
    /// </summary>
    public enum BrushEasePressure : byte
    {
        /// <summary> f (x) = 1 </summary>
        None,

        /// <summary> f (x) = x </summary>
        Linear,
        /// <summary> f (x) = 1 - x </summary>
        LinearFlip,

        /// <summary> f (x) = x * x </summary>
        Quadratic,
        /// <summary> f (x) = 1 - x * x </summary>
        QuadraticFlip,
        /// <summary> f (x) = (1 - x) * (1 - x) </summary>
        QuadraticReverse,
        /// <summary> f (x) = 1 - (1 - x) * (1 - x) </summary>
        QuadraticFlipReverse,

        /// <summary> f (x) = (x / 2 - 1) * (x / 2 - 1) </summary>
        Mirror,
        /// <summary> f (x) = 1 - (x / 2 - 1) * (x / 2 - 1) </summary>
        MirrorFlip,

        /// <summary> 2f (x) = 2x * 2x </summary>
        Symmetry,
        /// <summary> 2f (x) = 1 - 2x * 2x </summary>
        SymmetryFlip,
    }
}