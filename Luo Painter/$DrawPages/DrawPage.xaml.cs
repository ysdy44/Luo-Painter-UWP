using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public enum ContextAppBarDevice
    {
        None,
        Phone,
        Pad,
        PC,
        Hub,
    }

    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);
        private bool IsAlt => this.IsKeyDown(VirtualKey.Menu);
        private bool IsSpace => this.IsKeyDown(VirtualKey.Space);

        private bool IsCenter => this.IsCtrl || this.TransformCenterButton.IsChecked is true;
        private bool IsRatio => this.IsShift || this.TransformRatioButton.IsChecked is true;
        private bool IsSnap => this.TransformSnapButton.IsChecked is true;

        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        public double SizeConverter(double value) => this.SizeRange.ConvertXToY(value);
        private double FontSizeConverter(double value) => this.SizeConverter(value) / 4 + 1;
        private string SizeToStringConverter(double value) => string.Format("{0:F}", this.SizeConverter(value));
        private double OpacityConverter(double value) => value / 100;
        private string OpacityToStringConverter(double value) => $"{(int)value} %";

        private Visibility SymmetryIndexToVisibilityConverter(int value)
        {
            switch (value)
            {
                case 0: return Visibility.Collapsed;
                case 1: return Visibility.Collapsed;
                default: return Visibility.Visible;
            }
        }

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));


        #region DependencyProperty: Device


        // GradientStopSelector
        private double ReverseDevicePhoneToWidth740Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                case ContextAppBarDevice.Pad: return 500;
                default: return 740;
            }
        }

        // InkSlider
        private double ReverseDevicePhoneToWidth300Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                default: return 300;
            }
        }

        // Slider 
        private double ReverseDevicePhoneToWidth260Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                default: return 260;
            }
        }

        // Slider Slider 
        private double ReverseDevicePhoneAndPadToWidth260Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad: return double.NaN;
                default: return 260;
            }
        }
        private Orientation ReverseDevicePhoneAndPadToOrientationHorizontalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }

        // Slider Slider Slider Slider 
        private Orientation DevicePhoneToOrientationVerticalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }
        private Orientation DeviceHubToOrientationHorizontalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad:
                case ContextAppBarDevice.PC: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }


        /// <summary> Gets or set the device for <see cref="DrawPage"/>. </summary>
        public ContextAppBarDevice Device
        {
            get => (ContextAppBarDevice)base.GetValue(DeviceProperty);
            set => base.SetValue(DeviceProperty, value);
        }
        /// <summary> Identifies the <see cref = "DrawPage.Device" /> dependency property. </summary>
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(ContextAppBarDevice), typeof(DrawPage), new PropertyMetadata(ContextAppBarDevice.None, (sender, e) =>
        {
            DrawPage control = (DrawPage)sender;

            if (e.NewValue is ContextAppBarDevice value)
            {
                switch (value)
                {
                    case ContextAppBarDevice.Phone:
                        VisualStateManager.GoToState(control, "Phone", false);
                        break;
                    case ContextAppBarDevice.Pad:
                        VisualStateManager.GoToState(control, "Pad", false);
                        break;
                    case ContextAppBarDevice.PC:
                        VisualStateManager.GoToState(control, "PC", false);
                        break;
                    case ContextAppBarDevice.Hub:
                        VisualStateManager.GoToState(control, "Hub", false);
                        break;
                    default:
                        break;
                }
            }
        }));


        #endregion


        public CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        Historian<IHistory> History { get; } = new Historian<IHistory>();

        public LayerRootNodes LayerManager = new LayerRootNodes();
        public LayerNodes Nodes => this.LayerManager;
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.LayerListView.SelectedIndex; set => this.LayerListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.LayerListView.SelectedItem; set => this.LayerListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.LayerListView.SelectedItems;


        GradientMesh GradientMesh { get; set; }
        CanvasBitmap GrayAndWhiteMesh { get; set; }
        CanvasRenderTarget Mesh { get; set; }

        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        BitmapLayer Displacement { get; set; }
        CurveLayer CurveLayer { get; set; }

        bool IsFullScreen { get; set; }
        SelectionType SelectionType { get; set; } = SelectionType.None;
        OptionType OptionType { get; set; } = OptionType.PaintBrush;

        bool IsReferenceImageResizing { get; set; }
        ReferenceImage ReferenceImage { get; set; }
        IList<ReferenceImage> ReferenceImages { get; } = new List<ReferenceImage>();


        //@Task
        readonly object Locker = new object();
        //@ Paint
        readonly PaintTaskCollection Tasks = new PaintTaskCollection();

        SymmetryType SymmetryType;
        readonly Symmetryer Symmetryer = new Symmetryer();

        Vector2 StartingPosition;
        Vector2 Position;
        Vector2 StartingPoint;
        Vector2 Point;
        float StartingPressure;
        float Pressure;


        // Transform
        TransformerBorder Bounds;

        Vector2 StartingMove;
        Vector2 Move;

        Transformer StartingBoundsTransformer;
        Transformer BoundsTransformer;
        Matrix3x2 BoundsMatrix;
        TransformerMode BoundsMode;
        bool IsBoundsMove;

        Transformer BoundsFreeTransformer;
        Matrix3x2 BoundsFreeMatrix;
        Vector2 BoundsFreeDistance;


        #region IInkParameter

        public InkType InkType { get; set; } = InkType.Tip;
        public InkPresenter InkPresenter { get; } = new InkPresenter
        {
            Type = InkType.Tip,

            Size = 12f,
            Opacity = 1f,

            Spacing = 0.25f,
            Flow = 1f,
        };

        public Color Color => this.ColorButton.Color;
        public Vector4 ColorHdr => this.ColorButton.ColorHdr;

        public string TextureSelectedItem => this.TextureDialog.SelectedItem;
        public void ConstructTexture(string path) => this.TextureDialog.Construct(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.TextureDialog.ShowInstance();

        public void Construct(IInkParameter item)
        {
            this.PaintScrollViewer.Construct(item);
            this.ColorButton.Construct(item);
        }

        #endregion


        public bool Disabler
        {
            get => App.SourcePageType != SourcePageType.DrawPage;
            set => App.SourcePageType = value ? SourcePageType.Invalid : SourcePageType.DrawPage;
        }


        //@Construct
        public DrawPage()
        {
            this.InitializeComponent();
            this.Construct(this);

            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructSimulate();

            this.ConstructLayers();
            this.ConstructLayer();

            this.ConstructColorPicker();
            this.ConstructNumberPicker();

            this.ConstructAppBar();

            this.ConstructEffect();
            this.ConstructGeometry();

            this.ConstructInk();
            this.ConstructSymmetry();

            this.ConstructSetup();

            this.ConstructDisplacementLiquefaction();
            this.ConstructGradientMapping();
            this.ConstructRippleEffect();
            this.ConstructThreshold();

            this.ConstructRadian();
            this.ConstructScale();
            this.ConstructRemote();

            this.ConstructVector();
            this.ConstructPen();

            this.ConstructMove();
            this.ConstructTransform();
            this.ConstructFreeTransform();


            this.ContentGrid.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                double width = (int)e.NewSize.Width;
                if (width > 1200) this.Device = ContextAppBarDevice.Hub;
                else if (width > 900) this.Device = ContextAppBarDevice.PC;
                else if (width > 600) this.Device = ContextAppBarDevice.Pad;
                else this.Device = ContextAppBarDevice.Phone;
            };


            this.Command.Click += (s, type) => this.Click(type);
            this.LayerListView.ItemClick2 += (s, type) => this.Click(type);
            this.HeadButton.ItemClick += (s, type) => this.Click(type);
            this.LayerListView.Invalidate += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.LayerListView.History += (s, e) =>
            {
                // History
                int removes = this.History.Push(e);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.RaiseHistoryCanExecuteChanged();
            };


            this.ExportButton.Click += (s, e) => this.Click(OptionType.ExportMenu);
            this.HomeButton.Click += (s, e) => this.Click(OptionType.Close);
            //this.SaveButton.Click += (s, e) => this.Click(OptionType.Save);
            this.UndoButton.Click += (s, e) => this.Click(OptionType.Undo);
            this.RedoButton.Click += (s, e) => this.Click(OptionType.Redo);
            this.UnFullScreenButton.Click += (s, e) => this.Click(OptionType.UnFullScreen);
            this.FullScreenButton.Click += (s, e) => this.Click(OptionType.FullScreen);
            //this.KeyButton.Click += (s, e) => this.KeyboardShortcuts.Tip();


            this.SplitLeftButton.Click += (s, e) => this.SplitLeftView.IsPaneOpen = true;
            this.SplitLeftButton.PointerEntered += (s, e) =>
            {
                if (e.Pointer.PointerDeviceType is PointerDeviceType.Touch) return;
                this.SplitLeftView.IsPaneOpen = true;
            };

            this.SplitRightButton.Click += (s, e) => this.SplitRightView.IsPaneOpen = true;
            this.SplitRightButton.PointerEntered += (s, e) =>
            {
                if (e.Pointer.PointerDeviceType is PointerDeviceType.Touch) return;
                this.SplitRightView.IsPaneOpen = true;
            };


            this.Click(this.OptionType);
            this.ToolListView.SelectedType = this.OptionType;
            this.ToolListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is OptionType item)
                {
                    this.Click(item);
                }
            };
            this.EffectListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is OptionType item)
                {
                    this.Click(item);
                }
            };


            this.BrushListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintBrush brush)
                {
                    this.ConstructBrush(brush);
                }
            };
            this.SizeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintSize item)
                {
                    this.ConstructSize((float)item.Size);
                }
            };

            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems) is false) return;

                this.AddAsync(from file in await e.DataView.GetStorageItemsAsync() where file is IStorageFile select file as IStorageFile);
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
            switch (e.NavigationMode)
            {
                case NavigationMode.New: // Go to DrawPage
                    break;
                case NavigationMode.Back:
                    break;
                default:
                    break;
            }

            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.BackRequested -= this.BackRequested;
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        [MainPageToDrawPage(NavigationMode.New | NavigationMode.Back)]
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (e.NavigationMode)
            {
                case NavigationMode.Back: // Come to DrawPage
                    break;
                case NavigationMode.New:
                    // Frist Open: Page.OnNavigatedTo (ReadyToDraw=false) > Canvas.CreateResources (ReadyToDraw=true)
                    // Others Open: Page.OnNavigatedTo (ReadyToDraw=true)
                    if (e.Parameter is ProjectParameter item)
                    {
                        float w = (float)Window.Current.Bounds.Width;
                        float h = (float)Window.Current.Bounds.Height;
                        float cw = this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(w);
                        float ch = this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(h);
                        this.Transformer.ControlWidth = cw;
                        this.Transformer.ControlHeight = ch;

                        this.Transformer.Width = item.Width;
                        this.Transformer.Height = item.Height;
                        this.Transformer.Fit();

                        this.ConstructView(this.Transformer);

                        this.ApplicationView.Title = item.DisplayName;
                        this.ApplicationView.PersistedStateId = item.Path;

                        this.Load(item);

                        if (this.CanvasVirtualControl.ReadyToDraw)
                        {
                            this.CreateResources(item.Width, item.Height);
                            this.CreateMarqueeResources(item.Width, item.Height);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                        }
                    }
                    else
                    {
                        this.ApplicationView.Title = string.Empty;
                        this.ApplicationView.PersistedStateId = string.Empty;

                        if (base.Frame.CanGoBack)
                        {
                            base.Frame.GoBack();
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }

            DisplayInformation display = DisplayInformation.GetForCurrentView();
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                manager.BackRequested += this.BackRequested;
            }

            this.Disabler = false;
        }
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            if (this.Disabler) return;
            this.Click(OptionType.Close);
        }

    }
}