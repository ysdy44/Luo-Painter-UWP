using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.HSVColorPickers
{
    public interface IColorBase
    {
        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }

        Color Color { get; }
        void SetColor(Color color);
        void SetColor(Vector4 colorHdr);
    }
}