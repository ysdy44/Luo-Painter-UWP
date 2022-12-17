﻿using Luo_Painter.Blends;
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
    public sealed partial class DrawPage
    {

        //@Content
        public bool PasteIsEnabled { get; set; }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        private void ConstructDraw()
        {
            this.EditMenuFlyout.Opened += (s, e) =>
            {
                this.PasteItem.GoToState(this.PasteIsEnabled);
            };
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