using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerManagerExtensions
    {

        public ArrangeHistory Cut(ILayerManager self, ILayer layer)
        {
            this.Copy(self, layer);
            return this.Remove(self, layer, true);
        }

        public ArrangeHistory Cut(ILayerManager self, IEnumerable<object> layers)
        {
            this.Copy(self, layers);
            return this.Remove(self, layers);
        }

        public void Copy(ILayerManager self, ILayer layer)
        {
            self.ClipboardLayers.Clear();
            self.ClipboardLayers.Add(layer.Id);
        }

        public void Copy(ILayerManager self, IEnumerable<object> layers)
        {
            self.ClipboardLayers.Clear();
            foreach (ILayer layer in layers)
            {
                self.ClipboardLayers.Add(layer.Id);
            }
        }

        public ArrangeHistory Paste(ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, string id)
        {
            ILayer add = LayerDictionary.Instance[id].Crop(resourceCreator, width, height);
            return this.Add(self, add);
        }

        public ArrangeHistory Paste(ILayerManager self, ICanvasResourceCreator resourceCreator, int width, int height, IEnumerable<string> ids)
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
                        ILayer layer = LayerDictionary.Instance[id];
                        ILayer add = layer.Crop(resourceCreator, width, height);
                        add.Arrange(parent.Depth + 1);

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
                        ILayer layer = LayerDictionary.Instance[id];
                        ILayer add = layer.Crop(resourceCreator, width, height);
                        add.Arrange(neighbor.Depth);

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
                    ILayer layer = LayerDictionary.Instance[id];
                    ILayer add = layer.Crop(resourceCreator, width, height);
                    add.Arrange(0);

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