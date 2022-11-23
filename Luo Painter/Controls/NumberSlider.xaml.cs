using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    [TemplatePart(Name = nameof(HeaderButton), Type = typeof(Button))]
    public sealed partial class NumberSlider : Slider, INumberSlider
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

        public int Number => (int)base.Value;
        public int NumberMinimum => (int)base.Minimum;
        public int NumberMaximum => (int)base.Maximum;

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "NumberSlider" />'s unit. </summary>
        public string Unit
        {
            get => (string)base.GetValue(UnitProperty);
            set => base.SetValue(UnitProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberSlider.Unit" /> dependency property. </summary>
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumberSlider), new PropertyMetadata(string.Empty));


        #endregion

        Button HeaderButton;

        //@Construct
        public NumberSlider()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.HeaderButton is null is false) this.HeaderButton.Click -= this.Click;
            this.HeaderButton = base.GetTemplateChild(nameof(HeaderButton)) as Button;
            if (this.HeaderButton is null is false) this.HeaderButton.Click += this.Click;
        }

    }
}