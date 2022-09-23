using Luo_Painter.Brushes;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class SliderToolTipPage : Page
    {
        //@Converter
        private double SizeConverter(double value) => BrushSize.Convert(value) * 2;
        
        public SliderToolTipPage()
        {
            this.InitializeComponent();
        }
    }
}
