using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        //@Delegate
        public event EventHandler CanExecuteChanged;

        //@Command
        public bool CanExecute(object parameter) => parameter != default;
        public void Execute(object parameter)
        {
            if (parameter is OptionType item)
            {
                this.Click(item);
            }
        }

        public ICommand Command => this;
        public async void Click(OptionType type)
        {
            if (this.LayerListView.IsOpen) return;

            switch (type)
            {
                // None
                case OptionType.None: break;

                // Flag
                case OptionType.IsItemClickEnabled: break;
                case OptionType.ExistIcon: break;
                case OptionType.ExistThumbnail: break;
                case OptionType.HasMenu: break;
                case OptionType.HasPreview: break;
                case OptionType.HasDifference: break;
                case OptionType.WithState: break;
                case OptionType.WithTransform: break;

                // Root
                case OptionType.App: break;
                case OptionType.Option: break;
                case OptionType.Layer: break;
                case OptionType.Effect: break;
                case OptionType.Tool: break;

                #region App

                // Category
                case OptionType.File: break;
                case OptionType.Layout: break;
                case OptionType.Format: break;
                case OptionType.Menu: break;

                // File
                case OptionType.Home:
                case OptionType.Close:
                    if (this.ApplicationView.IsFullScreenMode)
                    {
                        this.Click(OptionType.UnFullScreen);
                        break;
                    }

                    if (this.Disabler) break;
                    this.Disabler = true;

                    //@Debug
                    // OptionType becomes Tool when it is Effect
                    this.OptionType = this.ToolListView.SelectedType;
                    this.ConstructAppBar(this.ToolListView.SelectedType);

                    if (type is OptionType.Home)
                    {
                        await this.SaveAsync(this.ApplicationView.PersistedStateId);
                    }

                    if (base.Frame.CanGoBack)
                    {
                        this.Clear();
                        base.Frame.GoBack();
                        break;
                    }

                    this.Disabler = false;
                    break;
                case OptionType.Save:
                    if (this.Disabler) break;
                    this.Disabler = true;

                    this.AnimationIcon.Begin(); // Storyboard
                    await this.SaveAsync(this.ApplicationView.PersistedStateId);

                    this.Disabler = false;
                    break;

                case OptionType.Export:
                    {
                        if (this.Nodes.Count() is 0)
                        {
                            this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                            break;
                        }

                        StorageFile file = await FileUtil.PickSingleFileAsync(PickerLocationId.Desktop, this.ExportDialog.FileChoices, this.ApplicationView.Title);
                        if (file is null) break;
                        this.ToastTip.Tip(TipType.Saving.GetString(), this.ApplicationView.Title);

                        float dpi = this.ExportDialog.DPI;
                        Rect rect = new Rect
                        {
                            Width = dpi.ConvertPixelsToDips(this.Transformer.Width),
                            Height = dpi.ConvertPixelsToDips(this.Transformer.Height)
                        };

                        // Export
                        bool result;

                        using (ICanvasImage background = (this.ExportDialog.FileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
                        {
                            Color = Colors.White
                        } : (ICanvasImage)new CanvasCommandList(this.CanvasDevice))
                        {
                            ICanvasImage image = this.Nodes.Render(background);

                            try
                            {
                                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                {
                                    await CanvasImage.SaveAsync(image, rect, dpi, this.CanvasDevice, stream, this.ExportDialog.FileFormat, 1);
                                }
                                result = true;
                            }
                            catch (Exception)
                            {
                                result = false;
                            }
                        }

                        if (result is false)
                        {
                            this.ToastTip.Tip(TipType.SaveFailed.GetString(), TipType.SaveFailed.GetString(true));
                            break;
                        }

                        if (result)
                            ToastExtensions.Show(file);
                        else
                            this.ToastTip.Tip(TipType.SaveFailed.GetString(), TipType.SaveFailed.GetString(true));
                    }
                    break;
                case OptionType.ExportAll:
                    {
                        if (this.Nodes.Count() is 0)
                        {
                            this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                            break;
                        }

                        IStorageFolder folder = await FileUtil.PickSingleFolderAsync(PickerLocationId.Desktop);
                        if (folder is null) break;
                        this.ToastTip.Tip(TipType.Saving.GetString(), this.ApplicationView.Title);

                        float dpi = this.ExportDialog.DPI;
                        Rect rect = new Rect
                        {
                            Width = dpi.ConvertPixelsToDips(this.Transformer.Width),
                            Height = dpi.ConvertPixelsToDips(this.Transformer.Height)
                        };

                        // Export
                        bool result;
                        int index = 0;
                        int count = 0;
                        FolderLauncherOptions options = new FolderLauncherOptions();

                        using (ICanvasImage background = (this.ExportDialog.FileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
                        {
                            Color = Colors.White
                        } : (ICanvasImage)new CanvasCommandList(this.CanvasDevice))
                        {
                            foreach (ILayerRender item in this.Nodes)
                            {
                                string name = $"{this.ApplicationView.Title} {index}{this.ExportDialog.FileChoices}";
                                StorageFile file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                                index++;

                                if (file is null) continue;
                                ICanvasImage image = item.Render(background);

                                try
                                {
                                    using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                    {
                                        await CanvasImage.SaveAsync(image, rect, dpi, this.CanvasDevice, accessStream, this.ExportDialog.FileFormat, 1);
                                    }
                                    result = true;
                                    count++;
                                }
                                catch (Exception)
                                {
                                    result = false;
                                }

                                if (result)
                                {
                                    this.ToastTip.Tip(TipType.SaveSuccess.GetString(), this.ApplicationView.Title);
                                    options.ItemsToSelect.Add(file);
                                }
                                else
                                    this.ToastTip.Tip(TipType.SaveFailed.GetString(), TipType.SaveFailed.GetString(true));
                            }
                        }

                        if (options.ItemsToSelect.Count is 0)
                            await Launcher.LaunchFolderAsync(folder);
                        else
                            await Launcher.LaunchFolderAsync(folder, options);
                    }
                    break;
                case OptionType.ExportCurrent:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            StorageFile file = await FileUtil.PickSingleFileAsync(PickerLocationId.Desktop, this.ExportDialog.FileChoices, this.ApplicationView.Title);
                            if (file is null) break;
                            this.ToastTip.Tip(TipType.Saving.GetString(), this.ApplicationView.Title);

                            float dpi = this.ExportDialog.DPI;
                            Rect rect = new Rect
                            {
                                Width = dpi.ConvertPixelsToDips(this.Transformer.Width),
                                Height = dpi.ConvertPixelsToDips(this.Transformer.Height)
                            };

                            // Export
                            bool result;

                            using (ICanvasImage background = (this.ExportDialog.FileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
                            {
                                Color = Colors.White
                            } : (ICanvasImage)new CanvasCommandList(this.CanvasDevice))
                            {
                                ICanvasImage image = layer.Render(background);

                                try
                                {
                                    using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                    {
                                        await CanvasImage.SaveAsync(image, rect, dpi, this.CanvasDevice, accessStream, this.ExportDialog.FileFormat, 1);
                                    }
                                    result = true;
                                }
                                catch (Exception)
                                {
                                    result = false;
                                }
                            }

                            if (result)
                                ToastExtensions.Show(file);
                            else
                                this.ToastTip.Tip(TipType.SaveFailed.GetString(), TipType.SaveFailed.GetString(true));
                        }
                        else
                            this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;

                case OptionType.Undo:
                    if (this.History.CanUndo)
                    {
                        // History
                        bool result = this.History.Undo(this.Undo);
                        if (result is false) break;

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                        this.ToastTip.Tip(TipType.Undo.GetString(), $"{this.History.Index} / {this.History.Count}");
                    }
                    break;
                case OptionType.Redo:
                    if (this.History.CanRedo)
                    {
                        // History
                        bool result = this.History.Redo(this.Redo);
                        if (result is false) break;

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                        this.ToastTip.Tip(TipType.Redo.GetString(), $"{this.History.Index} / {this.History.Count}");
                    }
                    break;

                // Layout
                case OptionType.FullScreen:
                    if (this.ApplicationView.IsFullScreenMode)
                    {
                        this.Click(OptionType.UnFullScreen);
                        break;
                    }

                    VisualStateManager.GoToState(this, nameof(FullScreen), false);
                    this.ApplicationView.TryEnterFullScreenMode();
                    break;
                case OptionType.UnFullScreen:
                    VisualStateManager.GoToState(this, nameof(UnFullScreen), false);
                    this.ApplicationView.ExitFullScreenMode();
                    break;

                case OptionType.DockLeft:
                    this.SplitLeftView.IsPaneOpen = !this.SplitLeftView.IsPaneOpen;
                    break;
                case OptionType.DockRight:
                    this.SplitRightView.IsPaneOpen = !this.SplitRightView.IsPaneOpen;
                    break;

                // Format
                case OptionType.JPEG:
                    this.ExportDialog.FileFormat = CanvasBitmapFileFormat.Jpeg;
                    this.Click(OptionType.ExportMenu);
                    break;
                case OptionType.PNG:
                    this.ExportDialog.FileFormat = CanvasBitmapFileFormat.Png;
                    this.Click(OptionType.ExportMenu);
                    break;
                case OptionType.BMP:
                    this.ExportDialog.FileFormat = CanvasBitmapFileFormat.Bmp;
                    this.Click(OptionType.ExportMenu);
                    break;
                case OptionType.GIF:
                    this.ExportDialog.FileFormat = CanvasBitmapFileFormat.Gif;
                    this.Click(OptionType.ExportMenu);
                    break;
                case OptionType.TIFF:
                    this.ExportDialog.FileFormat = CanvasBitmapFileFormat.Tiff;
                    this.Click(OptionType.ExportMenu);
                    break;

                // Menu
                case OptionType.FileMenu: break;
                case OptionType.ExportMenu:
                    if (ContentDialogExtensions.CanShow)
                    {
                        ContentDialogResult result = await this.ExportDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                switch (this.ExportDialog.Mode)
                                {
                                    case ExportMode.None:
                                        this.Click(OptionType.Export);
                                        break;
                                    case ExportMode.All:
                                        this.Click(OptionType.ExportAll);
                                        break;
                                    case ExportMode.Current:
                                        this.Click(OptionType.ExportCurrent);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case OptionType.HistogramMenu:
                    if (CanvasImage.IsHistogramSupported(this.CanvasDevice) is false)
                    {
                        await TipType.NoCompatible.ToDialog().ShowAsync();
                        break;
                    }

                    this.Histogram();
                    this.HistogramCanvasControl.Invalidate();
                    await this.HistogramDialog.ShowInstance();
                    break;

                case OptionType.PaintMenu:
                    this.PaintScrollViewer.Toggle();
                    break;
                case OptionType.BrushMenu: break;
                case OptionType.SizeMenu: break;
                case OptionType.EffectMenu: break;
                case OptionType.HistoryMenu: break;

                case OptionType.ToolMenu: break;
                case OptionType.LayerMenu: break;

                case OptionType.LayerPropertyMenu: break;
                case OptionType.LayerRenameMenu:
                    if (this.SplitLeftView.IsPaneOpen)
                        this.LayerListView.Rename();
                    break;

                #endregion

                #region Option

                // Category
                case OptionType.Edit: break;
                case OptionType.Select: break;
                case OptionType.Marquees: break;

                case OptionType.CropCanvas:
                    {
                        int width = this.Transformer.Width;
                        int height = this.Transformer.Height;

                        this.SetCropCanvas(width, height);
                    }

                    this.BitmapLayer = null;
                    this.OptionType = OptionType.CropCanvas;
                    this.ConstructAppBar(OptionType.CropCanvas);

                    this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case OptionType.ResizeCanvas: break;
                case OptionType.RotateCanvas: break;

                // Edit
                case OptionType.Cut: // Copy + Clear
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    {
                                        this.Clipboard.DrawCopy(bitmapLayer.GetMask(this.Marquee));

                                        // History
                                        IHistory history = bitmapLayer.Clear(this.Marquee, interpolationColors);
                                        history.Title = type.GetString();
                                        int removes = this.History.Push(history);

                                        bitmapLayer.Flush();
                                        bitmapLayer.RenderThumbnail();
                                    }
                                    break;
                                default:
                                    {
                                        this.Clipboard.CopyPixels(bitmapLayer);

                                        // History
                                        IHistory history = bitmapLayer.GetBitmapClearHistory(Colors.Transparent);
                                        history.Title = type.GetString();
                                        int removes = this.History.Push(history);

                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                                        bitmapLayer.ClearThumbnail(Colors.Transparent);
                                    }
                                    break;
                            }

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                            this.RaiseEditCanExecuteChanged();
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.Copy:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    this.Clipboard.DrawCopy(bitmapLayer.GetMask(this.Marquee));
                                    break;
                                default:
                                    this.Clipboard.CopyPixels(bitmapLayer);
                                    break;
                            }

                            this.RaiseEditCanExecuteChanged();
                        }
                    }
                    break;
                case OptionType.Paste:
                    {
                        Color[] interpolationColors = this.Clipboard.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Clipboard.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                            break;
                        }

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, this.Clipboard[BitmapType.Source], this.Transformer.Width, this.Transformer.Height);

                        // History
                        IHistory history = this.LayerManager.Add(this, add);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                case OptionType.Clear:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    {
                                        // History
                                        IHistory history = bitmapLayer.Clear(this.Marquee, interpolationColors);
                                        history.Title = type.GetString();
                                        int removes = this.History.Push(history);

                                        bitmapLayer.Flush();
                                        bitmapLayer.RenderThumbnail();
                                    }
                                    break;
                                default:
                                    {
                                        // History
                                        IHistory history = bitmapLayer.GetBitmapClearHistory(Colors.Transparent);
                                        history.Title = type.GetString();
                                        int removes = this.History.Push(history);

                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                                        bitmapLayer.ClearThumbnail(Colors.Transparent);
                                    }
                                    break;
                            }

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                // Select
                case OptionType.All:
                    {
                        // History
                        IHistory history = this.Marquee.GetBitmapClearHistory(Colors.DodgerBlue);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Origin);
                        this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Source);
                        this.Marquee.ClearThumbnail(Colors.DodgerBlue);

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.Deselect:
                    {
                        // History
                        IHistory history = this.Marquee.GetBitmapClearHistory(Colors.Transparent);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.Marquee.Clear(Colors.Transparent, BitmapType.Origin);
                        this.Marquee.Clear(Colors.Transparent, BitmapType.Source);
                        this.Marquee.ClearThumbnail(Colors.Transparent);

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.MarqueeInvert:
                    {
                        // History
                        IHistory history = this.Marquee.Invert(Colors.DodgerBlue);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.Marquee.Flush();
                        this.Marquee.RenderThumbnail();

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.Pixel:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            // History
                            IHistory history = this.Marquee.Pixel(bitmapLayer, Colors.DodgerBlue);
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);

                            this.Marquee.Flush();
                            this.Marquee.RenderThumbnail();

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                // Marquees
                case OptionType.Feather:
                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.ToastTip.Tip(TipType.NoPixelForMarquee.GetString(), TipType.NoPixelForMarquee.GetString(true));
                            break;
                        }

                        this.OptionType = OptionType.Feather;
                        this.ConstructAppBar(OptionType.Feather);

                        this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;
                case OptionType.MarqueeTransform:
                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.ToastTip.Tip(TipType.NoPixelForMarquee.GetString(), TipType.NoPixelForMarquee.GetString(true));
                            break;
                        }

                        switch (mode)
                        {
                            case PixelBoundsMode.None:
                                PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(interpolationColors);
                                PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, interpolationColors);
                                this.ResetTransform(bounds);
                                break;
                            default:
                                this.ResetTransform(this.Marquee.Bounds);
                                break;
                        }

                        this.OptionType = OptionType.MarqueeTransform;
                        this.ConstructAppBar(OptionType.MarqueeTransform);

                        this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;
                case OptionType.Grow:
                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.ToastTip.Tip(TipType.NoPixelForMarquee.GetString(), TipType.NoPixelForMarquee.GetString(true));
                            break;
                        }

                        this.OptionType = OptionType.Grow;
                        this.ConstructAppBar(OptionType.Grow);

                        this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;
                case OptionType.Shrink:
                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.ToastTip.Tip(TipType.NoPixelForMarquee.GetString(), TipType.NoPixelForMarquee.GetString(true));
                            break;
                        }

                        this.OptionType = OptionType.Shrink;
                        this.ConstructAppBar(OptionType.Shrink);

                        this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;

                // ResizeCanvas
                case OptionType.Stretch:
                    if (ContentDialogExtensions.CanShow)
                    {
                        this.StretchDialog.Resezing(this.Transformer.Width, this.Transformer.Height);
                        ContentDialogResult result = await this.StretchDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                int width = this.Transformer.Width;
                                int height = this.Transformer.Height;

                                int w = this.StretchDialog.Size.Width;
                                int h = this.StretchDialog.Size.Height;
                                if (w == width && h == height) break;

                                CanvasImageInterpolation interpolation = this.StretchDialog.Interpolation;
                                {
                                    this.Transformer.Width = w;
                                    this.Transformer.Height = h;
                                    this.Transformer.ReloadMatrix();

                                    this.CreateResources(w, h);
                                    this.CreateMarqueeResources(w, h);
                                }

                                // History
                                IHistory history = new CompositeHistory(
                                    this.LayerManager.Setup(this, this.Nodes.Select(c => c.Skretch(this.CanvasDevice, w, h, interpolation)).ToArray()),
                                    new SetupHistory(width, height, w, h)
                                );
                                history.Title = type.GetString();
                                int removes = this.History.Push(history);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                this.RaiseHistoryCanExecuteChanged();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case OptionType.Extend:
                    if (ContentDialogExtensions.CanShow)
                    {
                        this.ExtendDialog.Resezing(this.Transformer.Width, this.Transformer.Height);
                        ContentDialogResult result = await this.ExtendDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                int width = this.Transformer.Width;
                                int height = this.Transformer.Height;

                                int w = this.ExtendDialog.Size.Width;
                                int h = this.ExtendDialog.Size.Height;
                                if (w == width && h == height) break;

                                IndicatorMode indicator = this.ExtendDialog.Indicator;

                                Vector2 vect = this.Transformer.GetIndicatorVector(indicator);
                                {
                                    this.Transformer.Width = w;
                                    this.Transformer.Height = h;
                                    this.Transformer.ReloadMatrix();

                                    this.CreateResources(w, h);
                                    this.CreateMarqueeResources(w, h);
                                }
                                Vector2 vect2 = this.Transformer.GetIndicatorVector(indicator);

                                Vector2 offset = vect2 - vect;

                                // History
                                IHistory history = new CompositeHistory(
                                    this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, offset)).ToArray()),
                                    new SetupHistory(width, height, w, h)
                                );
                                history.Title = type.GetString();
                                int removes = this.History.Push(history);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                this.RaiseHistoryCanExecuteChanged();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case OptionType.Offset:
                    if (ContentDialogExtensions.CanShow)
                    {
                        this.OffsetDialog.Resezing(Vector2.Zero);
                        ContentDialogResult result = await this.OffsetDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                Vector2 offset = this.OffsetDialog.Offset;
                                if (offset == Vector2.Zero) break;

                                // History
                                IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Offset(this.CanvasDevice, offset)).ToArray());
                                history.Title = type.GetString();
                                int removes = this.History.Push(history);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                this.RaiseHistoryCanExecuteChanged();
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                // RotateCanvas
                case OptionType.FlipHorizontal:
                    {
                        // History
                        IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Flip(this.CanvasDevice, BitmapFlip.Horizontal)).ToArray());
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.FlipVertical:
                    {
                        // History
                        IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Flip(this.CanvasDevice, BitmapFlip.Vertical)).ToArray());
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                case OptionType.LeftTurn:
                    {
                        int width = this.Transformer.Width;
                        int height = this.Transformer.Height;

                        int w = height;
                        int h = width;

                        this.Transformer.Width = w;
                        this.Transformer.Height = h;
                        this.Transformer.ReloadMatrix();

                        this.CreateResources(w, h);
                        this.CreateMarqueeResources(w, h);

                        // History
                        if (width == height)
                        {
                            IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise270Degrees)).ToArray());
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);
                        }
                        else
                        {
                            IHistory history = new CompositeHistory(
                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise270Degrees)).ToArray()),
                                new SetupHistory(width, height, height, width)
                            );
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);
                        }
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.RightTurn:
                    {
                        int width = this.Transformer.Width;
                        int height = this.Transformer.Height;

                        int w = height;
                        int h = width;

                        this.Transformer.Width = w;
                        this.Transformer.Height = h;
                        this.Transformer.ReloadMatrix();

                        this.CreateResources(w, h);
                        this.CreateMarqueeResources(w, h);

                        // History
                        if (width == height)
                        {
                            IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise90Degrees)).ToArray());
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);
                        }
                        else
                        {
                            IHistory history = new CompositeHistory(
                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise90Degrees)).ToArray()),
                                new SetupHistory(width, height, height, width)
                            );
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);
                        }
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.OverTurn:
                    {
                        // History
                        IHistory history = this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise180Degrees)).ToArray());
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                #endregion

                #region Layer

                // Category
                case OptionType.New: break;
                case OptionType.Clipboard: break;
                case OptionType.Layering: break;
                case OptionType.Grouping: break;
                case OptionType.Combine: break;

                // New
                case OptionType.AddBitmapLayer:
                    {
                        ILayer add = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                        // History
                        IHistory history = this.LayerManager.Add(this, add);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.AddImageLayer:
                    this.AddAsync(await FileUtil.PickMultipleImageFilesAsync(Windows.Storage.Pickers.PickerLocationId.Desktop));
                    break;
                case OptionType.AddCurveLayer:
                    {
                        ILayer add = new CurveLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                        // History
                        IHistory history = this.LayerManager.Add(this, add);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;
                case OptionType.AddFillLayer:
                    {
                        ILayer add = new FillLayer(this.CanvasDevice, this.Color, this.Transformer.Width, this.Transformer.Height);

                        // History
                        IHistory history = this.LayerManager.Add(this, add);
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                // Clipboard
                case OptionType.CutLayer:
                    {
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                {
                                    // History
                                    IHistory history = this.LayerManager.Cut(this, layer);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                    this.RaiseLayerCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    // History
                                    IHistory history = this.LayerManager.Cut(this, items);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                    this.RaiseLayerCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.CopyLayer:
                    {
                        // History
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                    this.LayerManager.Copy(this, layer);
                                this.RaiseLayerCanExecuteChanged();
                                break;
                            default:
                                this.LayerManager.Copy(this, items);
                                this.RaiseLayerCanExecuteChanged();
                                break;
                        }
                    }
                    break;
                case OptionType.PasteLayer:
                    {
                        switch (this.ClipboardLayers.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                string id = this.ClipboardLayers.Single();
                                if (LayerDictionary.Instance.ContainsKey(id))
                                {
                                    // History
                                    IHistory history = this.LayerManager.Paste(this, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, id);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    // History
                                    IHistory history = this.LayerManager.Paste(this, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, this.ClipboardLayers);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;

                // Layering
                case OptionType.Remove:
                    {
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                {
                                    // History
                                    IHistory history = this.LayerManager.Remove(this, layer, true);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    // History
                                    IHistory history = this.LayerManager.Remove(this, items);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.Duplicate: // CopyLayer + PasteLayer
                    this.Click(OptionType.CopyLayer);
                    this.Click(OptionType.PasteLayer);
                    break;

                case OptionType.Extract:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            if (mode is PixelBoundsMode.Transarent)
                            {
                                this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                break;
                            }

                            BitmapLayer add;
                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    add = new BitmapLayer(this.CanvasDevice, bitmapLayer.GetMask(this.Marquee), this.Transformer.Width, this.Transformer.Height);
                                    break;
                                default:
                                    add = new BitmapLayer(this.CanvasDevice, bitmapLayer);
                                    break;
                            }

                            // History
                            IHistory history = this.LayerManager.Add(this, add);
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;
                case OptionType.Merge:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            switch (layer.Type)
                            {
                                case LayerType.Bitmap:
                                    if (layer is BitmapLayer bitmapLayer)
                                    {
                                        if (this.ObservableCollection.GetNeighbor(bitmapLayer) is ILayer neighbor)
                                        {
                                            // History
                                            bitmapLayer.Merge(neighbor);
                                            IHistory history = new CompositeHistory(
                                                bitmapLayer.GetBitmapResetHistory(),
                                                this.LayerManager.Remove(this, neighbor, false)
                                            );
                                            history.Title = type.GetString();
                                            int removes = this.History.Push(history);

                                            bitmapLayer.Flush();
                                            bitmapLayer.RenderThumbnail();

                                            this.RaiseHistoryCanExecuteChanged();
                                        }
                                    }
                                    break;
                                default:
                                    this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                                    break;
                            }
                        }
                        else
                        {
                            this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                        }
                    }
                    break;
                case OptionType.Flatten:
                    {
                        using (CanvasCommandList commandList = new CanvasCommandList(this.CanvasDevice))
                        {
                            ICanvasImage image = this.Nodes.Render(commandList);
                            ILayer add = new BitmapLayer(this.CanvasDevice, image, this.Transformer.Width, this.Transformer.Height);

                            // History
                            IHistory history = this.LayerManager.Clear(this, add);
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);
                        }

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                // Grouping
                case OptionType.Group:
                    {
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                {
                                    ILayer add = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                                    // History
                                    IHistory history = this.LayerManager.Group(this, add, layer);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    ILayer add = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                                    // History
                                    IHistory history = this.LayerManager.Group(this, add, items);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.Ungroup:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Children.Count is 0) break;

                            // History
                            IHistory history = this.LayerManager.Ungroup(this, layer);
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                case OptionType.Release:
                    {
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                {
                                    if (this.LayerManager.Release(this, layer) is IHistory history)
                                    {
                                        // History
                                        history.Title = type.GetString();
                                        int removes = this.History.Push(history);

                                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                                        this.RaiseHistoryCanExecuteChanged();
                                    }
                                }
                                break;
                            default:
                                {
                                    // History
                                    IHistory history = this.LayerManager.Release(this, items);
                                    history.Title = type.GetString();
                                    int removes = this.History.Push(history);

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;

                // Combine
                case OptionType.Union: break;
                case OptionType.Exclude: break;
                case OptionType.Xor: break;
                case OptionType.Intersect: break;

                case OptionType.ExpandStroke: break;

                #endregion

                #region Effect

                // Category
                case OptionType.Other: break;
                case OptionType.Adjustment: break;
                case OptionType.Adjustment2: break;
                case OptionType.Effect1: break;
                case OptionType.Effect2: break;
                case OptionType.Effect3: break;

                // Other
                case OptionType.Move:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                this.BitmapLayer = bitmapLayer;

                                this.ResetMove();

                                this.OptionType = OptionType.Move;
                                this.ConstructAppBar(OptionType.Move);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.Transform:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                switch (state)
                                {
                                    case SelectionType.PixelBounds:
                                        {
                                            PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.ResetTransform(bounds);
                                        }
                                        break;
                                    case SelectionType.MarqueePixelBounds:
                                        {
                                            PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.ResetTransform(bounds);
                                        }
                                        break;
                                    default:
                                        this.ResetTransform(bitmapLayer.Bounds);
                                        break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.Transform;
                                this.ConstructAppBar(OptionType.Transform);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.FreeTransform:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                switch (state)
                                {
                                    case SelectionType.PixelBounds:
                                        {
                                            PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.ResetFreeTransform(bounds);
                                        }
                                        break;
                                    case SelectionType.MarqueePixelBounds:
                                        {
                                            PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.ResetFreeTransform(bounds);
                                        }
                                        break;
                                    default:
                                        this.ResetFreeTransform(bitmapLayer.Bounds);
                                        break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.FreeTransform;
                                this.ConstructAppBar(OptionType.FreeTransform);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;

                case OptionType.DisplacementLiquefaction:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.ResetDisplacementLiquefaction();

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.DisplacementLiquefaction;
                                this.ConstructAppBar(OptionType.DisplacementLiquefaction);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.GradientMapping:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.ResetGradientMapping();

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.GradientMapping;
                                this.ConstructAppBar(OptionType.GradientMapping);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.RippleEffect:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.ResetRippleEffect(bitmapLayer);

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.RippleEffect;
                                this.ConstructAppBar(OptionType.RippleEffect);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                //case OptionType.Threshold: break;

                // Adjustment
                case OptionType.Sepia:
                //case OptionType.Posterize:

                case OptionType.Gray:
                case OptionType.Invert:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.PrimaryGrayOrInvert(type, bitmapLayer, this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;

                case OptionType.Border:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                int width = this.Transformer.Width;
                                int height = this.Transformer.Height;

                                this.ResetBorder(width, height);

                                this.BitmapLayer = bitmapLayer;

                                this.OptionType = OptionType.Border;
                                this.ConstructAppBar(OptionType.Border);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;
                case OptionType.Lighting:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.ResetLighting(bitmapLayer);

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;

                                this.OptionType = OptionType.Lighting;
                                this.ConstructAppBar(OptionType.Lighting);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                this.CanvasControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;

                case OptionType.Threshold:
                case OptionType.HSB:

                case OptionType.Exposure:
                case OptionType.Brightness:
                case OptionType.Saturation:
                case OptionType.HueRotation:
                case OptionType.Contrast:
                case OptionType.Temperature:
                case OptionType.HighlightsAndShadows:

                case OptionType.GammaTransfer:
                case OptionType.Vignette:
                case OptionType.ColorMatrix:
                case OptionType.ColorMatch:

                // Effect1
                case OptionType.GaussianBlur:
                case OptionType.DirectionalBlur:
                case OptionType.Sharpen:
                case OptionType.Shadow:
                case OptionType.EdgeDetection:
                case OptionType.Morphology:
                case OptionType.Emboss:
                case OptionType.Straighten:

                // Effect2
                //case OptionType.Sepia:
                case OptionType.Posterize:
                case OptionType.LuminanceToAlpha:
                case OptionType.ChromaKey:
                //case OptionType.Border:
                case OptionType.Colouring:
                case OptionType.Tint:
                case OptionType.DiscreteTransfer:

                //case OptionType.Lighting:
                case OptionType.Fog:
                case OptionType.Glass:
                    //case OptionType.PinchPunch:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                                    break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;
                                this.OptionType = type;
                                this.ConstructAppBar(type);

                                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            }
                            else this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        }
                        else this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                    }
                    break;

                #endregion

                #region Tool

                // Category
                case OptionType.Marquee:
                case OptionType.Selection:
                case OptionType.Paint:
                case OptionType.Vector:
                case OptionType.Curve:
                case OptionType.Text:
                case OptionType.Geometry:
                case OptionType.Pattern:

                // Marquee
                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:

                // Selection
                case OptionType.SelectionFlood:
                case OptionType.SelectionBrush:

                // Paint
                case OptionType.PaintBrush:
                case OptionType.PaintBrushMulti:
                case OptionType.PaintBrushForce:
                case OptionType.PaintLine:

                // Vector
                case OptionType.Cursor:
                case OptionType.View:
                case OptionType.Straw:
                case OptionType.Crop:

                case OptionType.Fill:
                case OptionType.Brush:
                case OptionType.Transparency:

                case OptionType.Image:

                // Curve
                case OptionType.Node:
                case OptionType.Pen:

                // Text
                case OptionType.TextArtistic:
                case OptionType.TextFrame:

                // Geometry
                // Geometry
                case OptionType.GeometryRectangle:
                case OptionType.GeometryEllipse:
                // Geometry
                case OptionType.GeometryRoundRect:
                case OptionType.GeometryTriangle:
                case OptionType.GeometryDiamond:
                // Geometry
                case OptionType.GeometryPentagon:
                case OptionType.GeometryStar:
                case OptionType.GeometryCog:
                // Geometry
                case OptionType.GeometryDonut:
                case OptionType.GeometryPie:
                case OptionType.GeometryCookie:
                // Geometry
                case OptionType.GeometryArrow:
                case OptionType.GeometryCapsule:
                case OptionType.GeometryHeart:

                // Pattern
                case OptionType.PatternGrid:
                case OptionType.PatternDiagonal:
                case OptionType.PatternSpotted:
                    if (this.OptionType.HasPreview())
                    {
                        // Tool
                        this.BitmapLayer = null;
                        this.OptionType = type;
                        this.ConstructAppBar(type);

                        this.CanvasAnimatedControl.Invalidate(this.OptionType.HasPreview()); // Invalidate
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    else
                    {
                        // Tool
                        this.BitmapLayer = null;
                        this.OptionType = type;
                        this.ConstructAppBar(type);

                        this.CanvasAnimatedControl.Invalidate(this.OptionType.HasPreview()); // Invalidate
                    }
                    break;

                #endregion

                // GeometryTransform
                // Geometry
                case OptionType.GeometryRectangleTransform: break;
                case OptionType.GeometryEllipseTransform: break;
                // Geometry
                case OptionType.GeometryRoundRectTransform: break;
                case OptionType.GeometryTriangleTransform: break;
                case OptionType.GeometryDiamondTransform: break;
                // Geometry
                case OptionType.GeometryPentagonTransform: break;
                case OptionType.GeometryStarTransform: break;
                case OptionType.GeometryCogTransform: break;
                // Geometry
                case OptionType.GeometryDonutTransform: break;
                case OptionType.GeometryPieTransform: break;
                case OptionType.GeometryCookieTransform: break;
                // Geometry
                case OptionType.GeometryArrowTransform: break;
                case OptionType.GeometryCapsuleTransform: break;
                case OptionType.GeometryHeartTransform: break;
                default: break;
            }
        }
    }

}