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
    public class EditsCanvas : Canvas
    {

        readonly BitmapSize HSize = new BitmapSize { Width = 400, Height = 327 };
        readonly BitmapSize HItemSize = new BitmapSize { Width = 128, Height = 40 };
        readonly BitmapSize[] HCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 140, Height = 157 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 140, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 276, Height = 4 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 244 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Merge"/>

            new BitmapSize{ Width = 136, Height = 181 }, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 136, Height = 223 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 136, Height = 271 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 136, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 136, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 136, Height = 112 }, /// <see cref="EditType.Invert"/>

            new BitmapSize{ Width = 272, Height = 28 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 272, Height = 70 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 272, Height = 112 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 272, Height = 154 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 272, Height = 202 }, /// <see cref="EditType.ExpandStroke"/>
        };

        readonly BitmapSize VEditSize = new BitmapSize { Width = 320, Height = 486 };
        readonly BitmapSize VEditItemSize = new BitmapSize { Width = 156, Height = 40 };
        readonly BitmapSize[] VEditCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 4, Height = 331 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 168, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 168, Height = 157 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 244 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Merge"/>

            new BitmapSize{ Width = 0, Height = 355 }, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 0, Height = 397 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 0, Height = 445 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 164, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 164, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 164, Height = 112 }, /// <see cref="EditType.Invert"/>
       
            new BitmapSize{ Width = 164, Height = 181 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 164, Height = 223 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 164, Height = 265 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 164, Height = 307 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 164, Height = 355 }, /// <see cref="EditType.ExpandStroke"/>
        };

        #region DependencyProperty


        /// <summary> Gets or set the orientation for <see cref="EditsCanvas"/>. </summary>
        public Orientation Orientation
        {
            get => (Orientation)base.GetValue(OrientationProperty);
            set => base.SetValue(OrientationProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditsCanvas.Type" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(EditsCanvas), new PropertyMetadata(Orientation.Vertical, (sender, e) =>
        {
            EditsCanvas control = (EditsCanvas)sender;

            if (e.NewValue is Orientation value)
            {
                control.Resizing(value);
            }
        }));


        #endregion

        public void Resizing() => this.Resizing(this.Orientation);

        public void Resizing(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Vertical: this.Resizing(this.VEditSize, this.VEditItemSize, this.VEditCoordinates); break;
                case Orientation.Horizontal: this.Resizing(this.HSize, this.HItemSize, this.HCoordinates); break;
                default: break;
            }
        }

        public void Resizing(BitmapSize size, BitmapSize itemSize, BitmapSize[] coordinates)
        {
            base.Width = size.Width;
            base.Height = size.Height;
            for (int i = 0; i < base.Children.Count; i++)
            {
                FrameworkElement item = base.Children[i] as FrameworkElement;
                item.Width = itemSize.Width;
                item.Height = itemSize.Height;

                BitmapSize p = coordinates[i];
                Canvas.SetLeft(item, p.Width);
                Canvas.SetTop(item, p.Height);
            }
        }
    }

    public sealed partial class DrawPage : Page
    {
        EditTypeCommand EditTypeCommand { get; } = new EditTypeCommand();

        private void ConstructEdits()
        {
            this.EditsCanvas.Resizing();

            this.EditButton.Click += (s, e) =>
            {
                switch (this.EditsCanvas.Orientation)
                {
                    case Orientation.Vertical: this.EditFlyout.ShowAt(this.OptionButton); break;
                    case Orientation.Horizontal: this.EditFlyout.ShowAt(this.EditButton); break;
                    default: break;
                }
            };

            this.EditTypeCommand.Click += (s, type) =>
            {
                this.EditFlyout.Hide();

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
                                        bitmapLayer.Clear(Colors.Transparent);
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
                                if (index + 1 >= this.ObservableCollection.Count) break;

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
                            if (index + 1 >= this.ObservableCollection.Count) break;

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
                                        bitmapLayer.Clear(Colors.Transparent);
                                        bitmapLayer.ClearThumbnail(Colors.Transparent);
                                        break;
                                }
                            }

                            this.CanvasControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
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
                                if (index + 1 >= this.ObservableCollection.Count) break;

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
                            if (index + 1 >= this.ObservableCollection.Count) break;

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

                                    this.UndoButton.IsEnabled = this.History.CanUndo;
                                    this.RedoButton.IsEnabled = this.History.CanRedo;
                                }
                            }
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
                            this.Marquee.Clear(Colors.DodgerBlue);
                            this.Marquee.ClearThumbnail(Colors.DodgerBlue);

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case EditType.Deselect:
                        {
                            // History
                            int removes = this.History.Push(this.Marquee.GetBitmapClearHistory(Colors.Transparent));
                            this.Marquee.Clear(Colors.Transparent);
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
        }

    }
}