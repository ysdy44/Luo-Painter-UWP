namespace Luo_Painter.Shaders
{
    /// <summary>
    /// Hardness of the brush edge.
    /// </summary>
    public enum BrushEdgeHardness : int
    {
        /// <summary> f (x) = 1 </summary>
        None = 0,

        /// <summary> f (x) = cos(x) </summary>
        Cosine = 1,

        /// <summary> f (x) = cos(x2) </summary>
        Quadratic = 2,

        /// <summary> f (x) = cos(x3) </summary>
        Cube = 3,

        /// <summary> f (x) = cos(x4) </summary>
        Quartic = 4,
    }
}