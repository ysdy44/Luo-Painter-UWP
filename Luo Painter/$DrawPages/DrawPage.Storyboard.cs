using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        bool StateLock;
        private async void SetFullScreenState(bool isFullScreen, bool isWriteable)
        {
            if (this.StateLock is true) return;
            this.StateLock = true;

            if (isWriteable)
            {
                this.ToolListView.IsShow = false;
                this.LayerListView.IsShow = false;
                VisualStateManager.GoToState(this, nameof(Writeable), useTransitions: true);
            }
            else if (isFullScreen)
            {
                this.ToolListView.IsShow = false;
                this.LayerListView.IsShow = false;
                VisualStateManager.GoToState(this, nameof(FullScreen), useTransitions: true);
            }
            else
            {
                this.ToolListView.IsShow = true;
                this.LayerListView.IsShow = true;
                VisualStateManager.GoToState(this, nameof(UnFullScreen), useTransitions: true);
            }

            await Task.Delay(200);
            this.StateLock = false;
        }

        private void ConstructStoryboard()
        {
            this.UnFullScreenButton.Click += (s, e) =>
            {
                this.IsFullScreen = false;
                this.SetFullScreenState(this.IsFullScreen, this.OptionType != default);
            };
            this.FullScreenButton.Click += (s, e) =>
            {
                if (this.OptionType == default)
                {
                    this.IsFullScreen = !this.IsFullScreen;
                }
                else
                {
                    this.IsFullScreen = false;

                    this.OptionType = default;
                    this.SetOptionType(default);
                    this.CanvasControl.Invalidate(); // Invalidate
                }

                this.SetFullScreenState(this.IsFullScreen, false);
            };
        }

        private void ConstructSplitStoryboard()
        {
            //this.SplitToolButton.ManipulationStarted += (s, e) =>
            //{
            //    this.StartingToolPaneX = this.ToolTransform.X;
            //    switch (this.ToolPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.SplitToolIcon.Symbol = Symbol.AlignLeft; break;
            //        case SplitViewPanePlacement.Right: this.SplitToolIcon.Symbol = Symbol.AlignRight; break;
            //    }
            //    this.SplitToolButton.IsEnabled = false;
            //};
            //this.SplitToolButton.ManipulationDelta += (s, e) =>
            //{
            //    this.ToolTransform.X = this.GetToolTransformX(this.StartingToolPaneX + e.Cumulative.Translation.X);
            //    switch (this.ToolPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.SplitToolIcon.Symbol = Symbol.AlignLeft; break;
            //        case SplitViewPanePlacement.Right: this.SplitToolIcon.Symbol = Symbol.AlignRight; break;
            //    }
            //};
            //this.SplitToolButton.ManipulationCompleted += (s, e) =>
            //{
            //    switch (this.ToolPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.ExpandToolStoryboard.Begin(); break;// Storyboard
            //        case SplitViewPanePlacement.Right: this.UnexpandToolStoryboard.Begin(); break;// Storyboard
            //    }
            //    this.SplitToolIcon.Symbol = Symbol.GlobalNavigationButton;
            //    this.SplitToolButton.IsEnabled = true;
            //};

            //this.SplitToolButton.Click += (s, e) =>
            //{
            //    switch (this.ToolPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.UnexpandToolStoryboard.Begin(); break;// Storyboard
            //        case SplitViewPanePlacement.Right: this.ExpandToolStoryboard.Begin(); break;// Storyboard
            //    }
            //};


            //this.SplitLayerButton.ManipulationStarted += (s, e) =>
            //{
            //    this.StartingLayerPaneX = this.LayerTransform.X;
            //    switch (this.LayerPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.SplitLayerIcon.Symbol = Symbol.AlignLeft; break;
            //        case SplitViewPanePlacement.Right: this.SplitLayerIcon.Symbol = Symbol.AlignRight; break;
            //    }
            //    this.SplitLayerButton.IsEnabled = false;
            //};
            //this.SplitLayerButton.ManipulationDelta += (s, e) =>
            //{
            //    this.LayerTransform.X = this.GetLayerTransformX(this.StartingLayerPaneX + e.Cumulative.Translation.X);
            //    switch (this.LayerPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.SplitLayerIcon.Symbol = Symbol.AlignLeft; break;
            //        case SplitViewPanePlacement.Right: this.SplitLayerIcon.Symbol = Symbol.AlignRight; break;
            //    }
            //};
            //this.SplitLayerButton.ManipulationCompleted += (s, e) =>
            //{
            //    switch (this.LayerPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
            //        case SplitViewPanePlacement.Right: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
            //    }
            //    this.SplitLayerIcon.Symbol = Symbol.GlobalNavigationButton;
            //    this.SplitLayerButton.IsEnabled = true;
            //};

            //this.SplitLayerButton.Click += (s, e) =>
            //{
            //    switch (this.LayerPanePlacement)
            //    {
            //        case SplitViewPanePlacement.Left: this.ExpandLayerStoryboard.Begin(); break;// Storyboard
            //        case SplitViewPanePlacement.Right: this.UnexpandLayerStoryboard.Begin(); break;// Storyboard
            //    }
            //};
        }

    }
}