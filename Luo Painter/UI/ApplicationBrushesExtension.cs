using Windows.UI.Xaml;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    internal class ApplicationBrushesExtension : List<SolidColorBrush>
    {
        public ApplicationBrushesExtension() : base(new SolidColorBrush[]
        {
            Application.Current.Resources["SliderTrackValueFill"] as SolidColorBrush,
            Application.Current.Resources["SliderThumbBackground"] as SolidColorBrush,
            Application.Current.Resources["SliderTrackValueFillPointerOver"] as SolidColorBrush,
            Application.Current.Resources["SliderTrackValueFillPressed"] as SolidColorBrush,

            Application.Current.Resources["ListViewItemBackgroundSelected"] as SolidColorBrush,
            Application.Current.Resources["ListViewItemBackgroundSelectedPointerOver"] as SolidColorBrush,
            Application.Current.Resources["ListViewItemBackgroundSelectedPressed"] as SolidColorBrush,
            Application.Current.Resources["HyperlinkButtonForeground"] as SolidColorBrush,

            Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush,
            Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush,
      })
        { }
    }
}