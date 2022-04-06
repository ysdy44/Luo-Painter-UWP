using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed partial class VisibleButton : Button
    {
        //@Content
        public Visibility Visible
        {
            get => this.VisibleIcon.Visibility;
            set => this.VisibleIcon.Visibility = value;
        }

        public VisibleButton()
        {
            this.InitializeComponent();
        }

    }
}