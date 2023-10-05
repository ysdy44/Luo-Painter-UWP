using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.HSVColorPickers
{
    public interface INumberBase
    {
        /// <summary> <see cref="RangeBase.Value"/> </summary>
        double Value { get; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        double Minimum { get; }
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        double Maximum { get; }

        string Unit { get; }

        // UI
        FlowDirection FlowDirection { get; }

        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }
    }
}