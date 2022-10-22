using Luo_Painter.Elements;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class HistoryListView : UserControl
    {

        public object ItemsSource { set => this.ListView.ItemsSource = value; }

        //@Construct
        public HistoryListView()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}