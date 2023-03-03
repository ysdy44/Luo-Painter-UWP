using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Models.Historys;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private IEnumerable<string> Ids()
        {
            foreach (object item in this.LayerSelectedItems)
            {
                if (item is ILayer layer)
                {
                    yield return layer.Id;
                }
            }
        }

        private void ConstructLayers()
        {
            this.LayerListView.DragItemsStarting += (s, e) => this.LayerManager.DragItemsStarting(this, e.Items);
            this.LayerListView.DragItemsCompleted += (s, e) =>
            {
                switch (e.DropResult)
                {
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.None:
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy:
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move:
                        /// History
                        int removes = this.History.Push(this.LayerManager.DragItemsCompleted(this, e.Items));

                        this.CanvasVirtualControl.Invalidate(); // Invalidate

                        this.RaiseHistoryCanExecuteChanged();
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link:
                        break;
                    default:
                        break;
                }
            };
        }

        private void ConstructLayer()
        {
            this.LayerListView.VisualClick += (s, layer) =>
            {
                Visibility undo = layer.Visibility;
                Visibility redo = layer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                string[] ids = this.Ids().ToArray();

                if (ids.Length <= 1)
                {
                    layer.Visibility = redo;

                    // History
                    int removes = this.History.Push(new PropertyHistory(HistoryType.Visibility, layer.Id, undo, redo));
                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.RaiseHistoryCanExecuteChanged();
                    return;
                }

                if (ids.Contains(layer.Id) is false)
                {
                    layer.Visibility = redo;

                    // History
                    int removes = this.History.Push(new PropertyHistory(HistoryType.Visibility, layer.Id, undo, redo));
                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.RaiseHistoryCanExecuteChanged();
                    return;
                }

                IEnumerable<IHistory> his = this.GetVisibilityHistory(ids, redo);
                switch (his.Count())
                {
                    case 0:
                        break;
                    case 1:
                        // History
                        int removes1 = this.History.Push(his.Single());
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.RaiseHistoryCanExecuteChanged();
                        break;
                    default:
                        // History
                        int removes2 = this.History.Push(new CompositeHistory(his.ToArray()));
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.RaiseHistoryCanExecuteChanged();
                        break;
                }
            };
        }

        private IEnumerable<IHistory> GetVisibilityHistory(string[] ids, Visibility redo)
        {
            foreach (string id in ids)
            {
                if (LayerDictionary.Instance.ContainsKey(id) is false) continue;

                ILayer layer2 = LayerDictionary.Instance[id];
                if (layer2.Visibility == redo) continue;

                Visibility undo = layer2.Visibility;
                layer2.Visibility = redo;

                yield return new PropertyHistory(HistoryType.Visibility, layer2.Id, undo, redo);
            }
        }

    }
}