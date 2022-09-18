using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerManagerExtensions
    {

        public ArrangeHistory Group(ILayerManager self, ILayer add, ILayer layer)
        {
            Layerage[] undo = self.Nodes.Convert();

            add.Children.Add(layer);

            // Run away from Home
            // Homestay
            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null)
            {
                int indexChild = self.Nodes.IndexOf(layer);
                self.Nodes.Remove(layer);
                self.Nodes.Insert(indexChild, add);

                add.Arrange(0);
            }
            else
            {
                int indexChild = parent.Children.IndexOf(layer);
                parent.Children.Remove(layer);
                parent.Children.Insert(indexChild, add);

                add.Arrange(parent.Depth + 1);
            }

            int index = self.LayerSelectedIndex;
            self.ObservableCollection.Insert(index, add);
            self.LayerSelectedIndex = index;

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Group(ILayerManager self, ILayer add, IEnumerable<object> layers)
        {
            Layerage[] undo = self.Nodes.Convert();

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
                    parent2 = self.ObservableCollection.GetParent(layer);
                    if (parent2 is null)
                    {
                        indexChild2 = self.Nodes.IndexOf(layer);
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
                ILayer parent = self.ObservableCollection.GetParent(layer);
                if (parent is null)
                {
                    self.Nodes.Remove(layer);
                }
                else
                {
                    parent.Children.Remove(layer);
                }
            }

            // Homestay
            if (parent2 is null)
            {
                if (indexChild2 >= self.Nodes.Count)
                    self.Nodes.Add(add);
                else
                    self.Nodes.Insert(indexChild2, add);
                add.Arrange(0);
            }
            else
            {
                parent2.Children.Insert(indexChild2, add);
                add.Arrange(parent2.Depth + 1);
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in self.Nodes)
            {
                item.Arrange(0);
                self.ObservableCollection.AddChild(item);
            }

            self.LayerSelectedIndex = self.ObservableCollection.IndexOf(add);

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Ungroup(ILayerManager self, ILayer layer)
        {
            Layerage[] undo = self.Nodes.Convert();

            int index = self.LayerSelectedIndex;

            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null)
            {
                int indexChild = self.Nodes.IndexOf(layer);
                self.Nodes.Remove(layer);

                // Homestay
                foreach (ILayer item in layer.Children)
                {
                    self.Nodes.Insert(indexChild, item);
                    item.Arrange(0);
                }
                // Run away from Home
                layer.Children.Clear();
            }
            else
            {
                int indexChild = parent.Children.IndexOf(layer);
                parent.Children.Remove(layer);

                // Homestay
                foreach (ILayer item in layer.Children)
                {
                    parent.Children.Insert(indexChild, item);
                    item.Arrange(parent.Depth + 1);
                }
                // Run away from Home
                layer.Children.Clear();
            }

            self.ObservableCollection.Remove(layer);

            self.LayerSelectedIndex = index;

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Release(ILayerManager self, ILayer layer)
        {
            Layerage[] undo;

            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null)
            {
                return null;
            }
            else
            {
                undo = self.Nodes.Convert();

                // Run away from Home
                parent.Children.Remove(layer);

                ILayer grand = self.ObservableCollection.GetParent(parent);
                if (grand is null)
                {
                    int indexChild = self.Nodes.IndexOf(parent);

                    // Homestay
                    self.Nodes.Insert(indexChild, layer);
                    layer.Arrange(1);
                }
                else
                {
                    int indexChild = grand.Children.IndexOf(parent);

                    // Homestay
                    grand.Children.Insert(indexChild, layer);
                    layer.Arrange(parent.Depth);
                }
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in self.Nodes)
            {
                self.ObservableCollection.AddChild(item);
            }

            self.LayerSelectedIndex = self.ObservableCollection.IndexOf(layer);

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Release(ILayerManager self, IEnumerable<object> layers)
        {
            Layerage[] undo = self.Nodes.Convert();

            foreach (ILayer layer in layers)
            {
                ILayer parent = self.ObservableCollection.GetParent(layer);
                if (parent is null)
                {
                }
                else
                {
                    // Run away from Home
                    parent.Children.Remove(layer);

                    ILayer grand = self.ObservableCollection.GetParent(parent);
                    if (grand is null)
                    {
                        int indexChild = self.Nodes.IndexOf(parent);

                        // Homestay
                        self.Nodes.Insert(indexChild, layer);
                        layer.Arrange(1);
                    }
                    else
                    {
                        int indexChild = grand.Children.IndexOf(parent);

                        // Homestay
                        grand.Children.Insert(indexChild, layer);
                        layer.Arrange(parent.Depth);
                    }
                }
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in self.Nodes)
            {
                self.ObservableCollection.AddChild(item);
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}