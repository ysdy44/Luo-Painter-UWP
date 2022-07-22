using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void ConstructHistory()
        {
            this.UndoButton.Click += (s, e) =>
            {
                if (this.History.CanUndo == false) return;

                // History
                bool result = this.History.Undo(this.Undo);
                if (result == false) return;

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.Tip("Undo", $"{this.History.Index} / {this.History.Count}"); // Tip
            };
            this.RedoButton.Click += (s, e) =>
            {
                if (this.History.CanRedo == false) return;

                // History
                bool result = this.History.Redo(this.Redo);
                if (result == false) return;

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.Tip("Redo", $"{this.History.Index} / {this.History.Count}"); // Tip
            };
        }

        public bool Undo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        if (arrangeHistory.Sizes is SetupSizes size)
                        {
                            int w = (int)size.UndoParameter.Width;
                            int h = (int)size.UndoParameter.Height;

                            this.Transformer.Width = w;
                            this.Transformer.Height = h;
                            this.Transformer.Fit();

                            this.CreateResources(w, h);
                            this.CreateMarqueeResources(w, h);
                        }
                        return this.Arrange(arrangeHistory.UndoParameter);
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.UndoParameter);
                        else if (LayerDictionary.Instance.ContainsKey(propertyHistory.Id))
                        {
                            if (LayerDictionary.Instance[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.UndoParameter))
                            {
                                switch (propertyHistory.Type)
                                {
                                    case HistoryType.Opacity:
                                    case HistoryType.BlendMode:
                                        this.LayerListView.OnSelectedItemChanged();
                                        return true;
                                    default:
                                        return true;
                                }
                            }
                            else return false;
                        }
                        else
                            return false;
                    }
                    return false;
                case HistoryMode.Propertys:
                    if (history is IPropertysHistory propertysHistory)
                    {
                        foreach (string id in propertysHistory.Ids)
                        {
                            if (LayerDictionary.Instance.ContainsKey(id))
                                return LayerDictionary.Instance[id].History(propertysHistory.PropertyType, propertysHistory[id]);
                        }
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }
        public bool Redo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        if (arrangeHistory.Sizes is SetupSizes size)
                        {
                            int w = (int)size.RedoParameter.Width;
                            int h = (int)size.RedoParameter.Height;

                            this.Transformer.Width = w;
                            this.Transformer.Height = h;
                            this.Transformer.Fit();

                            this.CreateResources(w, h);
                            this.CreateMarqueeResources(w, h);
                        }
                        return this.Arrange(arrangeHistory.RedoParameter);
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.RedoParameter);
                        else if (LayerDictionary.Instance.ContainsKey(propertyHistory.Id))
                        {
                            if (LayerDictionary.Instance[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.RedoParameter))
                            {
                                switch (propertyHistory.Type)
                                {
                                    case HistoryType.Opacity:
                                    case HistoryType.BlendMode:
                                        this.LayerListView.OnSelectedItemChanged();
                                        return true;
                                    default:
                                        return true;
                                }
                            }
                            else return false;
                        }
                        else
                            return false;
                    }
                    return false;
                case HistoryMode.Propertys:
                    if (history is IPropertysHistory propertysHistory)
                    {
                        foreach (string id in propertysHistory.Ids)
                        {
                            if (LayerDictionary.Instance.ContainsKey(id))
                                return LayerDictionary.Instance[id].History(propertysHistory.PropertyType, propertysHistory.RedoParameter);
                        }
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

        private bool Arrange(Layerage[] layerages)
        {
            if (layerages is null) return false;
            if (layerages.Length is 0) return false;

            this.ObservableCollection.Clear();
            foreach (Layerage layerage in layerages)
            {
                string id = layerage.Id;
                if (LayerDictionary.Instance.ContainsKey(id))
                {
                    ILayer layer = LayerDictionary.Instance[id];
                    this.ObservableCollection.Add(layer);
                }
            }
            return true;
        }

    }
}