using Luo_Painter.Models;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerRootNodes
    {

        public void DragItemsStarting(ILayerManager self, IList<object> layers)
        {
            foreach (ILayer item in layers)
            {
                item.CacheIsExpand();
            }
        }

        public ArrangeHistory DragItemsCompleted(ILayerManager self, IReadOnlyList<object> layers)
        {
            Layerage[] undo = base.Convert();

            foreach (ILayer item in layers)
            {
                self.ObservableCollection.RemoveDragItems(item);
            }

            foreach (ILayer item in layers)
            {
                self.ObservableCollection.AddDragItems(item);
            }

            foreach (ILayer item in layers)
            {
                item.ApplyIsExpand();
            }

            base.Clear();
            foreach (ILayer item in self.ObservableCollection)
            {
                if (item.Depth is 0)
                {
                    base.Add(item);
                }
            }

            /// History
            Layerage[] redo = base.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}