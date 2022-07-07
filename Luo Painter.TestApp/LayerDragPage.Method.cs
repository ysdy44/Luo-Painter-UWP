using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class LayerDragPage : Page
    {

        public ArrangeHistory Add()
        {
            ILayer add = new BitmapLayer(this.CanvasDevice, 128, 128);
            this.Layers.Push(add);
            return this.Add(add);
        }

        public ArrangeHistory Add(ILayer add)
        {
            Layerage[] undo = this.Nodes.Convert();

            int index = this.ListView.SelectedIndex;
            if (index > 0 && this.ListView.SelectedItem is ILayer neighbor)
            {
                ILayer parent = this.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = this.Nodes.IndexOf(neighbor);
                    this.Nodes.Insert(indexChild, add);
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);
                    parent.Children.Insert(indexChild, add);
                }

                add.Arrange(neighbor.Depth);
                add.IsExist = true;
                add.Exist(true);

                this.ObservableCollection.InsertChild(index, add);
                this.ListView.SelectedIndex = index;
            }
            else
            {
                add.Arrange(0);
                add.IsExist = true;
                add.Exist(true);

                this.Nodes.Insert(0, add);
                this.ObservableCollection.InsertChild(0, add);
                this.ListView.SelectedIndex = 0;
            }

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(ILayer layer)
        {
            Layerage[] undo = this.Nodes.Convert();

            // Run away from Home
            ILayer parent = this.ObservableCollection.GetParent(layer);
            if (parent is null) this.Nodes.Remove(layer);
            else
            {
                parent.Children.Remove(layer);
                parent.RaiseCanExecuteChanged();
            }

            int index = this.ListView.SelectedIndex;
            {
                this.ObservableCollection.RemoveChild(layer);
            }
            this.ListView.SelectedIndex = this.ObservableCollection.GetSelectedIndex(index, layer.Depth);

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(IEnumerable<object> layers)
        {
            Layerage[] undo = this.Nodes.Convert();

            foreach (ILayer layer in layers)
            {
                // Run away from Home
                ILayer parent = this.ObservableCollection.GetParent(layer);
                if (parent is null) this.Nodes.Remove(layer);
                else
                {
                    parent.Children.Remove(layer);
                    parent.RaiseCanExecuteChanged();
                }
            }

            this.ObservableCollection.Clear();
            foreach (ILayer item in this.Nodes)
            {
                item.Arrange(0);
                this.ObservableCollection.AddChild(item);
            }

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Group(ILayer layer)
        {
            Layerage[] undo = this.Nodes.Convert();

            int depth = layer.Depth;
            layer.Arrange(depth + 1);

            ILayer add = new BitmapLayer(this.CanvasDevice, 128, 128)
            {
                Depth = depth,
                Children = { layer }
            };
            this.Layers.Add(add.Id, add);

            // Run away from Home
            // Homestay
            ILayer parent = this.ObservableCollection.GetParent(layer);
            if (parent is null)
            {
                int indexChild = this.Nodes.IndexOf(layer);
                this.Nodes.Remove(layer);
                this.Nodes.Insert(indexChild, add);
            }
            else
            {
                int indexChild = parent.Children.IndexOf(layer);
                parent.Children.Remove(layer);
                parent.Children.Insert(indexChild, add);
            }

            int index = this.ListView.SelectedIndex;
            this.ObservableCollection.Insert(index, add);
            this.ListView.SelectedIndex = index;

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Group(IEnumerable<object> layers)
        {
            Layerage[] undo = this.Nodes.Convert();

            ILayer add = new BitmapLayer(this.CanvasDevice, 128, 128);
            this.Layers.Push(add);

            int depth = int.MaxValue;
            foreach (ILayer layer in layers)
            {
                add.Children.Add(layer);

                if (depth > layer.Depth)
                {
                    depth = layer.Depth;
                }
            }

            ILayer parent2 = null;
            int indexChild2 = 0;
            foreach (ILayer layer in layers)
            {
                if (depth == layer.Depth)
                {
                    parent2 = this.ObservableCollection.GetParent(layer);
                    if (parent2 is null)
                    {
                        indexChild2 = this.Nodes.IndexOf(layer);
                    }
                    else
                    {
                        indexChild2 = parent2.Children.IndexOf(layer);
                    }
                    break;
                }
            }

            foreach (ILayer layer in layers)
            {
                // Run away from Home
                ILayer parent = this.ObservableCollection.GetParent(layer);
                if (parent is null)
                {
                    this.Nodes.Remove(layer);
                }
                else
                {
                    parent.Children.Remove(layer);
                }
            }

            // Homestay
            if (parent2 is null)
            {
                this.Nodes.Insert(indexChild2, add);
                add.Arrange(0);
            }
            else
            {
                parent2.Children.Insert(indexChild2, add);
                add.Arrange(parent2.Depth + 1);
            }

            this.ObservableCollection.Clear();
            foreach (ILayer item in this.Nodes)
            {
                item.Arrange(0);
                this.ObservableCollection.AddChild(item);
            }

            this.ListView.SelectedIndex = this.ObservableCollection.IndexOf(add);

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }



        public ArrangeHistory Cut(ILayer layer)
        {
            this.Copy(layer);
            return this.Remove(layer);
        }

        public ArrangeHistory Cut(IEnumerable<object> layers)
        {
            this.Copy(layers);
            return this.Remove(layers);
        }

        public void Copy(ILayer layer)
        {
            this.ClipboardLayers.Clear();
            this.ClipboardLayers.Add(layer.Id);
        }

        public void Copy(IEnumerable<object> layers)
        {
            this.ClipboardLayers.Clear();
            foreach (ILayer layer in layers)
            {
                this.ClipboardLayers.Add(layer.Id);
            }
        }

        public ArrangeHistory Paste(string id)
        {
            ILayer add = this.Layers[id].Crop(this.CanvasDevice, 128, 128);
            this.Layers.PushChild(add);
            return this.Add(add);
        }

        public ArrangeHistory Paste(IEnumerable<string> ids)
        {
            Layerage[] undo = this.Nodes.Convert();

            int index = this.ListView.SelectedIndex;
            if (index > 0 && this.ListView.SelectedItem is ILayer neighbor)
            {
                ILayer parent = this.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    // Homestay
                    foreach (string id in ids)
                    {
                        ILayer layer = this.Layers[id];
                        ILayer add = layer.Crop(this.CanvasDevice, 128, 128);
                        add.Arrange(parent.Depth + 1);
                        this.Layers.PushChild(add);

                        this.Nodes.Insert(index, add);
                        this.ObservableCollection.InsertChild(index, add);
                    }
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    // Homestay
                    foreach (string id in ids)
                    {
                        ILayer layer = this.Layers[id];
                        ILayer add = layer.Crop(this.CanvasDevice, 128, 128);
                        add.Arrange(neighbor.Depth);
                        this.Layers.PushChild(add);

                        parent.Children.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                    }
                }

                this.ListView.SelectedIndex = index;
            }
            else
            {
                // Homestay
                foreach (string id in ids)
                {
                    ILayer layer = this.Layers[id];
                    ILayer add = layer.Crop(this.CanvasDevice, 128, 128);
                    add.Arrange(0);
                    this.Layers.PushChild(add);

                    this.Nodes.Insert(0, add);
                    this.ObservableCollection.InsertChild(0, add);
                }
                this.ListView.SelectedIndex = 0;
            }

            /// History
            Layerage[] redo = this.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}