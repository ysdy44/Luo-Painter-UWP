using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Luo_Painter.Models
{
    /// <summary>
    /// Mode of <see cref="PropertyHistory"/>
    /// </summary>
    public enum HistoryPropertyMode
    {
        /// <summary> Normal </summary>
        None,

        /// <summary> <see cref="double"/> </summary>
        Opacity,
        /// <summary> <see cref="Microsoft.Graphics.Canvas.Effects.BlendEffectMode"/> </summary>
        BlendMode,
        /// <summary> <see cref="Windows.UI.Xaml.Visibility"/> </summary>
        Visibility,

        /// <summary> <see cref="IDictionary{int, IBuffer}"/> </summary>
        Bitmap,
        /// <summary> <see cref="IBuffer"/> to <see cref="Windows.UI.Color"/> </summary>
        BitmapClear,
        /// <summary> <see cref="IBuffer"/> </summary>
        BitmapReset,
    }
}