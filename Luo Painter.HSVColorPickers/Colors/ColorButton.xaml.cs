using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class ColorButtonBase : Button, IColorHdrBase, IColorBase
    {
        //@Content
        public FrameworkElement PlacementTarget => this;

        public Color Color => this.SolidColorBrush.Color;
        public Vector4 ColorHdr { get; private set; }

        //@Construct
        public ColorButtonBase()
        {
            this.InitializeComponent();
        }

        public void SetColor(Color color) => this.SolidColorBrush.Color = color;
        public void SetColor(Vector4 colorHdr) => this.SetColor(Color.FromArgb((byte)(colorHdr.W * 255f), (byte)(colorHdr.X * 255f), (byte)(colorHdr.Y * 255f), (byte)(colorHdr.Z * 255f)));

        public void SetColorHdr(Vector4 colorHdr) => this.ColorHdr = colorHdr;
        public void SetColorHdr(Color color) => this.SetColorHdr(new Vector4(color.R, color.G, color.B, color.A) / 255f); // 0~1

        public void Reverse(ColorButtonBase other)
        {
            Color color = this.Color;
            Vector4 colorHdr = this.ColorHdr;

            this.SetColor(other.Color);
            this.SetColorHdr(other.ColorHdr);

            other.SetColor(color);
            other.SetColorHdr(colorHdr);
        }
    }
}