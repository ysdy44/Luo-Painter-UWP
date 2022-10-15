using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Menus;
using Luo_Painter.Options;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using Luo_Painter.Shaders;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);
        private bool IsAlt => this.IsKeyDown(VirtualKey.Menu);
        private bool IsSpace => this.IsKeyDown(VirtualKey.Space);

        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        public CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        Historian<IHistory> History { get; } = new Historian<IHistory>();

        public LayerRootNodes LayerManager = new LayerRootNodes();
        public LayerNodes Nodes => this.LayerManager;
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.LayerListView.SelectedIndex; set => this.LayerListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.LayerListView.SelectedItem; set => this.LayerListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.LayerListView.SelectedItems;

        InkMixer InkMixer { get; set; } = new InkMixer();
        public InkPresenter InkPresenter { get; } = new InkPresenter();

        GradientMesh GradientMesh { get; set; }
        CanvasBitmap GrayAndWhiteMesh { get; set; }

        //@Task
        readonly object Locker = new object();
        BitmapLayer Mesh { get; set; }
        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        BitmapLayer Displacement { get; set; }
        CurveLayer CurveLayer { get; set; }

        bool IsFullScreen { get; set; }
        SelectionType SelectionType { get; set; } = SelectionType.None;
        OptionType OptionType { get; set; } = OptionType.PaintBrush;
        public InkType InkType { get; set; } = InkType.None;

        bool IsReferenceImageResizing { get; set; }
        ReferenceImage ReferenceImage { get; set; }
        IList<ReferenceImage> ReferenceImages { get; } = new List<ReferenceImage>();

        Vector2 StartingPosition;
        Vector2 Position;
        Vector2 StartingPoint;
        Vector2 Point;
        float StartingPressure;
        float Pressure;

        Color Color => this.ColorMenu.Color;
        Vector4 ColorHdr => this.ColorMenu.ColorHdr;

        Transformer StartingBoundsTransformer;
        Transformer BoundsTransformer;
        TransformerMode BoundsMode;
        bool IsBoundsMove;
        Matrix3x2 BoundsMatrix;


        //@Construct
        public DrawPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructSimulate();

            this.ConstructLayers();
            this.ConstructLayer();

            this.ConstructFoots();
            this.ConstructFoot();

            this.ConstructBrush();
            this.ConstructInk();

            this.ConstructSetup();

            this.ConstructGradientMapping();
            this.ConstructVector();

            this.ConstructPen();


            this.Command.Click += (s, type) => this.Click(type);

            this.AddMenu.ItemClick += (s, type) => this.Click(type);
            this.LayerMenu.ItemClick += (s, type) => this.Click(type);
            this.EditMenu.ItemClick += (s, type) => this.Click(type);
            this.AdjustmentMenu.ItemClick += (s, type) => this.Click(type);
            this.OtherMenu.ItemClick += (s, type) => this.Click(type);
            this.ToolListView.ItemClick += (s, type) => this.Click(type);
            this.ToolListView.Construct(this.OptionType);


            this.LightDismissOverlay.Tapped += (s, e) => this.ExpanderLightDismissOverlay.Hide();
            this.ExpanderLightDismissOverlay.IsFlyoutChanged += (s, isFlyout) => this.LightDismissOverlay.Visibility = isFlyout ? Visibility.Visible : Visibility.Collapsed;


            this.ExportButton.Click += (s, e) => this.Click(OptionType.ExportMenu);

            this.ToolButton.Click += (s, e) => this.Click(OptionType.ToolMenu);
            this.HistoryButton.Click += (s, e) => this.Click(OptionType.HistoryMenu);
            this.ColorButton.Click += (s, e) => this.Click(OptionType.ColorMenu);

            this.EditButton.Click += (s, e) => this.Click(OptionType.EditMenu);
            this.AdjustmentButton.Click += (s, e) => this.Click(OptionType.AdjustmentMenu);
            this.OtherButton.Click += (s, e) => this.Click(OptionType.OtherMenu);

            this.PaintButton.Click += (s, e) => this.Click(OptionType.PaintMenu);
            this.BrushButton.Click += (s, e) => this.Click(OptionType.BrushMenu);
            this.SizeButton.Click += (s, e) => this.Click(OptionType.SizeMenu);

            this.LayerListView.Add += (s, e) => this.Click(OptionType.AddMenu);
            this.LayerListView.Remove += (s, e) => this.Click(OptionType.Remove);
            this.LayerListView.Opening += (s, e) => this.Click(OptionType.LayerMenu);


            this.HomeButton.Click += (s, e) => this.Click(OptionType.Close);
            this.SaveButton.Click += (s, e) => this.Click(OptionType.Save);

            this.UndoButton.Click += (s, e) => this.Click(OptionType.Undo);
            this.RedoButton.Click += (s, e) => this.Click(OptionType.Redo);

            this.UnFullScreenButton.Click += (s, e) => this.Click(OptionType.UnFullScreen);
            this.FullScreenButton.Click += (s, e) => this.Click(OptionType.FullScreen);

            this.KeyButton.Click += (s, e) => this.KeyboardShortcuts.Tip();
            this.KeyboardShortcuts.ItemsSource = from c in base.KeyboardAccelerators where c.Key != default select new Controls.KeyboardShortcut(c);


            this.ExportMenu.ExportClick += (s, e) =>
            {
                switch (this.ExportMenu.Mode)
                {
                    case ExportMode.None: this.Click(OptionType.Export); break;
                    case ExportMode.All: this.Click(OptionType.ExportAll); break;
                    case ExportMode.Current: this.Click(OptionType.ExportCurrent); break;
                    default: break;
                }
            };

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
                case NavigationMode.New:
                    break;
                case NavigationMode.Back:
                    break;
                default:
                    break;
            }

            base.IsEnabled = false;

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
                case NavigationMode.Back:
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

                        this.ViewTool.Construct(this.Transformer);

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
                            base.IsEnabled = false;
                            base.Frame.GoBack();
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }

            base.IsEnabled = true;

            DisplayInformation display = DisplayInformation.GetForCurrentView();
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                manager.BackRequested += this.BackRequested;
            }
        }
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            this.Click(OptionType.Close);
        }

    }
}