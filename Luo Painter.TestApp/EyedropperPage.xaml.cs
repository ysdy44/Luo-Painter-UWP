using Luo_Painter.HSVColorPickers;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public class EyedropperColorButton : EyedropperButton
    {
        public override void Recolor(Color color)
        {
            if (base.Foreground is SolidColorBrush brush)
            {
                brush.Color = color;
            }
        }
        public override void OnColorChanged(Color color)
        {
            // Do what ?
        }
    }

    public sealed partial class EyedropperPage : Page
    {
        public EyedropperPage()
        {
            this.InitializeComponent();
        }
    }
}