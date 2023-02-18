using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class OffsetDialog : ContentDialog
    {
        //@Content
        public Vector2 Offset => this.OffsetPicker.Offset;

        //@Construct
        public OffsetDialog()
        {
            this.InitializeComponent();
        }

        public void Resezing(Vector2 offset)
        {
            this.OffsetPicker.ResezingX(offset.X);
            this.OffsetPicker.ResezingY(offset.Y);
        }
    }
}