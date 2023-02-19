using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Elements
{
    public enum ViewMode
    {
        None,
        Center,
        Wide,
        Tall,
        Full,
    }

    [ContentProperty(Name = nameof(Content))]
    public class Sheet : ContentControl
    {

        public Visibility WideConverter(ViewMode value) => value == ViewMode.Wide ? Visibility.Visible : Visibility.Collapsed;
        public Visibility TallConverter(ViewMode value) => value == ViewMode.Tall ? Visibility.Visible : Visibility.Collapsed;


        Storyboard HideStoryboard;
        Storyboard ShowStoryboard;
        Storyboard HideLeftStoryboard;
        Storyboard ShowLeftStoryboard;
        Storyboard HideRightStoryboard;
        Storyboard ShowRightStoryboard;
        Storyboard HideBottomStoryboard;
        Storyboard ShowBottomStoryboard;
        TranslateTransform TranslateTransform;
        Canvas RootGrid;

        Grid Grid;

        ContentPresenter ContentPresenter;
        Thumb Thumb;
        Button Button;

        Rectangle WideShader;
        Rectangle TailShader;


        double X;
        double Y;
        ViewMode Mode;
        public SplitViewPanePlacement PanePlacement { get; set; }

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "Sheet" />'s title. </summary>
        public string Title
        {
            get => (string)base.GetValue(TitleProperty);
            set => base.SetValue(TitleProperty, value);
        }
        /// <summary> Identifies the <see cref = "Sheet.Title" /> dependency property. </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Sheet), new PropertyMetadata(string.Empty));


        /// <summary> Gets or sets <see cref = "Sheet" />'s top AppBar. </summary>
        public UIElement TopAppBar
        {
            get => (UIElement)base.GetValue(TopAppBarProperty);
            set => base.SetValue(TopAppBarProperty, value);
        }
        /// <summary> Identifies the <see cref = "Sheet.TopAppBar" /> dependency property. </summary>
        public static readonly DependencyProperty TopAppBarProperty = DependencyProperty.Register(nameof(TopAppBar), typeof(UIElement), typeof(Sheet), new PropertyMetadata(null));


        /// <summary> Gets or sets <see cref = "Sheet" />'s bottom AppBar. </summary>
        public UIElement BottomAppBar
        {
            get => (UIElement)base.GetValue(BottomAppBarProperty);
            set => base.SetValue(BottomAppBarProperty, value);
        }
        /// <summary> Identifies the <see cref = "Sheet.BottomAppBar" /> dependency property. </summary>
        public static readonly DependencyProperty BottomAppBarProperty = DependencyProperty.Register(nameof(BottomAppBar), typeof(UIElement), typeof(Sheet), new PropertyMetadata(null));


        #endregion

        public Sheet()
        {
            this.DefaultStyleKey = typeof(Sheet);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                ViewMode mode = this.GetMode(e.NewSize);

                if (this.Mode != mode)
                {
                    this.Mode = mode;

                    switch (mode)
                    {
                        case ViewMode.Center:
                            double h = System.Math.Min(e.NewSize.Height, this.ContentPresenter.DesiredSize.Height + 70);

                            this.Grid.Width = 350;
                            this.Grid.Height = double.NaN;
                            Canvas.SetLeft(this.Grid, e.NewSize.Width / 2 - 200);
                            Canvas.SetTop(this.Grid, (e.NewSize.Height - h) / 2);

                            this.Grid.CornerRadius = new CornerRadius(14);
                            this.WideShader.Visibility = Visibility.Collapsed;
                            this.TailShader.Visibility = Visibility.Collapsed;

                            this.Thumb.Visibility = Visibility.Visible;
                            this.TranslateTransform.X = 0;
                            this.TranslateTransform.Y = 0;
                            break;
                        case ViewMode.Wide:
                            this.Grid.Width = 350;
                            this.Grid.Height = e.NewSize.Height;
                            switch (this.PanePlacement)
                            {
                                case SplitViewPanePlacement.Left:
                                    Canvas.SetLeft(this.Grid, 0);
                                    break;
                                case SplitViewPanePlacement.Right:
                                    Canvas.SetLeft(this.Grid, e.NewSize.Width - 350);
                                    break;
                            }
                            Canvas.SetTop(this.Grid, 0);

                            this.WideShader.Height = e.NewSize.Height;
                            switch (this.PanePlacement)
                            {
                                case SplitViewPanePlacement.Left:
                                    Canvas.SetLeft(this.WideShader, 350 - 14);
                                    break;
                                case SplitViewPanePlacement.Right:
                                    Canvas.SetLeft(this.WideShader, e.NewSize.Width - 350 - 74 + 14);
                                    break;
                            }
                            Canvas.SetTop(this.WideShader, 0);

                            switch (this.PanePlacement)
                            {
                                case SplitViewPanePlacement.Left:
                                    this.Grid.CornerRadius = new CornerRadius(0, 14, 14, 0);
                                    break;
                                case SplitViewPanePlacement.Right:
                                    this.Grid.CornerRadius = new CornerRadius(14, 0, 0, 14);
                                    break;
                            }
                            this.WideShader.Visibility = Visibility.Visible;
                            this.TailShader.Visibility = Visibility.Collapsed;

                            this.Thumb.Visibility = Visibility.Collapsed;
                            this.TranslateTransform.X = 0;
                            this.TranslateTransform.Y = 0;
                            break;
                        case ViewMode.Tall:
                            double height = System.Math.Min(e.NewSize.Height, this.ContentPresenter.DesiredSize.Height + 70);
                            double top = e.NewSize.Height - height;

                            this.Grid.Width = e.NewSize.Width;
                            this.Grid.Height = double.NaN;
                            Canvas.SetLeft(this.Grid, 0);
                            Canvas.SetTop(this.Grid, top);

                            this.TailShader.Width = e.NewSize.Width;
                            Canvas.SetLeft(this.TailShader, 0);
                            Canvas.SetTop(this.TailShader, top - 74 + 14);

                            this.Grid.CornerRadius = new CornerRadius(14, 14, 0, 0);
                            this.WideShader.Visibility = Visibility.Collapsed;
                            this.TailShader.Visibility = Visibility.Visible;

                            this.Thumb.Visibility = Visibility.Visible;
                            this.TranslateTransform.X = 0;
                            this.TranslateTransform.Y = 0;
                            break;
                        case ViewMode.Full:
                            this.Grid.Width = e.NewSize.Width;
                            this.Grid.Height = e.NewSize.Height;
                            Canvas.SetLeft(this.Grid, 0);
                            Canvas.SetTop(this.Grid, 0);

                            this.Grid.CornerRadius = new CornerRadius(0);
                            this.WideShader.Visibility = Visibility.Collapsed;
                            this.TailShader.Visibility = Visibility.Collapsed;

                            this.Thumb.Visibility = Visibility.Visible;
                            this.TranslateTransform.X = 0;
                            this.TranslateTransform.Y = 0;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case ViewMode.Center:
                            double h = System.Math.Min(e.NewSize.Height, this.ContentPresenter.DesiredSize.Height + 70);

                            Canvas.SetLeft(this.Grid, e.NewSize.Width / 2 - 350 / 2);
                            Canvas.SetTop(this.Grid, (e.NewSize.Height - h) / 2);
                            break;
                        case ViewMode.Wide:
                            switch (this.PanePlacement)
                            {
                                case SplitViewPanePlacement.Right:
                                    Canvas.SetLeft(this.Grid, e.NewSize.Width - 350);
                                    Canvas.SetLeft(this.WideShader, e.NewSize.Width - 350 - 74 + 14);
                                    break;
                            }
                            this.Grid.Height = e.NewSize.Height;
                            break;
                        case ViewMode.Tall:
                            double height = this.ContentPresenter.ActualHeight + 70;
                            double top = e.NewSize.Height - height;

                            this.Grid.Width = e.NewSize.Width;
                            Canvas.SetTop(this.Grid, top);

                            this.TailShader.Width = e.NewSize.Width;
                            Canvas.SetTop(this.TailShader, top - 74 + 14);
                            break;
                        case ViewMode.Full:
                            this.Grid.Width = e.NewSize.Width;
                            this.Grid.Height = e.NewSize.Height;
                            break;
                        default:
                            break;
                    }
                }
            };
        }
        private ViewMode GetMode(Size size)
        {
            if (size.Width < 641) return ViewMode.Full;
            return ViewMode.Wide;


            bool outWidth = size.Width > 641;
            bool outHeight = size.Height > 641;

            if (outWidth && outHeight) return ViewMode.Center;
            else if (outWidth) return ViewMode.Wide;
            else if (outHeight) return ViewMode.Tall;
            else return ViewMode.Full;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.RootGrid is null is false)
            {
                this.HideStoryboard = null;
                this.ShowStoryboard = null;
                this.HideLeftStoryboard = null;
                this.ShowLeftStoryboard = null;
                this.HideRightStoryboard = null;
                this.ShowRightStoryboard = null;
                this.HideBottomStoryboard = null;
                this.ShowBottomStoryboard = null;
                this.TranslateTransform = null;
            }
            this.RootGrid = base.GetTemplateChild(nameof(RootGrid)) as Canvas;
            if (this.RootGrid is null is false)
            {
                this.HideStoryboard = this.RootGrid.Resources[nameof(HideStoryboard)] as Storyboard;
                this.ShowStoryboard = this.RootGrid.Resources[nameof(ShowStoryboard)] as Storyboard;
                this.HideLeftStoryboard = this.RootGrid.Resources[nameof(HideLeftStoryboard)] as Storyboard;
                this.ShowLeftStoryboard = this.RootGrid.Resources[nameof(ShowLeftStoryboard)] as Storyboard;
                this.HideRightStoryboard = this.RootGrid.Resources[nameof(HideRightStoryboard)] as Storyboard;
                this.ShowRightStoryboard = this.RootGrid.Resources[nameof(ShowRightStoryboard)] as Storyboard;
                this.HideBottomStoryboard = this.RootGrid.Resources[nameof(HideBottomStoryboard)] as Storyboard;
                this.ShowBottomStoryboard = this.RootGrid.Resources[nameof(ShowBottomStoryboard)] as Storyboard;
                this.TranslateTransform = this.RootGrid.RenderTransform as TranslateTransform;
            }

            this.Grid = base.GetTemplateChild(nameof(Grid)) as Grid;

            this.ContentPresenter = base.GetTemplateChild(nameof(ContentPresenter)) as ContentPresenter;

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

            this.WideShader = base.GetTemplateChild(nameof(WideShader)) as Rectangle;
            if (this.WideShader is null is false)
            {
                this.WideShader.Visibility = this.WideConverter(this.Mode);
                if (this.WideShader.Fill is LinearGradientBrush brush)
                {
                    switch (this.PanePlacement)
                    {
                        case SplitViewPanePlacement.Left:
                            brush.StartPoint = new Point(1, 0.5);
                            brush.EndPoint = new Point(0, 0.5);
                            break;
                        case SplitViewPanePlacement.Right:
                            brush.StartPoint = new Point(0, 0.5);
                            brush.EndPoint = new Point(1, 0.5);
                            break;
                        default:
                            break;
                    }
                }
            }

            this.TailShader = base.GetTemplateChild(nameof(TailShader)) as Rectangle;
            if (this.TailShader is null is false) this.TailShader.Visibility = this.TallConverter(this.Mode);
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.X = this.TranslateTransform.X;
            this.Y = this.TranslateTransform.Y;
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            switch (this.Mode)
            {
                case ViewMode.Center:
                    this.X += e.HorizontalChange;
                    this.Y += e.VerticalChange;
                    this.TranslateTransform.X = this.X;
                    this.TranslateTransform.Y = this.Y;
                    break;
                case ViewMode.Tall:
                case ViewMode.Full:
                    this.Y += e.VerticalChange;
                    this.TranslateTransform.Y = System.Math.Clamp(this.Y, 0, this.ContentPresenter.ActualHeight);
                    break;
                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void Show()
        {
            switch (this.Mode)
            {
                case ViewMode.Wide:
                    switch (this.PanePlacement)
                    {
                        case SplitViewPanePlacement.Left:
                            this.ShowLeftStoryboard.Begin(); // Storyboard
                            break;
                        case SplitViewPanePlacement.Right:
                            this.ShowRightStoryboard.Begin(); // Storyboard
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    this.ShowBottomStoryboard.Begin(); // Storyboard
                    break;
            }
            this.ShowStoryboard.Begin(); // Storyboard
        }
        public void Hide()
        {
            switch (this.Mode)
            {
                case ViewMode.Wide:
                    switch (this.PanePlacement)
                    {
                        case SplitViewPanePlacement.Left:
                            this.HideLeftStoryboard.Begin(); // Storyboard
                            break;
                        case SplitViewPanePlacement.Right:
                            this.HideRightStoryboard.Begin(); // Storyboard
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    this.HideBottomStoryboard.Begin(); // Storyboard
                    break;
            }
            this.HideStoryboard.Begin(); // Storyboard
        }


    }
}