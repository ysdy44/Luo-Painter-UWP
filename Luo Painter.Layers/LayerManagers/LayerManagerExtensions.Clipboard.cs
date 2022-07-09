using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public static partial class LayerManagerExtensions
    {

        public static ArrangeHistory Cut(this ILayerManager self, ILayer layer)
        {
            self.Copy(layer);
            return self.Remove(layer);
        }

        public static ArrangeHistory Cut(this ILayerManager self, IEnumerable<object> layers)
        {
            self.Copy(layers);
            return self.Remove(layers);
        }

        public static void Copy(this ILayerManager self, ILayer layer)
        {
            self.ClipboardLayers.Clear();
            self.ClipboardLayers.Add(layer.Id);
        }

        public static void Copy(this ILayerManager self, IEnumerable<object> layers)
        {
            self.ClipboardLayers.Clear();
            foreach (ILayer layer in layers)
            {
                self.ClipboardLayers.Add(layer.Id);
            }
        }

        public static ArrangeHistory Paste(this ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, string id)
        {
            ILayer add = self.Layers[id].Crop(resourceCreator, width, height);
            self.Layers.PushChild(add);
            return self.Add(add);
        }

        public static ArrangeHistory Paste(this ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, IEnumerable<string> ids)
        {
            Layerage[] undo = self.Nodes.Convert();

            int index = self.LayerSelectedIndex;
            if (index > 0 && self.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = self.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    // Homestay
                    foreach (string id in ids)
                    {
                        ILayer layer = self.Layers[id];
                        ILayer add = layer.Crop(resourceCreator, width, height);
                        add.Arrange(parent.Depth + 1);
                        self.Layers.PushChild(add);

                        self.Nodes.Insert(index, add);
                        self.ObservableCollection.InsertChild(index, add);
                    }
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    // Homestay
                    foreach (string id in ids)
                    {
                        ILayer layer = self.Layers[id];
                        ILayer add = layer.Crop(resourceCreator, width, height);
                        add.Arrange(neighbor.Depth);
                        self.Layers.PushChild(add);

                        parent.Children.Insert(indexChild, add);
                        self.ObservableCollection.InsertChild(index, add);
                    }
                }

                self.LayerSelectedIndex = index;
            }
            else
            {
                // Homestay
                foreach (string id in ids)
                {
                    ILayer layer = self.Layers[id];
                    ILayer add = layer.Crop(resourceCreator, width, height);
                    add.Arrange(0);
                    self.Layers.PushChild(add);

                    self.Nodes.Insert(0, add);
                    self.ObservableCollection.InsertChild(0, add);
                }
                self.LayerSelectedIndex = 0;
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}