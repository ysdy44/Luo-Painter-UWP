using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        WheelImageSource WheelImageSource;

        int BrushPageId;

        bool PasteIsEnabled;

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        private void ConstructDraw()
        {
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(300), dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;


            this.EditMenuFlyout.Opened += (s, e) =>
            {
                this.PasteItem.GoToState(this.PasteIsEnabled);
            };
        }

        private void SurfaceContentsLost(object sender, object e)
        {
            this.WheelImageSource.Redraw();
        }


        private async void TryShowBrushPage()
        {
            for (int i = 0; i < 5; i++)
            {
                if (this.BrushPageId == default)
                {
                    await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.CreateBrushPage);
                }
                else
                {
                    if (await ApplicationViewSwitcher.TryShowAsStandaloneAsync(this.BrushPageId)) return;
                }
            }
        }
        private void CreateBrushPage()
        {
            Frame frame = new Frame();
            IInkParameter parameter = this;
            frame.Navigate(typeof(BrushPage), parameter);

            Window.Current.Content = frame;
            Window.Current.Activate();

            this.BrushPageId = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Id;
        }


        public void RaiseHistoryCanExecuteChanged()
        {
            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }
        public void RaiseEditCanExecuteChanged()
        {
            this.PasteIsEnabled = true;
        }
        public void RaiseLayerCanExecuteChanged()
        {
            this.LayerListView.PasteLayerIsEnabled = this.ClipboardLayers.Count is 0 is false;
        }


        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }
        public void Tip(TipType type)
        {
            switch (type)
            {
                case TipType.NoLayer:
                    this.ToastTip.Tip("No Layer", "Create a new Layer?");
                    break;
                case TipType.NotBitmapLayer:
                    this.ToastTip.Tip("Not Bitmap Layer", "Can only operate on Bitmap Layer.");
                    break;
                case TipType.NotCurveLayer:
                    this.ToastTip.Tip("Not Curve Layer", "Can only operate on Curve Layer.");
                    break;

                case TipType.NoPixel:
                    this.ToastTip.Tip("No Pixel", "The current Pixel is Transparent.");
                    break;
                case TipType.NoPixelForMarquee:
                    this.ToastTip.Tip("No Pixel", "The Marquee is Transparent.");
                    break;
                case TipType.NoPixelForBitmapLayer:
                    this.ToastTip.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                    break;

                case TipType.NoPaintTool:
                    this.ToastTip.Tip("Paint Brush tool", "Brush Presets can only be used with the Paint");
                    break;

                case TipType.Spread:
                    this.ToastTip.Tip("Spread", $"{this.Rippler.Spread * 100:0.00}%");
                    break;
                case TipType.Zoom:
                    this.ToastTip.Tip("Zoom", $"{this.Transformer.Scale * 100:0.00}%");
                    break;
                case TipType.Undo:
                    this.ToastTip.Tip("Undo", $"{this.History.Index} / {this.History.Count}");
                    break;
                case TipType.Redo:
                    this.ToastTip.Tip("Redo", $"{this.History.Index} / {this.History.Count}");
                    break;

                case TipType.Saving:
                    this.ToastTip.Tip("Saving...", this.ApplicationView.Title);
                    break;
                case TipType.SaveSuccess:
                    this.ToastTip.Tip("Saved successfully", this.ApplicationView.Title);
                    break;
                case TipType.SaveFailed:
                    this.ToastTip.Tip("Failed to Save", "Try again?");
                    break;

                default:
                    break;
            }
        }

    }
}