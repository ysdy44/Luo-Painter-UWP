using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class NumberSlider : NumberSliderBase
    {
        //@Override
        public override int Number { get => (int)System.Math.Round(base.Value, System.MidpointRounding.ToEven); set => base.Value = value; }
        public override int NumberMinimum { get => (int)System.Math.Round(base.Minimum, System.MidpointRounding.ToEven); set => base.Minimum = value; }
        public override int NumberMaximum { get => (int)System.Math.Round(base.Maximum, System.MidpointRounding.ToEven); set => base.Maximum = value; }

        //@Construct
        public NumberSlider()
        {
            this.InitializeComponent();
            base.ValueChanged += (s, e) =>
            {
                if (this.HeaderButton is null) return;
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = (int)e.NewValue;
                else this.HeaderButton.Content = $"{(int)e.NewValue} {this.Unit}";
            };
        }
    }
    /*
    public sealed partial class NumberSlider2 : NumberSliderBase
    {
        //@Override
        public override int Number
        {
            get => this.number;
            set
            {
                this.number = value;
                if (base.HeaderButton is null) return;
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = value;
                else this.HeaderButton.Content = $"{value} {this.Unit}";
            }
        }
        private int number;
        public override int NumberMinimum { get; set; }
        public override int NumberMaximum { get; set; }

        //@Construct
        public NumberSlider2()
        {
            this.InitializeComponent();
        }
    }
     */
    [TemplatePart(Name = nameof(HeaderButton), Type = typeof(Button))]
    public abstract partial class NumberSliderBase : Slider, INumberSlider
    {

        //@Delegate
        public event RoutedEventHandler Click;

        //@Content
        public FrameworkElement PlacementTarget
        {
            get
            {
                if (this.HeaderButton is null) return this;
                else return this.HeaderButton;
            }
        }

        //@Abstract
        public abstract int Number { get; set; }
        public abstract int NumberMinimum { get; set; }
        public abstract int NumberMaximum { get; set; }

        public string Unit
        {
            get => this.unit;
            set
            {
                this.unit = value;
                if (this.HeaderButton is null) return;
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = this.Number;
                else this.HeaderButton.Content = $"{this.Number} {value}";
            }
        }
        private string unit = string.Empty;

        protected Button HeaderButton;

        //@Construct
        internal NumberSliderBase()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.HeaderButton is null is false)
            {
                this.HeaderButton.Content = null;
                this.HeaderButton.Click -= this.Click;
            }
            this.HeaderButton = base.GetTemplateChild(nameof(HeaderButton)) as Button;
            if (this.HeaderButton is null is false)
            {
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = this.Number;
                else this.HeaderButton.Content = $"{this.Number} {this.Unit}";
                this.HeaderButton.Click += this.Click;
            }
        }

    }
}