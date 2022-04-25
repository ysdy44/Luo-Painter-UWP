using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructHistory()
        {
            this.UndoButton.Click += (s, e) =>
            {
                if (this.History.CanUndo == false) return;

                // History
                bool result = this.History.Undo(this.Undo);
                if (result == false) return;

                this.CanvasControl.Invalidate(); // Invalidate

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

                this.CanvasControl.Invalidate(); // Invalidate

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
                        this.ObservableCollection.Clear();
                        foreach (string id in arrangeHistory.UndoParameter)
                        {
                            if (this.Layers.ContainsKey(id))
                            {
                                ILayer layer = this.Layers[id];
                                this.ObservableCollection.Add(layer);
                            }
                        }

                        return true;
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.UndoParameter);
                        else if (this.Layers.ContainsKey(propertyHistory.Id))
                            return this.Layers[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.UndoParameter);
                        else
                            return false;
                    }
                    return false;
                case HistoryMode.Propertys:
                    if (history is IPropertysHistory propertysHistory)
                    {
                        foreach (string id in propertysHistory.Ids)
                        {
                            if (this.Layers.ContainsKey(id))
                                return this.Layers[id].History(propertysHistory.PropertyType, propertysHistory[id]);
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
                        this.ObservableCollection.Clear();
                        foreach (string id in arrangeHistory.RedoParameter)
                        {
                            if (this.Layers.ContainsKey(id))
                            {
                                ILayer layer = this.Layers[id];
                                this.ObservableCollection.Add(layer);
                            }
                        }

                        return true;
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.RedoParameter);
                        else if (this.Layers.ContainsKey(propertyHistory.Id))
                            return this.Layers[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.RedoParameter);
                        else
                            return false;
                    }
                    return false;
                case HistoryMode.Propertys:
                    if (history is IPropertysHistory propertysHistory)
                    {
                        foreach (string id in propertysHistory.Ids)
                        {
                            if (this.Layers.ContainsKey(id))
                                return this.Layers[id].History(propertysHistory.PropertyType, propertysHistory.RedoParameter);
                        }
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

    }
}