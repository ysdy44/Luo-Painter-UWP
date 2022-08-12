using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public partial class Expander : ContentControl
    {

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

    }
}