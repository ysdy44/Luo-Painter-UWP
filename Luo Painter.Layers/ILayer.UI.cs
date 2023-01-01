using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public partial interface ILayer
    {

        /// <summary>
        /// <see cref="ColumnDefinition.MinWidth" />
        /// </summary>
        double UIDepth { get; }

        /// <summary>
        /// <see cref="UIElement.Opacity" />
        /// </summary>
        double UIVisibility { get; }

        /// <summary>
        /// <see cref="RotateTransform.Angle" />
        /// </summary>
        double UIIsExpand { get; }

    }
}