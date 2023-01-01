using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerRootNodes
    {

        public ArrangeHistory Clear(ILayerManager self, ILayer add)
        {
            Layerage[] undo = base.Convert();

            base.Clear();
            base.Add(add);
            self.ObservableCollection.Clear();
            self.ObservableCollection.Add(add);
            self.LayerSelectedIndex = 0;

            /// History
            Layerage[] redo = base.Convert();
            return new ArrangeHistory(undo, redo);
        }
        public ArrangeHistory Setup(ILayerManager self, IEnumerable<ILayer> adds)
        {
            int index = self.LayerSelectedIndex;

            Layerage[] undo = base.Convert();

            base.Clear();
            foreach (ILayer item in adds)
            {
                base.Add(item);
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in adds)
            {
                item.Arrange(0);
                self.ObservableCollection.AddChild(item);
            }

            self.LayerSelectedIndex = index;

            /// History
            Layerage[] redo = base.Convert();
            return (new ArrangeHistory(undo, redo));
        }

        public ArrangeHistory Add(ILayerManager self, ILayer add)
        {
            Layerage[] undo = base.Convert();

            int index = self.LayerSelectedIndex;
            if (index > 0 && self.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = self.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = base.IndexOf(neighbor);
                    base.Insert(indexChild, add);
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

                base.Insert(0, add);
                self.ObservableCollection.InsertChild(0, add);
                self.LayerSelectedIndex = 0;
            }

            /// History
            Layerage[] redo = base.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(ILayerManager self, ILayer layer, bool resetSelectedIndex)
        {
            Layerage[] undo = base.Convert();

            // Run away from Home
            ILayer parent = self.ObservableCollection.GetParent(layer);
            if (parent is null) base.Remove(layer);
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
            Layerage[] redo = base.Convert();
            return new ArrangeHistory(undo, redo);
        }

        public ArrangeHistory Remove(ILayerManager self, IEnumerable<object> layers)
        {
            Layerage[] undo = base.Convert();

            foreach (ILayer layer in layers.Cast<ILayer>())
            {
                // Run away from Home
                ILayer parent = self.ObservableCollection.GetParent(layer);
                if (parent is null) base.Remove(layer);
                else
                {
                    parent.Children.Remove(layer);
                    parent.RaiseCanExecuteChanged();
                }
            }

            self.ObservableCollection.Clear();
            foreach (ILayer item in this)
            {
                item.Arrange(0);
                self.ObservableCollection.AddChild(item);
            }

            /// History
            Layerage[] redo = base.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}