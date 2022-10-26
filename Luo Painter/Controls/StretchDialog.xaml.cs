using Microsoft.Graphics.Canvas;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class StretchDialog : ContentDialog
    {
        //@Content
        public System.Drawing.Size Size => this.SizePicker.Size;
        public CanvasImageInterpolation Interpolation
        {
            get
            {
                switch (this.InterpolationListView.SelectedIndex)
                {
                    case 0: return CanvasImageInterpolation.NearestNeighbor;
                    case 1: return CanvasImageInterpolation.Linear;
                    case 2: return CanvasImageInterpolation.Cubic;
                    case 3: return CanvasImageInterpolation.MultiSampleLinear;
                    case 4: return CanvasImageInterpolation.Anisotropic;
                    default: return CanvasImageInterpolation.HighQualityCubic;
                }
            }
        }

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