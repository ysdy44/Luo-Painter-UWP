using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter, IInkSlider, ICommand
    {

        //@String
        FlowDirection Direction => CultureInfoCollection.FlowDirection;

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));
        private Symbol BackSymbolConverter(FlowDirection value)
        {
            switch (value)
            {
                case FlowDirection.LeftToRight:
                    return Symbol.Back;
                case FlowDirection.RightToLeft:
                    return Symbol.Forward;
                default:
                    return default;
            }
        }
        private Symbol UndoSymbolConverter(FlowDirection value)
        {
            switch (value)
            {
                case FlowDirection.LeftToRight:
                    return Symbol.Undo;
                case FlowDirection.RightToLeft:
                    return Symbol.Redo;
                default:
                    return default;
            }
        }
        private Symbol RedoSymbolConverter(FlowDirection value)
        {
            switch (value)
            {
                case FlowDirection.LeftToRight:
                    return Symbol.Redo;
                case FlowDirection.RightToLeft:
                    return Symbol.Undo;
                default:
                    return default;
            }
        }

        //@Setting
        private readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        // Range
        readonly InverseProportionRange SizeRange = new InverseProportionRange(12, 1, 400, 100000);
        readonly InverseProportionRange SpacingRange = new InverseProportionRange(25, 10, 400, 1000000);
        readonly InverseProportionRange ScaleRange = new InverseProportionRange(1, 0.1, 10, 100);

        // Canvas
        public CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        readonly Historian<IHistory> History = new Historian<IHistory>();

        readonly LayerRootNodes LayerManager = new LayerRootNodes();
        public LayerNodes Nodes => this.LayerManager;
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.LayerListView.SelectedIndex; set => this.LayerListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.LayerListView.SelectedItem; set => this.LayerListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.LayerListView.SelectedItems;

        // Bitmap
        CanvasRenderTarget GradientMesh { get; set; }
        CanvasBitmap GrayAndWhiteMesh { get; set; }
        CanvasRenderTarget Mesh { get; set; }

        string AddImageId { get; set; } // ID of the target
        CanvasBitmap AddImage { get; set; } // Insert an image above the target
        ImageLayer ImageLayer { get; set; }
        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        BitmapLayer Displacement { get; set; }
        CurveLayer CurveLayer { get; set; }

        OptionType optionType = OptionType.PaintBrush;
        OptionTarget OptionTarget { get; set; }
        SelectionType SelectionType { get; set; } = SelectionType.None;
        OptionType OptionType
        {
            get => this.optionType;
            set
            {
                this.optionType = value;
                switch (value)
                {
                    case OptionType.Marquee: this.OptionTarget = OptionTarget.Marquee; break;
                    case OptionType.MarqueeTransform: this.OptionTarget = OptionTarget.Marquee; break;
                    case OptionType.AddImageTransform: this.OptionTarget = OptionTarget.Image; break;
                    default: this.OptionTarget = value.IsMarquee() ? OptionTarget.Marquee : OptionTarget.BitmapLayer; break;
                }
            }
        }

        bool IsReferenceImageResizing { get; set; }
        ReferenceImage ReferenceImage { get; set; }
        IList<ReferenceImage> ReferenceImages { get; } = new List<ReferenceImage>();

        //@Debug: Auto Save & Load
        string AutoSavePathOfProject;
        readonly Timer AutoSaveTimer = new Timer
        {
            Interval = 1000 * 60 // 1 Minute
        };

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

        // Transform
        TransformMatrix Transform;  // Transform
        TransformBase CreateTransform; // Create a Geometry or Pattern
        TransformMatrix3D FreeTransform; // Free Transform

        TransformBase CropTransform; // Crop
        Vector2 StartingPositionWithoutRadian;
        Vector2 PositionWithoutRadian;

        TransformBase BorderTransform; // Border Effect
        Rect StartingBorderCrop; // Crop
        Rect BorderCrop; // Crop

        // Position
        Vector2 StartingPosition;
        Vector2 Position;

        Vector2 StartingPoint;
        Vector2 Point;

        float StartingPressure;
        float Pressure;

        Vector2 StartingMove;
        Vector2 Move;

        // Ink
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

        // Disabler
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

            this.ConstructMenus();
            this.ConstructMenu();

            this.ConstructPropertys();
            this.ConstructProperty();

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

            //@Debug: Auto Save & Load
            base.Unloaded += delegate { this.AutoSaveTimer.Stop(); };
            base.Loaded += delegate
            {
                this.AutoSaveTimer.Stop();
                this.AutoSaveTimer.Start();
            };
            this.AutoSaveTimer.Elapsed += async delegate
            {
                // 1. Storyboard.Begin()
                await base.Dispatcher.RunAsync(CoreDispatcherPriority.Low, this.AnimationIcon.Begin); // Storyboard

                string pathOfProject = this.AutoSavePathOfProject;
                if (string.IsNullOrEmpty(pathOfProject))
                    return;

                // 2. Save all files to TemporaryFolder
                await this.SaveTemporaryFolderAsync();

                // 3. Save the PathOfProject file
                this.LocalSettings.Values[Project.PathOfProject] = pathOfProject;
            };
            Windows.ApplicationModel.Core.CoreApplication.Exiting += async delegate // Windows 8.0 API
            {
                // 1. Storyboard.Begin()
                //await base.Dispatcher.RunAsync(CoreDispatcherPriority.Low, this.AnimationIcon.Begin); // Storyboard

                string pathOfProject = this.AutoSavePathOfProject;
                if (string.IsNullOrEmpty(pathOfProject))
                    return;

                // 2. Save all files to TemporaryFolder
                await this.SaveTemporaryFolderAsync();

                // 3. Save the PathOfProject file
                this.LocalSettings.Values[Project.PathOfProject] = pathOfProject;
            };
            AppDomain.CurrentDomain.UnhandledException += delegate
            {
                // 1. Storyboard.Begin()
                //await base.Dispatcher.RunAsync(CoreDispatcherPriority.Low, this.AnimationIcon.Begin); // Storyboard

                string pathOfProject = this.AutoSavePathOfProject;
                if (string.IsNullOrEmpty(pathOfProject))
                    return;

                // 2. Save all files to TemporaryFolder
                //await this.SaveTemporaryFolderAsync();

                // 3. Save the PathOfProject file
                this.LocalSettings.Values[Project.PathOfProject] = pathOfProject;
            };

            this.CanvasDevice.DeviceLost += async (s, e) =>
            {
                await new MessageDialog
                (
                    "GPU device is lost, save the project and restart the application.",
                    "GPU device is lost"
                ).ShowAsync();
            };

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

            this.SplitLeftView.PaneClosing += (s, e) => this.TryHideAppBar();
            this.SplitLeftView.PaneClosed += (s, e) => this.TryShowAppBar();

            this.SplitLeftView.PaneOpening += (s, e) => this.TryHideAppBar();
            this.SplitLeftView.PaneOpened += (s, e) => this.TryShowAppBar();

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
                if (e.ClickedItem is PaintBrush brush)
                {
                    this.ConstructBrush(brush);

                    if (this.OptionType.IsPaint()) return;
                    this.ToastTip.Tip(TipType.NoPaintTool.GetString(), TipType.NoPaintTool.GetString(true));

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
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    foreach (IStorageItem item in items)
                    {
                        if (item is IStorageFile file)
                        {
                            /**
                             * Copy from <see cref="OptionType.AddImage"/> in DrawPage.Click.cs
                             */
                            CanvasBitmap bitmap = await this.CreateBitmap(file);
                            if (bitmap is null)
                            {
                                await TipType.NoSupport.ToDialog(item.Path).ShowAsync();
                                continue;
                            }

                            this.AddImage = bitmap;
                            this.AddImageId = this.LayerSelectedItem is ILayer layer ? layer.Id : null;

                            this.ResetTransform(bitmap);

                            this.OptionType = OptionType.AddImageTransform;
                            this.ConstructAppBar(OptionType.AddImageTransform);

                            this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.CanvasControl.Invalidate(); // Invalidate
                            break;
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

        private void TryHideAppBar()
        {
            if (App.UISettings.AnimationsEnabled)
            {
                switch (this.SplitLeftView.DisplayMode)
                {
                    case SplitViewDisplayMode.Inline:
                        this.HideStoryboard.Begin(); // Storyboard
                        break;
                    default:
                        break;
                }
            }
        }

        private void TryShowAppBar()
        {
            if (this.AppBarGrid.Opacity < 1.0)
            {
                this.ShowStoryboard.Begin(); // Storyboard
            }
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
        [SourceMainToDraw(NavigationMode.New | NavigationMode.Back)]
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
                        this.AutoSavePathOfProject = item.Path;

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
                        this.AutoSavePathOfProject = string.Empty;

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

            if (this.LocalSettings.Values["Touch"] is int touch)
            {
                InkTouchMode mode = (InkTouchMode)touch;
                switch (mode)
                {
                    case InkTouchMode.Draw: this.Operator.TouchMode = TouchMode.SingleFinger; break;
                    case InkTouchMode.Move: this.Operator.TouchMode = TouchMode.RightButton; break;
                    case InkTouchMode.Disable: this.Operator.TouchMode = TouchMode.Disable; break;
                    default: this.Operator.TouchMode = TouchMode.SingleFinger; break;
                }
            }
            else
            {
                this.Operator.TouchMode = TouchMode.SingleFinger;
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