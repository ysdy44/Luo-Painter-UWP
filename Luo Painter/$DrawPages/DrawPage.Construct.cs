using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }


        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }


        private void SetFullScreenState(bool isFullScreen)
        {
            if (isFullScreen)
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
        }


        private void SetCanvasState(bool isPaused)
        {
            // if (this.CanvasAnimatedControl.Paused == isPaused) return;

            if (isPaused)
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.CanvasAnimatedControl.Paused = true;
                this.CanvasAnimatedControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasAnimatedControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.CanvasAnimatedControl.Paused = false;
                this.CanvasAnimatedControl.Visibility = Visibility.Visible;
            }
        }


    }
}