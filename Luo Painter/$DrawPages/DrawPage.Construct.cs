using Luo_Painter.Brushes;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using System;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
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
        bool PasteLayerIsEnabled;
        
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
            this.PasteLayerIsEnabled = this.ClipboardLayers.Count is 0 is false;
        }


        public void Tip(TipType type)
        {
            this.ToastTip.Tip
            (
                App.Resource.GetString($"Tip_{type}"),
                App.Resource.GetString($"SubTip_{type}")
            );
        }
        public void Tip(TipType type, string subtitle)
        {
            this.ToastTip.Tip
            (
                App.Resource.GetString($"Tip_{type}"),
                subtitle
            );
        }

    }
}