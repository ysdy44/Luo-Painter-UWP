using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.HSVColorPickers
{
    public interface IColorBase
    {
        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }

        Color Color { get; }
        void SetColor(Color color);
        void SetColor(Vector4 colorHdr);
    }

    public interface IColorHdrBase
    {
        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }

        Vector4 ColorHdr { get; }
        void SetColorHdr(Vector4 colorHdr);
        void SetColorHdr(Color color);
    }

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