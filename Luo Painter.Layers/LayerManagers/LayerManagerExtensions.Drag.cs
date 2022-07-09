using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public static partial class LayerManagerExtensions
    {

        public static void DragItemsStarting(this ILayerManager self, IList<object> layers)
        {
            foreach (ILayer item in layers)
            {
                item.CacheIsExpand();
            }
        }

        public static ArrangeHistory DragItemsCompleted(this ILayerManager self, IReadOnlyList<object> layers)
        {
            Layerage[] undo = self.Nodes.Convert();

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

            self.Nodes.Clear();
            foreach (ILayer item in self.ObservableCollection)
            {
                if (item.Depth is 0)
                {
                    self.Nodes.Add(item);
                }
            }

            /// History
            Layerage[] redo = self.Nodes.Convert();
            return new ArrangeHistory(undo, redo);
        }

    }
}