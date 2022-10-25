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
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        public async void Click(OptionType type)
        {
            switch (type)
            {
                // None
                case OptionType.None: break;

                // Flag
                case OptionType.IsItemClickEnabled: break;
                case OptionType.ExistIcon: break;
                case OptionType.ExistThumbnail: break;
                case OptionType.AllowDrag: break;
                case OptionType.HasPreview: break;
                case OptionType.HasDifference: break;
                case OptionType.WithState: break;
                case OptionType.WithTransform: break;

                // Root
                case OptionType.File: break;
                case OptionType.Edit: break;
                case OptionType.Menu: break;
                case OptionType.Setup: break;
                case OptionType.Layer: break;
                case OptionType.Select: break;
                case OptionType.Effect: break;
                case OptionType.Tool: break;

                #region File

                case OptionType.Close:
                    if (this.IsFullScreen)
                    {
                        this.Click(OptionType.UnFullScreen);
                        return;
                    }

                    base.IsEnabled = false;

                    //@Debug
                    // OptionType becomes Tool when it is Effect
                    this.OptionType = this.ToolListView.SelectedItem;
                    this.AppBar.Construct(this.ToolListView.SelectedItem);

                    await this.SaveAsync(this.ApplicationView.PersistedStateId, true);
                    break;
                case OptionType.Save:
                    base.IsEnabled = false;
                    await this.SaveAsync(this.ApplicationView.PersistedStateId, false);
                    base.IsEnabled = true;
                    break;

                case OptionType.Export:
                    if (this.Nodes.Count() is 0)
                    {
                        this.Tip(TipType.NoLayer);
                        break;
                    }

                    {
                        StorageFile file = await FileUtil.PickSingleFileAsync(PickerLocationId.Desktop, this.ExportDialog.FileChoices, this.ApplicationView.Title);
                        if (file is null) break;
                        this.Tip(TipType.Saving);

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
                            this.Tip(TipType.SaveFailed);
                            break;
                        }

                        this.Tip(TipType.SaveSuccess);

                        if (this.ExportDialog.IsOpenFileExplorer)
                        {
                            try
                            {
                                await Launcher.LaunchFolderAsync(await file.GetParentAsync(), new FolderLauncherOptions
                                {
                                    ItemsToSelect = { file }
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    break;
                case OptionType.ExportAll:
                    if (this.Nodes.Count() is 0)
                    {
                        this.Tip(TipType.NoLayer);
                        break;
                    }

                    {
                        IStorageFolder folder = await FileUtil.PickSingleFolderAsync(PickerLocationId.Desktop);
                        if (folder is null) break;
                        this.Tip(TipType.Saving);

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
                                    this.Tip(TipType.SaveSuccess);
                                    options.ItemsToSelect.Add(file);
                                }
                                else
                                    this.Tip(TipType.SaveFailed);
                            }
                        }

                        if (options.ItemsToSelect.Count is 0) break;
                        if (this.ExportDialog.IsOpenFileExplorer)
                        {
                            await Launcher.LaunchFolderAsync(folder, options);
                        }
                    }
                    break;
                case OptionType.ExportCurrent:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            StorageFile file = await FileUtil.PickSingleFileAsync(PickerLocationId.Desktop, this.ExportDialog.FileChoices, this.ApplicationView.Title);
                            if (file is null) break;
                            this.Tip(TipType.Saving);

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

                            if (result is false)
                            {
                                this.Tip(TipType.SaveFailed);
                                break;
                            }

                            this.Tip(TipType.SaveSuccess);

                            if (this.ExportDialog.IsOpenFileExplorer)
                            {
                                try
                                {
                                    await Launcher.LaunchFolderAsync(await file.GetParentAsync(), new FolderLauncherOptions
                                    {
                                        ItemsToSelect = { file }
                                    });
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;

                case OptionType.Undo:
                    if (this.History.CanUndo is false) break;

                    // History
                    {
                        bool result = this.History.Undo(this.Undo);
                        if (result is false) break;
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    this.Tip(TipType.Undo);
                    break;
                case OptionType.Redo:
                    if (this.History.CanRedo is false) break;

                    // History
                    {
                        bool result = this.History.Redo(this.Redo);
                        if (result is false) break;
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    this.Tip(TipType.Redo);
                    break;

                case OptionType.FullScreen:
                    if (this.ExpanderLightDismissOverlay.Hide()) break;

                    if (this.IsFullScreen)
                    {
                        this.Click(OptionType.UnFullScreen);
                        return;
                    }

                    this.ApplicationView.TryEnterFullScreenMode();
                    this.IsFullScreen = true;

                    VisualStateManager.GoToState(this, nameof(FullScreen), useTransitions: true);

                    this.FullScreenKey.IsEnabled = false;
                    await Task.Delay(200);
                    this.FullScreenKey.IsEnabled = true;
                    break;
                case OptionType.UnFullScreen:
                    this.ApplicationView.ExitFullScreenMode();
                    this.IsFullScreen = false;

                    if (base.ActualWidth > 1200)
                        VisualStateManager.GoToState(this, nameof(Hub), false);
                    else
                        VisualStateManager.GoToState(this, nameof(PC), false);
                    break;

                #endregion

                #region Edit

                case OptionType.Cut: // Copy + Clear
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    this.Clipboard.DrawCopy(bitmapLayer.GetMask(this.Marquee));

                                    // History
                                    int removes = this.History.Push(bitmapLayer.Clear(this.Marquee, interpolationColors));
                                    bitmapLayer.Flush();
                                    bitmapLayer.RenderThumbnail();
                                    break;
                                default:
                                    this.Clipboard.CopyPixels(bitmapLayer);

                                    // History
                                    int removes2 = this.History.Push(bitmapLayer.GetBitmapClearHistory(Colors.Transparent));
                                    bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                                    bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                                    bitmapLayer.ClearThumbnail(Colors.Transparent);
                                    break;
                            }

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                            this.EditMenu.PasteIsEnabled = true;
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                case OptionType.Copy:
                    this.ExpanderLightDismissOverlay.Hide();

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

                            this.EditMenu.PasteIsEnabled = true;
                        }
                    }
                    break;
                case OptionType.Paste:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        Color[] interpolationColors = this.Clipboard.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Clipboard.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip(TipType.NoPixelForBitmapLayer);
                            break;
                        }

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, this.Clipboard[BitmapType.Source], this.Transformer.Width, this.Transformer.Height);

                        // History
                        int removes = this.History.Push(this.LayerManager.Add(this, add));

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                case OptionType.Clear:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            switch (mode)
                            {
                                case PixelBoundsMode.None:
                                    // History
                                    int removes = this.History.Push(bitmapLayer.Clear(this.Marquee, interpolationColors));
                                    bitmapLayer.Flush();
                                    bitmapLayer.RenderThumbnail();
                                    break;
                                default:
                                    // History
                                    int removes2 = this.History.Push(bitmapLayer.GetBitmapClearHistory(Colors.Transparent));
                                    bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                                    bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                                    bitmapLayer.ClearThumbnail(Colors.Transparent);
                                    break;
                            }

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                #endregion

                #region Menu

                case OptionType.ExportMenu:
                    {
                        ContentDialogResult result = await this.ExportDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                switch (this.ExportDialog.Mode)
                                {
                                    case ExportMode.None:
                                        this.Click(OptionType.Export);
                                        return;
                                    case ExportMode.All:
                                        this.Click(OptionType.ExportAll);
                                        return;
                                    case ExportMode.Current:
                                        this.Click(OptionType.ExportCurrent);
                                        return;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                case OptionType.ToolMenu:
                    this.HeadComboBox.SelectedIndex = 0;
                    break;
                case OptionType.HistoryMenu:
                    this.HeadComboBox.SelectedIndex = 5;
                    break;
                case OptionType.ColorMenu:
                    this.ColorMenu.Toggle(this.ColorButton, ExpanderPlacementMode.Bottom);
                    break;
                case OptionType.PaletteMenu:
                    this.PaletteMenu.Toggle(this.PaletteButton, ExpanderPlacementMode.Bottom);
                    break;

                case OptionType.EditMenu:
                    this.EditMenu.Toggle(this.EditButton, ExpanderPlacementMode.Bottom);
                    break;
                case OptionType.AdjustmentMenu:
                    this.AdjustmentMenu.Toggle(this.AdjustmentButton, ExpanderPlacementMode.Bottom);
                    break;
                case OptionType.OtherMenu:
                    this.OtherMenu.Toggle(this.OtherButton, ExpanderPlacementMode.Bottom);
                    break;

                case OptionType.PaintMenu:
                    this.HeadComboBox.SelectedIndex = 1;
                    break;
                case OptionType.BrushMenu:
                    this.HeadComboBox.SelectedIndex = 2;
                    break;
                case OptionType.SizeMenu:
                    this.HeadComboBox.SelectedIndex = 3;
                    break;

                case OptionType.LayerMenu:
                    this.LayerMenu.Toggle(this.LayerButton, ExpanderPlacementMode.Bottom);
                    break;
                case OptionType.AddMenu:
                    this.AddMenu.Show();
                    break;
                case OptionType.AddMenuWithRename:
                    this.AddMenu.Show();
                    this.AddMenu.Rename();
                    break;

                #endregion

                #region Setup

                case OptionType.CropCanvas:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        int width2 = this.Transformer.Width;
                        int height2 = this.Transformer.Height;

                        this.SetCropCanvas(width2, height2);
                    }

                    this.BitmapLayer = null;
                    this.OptionType = OptionType.CropCanvas;
                    this.AppBar.Construct(OptionType.CropCanvas);
                    this.SetCanvasState(true);
                    break;

                case OptionType.Stretch:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        this.StretchDialog.Resezing(this.Transformer.Width, this.Transformer.Height);
                        ContentDialogResult result = await this.StretchDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                int width2 = this.Transformer.Width;
                                int height2 = this.Transformer.Height;

                                uint width = (uint)width2;
                                uint height = (uint)height2;

                                uint w2 = this.StretchDialog.Size.Width;
                                uint h2 = this.StretchDialog.Size.Height;
                                if (w2 == width && h2 == height) break;

                                int w = (int)w2;
                                int h = (int)h2;

                                switch (this.StretchDialog.SelectedIndex)
                                {
                                    case 0:
                                        {
                                            CanvasImageInterpolation interpolation = this.StretchDialog.Interpolation;
                                            {
                                                this.Transformer.Width = w;
                                                this.Transformer.Height = h;
                                                this.Transformer.Fit();

                                                this.CreateResources(w, h);
                                                this.CreateMarqueeResources(w, h);
                                            }
                                            // History
                                            int removes = this.History.Push(new CompositeHistory(new IHistory[]
                                            {
                                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Skretch(this.CanvasDevice, w, h, interpolation)).ToArray()),
                                                new SetupHistory(new BitmapSize { Width = width, Height = height }, new BitmapSize { Width = w2, Height = h2 })
                                            }));

                                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                                            this.RaiseHistoryCanExecuteChanged();
                                        }
                                        break;
                                    case 1:
                                        {
                                            IndicatorMode indicator = this.StretchDialog.Indicator;

                                            Vector2 vect = this.Transformer.GetIndicatorVector(indicator);
                                            {
                                                this.Transformer.Width = w;
                                                this.Transformer.Height = h;
                                                this.Transformer.Fit();

                                                this.CreateResources(w, h);
                                                this.CreateMarqueeResources(w, h);
                                            }
                                            Vector2 vect2 = this.Transformer.GetIndicatorVector(indicator);

                                            Vector2 offset = vect2 - vect;
                                            // History
                                            int removes = this.History.Push(new CompositeHistory(new IHistory[]
                                            {
                                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, offset)).ToArray()),
                                                new SetupHistory(new BitmapSize { Width = width, Height = height }, new BitmapSize { Width = w2, Height = h2 })
                                            }));

                                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                                            this.RaiseHistoryCanExecuteChanged();
                                        }
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

                case OptionType.FlipHorizontal:
                    this.ExpanderLightDismissOverlay.Hide();

                    this.Transformer.Fit();

                    // History
                    {
                        int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Flip(this.CanvasDevice, BitmapFlip.Horizontal)).ToArray()));
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.FlipVertical:
                    this.ExpanderLightDismissOverlay.Hide();

                    this.Transformer.Fit();

                    // History
                    {
                        int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Flip(this.CanvasDevice, BitmapFlip.Vertical)).ToArray()));
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;

                case OptionType.LeftTurn:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        int width2 = this.Transformer.Width;
                        int height2 = this.Transformer.Height;

                        uint width = (uint)width2;
                        uint height = (uint)height2;
                        {
                            int w = height2;
                            int h = width2;

                            this.Transformer.Width = w;
                            this.Transformer.Height = h;
                            this.Transformer.Fit();

                            this.CreateResources(w, h);
                            this.CreateMarqueeResources(w, h);
                        }

                        // History
                        if (width2 == height2)
                        {
                            int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise270Degrees)).ToArray()));
                        }
                        else
                        {
                            int removes = this.History.Push(new CompositeHistory(new IHistory[]
                            {
                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise270Degrees)).ToArray()),
                                new SetupHistory(new BitmapSize { Width = width, Height = height }, new BitmapSize { Width = height, Height = width })
                            }));
                        }
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.RightTurn:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        int width2 = this.Transformer.Width;
                        int height2 = this.Transformer.Height;

                        uint width = (uint)width2;
                        uint height = (uint)height2;
                        {
                            int w = height2;
                            int h = width2;

                            this.Transformer.Width = w;
                            this.Transformer.Height = h;
                            this.Transformer.Fit();

                            this.CreateResources(w, h);
                            this.CreateMarqueeResources(w, h);
                        }

                        // History
                        if (width2 == height2)
                        {
                            int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise90Degrees)).ToArray()));
                        }
                        else
                        {
                            int removes = this.History.Push(new CompositeHistory(new IHistory[]
                            {
                                this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise90Degrees)).ToArray()),
                                new SetupHistory(new BitmapSize { Width = width, Height = height }, new BitmapSize { Width = height, Height = width })
                            }));
                        }
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.OverTurn:
                    this.ExpanderLightDismissOverlay.Hide();

                    this.Transformer.Fit();

                    // History
                    {
                        int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Rotation(this.CanvasDevice, BitmapRotation.Clockwise180Degrees)).ToArray()));
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;

                #endregion

                #region Layer

                // Category
                case OptionType.Add: break;
                case OptionType.Clipboard: break;
                case OptionType.Layering: break;
                case OptionType.Grouping: break;
                case OptionType.Combine: break;

                // Add
                case OptionType.AddLayer:
                    {
                        ILayer add = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                        // History
                        int removes = this.History.Push(this.LayerManager.Add(this, add));
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.AddImageLayer:
                    this.AddAsync(await FileUtil.PickMultipleImageFilesAsync(Windows.Storage.Pickers.PickerLocationId.Desktop));
                    break;
                case OptionType.AddCurveLayer:
                    {
                        ILayer add = new CurveLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                        // History
                        int removes = this.History.Push(this.LayerManager.Add(this, add));
                    }

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.RaiseHistoryCanExecuteChanged();
                    break;

                // Clipboard
                case OptionType.CutLayer:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                    int removes = this.History.Push(this.LayerManager.Cut(this, layer));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                    this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                }
                                break;
                            default:
                                {
                                    // History
                                    int removes = this.History.Push(this.LayerManager.Cut(this, items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                    this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.CopyLayer:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                break;
                            default:
                                this.LayerManager.Copy(this, items);
                                this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                break;
                        }
                    }
                    break;
                case OptionType.PasteLayer:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                    int removes = this.History.Push(this.LayerManager.Paste(this, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, id));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    // History
                                    int removes = this.History.Push(this.LayerManager.Paste(this, this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, this.ClipboardLayers));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;

                // Layering
                case OptionType.Remove:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                    int removes = this.History.Push(this.LayerManager.Remove(this, layer, true));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    // History
                                    int removes = this.History.Push(this.LayerManager.Remove(this, items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.Duplicate: // CopyLayer + PasteLayer
                    this.ExpanderLightDismissOverlay.Hide();

                    this.Click(OptionType.CopyLayer);
                    this.Click(OptionType.PasteLayer);
                    return;

                case OptionType.Extract:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            if (mode is PixelBoundsMode.Transarent)
                            {
                                this.Tip(TipType.NoPixelForBitmapLayer);
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
                            int removes = this.History.Push(this.LayerManager.Add(this, add));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;
                case OptionType.Merge:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                            int removes = this.History.Push(new CompositeHistory(new IHistory[]
                                            {
                                                bitmapLayer.GetBitmapResetHistory(),
                                                this.LayerManager.Remove(this, neighbor, false)
                                            }));
                                            bitmapLayer.Flush();
                                            bitmapLayer.RenderThumbnail();

                                            this.RaiseHistoryCanExecuteChanged();
                                        }
                                    }
                                    break;
                                default:
                                    this.Tip(TipType.NotBitmapLayer);
                                    break;
                            }
                        }
                        else
                        {
                            this.Tip(TipType.NoLayer);
                        }
                    }
                    break;
                case OptionType.Flatten:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        using (CanvasCommandList commandList = new CanvasCommandList(this.CanvasDevice))
                        {
                            ICanvasImage image = this.Nodes.Render(commandList);
                            ILayer add = new BitmapLayer(this.CanvasDevice, image, this.Transformer.Width, this.Transformer.Height);

                            // History
                            int removes = this.History.Push(this.LayerManager.Clear(this, add));
                        }

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                    }
                    break;

                // Grouping
                case OptionType.Group:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                    int removes = this.History.Push(this.LayerManager.Group(this, add, layer));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                            default:
                                {
                                    ILayer add = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                                    // History
                                    int removes = this.History.Push(this.LayerManager.Group(this, add, items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.RaiseHistoryCanExecuteChanged();
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.Ungroup:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Children.Count is 0) break;

                            // History
                            int removes = this.History.Push(this.LayerManager.Ungroup(this, layer));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                case OptionType.Release:
                    this.ExpanderLightDismissOverlay.Hide();

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
                                        int removes = this.History.Push(history);

                                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                                        this.RaiseHistoryCanExecuteChanged();
                                    }
                                }
                                break;
                            default:
                                {
                                    // History
                                    int removes = this.History.Push(this.LayerManager.Release(this, items));

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

                #region Select

                // Category
                case OptionType.Selecting: break;
                case OptionType.Marquees: break;

                // Selecting
                case OptionType.All:
                    // History
                    {
                        int removes = this.History.Push(this.Marquee.GetBitmapClearHistory(Colors.DodgerBlue));
                    }
                    this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Origin);
                    this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Source);
                    this.Marquee.ClearThumbnail(Colors.DodgerBlue);

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.Deselect:
                    // History
                    {
                        int removes = this.History.Push(this.Marquee.GetBitmapClearHistory(Colors.Transparent));
                    }
                    this.Marquee.Clear(Colors.Transparent, BitmapType.Origin);
                    this.Marquee.Clear(Colors.Transparent, BitmapType.Source);
                    this.Marquee.ClearThumbnail(Colors.Transparent);

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.MarqueeInvert:
                    // History
                    {
                        int removes = this.History.Push(this.Marquee.Invert(Colors.DodgerBlue));
                    }
                    this.Marquee.Flush();
                    this.Marquee.RenderThumbnail();

                    this.RaiseHistoryCanExecuteChanged();
                    break;
                case OptionType.Pixel:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            // History
                            int removes = this.History.Push(this.Marquee.Pixel(bitmapLayer, Colors.DodgerBlue));
                            this.Marquee.Flush();
                            this.Marquee.RenderThumbnail();

                            this.RaiseHistoryCanExecuteChanged();
                        }
                    }
                    break;

                // Marquees
                case OptionType.Feather:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip(TipType.NoPixelForMarquee);
                            break;
                        }

                        this.OptionType = type;
                        this.AppBar.Construct(type);
                        this.SetCanvasState(true);
                    }
                    break;
                case OptionType.MarqueeTransform:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip(TipType.NoPixelForMarquee);
                            break;
                        }

                        switch (mode)
                        {
                            case PixelBoundsMode.None:
                                PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(interpolationColors);
                                PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, interpolationColors);
                                this.SetTransform(bounds);
                                break;
                            default:
                                this.SetTransform(this.Marquee.Bounds);
                                break;
                        }

                        this.OptionType = type;
                        this.AppBar.Construct(type);
                        this.SetCanvasState(true);
                    }
                    break;
                case OptionType.Grow:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip(TipType.NoPixelForMarquee);
                            break;
                        }

                        this.OptionType = type;
                        this.AppBar.Construct(type);
                        this.SetCanvasState(true);
                    }
                    break;
                case OptionType.Shrink:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip(TipType.NoPixelForMarquee);
                            break;
                        }

                        this.OptionType = type;
                        this.AppBar.Construct(type);
                        this.SetCanvasState(true);
                    }
                    break;

                #endregion

                #region Effect

                // Category
                case OptionType.Other: break;
                case OptionType.Adjustment: break;
                case OptionType.Effect1: break;
                case OptionType.Effect2: break;
                case OptionType.Effect3: break;

                // Other
                case OptionType.Transform:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.Tip(TipType.NoPixelForBitmapLayer);
                                    break;
                                }

                                switch (state)
                                {
                                    case SelectionType.PixelBounds:
                                        {
                                            PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.SetTransform(bounds);
                                        }
                                        break;
                                    case SelectionType.MarqueePixelBounds:
                                        {
                                            PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(InterpolationColors);
                                            PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                            this.SetTransform(bounds);
                                        }
                                        break;
                                    default:
                                        this.SetTransform(bitmapLayer.Bounds);
                                        break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;
                                this.OptionType = type;
                                this.AppBar.Construct(type);
                                this.SetCanvasState(true);
                                break;
                            }
                            else this.Tip(TipType.NotBitmapLayer);
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.Tip(TipType.NoPixelForBitmapLayer);
                                    break;
                                }

                                switch (type)
                                {
                                    case OptionType.Transform:
                                        switch (state)
                                        {
                                            case SelectionType.PixelBounds:
                                                {
                                                    PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
                                                    PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                                    this.SetTransform(bounds);
                                                }
                                                break;
                                            case SelectionType.MarqueePixelBounds:
                                                {
                                                    PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(InterpolationColors);
                                                    PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds, InterpolationColors);
                                                    this.SetTransform(bounds);
                                                }
                                                break;
                                            default:
                                                this.SetTransform(bitmapLayer.Bounds);
                                                break;
                                        }
                                        break;
                                    case OptionType.DisplacementLiquefaction:
                                        this.SetDisplacementLiquefaction();
                                        break;
                                    case OptionType.GradientMapping:
                                        this.SetGradientMapping();
                                        break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;
                                this.OptionType = type;
                                this.AppBar.Construct(type);
                                this.SetCanvasState(true);
                                break;
                            }
                            else this.Tip(TipType.NotBitmapLayer);
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                case OptionType.GradientMapping:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.Tip(TipType.NoPixelForBitmapLayer);
                                    break;
                                }

                                this.SetGradientMapping();

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;
                                this.OptionType = type;
                                this.AppBar.Construct(type);
                                this.SetCanvasState(true);
                                break;
                            }
                            else this.Tip(TipType.NotBitmapLayer);
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                //case OptionType.RippleEffect: break;
                case OptionType.Fill: break;

                // Adjustment
                case OptionType.Gray:
                case OptionType.Invert:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.Tip(TipType.NoPixelForBitmapLayer);
                                    break;
                                }

                                this.Primary(bitmapLayer, this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                                break;
                            }
                            else this.Tip(TipType.NotBitmapLayer);
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                case OptionType.RippleEffect:
                case OptionType.Exposure:
                case OptionType.Brightness:
                case OptionType.Saturation:
                case OptionType.HueRotation:
                case OptionType.Contrast:
                case OptionType.Temperature:
                case OptionType.HighlightsAndShadows:

                // Effect1
                //case OptionType.GaussianBlur: break;
                //case OptionType.DirectionalBlur: break;
                //case OptionType.Sharpen: break;
                //case OptionType.Shadow: break;
                //case OptionType.ChromaKey: break;
                //case OptionType.EdgeDetection: break;
                //case OptionType.Border: break;
                //case OptionType.Emboss: break;
                //case OptionType.Lighting: break;

                // Effect2
                case OptionType.LuminanceToAlpha:
                    this.ExpanderLightDismissOverlay.Hide();

                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Type is LayerType.Bitmap && layer is BitmapLayer bitmapLayer)
                            {
                                SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
                                if (state is SelectionType.None)
                                {
                                    this.Tip(TipType.NoPixelForBitmapLayer);
                                    break;
                                }

                                this.BitmapLayer = bitmapLayer;
                                this.SelectionType = state;
                                this.OptionType = type;
                                this.AppBar.Construct(type);
                                this.SetCanvasState(true);
                                break;
                            }
                            else this.Tip(TipType.NotBitmapLayer);
                        }
                        else this.Tip(TipType.NoLayer);
                    }
                    break;
                case OptionType.Fog: break;
                case OptionType.Sepia: break;
                case OptionType.Posterize: break;
                case OptionType.Colouring: break;
                case OptionType.Tint: break;
                case OptionType.DiscreteTransfer: break;
                case OptionType.Vignette: break;
                case OptionType.GammaTransfer: break;

                // Effect3
                case OptionType.Glass: break;
                case OptionType.PinchPunch: break;
                case OptionType.Morphology: break;

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
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:

                // Vector
                case OptionType.Cursor:
                case OptionType.View:
                case OptionType.Crop:

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
                case OptionType.GeometryDount:
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
                    {
                        this.BitmapLayer = null;
                        this.OptionType = type;
                        this.AppBar.Construct(type);
                        this.SetCanvasState(default);
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
                case OptionType.GeometryDountTransform: break;
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