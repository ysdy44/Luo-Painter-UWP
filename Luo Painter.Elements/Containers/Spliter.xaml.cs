using System.Linq;
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
    [TemplatePart(Name = nameof(RootCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(HideStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(ShowStoryboard), Type = typeof(Storyboard))]
    [TemplatePart(Name = nameof(HideDoubleAnimation), Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = nameof(TranslateTransform), Type = typeof(TranslateTransform))]
    [TemplatePart(Name = nameof(ContentPresenter), Type = typeof(ContentPresenter))]
    [TemplatePart(Name = nameof(ALine), Type = typeof(Line))]
    [TemplatePart(Name = nameof(BLine), Type = typeof(Line))]
    [TemplatePart(Name = nameof(ThumbBorder), Type = typeof(Border))]
    [TemplatePart(Name = nameof(Thumb), Type = typeof(Thumb))]
    [ContentProperty(Name = nameof(Content))]
    public class Spliter : ContentControl
    {

        //@Converter
        private HorizontalAlignment SplitViewPanePlacementToHorizontalAlignmentConverter(SplitViewPanePlacement value)
        {
            switch (value)
            {
                case SplitViewPanePlacement.Right: return HorizontalAlignment.Right;
                default: return HorizontalAlignment.Left;
            }
        }

        Canvas RootCanvas;
        Storyboard HideStoryboard;
        Storyboard ShowStoryboard;
        DoubleAnimation HideDoubleAnimation;

        TranslateTransform TranslateTransform;

        ContentPresenter ContentPresenter;

        Line ALine;
        Line BLine;
        Border ThumbBorder;
        Thumb Thumb;

        // Position
        int Index;
        double StartingW;
        double W;
        double H;
        double A;
        double B;

        #region DependencyProperty


        /// <summary> Gets or set the placement for <see cref="Spliter"/>. </summary>
        public SplitViewPanePlacement Placement
        {
            get => (SplitViewPanePlacement)base.GetValue(PlacementProperty);
            set => base.SetValue(PlacementProperty, value);
        }
        /// <summary> Identifies the <see cref = "Spliter.Placement" /> dependency property. </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(SplitViewPanePlacement), typeof(Spliter), new PropertyMetadata(SplitViewPanePlacement.Left, (sender, e) =>
        {
            Spliter control = (Spliter)sender;

            if (e.NewValue is SplitViewPanePlacement value)
            {
                control.HorizontalAlignment = control.SplitViewPanePlacementToHorizontalAlignmentConverter(value);
                control.UpdateStoryboard(value);
                control.UpdateContentPresenter(value);
                control.UpdateALine(value);
                control.UpdateBLine(value);
                control.UpdateBorder(value);
                control.UpdateBorder2(value);

                if (control.TranslateTransform is null) return;
                control.TranslateTransform.X = -control.TranslateTransform.X;
            }
        }));

        /// <summary> Gets or set the state for <see cref="Spliter"/>. </summary>
        public bool IsShow
        {
            get => (bool)base.GetValue(IsShowProperty);
            set => base.SetValue(IsShowProperty, value);
        }
        /// <summary> Identifies the <see cref = "Spliter.IsShow" /> dependency property. </summary>
        public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(Spliter), new PropertyMetadata(true, (sender, e) =>
        {
            Spliter control = (Spliter)sender;

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


        #endregion

        //@Construct     
        /// <summary>
        /// Initializes a Spliter. 
        /// </summary>
        public Spliter()
        {
            this.DefaultStyleKey = typeof(Spliter);
            this.HorizontalAlignment = this.SplitViewPanePlacementToHorizontalAlignmentConverter(this.Placement);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                if (this.Thumb is null is false)
                {
                    if (this.Thumb.IsDragging) return;
                }

                this.Index = (int)(e.NewSize.Width / 70 + 0.5);
                this.W = this.Index * 70;
                this.H = e.NewSize.Height;
                this.A = (this.H - 120) / 2;
                this.B = (this.H + 120) / 2;

                this.UpdateStoryboard(this.Placement);
                this.UpdateContentPresenter(this.Placement);
                this.UpdateALine(this.Placement);
                this.UpdateBLine(this.Placement);
                this.UpdateBorder(this.Placement);
            };
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.RootCanvas is null is false)
            {
                this.HideStoryboard = null;
                this.ShowStoryboard = null;
                this.HideDoubleAnimation = null;
            }
            this.RootCanvas = base.GetTemplateChild(nameof(RootCanvas)) as Canvas;
            if (this.RootCanvas is null is false)
            {
                this.HideStoryboard = this.RootCanvas.Resources[nameof(HideStoryboard)] as Storyboard;
                this.ShowStoryboard = this.RootCanvas.Resources[nameof(ShowStoryboard)] as Storyboard;
                if (this.HideStoryboard is null is false)
                {
                    this.HideDoubleAnimation = this.HideStoryboard.Children.First() as DoubleAnimation;
                    this.UpdateStoryboard(this.Placement);
                }
            }

            this.TranslateTransform = base.GetTemplateChild(nameof(TranslateTransform)) as TranslateTransform;

            this.ContentPresenter = base.GetTemplateChild(nameof(ContentPresenter)) as ContentPresenter;
            this.UpdateContentPresenter(this.Placement);

            this.ALine = base.GetTemplateChild(nameof(ALine)) as Line;
            this.UpdateALine(this.Placement);

            this.BLine = base.GetTemplateChild(nameof(BLine)) as Line;
            this.UpdateBLine(this.Placement);

            this.ThumbBorder = base.GetTemplateChild(nameof(ThumbBorder)) as Border;
            this.UpdateBorder(this.Placement);
            this.UpdateBorder2(this.Placement);

            if (this.Thumb is null is false)
            {
                this.Thumb.DragStarted -= this.Thumb_DragStarted;
                this.Thumb.DragDelta -= this.Thumb_DragDelta;
                this.Thumb.DragCompleted -= this.Thumb_DragCompleted;
            }
            this.Thumb = base.GetTemplateChild(nameof(Thumb)) as Thumb;
            if (this.Thumb is null is false)
            {
                this.Thumb.DragStarted += this.Thumb_DragStarted;
                this.Thumb.DragDelta += this.Thumb_DragDelta;
                this.Thumb.DragCompleted += this.Thumb_DragCompleted;
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e) => this.StartingW = base.ActualWidth;
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            switch (this.Placement)
            {
                case SplitViewPanePlacement.Left:
                    this.StartingW += e.HorizontalChange;
                    break;
                case SplitViewPanePlacement.Right:
                    this.StartingW -= e.HorizontalChange;
                    break;
                default:
                    break;
            }

            double w = System.Math.Clamp(this.StartingW, base.MinWidth, base.MaxWidth);
            this.Index = (int)(w / 70 + 0.5);
            this.W = w;
            base.Width = w;

            this.UpdateStoryboard(this.Placement);
            this.UpdateContentPresenter(this.Placement);
            this.UpdateALine(this.Placement);
            this.UpdateBLine(this.Placement);
            this.UpdateBorder(this.Placement);
        }
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double w = System.Math.Clamp(this.StartingW, base.MinWidth, base.MaxWidth);
            this.Index = (int)(w / 70 + 0.5);
            this.W = this.Index * 70;
            base.Width = this.W;

            this.UpdateStoryboard(this.Placement);
            this.UpdateContentPresenter(this.Placement);
            this.UpdateALine(this.Placement);
            this.UpdateBLine(this.Placement);
            this.UpdateBorder(this.Placement);
        }

        private void UpdateStoryboard(SplitViewPanePlacement placement)
        {
            if (this.HideDoubleAnimation is null) return;
            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    this.HideDoubleAnimation.To = -22 - this.Index * 70;
                    break;
                case SplitViewPanePlacement.Right:
                    this.HideDoubleAnimation.To = 22 + this.Index * 70;
                    break;
                default:
                    break;
            }
        }

        private void UpdateContentPresenter(SplitViewPanePlacement placement)
        {
            if (this.ContentPresenter is null) return;

            switch (this.Index)
            {
                case 0:
                    this.ContentPresenter.Visibility = Visibility.Collapsed;
                    this.ContentPresenter.Width = 70;
                    break;
                case 1:
                    this.ContentPresenter.Visibility = Visibility.Visible;
                    this.ContentPresenter.Width = 70;
                    break;
                default:
                    this.ContentPresenter.Visibility = Visibility.Visible;
                    this.ContentPresenter.Width = this.Index * 70;
                    break;
            }

            this.ContentPresenter.Height = this.H;

            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    Canvas.SetLeft(this.ContentPresenter, this.W - this.Index * 70);
                    break;
                case SplitViewPanePlacement.Right:
                    Canvas.SetLeft(this.ContentPresenter, 0);
                    break;
                default:
                    break;
            }
        }

        private void UpdateALine(SplitViewPanePlacement placement)
        {
            if (this.ALine is null) return;

            this.ALine.Y1 = 0;
            this.ALine.Y2 = this.A;
            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    this.ALine.X1 = this.W;
                    this.ALine.X2 = this.W;
                    break;
                case SplitViewPanePlacement.Right:
                    this.ALine.X1 = 0;
                    this.ALine.X2 = 0;
                    break;
                default:
                    break;
            }
        }
        private void UpdateBLine(SplitViewPanePlacement placement)
        {
            if (this.BLine is null) return;

            this.BLine.Y1 = this.B;
            this.BLine.Y2 = this.H;
            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    this.BLine.X1 = this.W;
                    this.BLine.X2 = this.W;
                    break;
                case SplitViewPanePlacement.Right:
                    this.BLine.X1 = 0;
                    this.BLine.X2 = 0;
                    break;
                default:
                    break;
            }
        }
        private void UpdateBorder(SplitViewPanePlacement placement)
        {
            if (this.ThumbBorder is null) return;

            Canvas.SetTop(this.ThumbBorder, this.A);
            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    Canvas.SetLeft(this.ThumbBorder, this.W);
                    break;
                case SplitViewPanePlacement.Right:
                    Canvas.SetLeft(this.ThumbBorder, -22);
                    break;
                default:
                    break;
            }
        }
        private void UpdateBorder2(SplitViewPanePlacement placement)
        {
            if (this.ThumbBorder is null) return;

            this.ThumbBorder.Visibility = this.IsShow ? Visibility.Visible : Visibility.Collapsed;
            switch (placement)
            {
                case SplitViewPanePlacement.Left:
                    this.ThumbBorder.BorderThickness = new Thickness(0, 1, 1, 1);
                    this.ThumbBorder.CornerRadius = new CornerRadius(0, 8, 8, 0);
                    break;
                case SplitViewPanePlacement.Right:
                    this.ThumbBorder.BorderThickness = new Thickness(1, 1, 0, 1);
                    this.ThumbBorder.CornerRadius = new CornerRadius(8, 0, 0, 8);
                    break;
                default:
                    break;
            }
        }

    }
}