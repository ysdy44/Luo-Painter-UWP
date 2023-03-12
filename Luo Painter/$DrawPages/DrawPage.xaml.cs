using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
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
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter, IInkSlider, ICommand
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));


        readonly InverseProportionRange SizeRange = new InverseProportionRange(12, 1, 400, 100000);
        readonly InverseProportionRange SpacingRange = new InverseProportionRange(25, 10, 400, 1000000);
        readonly InverseProportionRange ScaleRange = new InverseProportionRange(1, 0.1, 10, 100);


        public CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        readonly Historian<IHistory> History = new Historian<IHistory>();

        readonly LayerRootNodes LayerManager = new LayerRootNodes();
        public LayerNodes Nodes => this.LayerManager;
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.LayerListView.SelectedIndex; set => this.LayerListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.LayerListView.SelectedItem; set => this.LayerListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.LayerListView.SelectedItems;


        CanvasRenderTarget GradientMesh { get; set; }
        CanvasBitmap GrayAndWhiteMesh { get; set; }
        CanvasRenderTarget Mesh { get; set; }

        ImageLayer ImageLayer { get; set; }
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
        readonly TaskCollection Tasks = new TaskCollection();

        //@Task
        readonly object InkLocker = new object();
        //@Ink
        public bool IsInkEnabled { get; set; } = true;
        CanvasRenderTarget InkRender { get; set; }

        SymmetryType SymmetryType;
        readonly Symmetryer Symmetryer = new Symmetryer();


        Transform Transform;
        Transform CreateTransform;
        FreeTransform FreeTransform;

        TransformBase CropTransform;
        Vector2 StartingPositionWithoutRadian;
        Vector2 PositionWithoutRadian;

        TransformBase BorderTransform;
        Rect StartingBorderCrop;
        Rect BorderCrop;


        Vector2 StartingPosition;
        Vector2 Position;

        Vector2 StartingPoint;
        Vector2 Point;

        float StartingPressure;
        float Pressure;

        Vector2 StartingMove;
        Vector2 Move;


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
        }
        public void TryInkAsync()
        {
            if (this.InkCanvasControl.ReadyToDraw is false) return;

            Task.Run(this.InkAsync);
        }
        public void TryInk()
        {
            if (this.InkCanvasControl.ReadyToDraw is false) return;

            lock (this.InkLocker) this.Ink();
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
            this.ConstructDraw();

            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructSimulate();

            this.ConstructLayers();
            this.ConstructLayer();

            this.ConstructColorPicker();
            this.ConstructNumberPicker();

            this.ConstructAppBar();

            this.ConstructEffect();
            this.ConstructGammaTransfer();
            this.ConstructDiscreteTransfer();
            this.ConstructLighting();

            this.ConstructSelection();
            this.ConstructGeometry();
            this.ConstructPattern();

            this.ConstructInk();
            this.ConstructSymmetry();

            this.ConstructSetup();

            this.ConstructDisplacementLiquefaction();
            this.ConstructGradientMapping();
            this.ConstructRippleEffect();
            this.ConstructThreshold();
            this.ConstructHSB();

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

                int width = (int)e.NewSize.Width;
                if (width > 1200) this.Device = ContextAppBarDevice.Hub;
                else if (width > 900) this.Device = ContextAppBarDevice.PC;
                else if (width > 600) this.Device = ContextAppBarDevice.Pad;
                else this.Device = ContextAppBarDevice.Phone;
            };


            this.PaintScrollViewer.ScratchpadClick += (s, e) => this.TryShowBrushPage();

            this.LayerListView.Invalidate += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.LayerListView.History += (s, e) =>
            {
                // History
                int removes = this.History.Push(e);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.RaiseHistoryCanExecuteChanged();
            };

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
                this.EffectFlyout.Hide();
                {
                    this.Click(e);
                }
            };


            this.BrushGridView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is InkAttributes brush)
                {
                    this.ConstructBrush(brush);

                    if (this.OptionType.IsPaint()) return;
                    this.Tip(TipType.NoPaintTool);

                    this.ToolListView.SelectedType = OptionType.PaintBrush;
                    this.Click(OptionType.PaintBrush);
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
                            this.Clear();
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
            this.Click(OptionType.Home);
        }

    }
}