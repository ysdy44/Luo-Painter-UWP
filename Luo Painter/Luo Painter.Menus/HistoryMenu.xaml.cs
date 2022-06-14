using Luo_Painter.Elements;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Menus
{
    public sealed partial class HistoryMenu : Expander
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