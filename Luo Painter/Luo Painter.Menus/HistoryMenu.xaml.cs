using Luo_Painter.Elements;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Menus
{
    public sealed partial class HistoryMenu : UserControl
    {

        public object ItemsSource { set => this.ListView.ItemsSource = value; }

        //@Construct
        public HistoryMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}