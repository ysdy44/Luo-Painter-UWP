using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace Luo_Painter.Elements
{
    public partial class Expander : ContentControl
    {
        
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

    }
}