using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
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
        private Visibility ShapeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIShape) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility GrainVisibilityConverter(InkType value) => value.HasFlag(InkType.UIGrain) ? Visibility.Visible : Visibility.Collapsed;

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        //@Task
        readonly object InkLocker = new object();
        CanvasRenderTarget InkRender { get; set; }

        bool InkIsEnabled = true;

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="PaintScrollViewer"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "PaintScrollViewer.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(InkType), typeof(PaintScrollViewer), new PropertyMetadata(default(InkType)));


        #endregion

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        bool IsDrak;
        public Color Color => this.IsDrak ? Colors.White : Colors.Black;
        public Vector4 ColorHdr => this.IsDrak ? Vector4.One : Vector4.UnitW;

        public object TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string texture) => this.InkParameter.ConstructTexture(texture);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;

            this.ConstructInk(item.InkPresenter);
            this.Type = item.InkPresenter.Type;
        }

        #endregion

        //@Construct
        public PaintScrollViewer()
        {
            this.InitializeComponent();
            this.ConstructCanvas();

            this.ConstructInk1();
            this.ConstructInk2();
            this.ConstructInk3();

            this.IsDrak = base.ActualTheme is ElementTheme.Dark;
            base.ActualThemeChanged += (s, e) => this.IsDrak = base.ActualTheme is ElementTheme.Dark;
        }

    }

    public sealed partial class PaletteMenu : Expander, IInkParameter
    {

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        //@Task
        readonly object Locker = new object();
        BitmapLayer BitmapLayer { get; set; }

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        readonly InkMixer InkMixer = new InkMixer();
        public Color Color => this.InkParameter.Color;
        public Vector4 ColorHdr => this.InkParameter.ColorHdr;

        public object TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string texture) => this.InkParameter.ConstructTexture(texture);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;

            this.CanvasControl.CustomDevice = this.CanvasDevice;
        }

        #endregion

        //@Construct
        public PaletteMenu()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

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
        }

    }

    public sealed partial class PaletteMenu : Expander, IInkParameter
    {
        readonly InkCanvasOperator Operator = new InkCanvasOperator();
        readonly Button ClearButton = new Button
        {
            Style = App.Current.Resources["AppButtonStyle"] as Style,
            Content = new SymbolIcon
            {
                Symbol = Symbol.Delete
            }
        };
        readonly Button ImageButton = new Button
        {
            Style = App.Current.Resources["AppButtonStyle"] as Style,
            Content = new SymbolIcon
            {
                Symbol = Symbol.Pictures
            }
        };
        readonly CanvasControl CanvasControl = new CanvasControl
        {
            ClearColor = Colors.White,
            UseSharedDevice = true,
        };

        public void InitializeComponent()
        {
            base.Height = 416;
            base.BorderBrush = App.Current.Resources["AppStroke"] as Brush;
            base.Background = App.Current.Resources["SystemControlAcrylicElementBrush"] as Brush;

            base.Title = "Palette";

            base.Content = this.CanvasControl;
            this.Operator.DestinationControl = this.CanvasControl;

            Grid.SetColumn(this.ImageButton, 1);
            Grid.SetColumn(this.ClearButton, 2);
            base.BottomAppBar = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width =new GridLength(1, GridUnitType.Star)
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    },
                },
                Children =
                {
                    this.ImageButton,
                    this.ClearButton,
                }
            };
        }
    }
}