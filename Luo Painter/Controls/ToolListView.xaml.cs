using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Controls
{
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