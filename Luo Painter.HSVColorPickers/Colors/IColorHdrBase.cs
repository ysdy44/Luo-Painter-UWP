using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.HSVColorPickers
{
    public interface IColorHdrBase
    {
        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }

        Vector4 ColorHdr { get; }
        void SetColorHdr(Vector4 colorHdr);
        void SetColorHdr(Color color);
    }
}