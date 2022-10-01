using Luo_Painter.Elements;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class SizeRange : InverseProportionRange
    {
        public SizeRange() : base(12, 1, 400, 100000) { }
    }

    public sealed partial class SliderToolTipPage : Page
    {
        //@Converter
        private double SizeConverter(double value) => this.SizeRange.ConvertXToY(value);
        private double FontSizeConverter(double value) => this.SizeConverter(value) / 4 + 1;
        private string SizeToStringConverter(double value) => System.String.Format("{0:F}", this.SizeConverter(value));
        private double OpacityConverter(double value) => value / 100;
        private string OpacityToStringConverter(double value) => $"{(int)value} %";

        public SliderToolTipPage()
        {
            this.InitializeComponent();
        }
    }
}