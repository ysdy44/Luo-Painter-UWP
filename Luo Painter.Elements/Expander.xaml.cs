using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// State of <see cref="Expander"/> 
    /// </summary>
    public enum ExpanderState : byte
    {
        Collapsed,
        Flyout,
        Overlay,
    }

    /// <summary>
    /// Mode of <see cref="Expander"/>'s placement target
    /// </summary>
    public enum ExpanderPlacementMode : byte
    {
        Center,
        Left,
        Top,
        Right,
        Bottom,
    }

    /// <summary>
    /// LightDismissOverlay for <see cref="Expander"/>.
    /// </summary>
    public sealed class ExpanderLightDismissOverlay : Canvas
    {

        //@Delegate
        public event TypedEventHandler<ExpanderLightDismissOverlay, bool> IsFlyoutChanged;

        readonly Stack<Expander> Items = new Stack<Expander>();

        //@Construct     
        /// <summary>
        /// Initializes a ExpanderLightDismissOverlay. 
        /// </summary>
        public ExpanderLightDismissOverlay()
        {
            base.Unloaded += (s, e) =>
            {
                foreach (Expander item in this.Items)
                {
                    item.IsFlyoutChanged -= this.OnIsFlyoutChanged;
                    item.IsShowChanged -= this.IsShowChanged;
                    item.OnZIndexChanging -= this.OnZIndexChanging;

                    base.SizeChanged -= item.CanvasSizeChanged;
                }
                this.Items.Clear();
            };
            base.Loaded += (s, e) =>
            {
                foreach (UIElement item in base.Children)
                {
                    if (item is Expander expander)
                    {
                        this.Items.Push(expander);
                        expander.IsFlyoutChanged += this.OnIsFlyoutChanged;
                        expander.IsShowChanged += this.IsShowChanged;
                        expander.OnZIndexChanging += this.OnZIndexChanging;

                        expander.CanvasSizeChanged(base.ActualWidth, base.ActualHeight);
                        base.SizeChanged += expander.CanvasSizeChanged;
                    }
                }
            };
        }

        /// <summary>
        /// Hide all flyout.
        /// </summary>
        public bool Hide()
        {
            bool result = false;
            foreach (Expander item in this.Items)
            {
                switch (item.State)
                {
                    case ExpanderState.Flyout:
                        result = true;
                        item.Hide();
                        break;
                }
            }
            return result;
        }

        private void OnIsFlyoutChanged(Expander sender, bool isFlyout)
        {
            if (isFlyout)
            {
                this.IsFlyoutChanged?.Invoke(this, true); // Delegate
                foreach (Expander item in this.Items)
                {
                    item.IsHitTestVisible = false;
                }
                sender.IsHitTestVisible = true;
            }
            else
            {
                this.IsFlyoutChanged?.Invoke(this, false); // Delegate
                foreach (Expander item in this.Items)
                {
                    item.IsHitTestVisible = true;
                }
                Canvas.SetZIndex(sender, 0);
            }
        }

        private void IsShowChanged(Expander sender, bool isFlyout)
        {
        }

        private void OnZIndexChanging(Expander sender, int args)
        {
            int top = base.Children.Count - 1;
            if (args == top) return;

            Canvas.SetZIndex(sender, top);

            foreach (Expander item in this.Items)
            {
                switch (item.State)
                {
                    case ExpanderState.Flyout:
                        continue;
                    case ExpanderState.Overlay:
                        if (item == sender) continue;

                        int index = Canvas.GetZIndex(item);
                        if (index is 0) continue;

                        Canvas.SetZIndex(item, index - 1);
                        break;
                }
            }
        }
    }

    /// <summary> 
    /// Represents the control that a drawer can be folded.
    /// </summary>
    [TemplatePart(Name = nameof(HideStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideLeftStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowLeftStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideTopStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowTopStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideRightStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowRightStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideBottomStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowBottomStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(RootGrid), Type = typeof(Grid))]
    [TemplatePart(Name = nameof(Thumb), Type = typeof(Thumb))]
    [TemplatePart(Name = nameof(Button), Type = typeof(Button))]
    [TemplatePart(Name = nameof(SymbolIcon), Type = typeof(SymbolIcon))]
    [ContentProperty(Name = nameof(Content))]
    public class Expander : ContentControl
    {

        //@Delegate
        internal event TypedEventHandler<Expander, bool> IsFlyoutChanged;
        internal event TypedEventHandler<Expander, bool> IsShowChanged;
        internal event TypedEventHandler<Expander, int> OnZIndexChanging;


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

        bool HasResize;
        bool AllowClick;


        ExpanderPlacementMode Placement = ExpanderPlacementMode.Center;
        ExpanderPlacementMode ActualPlacement = ExpanderPlacementMode.Center;
        double PlacementTargetW;
        double PlacementTargetH;
        Point PlacementTargetPosition;

        private ExpanderState state;
        public ExpanderState State
        {
            get => state;
            set
            {
                if (value is ExpanderState.Flyout && this.state is ExpanderState.Flyout is false)
                {
                    this.IsFlyoutChanged?.Invoke(this, true); // Delegate
                }
                if (value is ExpanderState.Flyout is false && this.state is ExpanderState.Flyout)
                {
                    this.IsFlyoutChanged?.Invoke(this, false); // Delegate
                }

                if (value is ExpanderState.Collapsed && this.state is ExpanderState.Collapsed is false)
                {
                    this.IsShowChanged?.Invoke(this, false); // Delegate
                }
                if (value is ExpanderState.Collapsed is false && this.state is ExpanderState.Collapsed)
                {
                    this.IsShowChanged?.Invoke(this, true); // Delegate
                }

                this.state = value;
            }
        }


        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "Expander" />'s title. </summary>
        public string Title
        {
            get => (string)base.GetValue(TitleProperty);
            set => base.SetValue(TitleProperty, value);
        }
        /// <summary> Identifies the <see cref = "Expander.Title" /> dependency property. </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Expander), new PropertyMetadata(string.Empty));


        /// <summary> Gets or sets <see cref = "Expander" />'s top AppBar. </summary>
        public UIElement TopAppBar
        {
            get => (UIElement)base.GetValue(TopAppBarProperty);
            set => base.SetValue(TopAppBarProperty, value);
        }
        /// <summary> Identifies the <see cref = "Expander.TopAppBar" /> dependency property. </summary>
        public static readonly DependencyProperty TopAppBarProperty = DependencyProperty.Register(nameof(TopAppBar), typeof(UIElement), typeof(Expander), new PropertyMetadata(null));


        /// <summary> Gets or sets <see cref = "Expander" />'s bottom AppBar. </summary>
        public UIElement BottomAppBar
        {
            get => (UIElement)base.GetValue(BottomAppBarProperty);
            set => base.SetValue(BottomAppBarProperty, value);
        }
        /// <summary> Identifies the <see cref = "Expander.BottomAppBar" /> dependency property. </summary>
        public static readonly DependencyProperty BottomAppBarProperty = DependencyProperty.Register(nameof(BottomAppBar), typeof(UIElement), typeof(Expander), new PropertyMetadata(null));


        #endregion


        //@Construct     
        /// <summary>
        /// Initializes a Expander. 
        /// </summary>
        public Expander()
        {
            this.DefaultStyleKey = typeof(Expander);
        }


        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.RootGrid is null is false)
            {
                this.RootGrid.SizeChanged -= this.RootGrid_SizeChanged;

                if (this.HideStoryboard is null is false) this.HideStoryboard.Completed -= this.HideStoryboard_Completed;
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
            if (this.RootGrid is null is false)
            {
                this.RootGrid.SizeChanged += this.RootGrid_SizeChanged;

                if (this.HideStoryboard is null is false) this.HideStoryboard.Completed -= this.HideStoryboard_Completed;
                this.HideStoryboard = this.RootGrid.Resources[nameof(HideStoryboard)] as Storyboard;
                if (this.HideStoryboard is null is false) this.HideStoryboard.Completed += this.HideStoryboard_Completed;

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

            if (this.Thumb is null is false)
            {
                this.Thumb.DragStarted -= this.Thumb_DragStarted;
                this.Thumb.DragDelta -= this.Thumb_DragDelta;
            }
            this.Thumb = base.GetTemplateChild(nameof(Thumb)) as Thumb;
            if (this.Thumb is null is false)
            {
                this.Thumb.DragStarted += this.Thumb_DragStarted;
                this.Thumb.DragDelta += this.Thumb_DragDelta;
            }

            if (this.Button is null is false)
            {
                this.Button.Click -= this.Button_Click;
            }
            this.Button = base.GetTemplateChild(nameof(Button)) as Button;
            if (this.Button is null is false)
            {
                this.Button.Click += this.Button_Click;
            }

            this.SymbolIcon = base.GetTemplateChild(nameof(SymbolIcon)) as SymbolIcon;
        }


        /// <inheritdoc/>
        internal void CanvasSizeChanged(double u, double v)
        {
            this.U = u;
            this.V = v;

            base.MaxWidth = u;
            base.MaxHeight = v - 70;

            switch (this.State)
            {
                case ExpanderState.Flyout:
                    this.Hide();
                    break;
                case ExpanderState.Overlay:
                    if (this.X > this.U - this.W) this.X = this.U - this.W;
                    if (this.Y > this.V - 50) this.Y = this.V - 50;
                    break;
            }
        }
        /// <inheritdoc/>
        internal void CanvasSizeChanged(object sender, SizeChangedEventArgs e) => this.CanvasSizeChanged(e.NewSize.Width, e.NewSize.Height);

        private void HideStoryboard_Completed(object sender, object e) => this.Close();
        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize == Size.Empty) return;
            if (e.NewSize == e.PreviousSize) return;

            if (e.NewSize.Width < 100) return;
            if (e.NewSize.Height < 100) return;

            this.W = e.NewSize.Width;
            this.H = e.NewSize.Height;
            this.ResizeShow();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e) => this.Pin();
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.X += e.HorizontalChange;
            this.SetLeft(this.X);

            this.Y += e.VerticalChange;
            this.SetTopWithHeader(this.Y);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (this.State)
            {
                case ExpanderState.Flyout:
                    this.Pin();
                    break;
                case ExpanderState.Overlay:
                    this.State = ExpanderState.Collapsed;
                    this.SymbolIcon.Symbol = Symbol.Pin;

                    this.Hide();
                    break;
            }
        }



        /// <summary>
        /// Closes the <see cref = "Expander" />.
        /// </summary>
        public void Hide()
        {
            this.State = ExpanderState.Collapsed;
            this.SymbolIcon.Symbol = Symbol.Pin;

            this.HideBegin();
            this.ActualPlacement = ExpanderPlacementMode.Center;
            this.Placement = ExpanderPlacementMode.Center;
        }
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed.
        /// </summary>
        public void Show()
        {
            if (this.HasResize is false)
            {
                this.OpenWithResize();
                return;
            }

            this.Open();
            this.ShowCore();
            this.AllowClick = true;
        }
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed in relation to the specified element.
        /// </summary>
        /// <param name="placementTarget"> The element to use as the <see cref = "Expander" />'s placement target. </param>
        /// <param name="placement"> Gets or sets the default placement to be used for the <see cref = "Expander" />, in relation to its placement target. </param>
        public void ShowAt(FrameworkElement placementTarget, ExpanderPlacementMode placement)
        {
            this.PlacementTargetW = placementTarget.ActualWidth;
            this.PlacementTargetH = placementTarget.ActualHeight;
            this.PlacementTargetPosition = placementTarget.TransformToVisual(base.Parent as UIElement).TransformPoint(new Point(0, 0));

            this.Placement = placement;
            this.Show();
        }
        /// <summary>
        /// Show or Hide.
        /// </summary>
        /// <param name="placementTarget"> The element to use as the <see cref = "Expander" />'s placement target. </param>
        /// <param name="placement"> Gets or sets the default placement to be used for the <see cref = "Expander" />, in relation to its placement target. </param>
        public void Toggle(FrameworkElement placementTarget, ExpanderPlacementMode placement)
        {
            switch (this.State)
            {
                case ExpanderState.Collapsed:
                    this.ShowAt(placementTarget, placement);
                    break;
                case ExpanderState.Flyout:
                    this.Hide();
                    break;
            }
        }


        private void ResizeShow()
        {
            this.HasResize = true;

            if (this.AllowClick) return;

            this.ShowCore();
        }
        private void ShowCore()
        {
            this.State = ExpanderState.Flyout;
            this.SymbolIcon.Symbol = Symbol.Pin;

            this.ActualPlacement = this.GetPlacement(this.Placement);

            this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
            this.ShowBegin();
        }
        private void Pin()
        {
            this.X = Canvas.GetLeft(this);
            this.Y = Canvas.GetTop(this);

            this.State = ExpanderState.Overlay;
            this.SymbolIcon.Symbol = Symbol.UnPin;

            this.ActualPlacement = ExpanderPlacementMode.Center;
            this.Placement = ExpanderPlacementMode.Center;

            this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
        }


        private void ShowBegin(double space = 12)
        {
            switch (this.ActualPlacement)
            {
                case ExpanderPlacementMode.Center:
                    this.SetLeft(this.U / 2 - this.W / 2);
                    this.SetTop(this.V / 2 - this.H / 2);
                    break;
                case ExpanderPlacementMode.Left:
                    this.SetLeft(this.PlacementTargetPosition.X - this.W - space);
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH / 2 - this.H / 2);
                    this.ShowRightStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Top:
                    this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                    this.SetTop(this.PlacementTargetPosition.Y - this.H - space);
                    this.ShowBottomStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Right:
                    this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW + space);
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH / 2 - this.H / 2);
                    this.ShowLeftStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Bottom:
                    this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH + space);
                    this.ShowTopStoryboard.Begin(); // Storyboard
                    break;
                default:
                    break;
            }
            this.ShowStoryboard.Begin(); // Storyboard
        }
        private void HideBegin()
        {
            switch (this.ActualPlacement)
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
        }


        private void Close() => base.Visibility = Visibility.Collapsed;
        private void Open() => base.Visibility = Visibility.Visible;
        private void OpenWithResize()
        {
            base.Opacity = 0;
            base.Visibility = Visibility.Visible;
            base.Opacity = 1;
        }


        private void SetLeft(double value) => Canvas.SetLeft(this, this.W >= this.U ? 0 : System.Math.Clamp(value, 0, this.U - this.W));
        private void SetTop(double value) => Canvas.SetTop(this, this.H >= this.V ? 0 : System.Math.Clamp(value, 0, this.V - this.H));
        private void SetTopWithHeader(double value) => Canvas.SetTop(this, System.Math.Clamp(value, 0, this.V - 50));

        private bool InsideLeft() => this.PlacementTargetPosition.X > this.W;
        private bool InsideTop() => true; // this.PlacementTargetPosition.Y > this.H;
        private bool InsideRight() => this.U - this.PlacementTargetPosition.X - this.PlacementTargetW > this.W;
        private bool InsideBottom() => true; // this.V - this.PlacementTargetPosition.Y - this.PlacementTargetH > this.H;
        private ExpanderPlacementMode GetPlacement(ExpanderPlacementMode placement)
        {
            switch (placement)
            {
                case ExpanderPlacementMode.Center: return ExpanderPlacementMode.Center;

                case ExpanderPlacementMode.Left:
                case ExpanderPlacementMode.Right:
                    {
                        switch (placement)
                        {
                            case ExpanderPlacementMode.Left:
                                if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else break;
                            case ExpanderPlacementMode.Right:
                                if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else break;
                            default:
                                return ExpanderPlacementMode.Center;
                        }

                        if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                        else if (this.InsideTop()) return ExpanderPlacementMode.Top;
                        else return ExpanderPlacementMode.Center;
                    }

                case ExpanderPlacementMode.Top:
                case ExpanderPlacementMode.Bottom:
                    {
                        switch (placement)
                        {
                            case ExpanderPlacementMode.Top:
                                if (this.InsideTop()) return ExpanderPlacementMode.Top;
                                else if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                                else break;
                            case ExpanderPlacementMode.Bottom:
                                if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                                else if (this.InsideTop()) return ExpanderPlacementMode.Top;
                                else break;
                            default:
                                return ExpanderPlacementMode.Center;
                        }

                        switch (base.FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else return ExpanderPlacementMode.Center;
                            case FlowDirection.RightToLeft:
                                if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else return ExpanderPlacementMode.Center;
                            default:
                                return ExpanderPlacementMode.Center;
                        }
                    }

                default:
                    return ExpanderPlacementMode.Center;
            }
        }

    }
}