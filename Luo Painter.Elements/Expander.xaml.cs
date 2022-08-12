using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
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
    public partial class Expander : ContentControl
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

    }
}