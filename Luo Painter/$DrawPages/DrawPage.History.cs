using Luo_Painter.Brushes;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
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

    }
}