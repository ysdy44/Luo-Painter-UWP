using Luo_Painter.Brushes;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class SizeListView : UserControl
    {
        //@Delegate
        public event EventHandler<double> ItemClick;

        //@Construct
        public SizeListView()
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