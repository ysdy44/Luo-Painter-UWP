using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Layers
{
    public partial interface ILayer
    {

        /// <summary>
        /// <see cref="Grid" /> <para/>
        /// <see cref="Grid.ColumnDefinitions" /> <para/>
        /// <see cref="ColumnDefinition.MinWidth" /> <para/>
        /// </summary>
        double UIDepth { get; }

        /// <summary>
        /// <see cref="TextBlock" /> <para/>
        /// <see cref="UIElement.Opacity" /> <para/>
        /// </summary>
        double UIVisibility { get; }

        /// <summary>
        /// <see cref="FontIcon" /> <para/>
        /// <see cref="UIElement.RenderTransform" /> <para/>
        /// <see cref="RotateTransform.Angle" /> <para/>
        /// </summary>
        double UIIsExpand { get; }

        /// <summary>
        /// <see cref="Rectangle" /> <para/>
        /// <see cref="Shape.Fill" /> <para/>
        /// <see cref="SolidColorBrush.Color" /> <para/>
        /// </summary>
        Color UITagType { get; }

    }
}