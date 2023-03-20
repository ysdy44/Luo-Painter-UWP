namespace Luo_Painter.Brushes
{
    /// <summary>
    /// Pressure of the brush easing.
    /// </summary>
    public enum BrushEasePressure
    {
        /// <summary> f (x) = 1 </summary>
        None = 0,

        /// <summary> f (x) = x </summary>
        Linear = 1,
        /// <summary> f (x) = 1 - x </summary>
        LinearFlip = 2,

        /// <summary> f (x) = x * x </summary>
        Quadratic = 3,
        /// <summary> f (x) = 1 - x * x </summary>
        QuadraticFlip = 4,
        /// <summary> f (x) = (1 - x) * (1 - x) </summary>
        QuadraticReverse = 5,
        /// <summary> f (x) = 1 - (1 - x) * (1 - x) </summary>
        QuadraticFlipReverse = 6,

        /// <summary> f (x) = (x / 2 - 1) * (x / 2 - 1) </summary>
        Mirror = 7,
        /// <summary> f (x) = 1 - (x / 2 - 1) * (x / 2 - 1) </summary>
        MirrorFlip = 8,

        /// <summary> 2f (x) = 2x * 2x </summary>
        Symmetry = 9,
        /// <summary> 2f (x) = 1 - 2x * 2x </summary>
        SymmetryFlip = 10,
    }
}