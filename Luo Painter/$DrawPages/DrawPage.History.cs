using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Models.Historys;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public bool Undo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Composite:
                    if (history is CompositeHistory compositeHistory)
                    {
                        foreach (IHistory item in compositeHistory.Histories)
                        {
                            this.Undo(item);
                        }
                        return true;
                    }
                    return false;
                case HistoryMode.Setup:
                    if (history is SetupHistory setupHistory)
                    {
                        int w = (int)setupHistory.UndoParameter.Width;
                        int h = (int)setupHistory.UndoParameter.Height;

                        this.Transformer.Width = w;
                        this.Transformer.Height = h;
                        this.Transformer.Fit();

                        this.CreateResources(w, h);
                        this.CreateMarqueeResources(w, h);
                        return true;
                    }
                    return false;
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        this.Nodes.Clear();
                        this.Nodes.Load(arrangeHistory.UndoParameter);

                        this.ObservableCollection.Clear();
                        foreach (ILayer item in this.Nodes)
                        {
                            item.Arrange(0);
                            this.ObservableCollection.AddChild(item);
                        }

                        return true;
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is PropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.UndoParameter);
                        else if (LayerDictionary.Instance.ContainsKey(propertyHistory.Id))
                            return LayerDictionary.Instance[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.UndoParameter);
                    }
                    return false;
                default:
                    return false;
            }
        }
        public bool Redo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Composite:
                    if (history is CompositeHistory compositeHistory)
                    {
                        foreach (IHistory item in compositeHistory.Histories)
                        {
                            this.Redo(item);
                        }
                        return true;
                    }
                    return false;
                case HistoryMode.Setup:
                    if (history is SetupHistory setupHistory)
                    {
                        int w = (int)setupHistory.RedoParameter.Width;
                        int h = (int)setupHistory.RedoParameter.Height;

                        this.Transformer.Width = w;
                        this.Transformer.Height = h;
                        this.Transformer.Fit();

                        this.CreateResources(w, h);
                        this.CreateMarqueeResources(w, h);
                        return true;
                    }
                    return false;
                case HistoryMode.Arrange:
                    if (history is ArrangeHistory arrangeHistory)
                    {
                        this.Nodes.Load(arrangeHistory.RedoParameter);

                        this.ObservableCollection.Clear();
                        foreach (ILayer item in this.Nodes)
                        {
                            item.Arrange(0);
                            this.ObservableCollection.AddChild(item);
                        }

                        return true;
                    }
                    return false;
                case HistoryMode.Property:
                    if (history is PropertyHistory propertyHistory)
                    {
                        if (this.Marquee.Id == propertyHistory.Id)
                            return this.Marquee.History(propertyHistory.Type, propertyHistory.RedoParameter);
                        else if (LayerDictionary.Instance.ContainsKey(propertyHistory.Id))
                            return LayerDictionary.Instance[propertyHistory.Id].History(propertyHistory.Type, propertyHistory.RedoParameter);
                    }
                    return false;
                default:
                    return false;
            }
        }

    }
}