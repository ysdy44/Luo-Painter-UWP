namespace Luo_Painter.Models
{
    /// <summary>
    /// Mode of History
    /// </summary>
    public enum HistoryMode
    {
        /// <summary>
        /// Normal
        /// </summary>
        None,

        /// <summary>
        /// Mult-History
        /// </summary>
        Composite,

        /// <summary>
        /// Setup Layers,
        /// </summary>
        Setup,

        /// <summary>
        /// Add or Remove Layer(s),
        /// </summary>
        Arrange,

        /// <summary>
        /// Property of Layer
        /// </summary>
        Property,
    }
}