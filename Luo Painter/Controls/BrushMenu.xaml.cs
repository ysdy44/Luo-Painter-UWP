using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class BrushListView : UserControl
    {
        //@Delegate
        public event EventHandler<PaintBrush> ItemClick;
        public event RoutedEventHandler Add { remove => this.AddButton.Click -= value; add => this.AddButton.Click += value; }

        //@Construct
        public BrushListView()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintBrush item)
                {
                    this.ItemClick?.Invoke(this, item);//Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}