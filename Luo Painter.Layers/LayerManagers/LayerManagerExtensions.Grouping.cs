using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public static partial class LayerManagerExtensions
    {

        public static ArrangeHistory Group(this ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, ILayer layer)
        {
            Layerage[] undo = self.Nodes.Convert();

            int depth = layer.Depth;
            layer.Arrange(depth + 1);

            ILayer add = new BitmapLayer(resourceCreator, width, height)
            {
                Depth = depth,
                Children = { layer }
            };
            self.Layers.Add(add.Id, add);

            // Run away from Home
            // Homestay
            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null)
            {
                int indexChild = self.Nodes.IndexOf(layer);
                self.Nodes.Remove(layer);
                self.Nodes.Insert(indexChild, add);
            }
            else
            {
                int indexChild = parent.Children.IndexOf(layer);
                parent.Children.Remove(layer);
                parent.Children.Insert(indexChild, add);
            }

            int index = self.LayerSelectedIndex;
            self.ObservableCollection.Insert(index, add);
            self.LayerSelectedIndex = index;

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public static ArrangeHistory Group(this ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, IEnumerable<object> layers)
        {
            Layerage[] undo = self.Nodes.Convert();

            ILayer add = new BitmapLayer(resourceCreator, width, height);
            self.Layers.Push(add);

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

    }
}