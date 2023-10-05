using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.UI
{
    public sealed class EdgeDetectionModeButton : ToggleButton
    {
        //@Delegate
        public event RoutedEventHandler Toggled;
        //@Content
        public EdgeDetectionEffectMode Mode { get; private set; }
        //@Construct
        public EdgeDetectionModeButton()
        {
            base.Content = this.Mode;
            base.Unchecked += (s, e) =>
            {
                this.Mode = EdgeDetectionEffectMode.Sobel;
                base.Content = this.Mode;
                this.Toggled?.Invoke(s, e); //Delegate
            };
            base.Checked += (s, e) =>
            {
                this.Mode = EdgeDetectionEffectMode.Prewitt;
                base.Content = this.Mode;
                this.Toggled?.Invoke(s, e); //Delegate
            };
        }
    }
}