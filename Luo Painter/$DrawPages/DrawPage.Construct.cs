using Luo_Painter.Blends;
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


        public void RaiseHistoryCanExecuteChanged()
        {
            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.UndoButton2.IsEnabled = this.History.CanUndo;

            this.RedoButton.IsEnabled = this.History.CanRedo;
            this.RedoButton2.IsEnabled = this.History.CanRedo;
        }
        public void RaiseEditCanExecuteChanged()
        {
            this.EditMenu.PasteIsEnabled = true;
        }
        public void RaiseLayerCanExecuteChanged()
        {
            this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
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