using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.HSVColorPickers
{
    [TemplatePart(Name = nameof(HeaderButton), Type = typeof(NumberButtonBase))]
    public partial class NumberSlider : Slider, INumberBase
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

        public string Unit
        {
            get => this.unit;
            set
            {
                this.unit = value;
                if (this.HeaderButton is null) return;
                if (string.IsNullOrEmpty(value)) this.HeaderButton.Content = System.Math.Round(this.Value, 2, System.MidpointRounding.ToEven);
                else this.HeaderButton.Content = $"{System.Math.Round(this.Value, 2, System.MidpointRounding.ToEven)}{value}";
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
        public NumberSlider()
        {
            this.InitializeComponent();
            base.ValueChanged += (s, e) =>
            {
                if (this.HeaderButton is null) return;
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = System.Math.Round(e.NewValue, 2, System.MidpointRounding.ToEven);
                else this.HeaderButton.Content = $"{System.Math.Round(e.NewValue, 2, System.MidpointRounding.ToEven)}{this.Unit}";
            };
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
                if (string.IsNullOrEmpty(this.Unit)) this.HeaderButton.Content = this.Value;
                else this.HeaderButton.Content = $"{this.Value}{this.Unit}";
                this.HeaderButton.Click += this.Click;
            }

            this.HorizontalDecreaseRect = base.GetTemplateChild(nameof(HorizontalDecreaseRect)) as Rectangle;
            this.VerticalDecreaseRect = base.GetTemplateChild(nameof(VerticalDecreaseRect)) as Rectangle;

            if (this.HorizontalDecreaseRect is null is false) this.HorizontalDecreaseRect.Opacity = this.Decrease ? 1 : 0;
            if (this.VerticalDecreaseRect is null is false) this.VerticalDecreaseRect.Opacity = this.Decrease ? 1 : 0;
        }

    }
}