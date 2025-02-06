using FanKit.Transformers;

namespace Luo_Painter.UI
{
    public enum InkTouchMode
    {
        /// <summary>
        /// <see cref="CanvasOperator.TouchMode"/> is <see cref="TouchMode.SingleFinger"/>
        /// </summary>
        Draw = 0,
        /// <summary>
        /// <see cref="CanvasOperator.TouchMode"/> is <see cref="TouchMode.RightButton"/>
        /// </summary>
        Move = 1,
        /// <summary>
        /// <see cref="CanvasOperator.TouchMode"/> is <see cref="TouchMode.Disable"/>
        /// </summary>
        Disable = 2,
    }
}