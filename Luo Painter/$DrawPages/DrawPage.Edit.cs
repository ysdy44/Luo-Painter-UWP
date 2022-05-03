using Luo_Painter.Edits;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
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
                                        this.Clipboard.Copy(bitmapLayer, this.Marquee);

                                        // History
                                        int removes = this.History.Push(bitmapLayer.Clear(this.Marquee, interpolationColors));
                                        bitmapLayer.Flush();
                                        bitmapLayer.RenderThumbnail();
                                        break;
                                    default:
                                        this.Clipboard.Copy(bitmapLayer);

                                        // History
                                        int removes2 = this.History.Push(bitmapLayer.GetBitmapClearHistory(Colors.Transparent));
                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                                        bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                                        bitmapLayer.ClearThumbnail(Colors.Transparent);
                                        break;
                                }
                            }

                            this.CanvasControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;

                            this.PhaseButton.IsEnabled = true;
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

                                BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasControl, bitmapLayer.Source, this.Transformer.Width, this.Transformer.Height);
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
                    case EditType.Copy:
                        {
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                            {
                                Color[] interpolationColors = this.Marquee.GetInterpolationColorsBySource();
                                PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

                                switch (mode)
                                {
                                    case PixelBoundsMode.None:
                                        this.Clipboard.Copy(bitmapLayer, this.Marquee);
                                        break;
                                    default:
                                        this.Clipboard.Copy(bitmapLayer);
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

                            BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasControl, this.Clipboard.Source, this.Transformer.Width, this.Transformer.Height);

                            this.Layers.Add(bitmapLayer2.Id, bitmapLayer2);
                            this.ObservableCollection.Insert(index, bitmapLayer2);
                            this.LayerListView.SelectedIndex = index;

                            // History
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.CanvasControl.Invalidate(); // Invalidate

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

                            this.CanvasControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Remove:
                        {
                            int index = this.LayerListView.SelectedIndex;
                            if (index < 0) break;
                            if (index + 1 > this.ObservableCollection.Count) break;

                            foreach (string id in this.Ids().ToArray())
                            {
                                if (this.Layers.ContainsKey(id))
                                {
                                    ILayer layer = this.Layers[id];
                                    this.ObservableCollection.Remove(layer);
                                }
                            }

                            int index2 = System.Math.Min(index, this.ObservableCollection.Count - 1);
                            this.LayerListView.SelectedIndex = index2;
                            this.LayerTool.SetLayer(this.LayerListView.SelectedItem as ILayer);

                            this.CanvasControl.Invalidate(); // Invalidate
                        }
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

                                BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height);
                                switch (mode)
                                {
                                    case PixelBoundsMode.None:
                                        bitmapLayer2.Copy(bitmapLayer, this.Marquee);
                                        bitmapLayer2.RenderThumbnail();
                                        break;
                                    default:
                                        bitmapLayer2.Copy(bitmapLayer);
                                        bitmapLayer2.RenderThumbnail();
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
                                    BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasControl, image, this.Transformer.Width, this.Transformer.Height);
                                    this.Layers.Add(bitmapLayer.Id, bitmapLayer);

                                    this.ObservableCollection.Remove(current);
                                    this.ObservableCollection.Remove(previous);
                                    this.ObservableCollection.Insert(index, bitmapLayer);
                                    this.LayerListView.SelectedIndex = index;

                                    // History
                                    string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                                    int removes = this.History.Push(new ArrangeHistory(undo, redo));

                                    this.CanvasControl.Invalidate(); // Invalidate

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
                            BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasControl, image, this.Transformer.Width, this.Transformer.Height);
                            this.Layers.Add(bitmapLayer.Id, bitmapLayer);

                            this.ObservableCollection.Clear();
                            this.ObservableCollection.Add(bitmapLayer);
                            this.LayerListView.SelectedIndex = 0;

                            // History
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.CanvasControl.Invalidate(); // Invalidate

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

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                            }
                        }
                        break;
                    case EditType.Feather:
                        break;
                    case EditType.Transform:
                        break;
                    case EditType.Grow:
                        break;
                    case EditType.Shrink:
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

    }
}