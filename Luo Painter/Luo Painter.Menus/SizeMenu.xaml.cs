using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Menus
{
    public sealed partial class SizeMenu : Expander
    {
        //@Delegate
        public event EventHandler<double> ItemClick;

        //@Construct
        public SizeMenu()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintSize item)
                {
                    this.ItemClick?.Invoke(this, item.Size);//Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}