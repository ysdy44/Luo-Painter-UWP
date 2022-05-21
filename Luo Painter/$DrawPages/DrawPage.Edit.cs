using Luo_Painter.Edits;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructEdits()
        {
            this.EditButton.ItemClick += (s, type) =>
            {
                switch (type)
                {
                    case EditType.None:
                        break;
                    case EditType.Cut: // Copy + Clear
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
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

                                this.PhaseButton.IsEnabled = true;
                            }
                            else
                            {
                                this.Tip("No Layer", "Create a new Layer?");
                            }
                        }
                        break;
                    case EditType.Duplicate:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                            {
                                int index = this.LayerListView.SelectedIndex;
                                if (index < 0) break;
                                if (index + 1 > this.ObservableCollection.Count) break;

                                string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                                BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasDevice, bitmapLayer);
                                this.Layers.Add(bitmapLayer2.Id, bitmapLayer2);
                                this.ObservableCollection.Insert(index, bitmapLayer2);
                                this.LayerListView.SelectedIndex = index;

                                // History
                                string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                            }
                            else
                            {
                                this.Tip("No Layer", "Create a new Layer?");
                            }
                        }
                        break;
                    case EditType.Copy:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
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

                                this.PhaseButton.IsEnabled = true;
                            }
                        }
                        break;
                    case EditType.Paste:
                        {
                            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                            if (mode is PixelBoundsMode.Transarent)
                            {
                                this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                                break;
                            }
                            int index = this.LayerListView.SelectedIndex;
                            if (index < 0) break;
                            if (index + 1 > this.ObservableCollection.Count) break;

                            string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                            BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasDevice, this.Clipboard.Source, this.Transformer.Width, this.Transformer.Height);

                            this.Layers.Add(bitmapLayer2.Id, bitmapLayer2);
                            this.ObservableCollection.Insert(index, bitmapLayer2);
                            this.LayerListView.SelectedIndex = index;

                            // History
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Clear:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
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
                            }

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Remove:
                        this.Remove();
                        break;
                    case EditType.Extract:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                            {
                                Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                                PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                                if (mode is PixelBoundsMode.Transarent)
                                {
                                    this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                                    break;
                                }

                                int index = this.LayerListView.SelectedIndex;
                                if (index < 0) break;
                                if (index + 1 > this.ObservableCollection.Count) break;

                                string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                                BitmapLayer bitmapLayer2;
                                switch (mode)
                                {
                                    case PixelBoundsMode.None:
                                        bitmapLayer2 = new BitmapLayer(this.CanvasDevice, bitmapLayer.GetMask(this.Marquee), this.Transformer.Width, this.Transformer.Height);
                                        break;
                                    default:
                                        bitmapLayer2 = new BitmapLayer(this.CanvasDevice, bitmapLayer);
                                        break;
                                }
                                this.Layers.Add(bitmapLayer2.Id, bitmapLayer2);
                                this.ObservableCollection.Insert(index, bitmapLayer2);
                                this.LayerListView.SelectedIndex = index;

                                // History
                                string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                            }
                        }
                        break;
                    case EditType.Merge:
                        {
                            int index = this.LayerListView.SelectedIndex;
                            if (index < 0) break;
                            if (index + 2 > this.ObservableCollection.Count) break;

                            if (this.ObservableCollection[index] is ILayer current)
                            {
                                if (this.ObservableCollection[index + 1] is ILayer previous)
                                {
                                    string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                                    ICanvasImage image = current.Render(previous.Source, current.Source);
                                    BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, image, this.Transformer.Width, this.Transformer.Height);
                                    this.Layers.Add(bitmapLayer.Id, bitmapLayer);

                                    this.ObservableCollection.Remove(current);
                                    this.ObservableCollection.Remove(previous);
                                    this.ObservableCollection.Insert(index, bitmapLayer);
                                    this.LayerListView.SelectedIndex = index;

                                    // History
                                    string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                                    int removes = this.History.Push(new ArrangeHistory(undo, redo));

                                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                            }
                        }
                        break;
                    case EditType.Flatten:
                        {
                            int index = this.LayerListView.SelectedIndex;
                            if (index < 0) break;
                            if (index + 1 > this.ObservableCollection.Count) break;
                            if (1 >= this.ObservableCollection.Count) break;

                            string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                            ICanvasImage image = this.Render(this.Transparent);
                            BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, image, this.Transformer.Width, this.Transformer.Height);
                            this.Layers.Add(bitmapLayer.Id, bitmapLayer);

                            this.ObservableCollection.Clear();
                            this.ObservableCollection.Add(bitmapLayer);
                            this.LayerListView.SelectedIndex = 0;

                            // History
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Group:
                        break;
                    case EditType.Ungroup:
                        break;
                    case EditType.Release:
                        break;
                    case EditType.All:
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
                    case EditType.Deselect:
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
                    case EditType.Invert:
                        {
                            // History
                            int removes = this.History.Push(this.Marquee.Invert(Colors.DodgerBlue));
                            this.Marquee.Flush();
                            this.Marquee.RenderThumbnail();

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Pixel:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
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
                    case EditType.Feather:
                        this.EditClick(EditType.Feather);
                        break;
                    case EditType.Transform:
                        this.EditClick(EditType.Transform);
                        break;
                    case EditType.Grow:
                        this.EditClick(EditType.Grow);
                        break;
                    case EditType.Shrink:
                        this.EditClick(EditType.Shrink);
                        break;
                    case EditType.Union:
                        break;
                    case EditType.Exclude:
                        break;
                    case EditType.Xor:
                        break;
                    case EditType.Intersect:
                        break;
                    case EditType.ExpandStroke:
                        break;
                    default:
                        break;
                }
            };
            this.SetupButton.ItemClick += (s, type) =>
            {
                switch (type)
                {
                    case EditType.Crop:
                        break;
                    case EditType.Stretch:
                        break;
                    case EditType.FlipHorizontal:
                        break;
                    case EditType.FlipVertical:
                        break;
                    case EditType.LeftTurn:
                        break;
                    case EditType.RightTurn:
                        break;
                    case EditType.OverTurn:
                        break;
                    default:
                        break;
                }
            };
        }

        private bool EditClick(EditType type)
        {
            this.ExpanderLightDismissOverlay.Hide();

            Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

            if (mode is PixelBoundsMode.Transarent)
            {
                this.Tip("No Pixel", "The Marquee is Transparent.");
                return false;
            }

            switch (type)
            {
                case EditType.Feather:
                    break;
                case EditType.Transform:
                    switch (mode)
                    {
                        case PixelBoundsMode.None:
                            PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(interpolationColors);
                            PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds);
                            this.SetTransform(bounds);
                            break;
                        default:
                            this.SetTransform(this.Marquee.Bounds);
                            break;
                    }
                    break;
                case EditType.Grow:
                    break;
                case EditType.Shrink:
                    break;
                default:
                    break;
            }

            this.FootType = this.SetFootType(type, default, this.ToolType);
            this.EditType = type;
            this.OptionType = default;
            this.SetCanvasState(true);
            return true;
        }

    }
}