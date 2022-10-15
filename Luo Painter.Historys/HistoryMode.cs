namespace Luo_Painter.Historys
{
    /// <summary>
    /// Mode of History
    /// </summary>
    public enum HistoryMode : byte
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

        /// <summary>
        /// Property of Layers
        /// </summary>
        Propertys,
    }
}