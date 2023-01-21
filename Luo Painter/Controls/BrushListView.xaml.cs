using Luo_Painter.Brushes;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class BrushListView : XamlListView
    {
        //@Delegate
        public event RoutedEventHandler Add { remove => this.AddButton.Click -= value; add => this.AddButton.Click += value; }

        //@Construct
        public BrushListView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 2;
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}