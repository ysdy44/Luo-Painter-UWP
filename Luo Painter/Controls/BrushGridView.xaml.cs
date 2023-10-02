using Luo_Painter.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class BrushGridView : XamlGridView
    {

        //@Construct
        public BrushGridView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 2;
        }

    }
}