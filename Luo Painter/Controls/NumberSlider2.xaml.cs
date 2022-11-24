using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    [TemplatePart(Name = nameof(HeaderButton), Type = typeof(Button))]
    public sealed partial class NumberSlider2 : Slider, INumberSlider
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


        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "NumberSlider2" />'s number. </summary>
        public int Number
        {
            get => (int)base.GetValue(NumberProperty);
            set => base.SetValue(NumberProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberSlider2.Number" /> dependency property. </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(nameof(Number), typeof(int), typeof(NumberSlider2), new PropertyMetadata(0));


        /// <summary> Gets or sets <see cref = "NumberSlider2" />'s number. </summary>
        public int NumberMinimum
        {
            get => (int)base.GetValue(NumberMinimumProperty);
            set => base.SetValue(NumberMinimumProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberSlider2.NumberMinimum" /> dependency property. </summary>
        public static readonly DependencyProperty NumberMinimumProperty = DependencyProperty.Register(nameof(NumberMinimum), typeof(int), typeof(NumberSlider2), new PropertyMetadata(0));


        /// <summary> Gets or sets <see cref = "NumberSlider2" />'s number. </summary>
        public int NumberMaximum
        {
            get => (int)base.GetValue(NumberMaximumProperty);
            set => base.SetValue(NumberMaximumProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberSlider2.NumberMaximum" /> dependency property. </summary>
        public static readonly DependencyProperty NumberMaximumProperty = DependencyProperty.Register(nameof(NumberMaximum), typeof(int), typeof(NumberSlider2), new PropertyMetadata(100));


        /// <summary> Gets or sets <see cref = "NumberSlider2" />'s unit. </summary>
        public string Unit
        {
            get => (string)base.GetValue(UnitProperty);
            set => base.SetValue(UnitProperty, value);
        }
        /// <summary> Identifies the <see cref = "NumberSlider2.Unit" /> dependency property. </summary>
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(NumberSlider2), new PropertyMetadata(string.Empty));


        #endregion

        Button HeaderButton;

        //@Construct
        public NumberSlider2()
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