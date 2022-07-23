using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

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