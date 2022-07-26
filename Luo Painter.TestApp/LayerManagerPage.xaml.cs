using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal class VisibilityButton : Button
    {
        public VisibilityButton()
        {
            base.Click += (s, e) =>
            {
                if (base.Content is UIElement content)
                {
                    content.Visibility = content.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
            };
        }
    }

    public sealed partial class LayerManagerPage : Page, ILayerManager
    {

        readonly CanvasDevice CanvasDevice = new CanvasDevice();
        readonly Historian<IHistory> History = new Historian<IHistory>(20);

        public LayerNodes Nodes { get; }
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.ListView.SelectedIndex; set => this.ListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.ListView.SelectedItem; set => this.ListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.ListView.SelectedItems;

        public LayerManagerPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();
            this.ConstructLayer();
            this.ConstructHistory();


            this.Nodes = new LayerNodes
            {
                new GroupLayer(this.CanvasDevice, 128, 128)
                {
                    Children =
                    {
                        new GroupLayer(this.CanvasDevice, 128, 128)
                        {
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128, 128),
                                new BitmapLayer(this.CanvasDevice, 128, 128)
                            }
                        },
                        new GroupLayer(this.CanvasDevice, 128, 128)
                        {
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128, 128),
                                new BitmapLayer(this.CanvasDevice, 128, 128)
                             }
                        }
                    }
                }
            };


            this.Push(this.Nodes);

            foreach (ILayer item in this.Nodes)
            {
                item.Arrange(0);
                this.ObservableCollection.AddChild(item);
            }
        }

        private void Push(LayerNodes layers)
        {
            foreach (ILayer item in layers)
            {
                this.Push(item.Children);
            }
        }

        private void ConstructLayers()
        {
            this.ListView.DragItemsStarting += (s, e) => this.DragItemsStarting(e.Items);
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                /// History
                int removes = this.History.Push(this.DragItemsCompleted(e.Items));

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

        private void ConstructLayer()
        {
            this.AddButton.Click += (s, e) =>
            {
                ILayer add = new BitmapLayer(this.CanvasDevice, 128, 128);

                /// History
                int removes = this.History.Push(this.Add(add));

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            this.RemoveButton.Click += (s, e) =>
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

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Remove(items));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };
            this.GroupButton.Click += (s, e) =>
            {
                var items = this.LayerSelectedItems;
                switch (items.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            ILayer add = new GroupLayer(this.CanvasDevice, 128, 128);

                            /// History
                            int removes = this.History.Push(this.Group(add, layer));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        {
                            ILayer add = new GroupLayer(this.CanvasDevice, 128, 128);

                            /// History
                            int removes = this.History.Push(this.Group(add, items));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };
            this.UngroupButton.Click += (s, e) =>
            {
                if (this.LayerSelectedItem is ILayer layer)
                {
                    if (layer.Children.Count is 0) return;

                    /// History
                    int removes = this.History.Push(this.Ungroup(layer));

                    this.UndoButton.IsEnabled = this.History.CanUndo;
                    this.RedoButton.IsEnabled = this.History.CanRedo;
                }
            };
            this.ReleaseButton.Click += (s, e) =>
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

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                            }
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Release(items));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };


            this.CutButton.Click += (s, e) =>
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

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            this.PasteButton.IsEnabled = this.ClipboardLayers.Count is 0 is false;
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Cut(items));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            this.PasteButton.IsEnabled = this.ClipboardLayers.Count is 0 is false;
                        }
                        break;
                }
            };
            this.CopyButton.Click += (s, e) =>
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
                        this.PasteButton.IsEnabled = this.ClipboardLayers.Count is 0 is false;
                        break;
                    default:
                        this.Copy(items);
                        this.PasteButton.IsEnabled = this.ClipboardLayers.Count is 0 is false;
                        break;
                }
            };
            this.PasteButton.Click += (s, e) =>
            {
                switch (this.ClipboardLayers.Count)
                {
                    case 0:
                        break;
                    case 1:
                        string id = this.ClipboardLayers.Single();
                        if (LayerDictionary.Instance.ContainsKey(id))
                        {
                            /// History
                            int removes = this.History.Push(this.Paste(this.CanvasDevice, 128, 128, id));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Paste(this.CanvasDevice, 128, 128, this.ClipboardLayers));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };
            this.MergeButton.Click += (s, e) =>
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
            };
            this.FlattenButton.Click += (s, e) =>
            {
                using (CanvasCommandList commandList = new CanvasCommandList(this.CanvasDevice))
                {
                    ICanvasImage image = this.Nodes.Merge(null, commandList);
                    ILayer add = new BitmapLayer(this.CanvasDevice, image, 128, 128);

                    /// History
                    int removes = this.History.Push(this.Clear(add));
                }

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

        private void ConstructHistory()
        {
            this.UndoButton.Click += (s, e) =>
            {
                if (this.History.CanUndo is false) return;

                // History
                bool result = this.History.Undo(this.Undo);
                if (result is false) return;

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            this.RedoButton.Click += (s, e) =>
            {
                if (this.History.CanRedo is false) return;

                // History
                bool result = this.History.Redo(this.Redo);
                if (result is false) return;

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

        public bool Undo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        this.Nodes.Clear();
                        this.ObservableCollection.Clear();
                        foreach (ILayer item in LayerDictionary.Instance.Permutation(arrangeHistory.UndoParameter))
                        {
                            this.Nodes.Add(item);
                            this.ObservableCollection.AddChild(item);
                        }
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }
        public bool Redo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        this.Nodes.Clear();
                        this.ObservableCollection.Clear();
                        foreach (ILayer item in LayerDictionary.Instance.Permutation(arrangeHistory.RedoParameter))
                        {
                            this.Nodes.Add(item);
                            this.ObservableCollection.AddChild(item);
                        }
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

    }
}