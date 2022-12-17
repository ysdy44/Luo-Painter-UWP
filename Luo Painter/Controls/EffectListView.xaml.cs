using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class EffectListView : UserControl
    {
        //@Delegate
        public event ItemClickEventHandler ItemClick { remove => this.ListView.ItemClick -= value; add => this.ListView.ItemClick += value; }

        //@Converter
        private object ItemsSourceConverter(int value) => this.Collection[value];

        //@Construct
        public EffectListView()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}