using Luo_Painter.Brushes;
using Luo_Painter.Strings;
using System.Collections.Generic;
using System.Linq;

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