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
    public sealed partial class DrawPage : Page, ILayerManager
    {

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }


        private void ConstructDialog()
        {
            this.ExportMenu.ExportClick += async (s, e) =>
            {
                if (this.ExportMenu.IsAllLayers)
                {
                    IStorageFolder folder = await FileUtil.PickSingleFolderAsync(PickerLocationId.Desktop);
                    if (folder is null) return;
                    this.Tip("Saving...", this.ApplicationView.Title); // Tip

                    // Export
                    int result = await this.Nodes.ExportAllthis(folder, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, this.ExportMenu.DPI, this.ExportMenu.FileChoices, this.ExportMenu.FileFormat, 1);
                    this.Tip("Saved successfully", $"A total of {result} files"); // Tip
                }
                else
                {
                    IStorageFile file = await FileUtil.PickSingleFileAsync(PickerLocationId.Desktop, this.ExportMenu.FileChoices, this.ApplicationView.Title);
                    if (file is null) return;
                    this.Tip("Saving...", this.ApplicationView.Title); // Tip

                    // Export
                    bool result = await this.Nodes.Export(file, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, this.ExportMenu.DPI, this.ExportMenu.FileFormat, 1);
                    if (result)
                        this.Tip("Saved successfully", this.ApplicationView.Title); // Tip
                    else
                        this.Tip("Failed to Save", "Try again?"); // Tip
                }
            };
        }


        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }

        private void ConstructColor()
        {
            this.ColorMenu.ColorChanged += (s, e) =>
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