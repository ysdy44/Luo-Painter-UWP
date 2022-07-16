using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Luo_Painter.Elements
{
    [TemplatePart(Name = nameof(RootGrid), Type = typeof(Border))]
    [TemplatePart(Name = nameof(HideStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideDoubleAnimation), Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = nameof(TranslateTransform), Type = typeof(TranslateTransform))]
    [TemplatePart(Name = nameof(PrimaryButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(SecondaryButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(Badge), Type = typeof(Border))]
    [ContentProperty(Name = nameof(Content))]
    public class Docker : ContentControl
    {
        //@Delegate
        public event RoutedEventHandler PrimaryButtonClick;
        public event RoutedEventHandler SecondaryButtonClick;

        public bool PrimaryButtonIsEnabledFollowCount { get; set; }

        Border RootGrid;
        Storyboard HideStoryboard;
        Storyboard ShowStoryboard;
        DoubleAnimation HideDoubleAnimation;

        TranslateTransform TranslateTransform;

        Button PrimaryButton;
        Button SecondaryButton;

        Border Badge;

        #region DependencyProperty


        /// <summary> Gets or set the text for <see cref="Docker"/>'s PrimaryButton. </summary>
        public string PrimaryButtonText
        {
            get => (string)base.GetValue(PrimaryButtonTextProperty);
            set => base.SetValue(PrimaryButtonTextProperty, value);
        }
        /// <summary> Identifies the <see cref = "Docker.PrimaryButtonText" /> dependency property. </summary>
        public static readonly DependencyProperty PrimaryButtonTextProperty = DependencyProperty.Register(nameof(PrimaryButtonText), typeof(string), typeof(Docker), new PropertyMetadata(string.Empty));


        /// <summary> Gets or set the text for <see cref="Docker"/>'s SecondaryButton. </summary>
        public string SecondaryButtonText
        {
            get => (string)base.GetValue(SecondaryButtonTextProperty);
            set => base.SetValue(SecondaryButtonTextProperty, value);
        }
        /// <summary> Identifies the <see cref = "Docker.SecondaryButtonText" /> dependency property. </summary>
        public static readonly DependencyProperty SecondaryButtonTextProperty = DependencyProperty.Register(nameof(SecondaryButtonText), typeof(string), typeof(Docker), new PropertyMetadata(string.Empty));


        /// <summary> Gets or set the orientation for <see cref="Docker"/>. </summary>
        public Orientation Orientation
        {
            get => (Orientation)base.GetValue(OrientationProperty);
            set => base.SetValue(OrientationProperty, value);
        }
        /// <summary> Identifies the <see cref = "Docker.Orientation" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Docker), new PropertyMetadata(Orientation.Horizontal, (sender, e) =>
        {
            Docker control = (Docker)sender;

            if (e.NewValue is Orientation value)
            {
                control.UpdateStoryboard();
            }
        }));

        /// <summary> Gets or set the state for <see cref="Docker"/>. </summary>
        public bool IsShow
        {
            get => (bool)base.GetValue(IsShowProperty);
            set => base.SetValue(IsShowProperty, value);
        }
        /// <summary> Identifies the <see cref = "Docker.IsShow" /> dependency property. </summary>
        public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(Docker), new PropertyMetadata(false, (sender, e) =>
        {
            Docker control = (Docker)sender;

            if (e.NewValue is bool value)
            {
                if (value)
                {
                    if (control.ShowStoryboard is null) return;
                    control.ShowStoryboard.Begin();
                }
                else
                {
                    if (control.HideStoryboard is null) return;
                    control.HideStoryboard.Begin();
                }
            }
        }));


        /// <summary> Gets or set the count for <see cref="Docker"/>'s badge. </summary>
        public int Count
        {
            get => (int)base.GetValue(CountProperty);
            set => base.SetValue(CountProperty, value);
        }
        /// <summary> Identifies the <see cref = "Docker.Count" /> dependency property. </summary>
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(int), typeof(Docker), new PropertyMetadata(0, (sender, e) =>
        {
            Docker control = (Docker)sender;

            if (e.NewValue is int value)
            {
                if (value is 0)
                {
                    if (control.PrimaryButtonIsEnabledFollowCount) if (control.PrimaryButton is null is false) control.PrimaryButton.IsEnabled = false;
                    if (control.Badge is null is false) control.Badge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (control.PrimaryButtonIsEnabledFollowCount) if (control.PrimaryButton is null is false) control.PrimaryButton.IsEnabled = true;
                    if (control.Badge is null is false) control.Badge.Visibility = Visibility.Visible;
                }
            }
        }));


        #endregion

        //@Construct     
        /// <summary>
        /// Initializes a Docker. 
        /// </summary>
        public Docker()
        {
            this.DefaultStyleKey = typeof(Docker);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                this.UpdateStoryboard();
                this.UpdateTranslateTransform();
            };
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.RootGrid is null is false)
            {
                this.HideStoryboard = null;
                this.ShowStoryboard = null;
                this.HideDoubleAnimation = null;
            }
            this.RootGrid = base.GetTemplateChild(nameof(RootGrid)) as Border;
            if (this.RootGrid is null is false)
            {
                this.HideStoryboard = this.RootGrid.Resources[nameof(HideStoryboard)] as Storyboard;
                this.ShowStoryboard = this.RootGrid.Resources[nameof(ShowStoryboard)] as Storyboard;
                if (this.HideStoryboard is null is false)
                {
                    this.HideDoubleAnimation = this.HideStoryboard.Children.First() as DoubleAnimation;
                    this.UpdateStoryboard();
                }
            }

            this.TranslateTransform = base.GetTemplateChild(nameof(TranslateTransform)) as TranslateTransform;
            this.UpdateTranslateTransform();

            if (this.PrimaryButton is null is false) this.PrimaryButton.Click -= this.PrimaryButtonClick;
            this.PrimaryButton = base.GetTemplateChild(nameof(PrimaryButton)) as Button;
            if (this.PrimaryButton is null is false)
            {
                if (this.PrimaryButtonIsEnabledFollowCount) this.PrimaryButton.IsEnabled = this.Count is 0 is false;
                this.PrimaryButton.Click += this.PrimaryButtonClick;
            }

            if (this.SecondaryButton is null is false) this.SecondaryButton.Click -= this.SecondaryButtonClick;
            this.SecondaryButton = base.GetTemplateChild(nameof(SecondaryButton)) as Button;
            if (this.SecondaryButton is null is false) this.SecondaryButton.Click += this.SecondaryButtonClick;

            this.Badge = base.GetTemplateChild(nameof(Badge)) as Border;
            if (this.Badge is null is false) this.Badge.Visibility = (this.Count is 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateStoryboard()
        {
            if (this.HideDoubleAnimation is null) return;
            this.HideDoubleAnimation.To = base.ActualHeight;
        }

        private void UpdateTranslateTransform()
        {
            if (this.TranslateTransform is null) return;
            if (this.IsShow) return;
            this.TranslateTransform.Y = base.ActualHeight;
        }

    }
}