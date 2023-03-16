using Luo_Painter.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Controls
{
    [ContentProperty(Name = nameof(Child))]
    public sealed partial class HistogramDialog : ContentDialog
    {
        //@Content
        public UIElement Child { get => this.ChildBorder.Child; set => this.ChildBorder.Child = value; }

        //@Construct
        public HistogramDialog()
        {
            this.InitializeComponent();
        }
    }
}