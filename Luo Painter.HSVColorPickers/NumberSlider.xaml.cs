using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.HSVColorPickers
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
                else this.HeaderButton.Content = $"{(int)e.NewValue}{this.Unit}";
            };
        }
    }

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
                else this.HeaderButton.Content = $"{value}{this.Unit}";
            }
        }
        private int number;
        public override int NumberMinimum { get; set; }
        public override int NumberMaximum { get; set; } = 100;

        //@Construct
        public NumberSlider2()
        {
            this.InitializeComponent();
        }
    }

    [TemplatePart(Name = nameof(HeaderButton), Type = typeof(NumberButtonBase))]
    public abstract partial class NumberSliderBase : Slider, INumberBase
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
                else this.HeaderButton.Content = $"{this.Number}{value}";
            }
        }
        private string unit = string.Empty;

        public bool Decrease
        {
            get => this.decrease;
            set
            {
                this.decrease = value;
                if (this.HorizontalDecreaseRect is null is false) this.HorizontalDecreaseRect.Opacity = value ? 1 : 0;
                if (this.VerticalDecreaseRect is null is false) this.VerticalDecreaseRect.Opacity = value ? 1 : 0;
            }
        }
        private bool decrease = false;

        protected NumberButtonBase HeaderButton;
        Rectangle HorizontalDecreaseRect;
        Rectangle VerticalDecreaseRect;

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
            this.HeaderButton = base.GetTemplateChild(nameof(HeaderButton)) as NumberButtonBase;
            if (this.HeaderButton is null is false)
            {
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = this.Number;
                else this.HeaderButton.Content = $"{this.Number}{this.Unit}";
                this.HeaderButton.Click += this.Click;
            }

            this.HorizontalDecreaseRect = base.GetTemplateChild(nameof(HorizontalDecreaseRect)) as Rectangle;
            this.VerticalDecreaseRect = base.GetTemplateChild(nameof(VerticalDecreaseRect)) as Rectangle;
         
            if (this.HorizontalDecreaseRect is null is false) this.HorizontalDecreaseRect.Opacity = this.Decrease ? 1 : 0;
            if (this.VerticalDecreaseRect is null is false) this.VerticalDecreaseRect.Opacity = this.Decrease ? 1 : 0;
        }

    }
}