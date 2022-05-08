﻿using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
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
    /// State of <see cref="Expander"/>.
    /// </summary>
    public enum ExpanderState
    {
        Hide,
        Flyout,
        Overlay,
    }

    /// <summary>
    /// LightDismissOverlay for <see cref="Expander"/>.
    /// </summary>
    public sealed class ExpanderLightDismissOverlay : Canvas
    {
        readonly Stack<Expander> Items = new Stack<Expander>();
        readonly SolidColorBrush Transparent = new SolidColorBrush(Colors.Transparent);

        //@Construct     
        /// <summary>
        /// Initializes a Expander. 
        /// </summary>
        public ExpanderLightDismissOverlay()
        {
            base.Unloaded += (s, e) =>
            {
                foreach (Expander item in this.Items)
                {
                    item.StateChanged -= this.StateChanged;
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
                        expander.StateChanged += this.StateChanged;
                        expander.OnZIndexChanging += this.OnZIndexChanging;

                        expander.CanvasSizeChanged(base.ActualWidth, base.ActualHeight);
                        base.SizeChanged += expander.CanvasSizeChanged;
                    }
                }
            };
            base.PointerPressed += (s, e) =>
            {
                foreach (Expander item in this.Items)
                {
                    switch (item.State)
                    {
                        case ExpanderState.Flyout:
                            item.Hide();
                            break;
                    }
                }
                base.Background = null;
            };
        }

        private void StateChanged(Expander sender, ExpanderState args)
        {
            switch (args)
            {
                case ExpanderState.Hide:
                case ExpanderState.Overlay:
                    base.Background = null;
                    foreach (Expander item in this.Items)
                    {
                        item.IsHitTestVisible = true;
                    }
                    Canvas.SetZIndex(sender, 0);
                    break;
                case ExpanderState.Flyout:
                    base.Background = this.Transparent;
                    foreach (Expander item in this.Items)
                    {
                        item.IsHitTestVisible = false;
                    }
                    sender.IsHitTestVisible = true;
                    break;
                default:
                    break;
            }
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
                    case ExpanderState.Hide:
                        break;
                    case ExpanderState.Flyout:
                        break;
                    case ExpanderState.Overlay:
                        if (item == sender) break;

                        int index = Canvas.GetZIndex(item);
                        if (index == 0) break;

                        Canvas.SetZIndex(item, index - 1);
                        break;
                    default:
                        break;
                }
            }
        }
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
        internal event TypedEventHandler<Expander, ExpanderState> StateChanged;
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

        ExpanderPlacementMode Placement = ExpanderPlacementMode.Top;
        double PlacementTargetW;
        double PlacementTargetH;
        Point PlacementTargetPosition;

        bool IsFristOpen = true;


        /// <summary> Gets a state that indicates whether the <see cref = "Expander" />. </summary>
        public ExpanderState State { get; private set; }


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
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                if (e.NewSize.Width < 100) return;
                if (e.NewSize.Height < 100) return;

                this.W = e.NewSize.Width;
                this.H = e.NewSize.Height;

                if (this.IsFristOpen)
                {
                    this.IsFristOpen = false;
                    this.ShowBegin();
                }
                else if (this.IsLoaded)
                {
                    this.SetLeft(Canvas.GetLeft(this));
                    this.SetTop(Canvas.GetTop(this));
                }
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

        /// <inheritdoc/>
        internal void CanvasSizeChanged(double u, double v)
        {
            this.U = u;
            this.V = v;

            base.MaxWidth = u;
            base.MaxHeight = v;
        }
        /// <inheritdoc/>
        internal void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize == Size.Empty) return;
            if (e.NewSize == e.PreviousSize) return;

            this.CanvasSizeChanged(e.NewSize.Width, e.NewSize.Height);

            switch (this.State)
            {
                case ExpanderState.Hide:
                    break;
                case ExpanderState.Flyout:
                    this.Placement = ExpanderPlacementMode.Center;
                    this.HideBegin();

                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.State = ExpanderState.Hide;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    break;
                case ExpanderState.Overlay:
                    this.SetLeft(Canvas.GetLeft(this));
                    this.SetTop(Canvas.GetTop(this));
                    break;
                default:
                    break;
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.X = Canvas.GetLeft(this);
            this.Y = Canvas.GetTop(this);

            switch (this.State)
            {
                case ExpanderState.Hide:
                    break;
                case ExpanderState.Flyout:
                    this.Placement = ExpanderPlacementMode.Center;

                    this.SymbolIcon.Symbol = Symbol.UnPin;
                    this.State = ExpanderState.Overlay;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                case ExpanderState.Overlay:
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                default:
                    break;
            }
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
            switch (this.State)
            {
                case ExpanderState.Hide:
                    break;
                case ExpanderState.Flyout:
                    this.Placement = ExpanderPlacementMode.Center;

                    this.SymbolIcon.Symbol = Symbol.UnPin;
                    this.State = ExpanderState.Overlay;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                case ExpanderState.Overlay:
                    this.HideBegin();

                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.State = ExpanderState.Hide;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Closes the <see cref = "Expander" />.
        /// </summary>
        public void Hide()
        {
            switch (this.State)
            {
                case ExpanderState.Hide:
                    break;
                case ExpanderState.Flyout:
                    this.Placement = ExpanderPlacementMode.Center;
                    this.HideBegin();

                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.State = ExpanderState.Hide;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    break;
                case ExpanderState.Overlay:
                    this.HideBegin();

                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.State = ExpanderState.Hide;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed.
        /// </summary>
        public void Show()
        {
            switch (this.State)
            {
                case ExpanderState.Hide:
                    this.ShowStoryboard.Begin(); // Storyboard
                    this.State = ExpanderState.Flyout;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                case ExpanderState.Flyout:
                    break;
                case ExpanderState.Overlay:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Shows the <see cref = "Expander" /> placed in relation to the specified element.
        /// </summary>
        /// <param name="placementTarget"> The element to use as the <see cref = "Expander" />'s placement target. </param>
        /// <param name="placement"> Gets or sets the default placement to be used for the <see cref = "Expander" />, in relation to its placement target. </param>
        public void ShowAt(FrameworkElement placementTarget, ExpanderPlacementMode placement)
        {
            switch (this.State)
            {
                case ExpanderState.Hide:
                    this.PlacementTargetW = placementTarget.ActualWidth;
                    this.PlacementTargetH = placementTarget.ActualHeight;
                    this.PlacementTargetPosition = placementTarget.TransformToVisual(base.Parent as UIElement).TransformPoint(new Point(0, 0));

                    this.Placement = placement;

                    if (this.IsFristOpen)
                        this.RootGrid.Visibility = Visibility.Visible;
                    else
                        this.ShowBegin();

                    this.State = ExpanderState.Flyout;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                case ExpanderState.Flyout:
                    break;
                case ExpanderState.Overlay:
                    break;
                default:
                    break;
            }
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
                case ExpanderState.Hide:
                    this.PlacementTargetW = placementTarget.ActualWidth;
                    this.PlacementTargetH = placementTarget.ActualHeight;
                    this.PlacementTargetPosition = placementTarget.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

                    this.Placement = placement;

                    if (this.IsFristOpen)
                        this.RootGrid.Visibility = Visibility.Visible;
                    else
                        this.ShowBegin();

                    this.State = ExpanderState.Flyout;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    this.OnZIndexChanging?.Invoke(this, Canvas.GetZIndex(this)); // Delegate
                    break;
                case ExpanderState.Flyout:
                    this.Placement = ExpanderPlacementMode.Center;
                    this.HideBegin();

                    this.SymbolIcon.Symbol = Symbol.Pin;
                    this.State = ExpanderState.Hide;
                    this.StateChanged?.Invoke(this, this.State); // Delegate
                    break;
                case ExpanderState.Overlay:
                    break;
                default:
                    break;
            }
        }


        private void ShowBegin()
        {
            switch (this.Placement)
            {
                case ExpanderPlacementMode.Center:
                    this.SetLeft(this.U / 2 - this.W / 2);
                    this.SetTop(this.V / 2 - this.H / 2);
                    break;
                case ExpanderPlacementMode.Left:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(this.PlacementTargetPosition.X - this.W);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(this.U - this.W - this.PlacementTargetW);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH / 2 - this.H / 2);
                    this.ShowRightStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Top:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(this.PlacementTargetPosition.Y - this.H);
                    this.ShowBottomStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Right:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(this.PlacementTargetW);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH / 2 - this.H / 2);
                    this.ShowLeftStoryboard.Begin(); // Storyboard
                    break;
                case ExpanderPlacementMode.Bottom:
                    switch (base.FlowDirection)
                    {
                        case FlowDirection.LeftToRight:
                            this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                            break;
                        case FlowDirection.RightToLeft:
                            this.SetLeft(this.PlacementTargetPosition.X + this.PlacementTargetW / 2 - this.W / 2);
                            break;
                        default:
                            break;
                    }
                    this.SetTop(this.PlacementTargetPosition.Y + this.PlacementTargetH);
                    this.ShowTopStoryboard.Begin(); // Storyboard
                    break;
                default:
                    break;
            }
            this.ShowStoryboard.Begin(); // Storyboard
        }
        private void HideBegin()
        {
            switch (this.Placement)
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


        private void SetLeft(double value) => Canvas.SetLeft(this, this.W >= this.U ? 0 : System.Math.Clamp(value, 0, this.U - this.W));
        private void SetTop(double value) => Canvas.SetTop(this, this.H >= this.V ? 0 : System.Math.Clamp(value, 0, this.V - this.H));

    }
}