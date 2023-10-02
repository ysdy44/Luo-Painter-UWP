using Luo_Painter.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class SizeListView : XamlGridView
    {

        //@Construct
        public SizeListView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 16;
        }

    }
}