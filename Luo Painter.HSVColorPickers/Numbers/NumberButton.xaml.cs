using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class NumberButton : NumberButtonBase, INumberBase
    {
        //@Content
        public FrameworkElement PlacementTarget => this;

        public double Value
        {
            get => this.number;
            set
            {
                this.number = value;
                if (string.IsNullOrEmpty(this.Unit)) base.Content = System.Math.Round(value, 2, System.MidpointRounding.ToEven);
                else base.Content = $"{System.Math.Round(value, 2, System.MidpointRounding.ToEven)}{this.Unit}";
            }
        }
        private double number;
        public double Minimum { get; set; }
        public double Maximum { get; set; } = 100;

        public string Unit
        {
            get => this.unit;
            set
            {
                this.unit = value;
                if (string.IsNullOrEmpty(value)) base.Content = System.Math.Round(this.Value, 2, System.MidpointRounding.ToEven);
                else base.Content = $"{System.Math.Round(this.Value, 2, System.MidpointRounding.ToEven)}{value}";
            }
        }
        private string unit = string.Empty;

    }

    public partial class NumberButtonBase : Button
    {
        //@Construct
        public NumberButtonBase()
        {
            this.InitializeComponent();
        }
    }
}