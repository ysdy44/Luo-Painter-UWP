using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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

    public sealed partial class LayerDragPage : Page
    {

        readonly CanvasDevice CanvasDevice = new CanvasDevice();

        Historian<IHistory> History { get; } = new Historian<IHistory>(20);
        LayerDictionary Layers { get; } = new LayerDictionary();
        LayerNodes Nodes { get; }
        LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        IList<string> ClipboardLayers { get; } = new List<string>();

        public LayerDragPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();
            this.ConstructLayer();
            this.ConstructHistory();


            this.Nodes = new LayerNodes
            {
                new BitmapLayer(this.CanvasDevice, 128, 128)
                {
                    Children =
                    {
                        new BitmapLayer(this.CanvasDevice, 128, 128)
                        {
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128, 128),
                                new BitmapLayer(this.CanvasDevice, 128, 128)
                            }
                        },
                        new BitmapLayer(this.CanvasDevice, 128, 128)
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


            foreach (ILayer item in this.Nodes)
            {
                this.Layers.PushChild(item);

                item.Arrange(0);
                this.ObservableCollection.AddChild(item);
            }
        }

        private void ConstructLayers()
        {
            this.ListView.DragItemsStarting += (s, e) =>
            {
                //e.Cancel = true;

                foreach (ILayer item in e.Items)
                {
                    item.CacheIsExpand();
                }
            };
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                Layerage[] undo = this.Nodes.Convert();

                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.RemoveDragItems(item);
                }

                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.AddDragItems(item);
                }

                foreach (ILayer item in e.Items)
                {
                    item.ApplyIsExpand();
                }

                this.Nodes.Clear();
                foreach (ILayer item in this.ObservableCollection)
                {
                    if (item.Depth is 0)
                    {
                        this.Nodes.Add(item);
                    }
                }

                /// History
                Layerage[] redo = this.Nodes.Convert();
                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

        private void ConstructLayer()
        {
            this.AddButton.Click += (s, e) =>
            {
                /// History
                int removes = this.History.Push(this.Add());

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            this.RemoveButton.Click += (s, e) =>
            {
                var items = this.ListView.SelectedItems;
                switch (items.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.ListView.SelectedItem is ILayer layer)
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
                var items = this.ListView.SelectedItems;
                switch (items.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.ListView.SelectedItem is ILayer layer)
                        {
                            /// History
                            int removes = this.History.Push(this.Group(layer));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Group(items));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };


            this.CutButton.Click += (s, e) =>
            {
                var items = this.ListView.SelectedItems;
                switch (items.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.ListView.SelectedItem is ILayer layer)
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
                var items = this.ListView.SelectedItems;
                switch (items.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.ListView.SelectedItem is ILayer layer)
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
                        if (this.Layers.ContainsKey(id))
                        {
                            /// History
                            int removes = this.History.Push(this.Paste(id));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        {
                            /// History
                            int removes = this.History.Push(this.Paste(this.ClipboardLayers));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
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
                        foreach (ILayer item in this.Layers.Permutation(arrangeHistory.UndoParameter))
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
                        foreach (ILayer item in this.Layers.Permutation(arrangeHistory.RedoParameter))
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