using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class OffsetDialog : ContentDialog
    {
        //@Content
        public System.Drawing.Point Offset => this.OffsetPicker.Offset;

        //@Construct
        public OffsetDialog()
        {
            this.InitializeComponent();
        }

        public void Resezing(int x, int y)
        {
            this.OffsetPicker.ResezingX(x);
            this.OffsetPicker.ResezingY(y);
        }
    }
}