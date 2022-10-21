using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
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
        private string RoundConverter(double value) => $"{value:0}";
        private string SizeXToYConverter(double value) => this.RoundConverter(this.SizeRange.ConvertXToY(value));
        private string SpacingXToYConverter(double value) => this.RoundConverter(this.SpacingRange.ConvertXToY(value));

        private bool BooleanConverter(bool? value) => value is true;
        private double PercentageConverter(double value) => System.Math.Clamp(value / 100d, 0d, 1d);

        private Visibility BooleanToVisibilityConverter(bool? value) => value is true ? Visibility.Visible : Visibility.Collapsed;
        private Visibility SpacingVisibilityConverter(InkType value) => value.HasFlag(InkType.UISpacing) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility FlowVisibilityConverter(InkType value) => value.HasFlag(InkType.UIFlow) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility TipVisibilityConverter(InkType value) => value.HasFlag(InkType.UITip) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility BlendModeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIBlendMode) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility HardnessVisibilityConverter(InkType value) => value.HasFlag(InkType.UIHardness) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility MaskVisibilityConverter(InkType value) => value.HasFlag(InkType.UIMask) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility PatternVisibilityConverter(InkType value) => value.HasFlag(InkType.UIPattern) ? Visibility.Visible : Visibility.Collapsed;

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

        bool IsDrak;
        Color Color => this.IsDrak ? Colors.White : Colors.Black;
        Vector4 ColorHdr => this.IsDrak ? Vector4.One : Vector4.UnitW;

        IInkParameter InkParameter;

        bool InkIsEnabled = true;

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="BrushPage"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "BrushPage.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(InkType), typeof(BrushPage), new PropertyMetadata(default(InkType)));


        #endregion


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

            this.IsDrak = base.ActualTheme is ElementTheme.Dark;
            base.ActualThemeChanged += (s, e) => this.IsDrak = base.ActualTheme is ElementTheme.Dark;

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

                this.ConstructInk(item.InkPresenter);
                this.Type = item.InkPresenter.Type;
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