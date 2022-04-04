using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        double StartingPaneX;
        SplitViewPanePlacement PanePlacement => (this.LayerTransform.X > 75) ? SplitViewPanePlacement.Right : SplitViewPanePlacement.Left;

        private void ConstructStoryboard()
        {
            this.ShowStoryboard.Completed += (s, e) => this.DismissOverlay.IsHitTestVisible = true;
            this.HideStoryboard.Completed += (s, e) => this.DismissOverlay.IsHitTestVisible = false;
            this.DismissOverlay.Tapped += (s, e) =>
            {
                this.ToolButton.IsChecked = false;
                this.LayerButton.IsChecked = false;
            };

            this.LayerButton.Unchecked += (s, e) =>
            {
                this.HideLayerStoryboard.Begin(); // Storyboard
                if (this.ToolButton.IsChecked == false) this.HideStoryboard.Begin(); // Storyboard
            };
            this.LayerButton.Checked += (s, e) =>
            {
                this.ShowLayerStoryboard.Begin(); // Storyboard
                if (this.ToolButton.IsChecked == false)
                {
                    this.ShowStoryboard.Begin(); // Storyboard
                }
            };
            this.LayerButton.Tapped += (s, e) =>
            {
                this.HideToolStoryboard.Begin(); // Storyboard
            };
            this.ShowLayerStoryboard.Completed += (s, e) =>
            {
                if (this.ToolButton.Visibility == Visibility.Visible) this.ToolButton.IsChecked = false;
            };

            this.ToolButton.Unchecked += (s, e) =>
            {
                this.HideToolStoryboard.Begin(); // Storyboard
                if (this.LayerButton.IsChecked == false) this.HideStoryboard.Begin(); // Storyboard
            };
            this.ToolButton.Checked += (s, e) =>
            {
                this.ShowToolStoryboard.Begin(); // Storyboard
                if (this.LayerButton.IsChecked == false)
                {
                    this.ShowStoryboard.Begin(); // Storyboard
                }
            };
            this.ToolButton.Tapped += (s, e) =>
            {
                this.HideLayerStoryboard.Begin(); // Storyboard
            };
            this.ShowToolStoryboard.Completed += (s, e) =>
            {
                if (this.LayerButton.Visibility == Visibility.Visible) this.LayerButton.IsChecked = false;
            };
        }

        private void ConstructSplitStoryboard()
        {
            this.SplitButton.ManipulationStarted += (s, e) =>
            {
                this.StartingPaneX = this.LayerTransform.X;
                switch (this.PanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitIcon.Symbol = Symbol.AlignRight; break;
                }
                this.SplitButton.IsEnabled = false;
            };
            this.SplitButton.ManipulationDelta += (s, e) =>
            {
                this.LayerTransform.X = Math.Max(0, Math.Min(150, this.StartingPaneX + e.Cumulative.Translation.X));
                switch (this.PanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitIcon.Symbol = Symbol.AlignRight; break;
                }
            };
            this.SplitButton.ManipulationCompleted += (s, e) =>
            {
                switch (this.PanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
                }
                this.SplitIcon.Symbol = Symbol.GlobalNavigationButton;
                this.SplitButton.IsEnabled = true;
            };
           
            this.SplitButton.Click += (s, e) =>
            {
                switch (this.PanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
                }
            };
        }

    }
}