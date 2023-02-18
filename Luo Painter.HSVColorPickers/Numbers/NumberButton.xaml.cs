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
                if (string.IsNullOrEmpty(this.Unit)) base.Content = value;
                else base.Content = $"{value}{this.Unit}";
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
                if (string.IsNullOrEmpty(this.Unit)) base.Content = this.Value;
                else base.Content = $"{this.Value}{value}";
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