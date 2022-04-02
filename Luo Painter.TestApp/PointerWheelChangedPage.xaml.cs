using System.Numerics;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PointerWheelChangedPage : Page
    {
        Vector2 Position;
        float Scale2 = 1;

        public PointerWheelChangedPage()
        {
            this.InitializeComponent();
            this.Rectangle.PointerWheelChanged += (s, e) =>
            {
                PointerPoint point = e.GetCurrentPoint(this.Rectangle);
                PointerPointProperties properties = point.Properties;
                Vector2 position = point.Position.ToVector2();

                switch (e.KeyModifiers)
                {
                    // Move on Touchpad
                    case VirtualKeyModifiers.None:
                        if (properties.IsHorizontalMouseWheel)
                            this.Position.X -= properties.MouseWheelDelta;
                        else
                            this.Position.Y += properties.MouseWheelDelta;
                        Canvas.SetLeft(this.Rectangle2, this.Position.X);
                        Canvas.SetTop(this.Rectangle2, this.Position.Y);
                        break;

                    // Pinch on Touchpad
                    case VirtualKeyModifiers.Control:
                        if (properties.MouseWheelDelta >= 0)
                        {
                            this.Scale2 *= 1.1f;
                            this.Position = position + (this.Position - position) * 1.1f;
                        }
                        else
                        {
                            this.Scale2 /= 1.1f;
                            this.Position = position + (this.Position - position) / 1.1f;
                        }
                        Canvas.SetLeft(this.Rectangle2, this.Position.X);
                        Canvas.SetTop(this.Rectangle2, this.Position.Y);
                        this.ScaleTransform.ScaleX = this.Scale2;
                        this.ScaleTransform.ScaleY = this.Scale2;
                        break;
                    default:
                        break;
                }
            };
        }

    }
}