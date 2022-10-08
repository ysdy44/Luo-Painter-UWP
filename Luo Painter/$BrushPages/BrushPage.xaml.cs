using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        //@Converter
        private Symbol StarSymbolConverter(bool value) => value ? Symbol.SolidStar : Symbol.OutlineStar;
        private Symbol PinSymbolConverter(SplitViewDisplayMode value)
        {
            switch (value)
            {
                case SplitViewDisplayMode.Overlay: return Symbol.Pin;
                default: return Symbol.UnPin;
            }
        }


        CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        InkMixer InkMixer { get; set; } = new InkMixer();
        InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        //@Task
        readonly object InkLocker = new object();
        CanvasRenderTarget InkRender { get; set; }

        //@Task
        readonly object Locker = new object();
        BitmapLayer BitmapLayer { get; set; }

        InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }


        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;
        Color Color = Colors.White;
        Vector4 ColorHdr = Vector4.One;

        IInkParameter InkParameter;


        //@Construct
        [DrawPageToBrushPage(NavigationMode.Back)]
        public BrushPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructInk1();
            this.ConstructInk2();
            this.ConstructInk3();

            base.ActualThemeChanged += (s, e) =>
            {
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.Color = Colors.Black;
                        this.ColorHdr = new Vector4(0, 0, 0, 1);
                        break;
                    case ElementTheme.Dark:
                        this.Color = Colors.White;
                        this.ColorHdr = Vector4.One;
                        break;
                    default:
                        break;
                }
            };

            this.CloseButton.Click += (s, e) => this.SplitView.IsPaneOpen = false;
            this.ShowButton.Click += (s, e) => this.SplitView.IsPaneOpen = !this.SplitView.IsPaneOpen;
            this.PinButton.Click += (s, e) =>
            {
                switch (this.SplitView.DisplayMode)
                {
                    case SplitViewDisplayMode.Overlay:
                        this.SplitView.DisplayMode = SplitViewDisplayMode.Inline;
                        break;
                    case SplitViewDisplayMode.Inline:
                        this.SplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                        this.SplitView.IsPaneOpen = false;
                        break;
                    default:
                        break;
                }
            };

            this.ToolComboBox.SelectionChanged += (s, e) =>
            {
                switch (this.ToolComboBox.SelectedIndex)
                {
                    case 0: // OptionType.PaintBrush
                        if (this.InkPresenter.ToolType == InkType.Brush) return;
                        this.InkPresenter.ToolType = InkType.Brush;
                        this.Type = InkType.Brush;
                        break;
                    case 1: // OptionType.PaintWatercolorPen
                        if (this.InkPresenter.ToolType == InkType.Circle) return;
                        this.InkPresenter.ToolType = InkType.Circle;
                        this.Type = InkType.Circle;
                        break;
                    case 2: // OptionType.PaintPencil
                        if (this.InkPresenter.ToolType == InkType.Line) return;
                        this.InkPresenter.ToolType = InkType.Line;
                        this.Type = InkType.Line;
                        break;
                    case 3: // OptionType.PaintEraseBrush
                        if (this.InkPresenter.ToolType == InkType.Erase) return;
                        this.InkPresenter.ToolType = InkType.Erase;
                        this.Type = InkType.Erase;
                        break;
                    case 4: // OptionType.PaintLiquefaction
                        if (this.InkPresenter.ToolType == InkType.Liquefy) return;
                        this.InkPresenter.ToolType = InkType.Liquefy;
                        this.Type = InkType.Liquefy;
                        break;
                    default: // default
                        if (this.InkPresenter.ToolType == default) return;
                        this.InkPresenter.ToolType = default;
                        this.Type = default;
                        break;
                }

                this.InkType = this.InkPresenter.GetType();

                this.InkCanvasControl.Invalidate();
            };


            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.ClearButton.Click += (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker))
                {
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);

                    this.CanvasControl.Invalidate();

                    System.Threading.Monitor.Exit(this.Locker);
                }
            };
            this.ImageButton.Click += async (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker)) System.Threading.Monitor.Exit(this.Locker);
                else return;

                IRandomAccessStreamReference file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                try
                {
                    using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                    {
                        CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Draw(bitmap);

                            // History
                            this.BitmapLayer.Flush();

                            this.CanvasControl.Invalidate(); // Invalidate
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            };


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker)) System.Threading.Monitor.Exit(this.Locker);
                else return;

                if (e.DataView.Contains(StandardDataFormats.StorageItems) is false) return;

                foreach (IStorageItem item in await e.DataView.GetStorageItemsAsync())
                {
                    if (item is StorageFile file)
                    {
                        using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                        {
                            CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                            lock (this.Locker)
                            {
                                this.BitmapLayer.Draw(bitmap);

                                // History
                                this.BitmapLayer.Flush();

                                this.CanvasControl.Invalidate(); // Invalidate
                                return;
                            }
                        }
                    }
                }
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };
        }

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.InkParameter = null;
            this.IsEnabled = false;

            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.BackRequested -= this.BackRequested;
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        [DrawPageToBrushPage(NavigationMode.New)]
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IInkParameter item)
            {
                this.InkParameter = item;

                this.InkPresenter.ToolType = InkType.Brush;
                this.Type = InkType.Brush;
                this.InkType = this.InkPresenter.GetType();
            }
            else
            {
                this.InkParameter = null;

                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }

                throw new NullReferenceException($"{nameof(this.InkParameter)} is null.");
            }

            this.IsEnabled = true;

            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                manager.BackRequested += this.BackRequested;
            }
        }
        [DrawPageToBrushPage(NavigationMode.Back)]
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            lock (this.Locker)
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            }
        }

    }
}