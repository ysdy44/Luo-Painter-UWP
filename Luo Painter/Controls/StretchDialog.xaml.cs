using Microsoft.Graphics.Canvas;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class StretchDialog : ContentDialog
    {
        //@Content
        public System.Drawing.Size Size => this.SizePicker.Size;
        public CanvasImageInterpolation Interpolation => (CanvasImageInterpolation)this.InterpolationListView.SelectedIndex;

        //@Construct
        public StretchDialog()
        {
            this.InitializeComponent();
        }

        public void Resezing(int width, int height)
        {
            this.SizePicker.ResezingWidth(width);
            this.SizePicker.ResezingHeight(height);
        }
    }
}