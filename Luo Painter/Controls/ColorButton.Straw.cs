using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton
    {

        private void ConstructStraw()
        {
            base.ManipulationStarted += async (s, e) =>
            {
                if (this.Eyedropper is null) return;

                Point position = base.TransformToVisual(Window.Current.Content).TransformPoint(default);
                this.StartingStraw.X = position.X + base.ActualWidth / 2;
                this.StartingStraw.Y = position.Y + base.ActualHeight / 2;

                bool result = await this.Eyedropper.RenderAsync(this.GetTarget());
            };
            base.ManipulationDelta += (s, e) =>
            {
                if (this.Eyedropper is null) return;

                switch (this.Eyedropper.Visibility)
                {
                    case Visibility.Collapsed:
                        if (e.Cumulative.Translation.ToVector2().LengthSquared() > 625)
                        {
                            Window.Current.CoreWindow.PointerCursor = null;
                            this.Eyedropper.Visibility = Visibility.Visible;
                        }
                        break;
                    case Visibility.Visible:
                        this.StartingStraw.X += e.Delta.Translation.X;
                        this.StartingStraw.Y += e.Delta.Translation.Y;
                        this.Eyedropper.Move(this.StartingStraw);
                        break;
                    default:
                        break;
                }
            };
            base.ManipulationCompleted += (s, e) =>
            {
                if (this.Eyedropper is null) return;

                switch (this.Eyedropper.Visibility)
                {
                    case Visibility.Visible:
                        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                        this.Eyedropper.Visibility = Visibility.Collapsed;

                        Color color = this.Eyedropper.Color;
                        this.SetColor(color);
                        this.SetColorHdr(color);
                        this.ColorChanged?.Invoke(this, color); // Delegate
                        break;
                    default:
                        break;
                }
            };

            this.StrawButton.Click += async (s, e) =>
            {
                base.Flyout.Hide();
                if (this.ClickEyedropper is null) return;

                this.StartingStraw = this.StrawButton.TransformToVisual(Window.Current.Content).TransformPoint(new Point(20, 20));

                bool result = await this.ClickEyedropper.RenderAsync(this.GetTarget());
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingStraw);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;

                Color color = await this.ClickEyedropper.OpenAsync();
                this.SetColor(color);
                this.SetColorHdr(color);
                this.ColorChanged?.Invoke(this, color); // Delegate

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };
        }

        public UIElement GetTarget()
        {
            if (Window.Current.Content is FrameworkElement frame)
            {
                if (frame.Parent is FrameworkElement border)
                {
                    if (border.Parent is FrameworkElement rootScrollViewer)
                        return rootScrollViewer;
                    else
                        return border;
                }
                else
                    return frame;
            }
            else return Window.Current.Content;
        }

    }
}