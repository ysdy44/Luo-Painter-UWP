using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }


        private void ConstructDialog()
        {
            this.SettingButton.Click += async (s, e) =>
            {
                await this.SettingDislog.ShowInstance();
            };

            this.ExportButton.ExportClick += async (s, e) =>
            {
                this.Tip("Saving...", this.ApplicationView.Title); // Tip

                bool? result = await this.Export();
                if (result == null) return;

                if (result.Value)
                    this.Tip("Saved successfully", this.ApplicationView.Title); // Tip
                else
                    this.Tip("Failed to Save", "Try again?"); // Tip
            };
        }


        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }

        private void ConstructColor()
        {
            this.ColorButton.ColorChanged += (s, e) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingColorChanged(e.NewColor);
                        break;
                    default:
                        break;
                }
            };
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
            if (this.CanvasAnimatedControl.Paused == isPaused) return;

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


        private void ConstructStoryboard()
        {
            this.UnFullScreenButton.Click += (s, e) =>
            {
                this.IsFullScreen = false;
                this.SetFullScreenState(false);
            };
            this.FullScreenButton.Click += async (s, e) =>
            {
                if (this.FootType.HasHead()) return;

                if (this.IsFullScreen)
                {
                    this.IsFullScreen = false;
                    this.SetFullScreenState(false);
                }
                else
                {
                    this.IsFullScreen = true;
                    this.SetFullScreenState(true);
                }

                this.FullScreenKey.IsEnabled = false;
                await Task.Delay(200);
                this.FullScreenKey.IsEnabled = true;
            };
        }



    }
}