using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void ConstructEdits()
        {
            this.AddMenu.ItemClick += (s, type) => this.Edit(type);
            this.LayerMenu.ItemClick += (s, type) => this.Edit(type);
            this.EditMenu.ItemClick += (s, type) => this.Edit(type);
        }

        public void Edit(OptionType type)
        {
            switch (type)
            {
                case OptionType.None:
                    break;

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
                                    /// History
                                    int removes = this.History.Push(this.Remove(layer));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                            default:
                                {
                                    /// History
                                    int removes = this.History.Push(this.Remove(items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.AddLayer:
                    {
                        ILayer add = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                        this.Layers.Push(add);

                        /// History
                        int removes = this.History.Push(this.Add(add));

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                case OptionType.AddImageLayer:
                    this.AddAsync();
                    break;
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
                                    /// History
                                    int removes = this.History.Push(this.Cut(layer));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                    this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                }
                                break;
                            default:
                                {
                                    /// History
                                    int removes = this.History.Push(this.Cut(items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                    this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.CopyLayer:
                    {
                        /// History
                        var items = this.LayerSelectedItems;
                        switch (items.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                if (this.LayerSelectedItem is ILayer layer)
                                    this.Copy(layer);
                                this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
                                break;
                            default:
                                this.Copy(items);
                                this.LayerMenu.PasteIsEnabled = this.ClipboardLayers.Count is 0 is false;
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
                                if (this.Layers.ContainsKey(id))
                                {
                                    /// History
                                    int removes = this.History.Push(this.Paste(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, id));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                            default:
                                {
                                    /// History
                                    int removes = this.History.Push(this.Paste(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height, this.ClipboardLayers));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                        }
                    }
                    break;

                case OptionType.Cut: // Copy + Clear
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

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            this.LayerMenu.PasteIsEnabled = true;
                        }
                        else
                        {
                            this.Tip("No Layer", "Create a new Layer?");
                        }
                    }
                    break;
                case OptionType.Duplicate: // CopyLayer + PasteLayer
                    this.Edit(OptionType.CopyLayer);
                    this.Edit(OptionType.PasteLayer);
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

                            this.LayerMenu.PasteIsEnabled = true;
                        }
                    }
                    break;
                case OptionType.Paste:
                    {
                        Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                        if (mode is PixelBoundsMode.Transarent)
                        {
                            this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                            break;
                        }

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, this.Clipboard[BitmapType.Source], this.Transformer.Width, this.Transformer.Height);

                        // History
                        int removes = this.History.Push(this.Add(add));

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
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

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                    }
                    break;
                case OptionType.Extract:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            if (mode is PixelBoundsMode.Transarent)
                            {
                                this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
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
                            int removes = this.History.Push(this.Add(add));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
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
                                        if (this.ObservableCollection.GetNeighbor(layer) is ILayer neighbor)
                                        {
                                            if (neighbor.Merge(bitmapLayer, bitmapLayer[BitmapType.Origin]) is ICanvasImage source)
                                            {
                                                /// History
                                                bitmapLayer.DrawCopy(source);
                                                int removes1 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                                                bitmapLayer.Flush();
                                                bitmapLayer.RenderThumbnail();

                                                /// History
                                                int removes2 = this.History.Push(this.Remove(neighbor));

                                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                                this.RedoButton.IsEnabled = this.History.CanRedo;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    break;
                case OptionType.Flatten:
                    {
                        ICanvasImage image = this.Nodes.Merge(null, null);
                        ILayer add = new BitmapLayer(this.CanvasDevice, image, this.Transformer.Width, this.Transformer.Height);
                        this.Layers.Push(add);

                        /// History
                        int removes = this.History.Push(this.Clear(add));

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
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

                                    /// History
                                    int removes = this.History.Push(this.Group(add, layer));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                            default:
                                {
                                    ILayer add = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                                    /// History
                                    int removes = this.History.Push(this.Group(add, items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.Ungroup:
                    {
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            if (layer.Children.Count is 0) return;

                            /// History
                            int removes = this.History.Push(this.Ungroup(layer));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
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
                                    if (this.Release(layer) is IHistory history)
                                    {
                                        /// History
                                        int removes = this.History.Push(history);

                                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                                        this.UndoButton.IsEnabled = this.History.CanUndo;
                                        this.RedoButton.IsEnabled = this.History.CanRedo;
                                    }
                                }
                                break;
                            default:
                                {
                                    /// History
                                    int removes = this.History.Push(this.Release(items));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                                break;
                        }
                    }
                    break;
                case OptionType.All:
                    {
                        // History
                        int removes = this.History.Push(this.Marquee.GetBitmapClearHistory(Colors.DodgerBlue));
                        this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Origin);
                        this.Marquee.Clear(Colors.DodgerBlue, BitmapType.Source);
                        this.Marquee.ClearThumbnail(Colors.DodgerBlue);

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                case OptionType.Deselect:
                    {
                        // History
                        int removes = this.History.Push(this.Marquee.GetBitmapClearHistory(Colors.Transparent));
                        this.Marquee.Clear(Colors.Transparent, BitmapType.Origin);
                        this.Marquee.Clear(Colors.Transparent, BitmapType.Source);
                        this.Marquee.ClearThumbnail(Colors.Transparent);

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                case OptionType.MarqueeInvert:
                    {
                        // History
                        int removes = this.History.Push(this.Marquee.Invert(Colors.DodgerBlue));
                        this.Marquee.Flush();
                        this.Marquee.RenderThumbnail();

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                case OptionType.Pixel:
                    {
                        if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
                        {
                            // History
                            int removes = this.History.Push(this.Marquee.Pixel(bitmapLayer, Colors.DodgerBlue));
                            this.Marquee.Flush();
                            this.Marquee.RenderThumbnail();

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                    }
                    break;
                case OptionType.Feather:
                    this.EditClick(OptionType.Feather);
                    break;
                case OptionType.MarqueeTransform:
                    this.EditClick(OptionType.MarqueeTransform);
                    break;
                case OptionType.Grow:
                    this.EditClick(OptionType.Grow);
                    break;
                case OptionType.Shrink:
                    this.EditClick(OptionType.Shrink);
                    break;
                case OptionType.Union:
                    break;
                case OptionType.Exclude:
                    break;
                case OptionType.Xor:
                    break;
                case OptionType.Intersect:
                    break;
                case OptionType.ExpandStroke:
                    break;
                default:
                    break;
            }
        }

        private bool EditClick(OptionType type)
        {

            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

            if (mode is PixelBoundsMode.Transarent)
            {
                this.Tip("No Pixel", "The Marquee is Transparent.");
                return false;
            }

            switch (type)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
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
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;
                default:
                    break;
            }

            this.OptionType = type;
            this.SetFootType(type);
            this.SetCanvasState(true);
            return true;
        }

    }
}