using Luo_Painter.Elements;
using Luo_Painter.Options;
using Luo_Painter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class ToolIcon : TIcon<OptionType>
    {
        public ToolIcon()
        {
            base.Loaded += (s, e) =>
            {
                ListViewItem parent = this.FindAncestor<ListViewItem>();
                if (parent is null) return;
                ToolTipService.SetToolTip(parent, new ToolTip
                {
                    Content = this.Type,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            };
        }
        protected override void OnTypeChanged(OptionType value)
        {
            base.Width = 32;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.Content = value;
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class ToolGroupingList : List<ToolGrouping> { }
    internal class ToolGrouping : List<OptionType>, IList<OptionType>, IGrouping<OptionType, OptionType>
    {
        public OptionType Key { set; get; }
    }

    public sealed partial class ToolListView : Spliter
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick;

        public OptionType SelectedItem { get; private set; }

        //@Construct
        public ToolListView()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (this.ItemClick is null) return;

                if (e.ClickedItem is OptionType item)
                {
                    this.SelectedItem = item;
                    this.ItemClick(this, item); // Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Construct(OptionType type)
        {
            this.SelectedItem = type;
            this.ItemClick?.Invoke(this, type); // Delegate

            int index = 0;
            foreach (ToolGrouping grouping in this.Collection)
            {
                foreach (OptionType item in grouping)
                {
                    if (item.Equals(type))
                    {
                        this.ListView.SelectedIndex = index;
                        return;
                    }

                    index++;
                }
            }
        }

    }
}