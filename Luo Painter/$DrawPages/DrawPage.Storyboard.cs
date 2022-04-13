using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        double StartingToolPaneX;
        double StartingLayerPaneX;

        SplitViewPanePlacement LayerPanePlacement => (this.LayerTransform.X > 70) ? SplitViewPanePlacement.Right : SplitViewPanePlacement.Left;
        SplitViewPanePlacement ToolPanePlacement => (this.ToolTransform.X < -70) ? SplitViewPanePlacement.Left : SplitViewPanePlacement.Right;

        private double GetToolTransformX(double value) => Math.Max(-70 - 70, Math.Min(0, value));
        private double GetLayerTransformX(double value) => Math.Max(0, Math.Min(70 + 70, value));

        private void ConstructStoryboard()
        {
            this.FullScreenButton.Click += (s, e) => this.HideStoryboard.Begin(); // Storyboard
            this.UnFullScreenButton.Click += (s, e) => this.ShowStoryboard.Begin(); // Storyboard
        }

        private void ConstructSplitStoryboard()
        {
            this.SplitToolButton.ManipulationStarted += (s, e) =>
            {
                this.StartingToolPaneX = this.ToolTransform.X;
                switch (this.ToolPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitToolIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitToolIcon.Symbol = Symbol.AlignRight; break;
                }
                this.SplitToolButton.IsEnabled = false;
            };
            this.SplitToolButton.ManipulationDelta += (s, e) =>
            {
                this.ToolTransform.X = this.GetToolTransformX(this.StartingToolPaneX + e.Cumulative.Translation.X);
                switch (this.ToolPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitToolIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitToolIcon.Symbol = Symbol.AlignRight; break;
                }
            };
            this.SplitToolButton.ManipulationCompleted += (s, e) =>
            {
                switch (this.ToolPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.ExpandToolStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.UnexpandToolStoryboard.Begin(); break;// Storyboard
                }
                this.SplitToolIcon.Symbol = Symbol.GlobalNavigationButton;
                this.SplitToolButton.IsEnabled = true;
            };

            this.SplitToolButton.Click += (s, e) =>
            {
                switch (this.ToolPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.UnexpandToolStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.ExpandToolStoryboard.Begin(); break;// Storyboard
                }
            };


            this.SplitLayerButton.ManipulationStarted += (s, e) =>
            {
                this.StartingLayerPaneX = this.LayerTransform.X;
                switch (this.LayerPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitLayerIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitLayerIcon.Symbol = Symbol.AlignRight; break;
                }
                this.SplitLayerButton.IsEnabled = false;
            };
            this.SplitLayerButton.ManipulationDelta += (s, e) =>
            {
                this.LayerTransform.X = this.GetLayerTransformX(this.StartingLayerPaneX + e.Cumulative.Translation.X);
                switch (this.LayerPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.SplitLayerIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitLayerIcon.Symbol = Symbol.AlignRight; break;
                }
            };
            this.SplitLayerButton.ManipulationCompleted += (s, e) =>
            {
                switch (this.LayerPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
                }
                this.SplitLayerIcon.Symbol = Symbol.GlobalNavigationButton;
                this.SplitLayerButton.IsEnabled = true;
            };

            this.SplitLayerButton.Click += (s, e) =>
            {
                switch (this.LayerPanePlacement)
                {
                    case SplitViewPanePlacement.Left: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
                    case SplitViewPanePlacement.Right: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
                }
            };
        }

    }
}