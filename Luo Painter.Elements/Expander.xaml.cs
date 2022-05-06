using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Mode of <see cref="Expander"/>'s placement target
    /// </summary>
    public enum ExpanderPlacementMode
    {
        Center,
        Left,
        Top,
        Right,
        Bottom,
    }

    /// <summary> 
    /// Represents the control that a drawer can be folded.
    /// </summary>
    [TemplateVisualState(Name = nameof(HideStoryboard), GroupName = nameof(HideStoryboard))]
    [TemplateVisualState(Name = nameof(ShowStoryboard), GroupName = nameof(ShowStoryboard))]
    [TemplateVisualState(Name = nameof(HideLeftStoryboard), GroupName = nameof(HideLeftStoryboard))]
    [TemplateVisualState(Name = nameof(ShowLeftStoryboard), GroupName = nameof(ShowLeftStoryboard))]
    [TemplateVisualState(Name = nameof(HideTopStoryboard), GroupName = nameof(HideTopStoryboard))]
    [TemplateVisualState(Name = nameof(ShowTopStoryboard), GroupName = nameof(ShowTopStoryboard))]
    [TemplateVisualState(Name = nameof(HideRightStoryboard), GroupName = nameof(HideRightStoryboard))]
    [TemplateVisualState(Name = nameof(ShowRightStoryboard), GroupName = nameof(ShowRightStoryboard))]
    [TemplateVisualState(Name = nameof(HideBottomStoryboard), GroupName = nameof(HideBottomStoryboard))]
    [TemplateVisualState(Name = nameof(ShowBottomStoryboard), GroupName = nameof(ShowBottomStoryboard))]
    [TemplateVisualState(Name = nameof(RootGrid), GroupName = nameof(RootGrid))]
    [TemplateVisualState(Name = nameof(Thumb), GroupName = nameof(Thumb))]
    [TemplateVisualState(Name = nameof(Button), GroupName = nameof(Button))]
    [TemplateVisualState(Name = nameof(SymbolIcon), GroupName = nameof(SymbolIcon))]
    [ContentProperty(Name = nameof(Content))]
    public class Expander : ContentControl
    {

        //@Delegate
        public event TypedEventHandler<Expander, bool> PinChanged;


        Storyboard HideStoryboard;
        Storyboard ShowStoryboard;
        Storyboard HideLeftStoryboard;
        Storyboard ShowLeftStoryboard;
        Storyboard HideTopStoryboard;
        Storyboard ShowTopStoryboard;
        Storyboard HideRightStoryboard;
        Storyboard ShowRightStoryboard;
        Storyboard HideBottomStoryboard;
        Storyboard ShowBottomStoryboard;
        Grid RootGrid;
        Thumb Thumb;
        Button Button;
        SymbolIcon SymbolIcon;


        double X;
        double Y;
        double W;
        double H;
        double U;
        double V;
        ExpanderPlacementMode Placement = ExpanderPlacementMode.Top;


        /// <summary> Gets a value that indicates whether the <see cref = "Expander" /> is open. </summary>
        public bool IsOpen { get; private set; }


        #region DependencyProperty


        /// <summary> Gets or sets <see cref = "Expander" />'s title. </summary>
        public string Title
        {
            get => (string)base.GetValue(TitleProperty);
            set => base.SetValue(TitleProperty, value);
        }
        /// <summary> Identifies the <see cref = "Expander.Title" /> dependency property. </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Expander), new PropertyMetadata(string.Empty));


        #endregion


        //@Construct     
        /// <summary>
        /// Initializes a Expander. 
        /// </summary>
        public Expander()
        {
            this.DefaultStyleKey = typeof(Expander);
            this.U = Window.Current.Bounds.Width;
            this.V = Window.Current.Bounds.Height;

            base.Unloaded += (s, e) => Window.Current.SizeChanged -= this.WindowSizeChanged;
            base.Loaded += (s, e) => Window.Current.SizeChanged += this.WindowSizeChanged;

            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                if (e.NewSize.Width < 100) return;
                if (e.NewSize.Height < 100) return;

                this.W = e.NewSize.Width;
                this.H = e.NewSize.Height;

                if (this.IsLoaded is false) return;

                this.SetLeft(Canvas.GetLeft(this));
                this.SetTop(Canvas.GetTop(this));
            };
        }


        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if ((this.RootGrid is null) == false)
            {
                this.HideStoryboard = null;
                this.ShowStoryboard = null;
                this.HideLeftStoryboard = null;
                this.ShowLeftStoryboard = null;
                this.HideTopStoryboard = null;
                this.ShowTopStoryboard = null;
                this.HideRightStoryboard = null;
                this.ShowRightStoryboard = null;
                this.HideBottomStoryboard = null;
                this.ShowBottomStoryboard = null;
            }
            this.RootGrid = base.GetTemplateChild(nameof(RootGrid)) as Grid;
            if ((this.RootGrid is null) == false)
            {
                this.HideStoryboard = this.RootGrid.Resources[nameof(HideStoryboard)] as Storyboard;
                this.ShowStoryboard = this.RootGrid.Resources[nameof(ShowStoryboard)] as Storyboard;
                this.HideLeftStoryboard = this.RootGrid.Resources[nameof(HideLeftStoryboard)] as Storyboard;
                this.ShowLeftStoryboard = this.RootGrid.Resources[nameof(ShowLeftStoryboard)] as Storyboard;
                this.HideTopStoryboard = this.RootGrid.Resources[nameof(HideTopStoryboard)] as Storyboard;
                this.ShowTopStoryboard = this.RootGrid.Resources[nameof(ShowTopStoryboard)] as Storyboard;
                this.HideRightStoryboard = this.RootGrid.Resources[nameof(HideRightStoryboard)] as Storyboard;
                this.ShowRightStoryboard = this.RootGrid.Resources[nameof(ShowRightStoryboard)] as Storyboard;
                this.HideBottomStoryboard = this.RootGrid.Resources[nameof(HideBottomStoryboard)] as Storyboard;
                this.ShowBottomStoryboard = this.RootGrid.Resources[nameof(ShowBottomStoryboard)] as Storyboard;
            }

            if ((this.Thumb is null) == false)
            {
                this.Thumb.DragStarted -= this.Thumb_DragStarted;
                this.Thumb.DragDelta -= this.Thumb_DragDelta;
            }
            this.Thumb = base.GetTemplateChild(nameof(Thumb)) as Thumb;
            if ((this.Thumb is null) == false)
            {
                this.Thumb.DragStarted += this.Thumb_DragStarted;
                this.Thumb.DragDelta += this.Thumb_DragDelta;
            }

            if ((this.Button is null) == false)
            {
                this.Button.Click -= this.Button_Click;
            }
            this.Button = base.GetTemplateChild(nameof(Button)) as Button;
            if ((this.Button is null) == false)
            {
                this.Button.Click += this.Button_Click;
            }

            this.SymbolIcon = base.GetTemplateChild(nameof(SymbolIcon)) as SymbolIcon;
        }


        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.U = e.Size.Width;
            this.V = e.Size.Height;

            this.SetLeft(Canvas.GetLeft(this));
            this.SetTop(Canvas.GetTop(this));
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.X = Canvas.GetLeft(this);
            this.Y = Canvas.GetTop(this);
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.X += e.HorizontalChange;
            this.Y += e.VerticalChange;
            this.SetLeft(this.X);
            this.SetTop(this.Y);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Thumb is null) return;

            switch (this.Thumb.Visibility)
            {
                case Visibility.Visible:
                    this.Hide(ExpanderPlacementMode.Center);
                    this.Thumb.Visibility = Visibility.Collapsed;
                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.PinChanged?.Invoke(this, true); // Delegate
                    break;
                case Visibility.Collapsed:
                    this.Thumb.Visibility = Visibility.Visible;
                    this.SymbolIcon.Symbol = Symbol.UnPin;
                    this.PinChanged?.Invoke(this, false); // Delegate
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Closes the <see cref = "Expander" />.
        /// </summary>
        public void Hide() => this.Hide(this.Placement);
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed.
        /// </summary>
        public void Show()
        {
            if (this.IsOpen) return;
            this.ShowStoryboard.Begin(); // Storyboard
            this.IsOpen = true;
        }
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed in relation to the specified element.
        /// </summary>
        /// <param name="placementTarget"> The element to use as the <see cref = "Expander" />'s placement target. </param>
        /// <param name="placement"> Gets or sets the default placement to be used for the <see cref = "Expander" />, in relation to its placement target. </param>
        public void ShowAt(FrameworkElement placementTarget, ExpanderPlacementMode placement)
        {
            if (this.IsOpen) return;

            double w = placementTarget.ActualWidth;
            double h = placementTarget.ActualHeight;
            Point position = placementTarget.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

            switch (placement)
            {
                case ExpanderPlacementMode.Center:
                    this.SetLeft(this.U / 2 - this.W / 2);
                    this.SetTop(this.V / 2 - this.H / 2);
                    break;
                case ExpanderPlacementMode.Left:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(position.X - this.W);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(this.U - this.W - w);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(position.Y + h / 2 - this.H / 2);
                    this.ShowRightStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Top:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(position.X + w / 2 - this.W / 2);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(position.X + w / 2 - this.W / 2);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(position.Y - this.H);
                    this.ShowBottomStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Right:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(position.X + w);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(w);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(position.Y + h / 2 - this.H / 2);
                    this.ShowLeftStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Bottom:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(position.X + w / 2 - this.W / 2);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(position.X + w / 2 - this.W / 2);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(position.Y + h);
                    this.ShowTopStoryboard.Begin(); // Storyboard
                    break;
                default:
                    break;
            }
            this.ShowStoryboard.Begin(); // Storyboard

            this.Placement = placement;
            this.IsOpen = true;
        }
        private void Hide(ExpanderPlacementMode placement)
        {
            if (this.IsOpen is false) return;

            switch (placement)
            {
                case ExpanderPlacementMode.Left:
                    this.HideRightStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Top:
                    this.HideBottomStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Right:
                    this.HideLeftStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Bottom:
                    this.HideTopStoryboard.Begin(); // Storyboard
                    break;
                default:
                    break;
            }
            this.HideStoryboard.Begin(); // Storyboard

            this.IsOpen = false;
        }


        private void SetLeft(double value) => Canvas.SetLeft(this, this.W >= this.U ? 0 : System.Math.Clamp(value, 0, this.U - this.W));
        private void SetTop(double value) => Canvas.SetTop(this, this.H >= this.V ? 0 : System.Math.Clamp(value, 0, this.V - this.H));

    }
}