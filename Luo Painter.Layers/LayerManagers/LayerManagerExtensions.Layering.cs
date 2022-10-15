using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerManagerExtensions
    {

        public ArrangeHistory Clear(ILayerManager self, ILayer add)
        {
            Layerage[] undo = self.Nodes.Convert();

            self.Nodes.Clear();
            self.Nodes.Add(add);
            self.ObservableCollection.Clear();
            self.ObservableCollection.Add(add);
            self.LayerSelectedIndex = 0;

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }
        public ArrangeHistory Setup(ILayerManager self, IEnumerable<ILayer> adds, SetupSizes sizes = null)
        {
            int index = self.LayerSelectedIndex;

            Layerage[] undo = self.Nodes.Convert();

            self.Nodes.Clear();
            foreach (ILayer item in adds)
            {
                self.Nodes.Add(item);
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in adds)
            {
                item.Arrange(0);
                self.ObservableCollection.AddChild(item);
            }

            self.LayerSelectedIndex = index;

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return (new ArrangeHistory(undo, redo));
        }

        public ArrangeHistory Add(ILayerManager self, ILayer add)
        {
            Layerage[] undo = self.Nodes.Convert();

            int index = self.LayerSelectedIndex;
            if (index > 0 && self.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = self.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = self.Nodes.IndexOf(neighbor);
                    self.Nodes.Insert(indexChild, add);
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);
                    parent.Children.Insert(indexChild, add);
                }

                add.Arrange(neighbor.Depth);
                add.IsExist = true;
                add.Exist(true);

                self.ObservableCollection.InsertChild(index, add);
                self.LayerSelectedIndex = index;
            }
            else
            {
                add.Arrange(0);
                add.IsExist = true;
                add.Exist(true);

                self.Nodes.Insert(0, add);
                self.ObservableCollection.InsertChild(0, add);
                self.LayerSelectedIndex = 0;
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(ILayerManager self, ILayer layer, bool resetSelectedIndex)
        {
            Layerage[] undo = self.Nodes.Convert();

            // Run away from Home
            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null) self.Nodes.Remove(layer);
            else
            {
                parent.Children.Remove(layer);
                parent.RaiseCanExecuteChanged();
            }

            if (resetSelectedIndex)
            {
                int index = self.LayerSelectedIndex;
                self.ObservableCollection.RemoveChild(layer);
                self.LayerSelectedIndex = self.ObservableCollection.GetSelectedIndex(index, layer.Depth);
            }
            else
            {
                self.ObservableCollection.RemoveChild(layer);
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(ILayerManager self, IEnumerable<object> layers)
        {
            Layerage[] undo = self.Nodes.Convert();

            foreach (ILayer layer in layers.Cast<ILayer>())
            {
                // Run away from Home
                ILayer parent = self.ObservableCollection.GetParent(layer);
                if (parent is null) self.Nodes.Remove(layer);
                else
                {
                    parent.Children.Remove(layer);
                    parent.RaiseCanExecuteChanged();
                }
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in self.Nodes)
            {
                item.Arrange(0);
                self.ObservableCollection.AddChild(item);
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}