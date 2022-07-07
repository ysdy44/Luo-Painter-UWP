using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
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

        public LayerDragPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();
            this.ConstructLayer();
            this.ConstructHistory();

            ILayer c4 = new BitmapLayer(this.CanvasDevice, 128, 128);
            ILayer c3 = new BitmapLayer(this.CanvasDevice, 128, 128);
            ILayer c2 = new BitmapLayer(this.CanvasDevice, 128, 128);
            ILayer c1 = new BitmapLayer(this.CanvasDevice, 128, 128);
            ILayer b2 = new BitmapLayer(this.CanvasDevice, 128, 128) { Children = { c3, c4 } };
            ILayer b1 = new BitmapLayer(this.CanvasDevice, 128, 128) { Children = { c1, c2 } };
            ILayer a = new BitmapLayer(this.CanvasDevice, 128, 128) { Children = { b1, b2 } };

            this.Layers.Add(a.Id, a);
            this.Layers.Add(b1.Id, b1);
            this.Layers.Add(b2.Id, b2);
            this.Layers.Add(c1.Id, c1);
            this.Layers.Add(c2.Id, c2);
            this.Layers.Add(c3.Id, c3);
            this.Layers.Add(c4.Id, c4);

            this.Nodes = new LayerNodes { a };
            foreach (ILayer item in this.Nodes)
            {
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