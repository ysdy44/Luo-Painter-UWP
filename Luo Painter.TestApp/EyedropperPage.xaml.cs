using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class EyedropperPage : Page
    {

        Point StartingEyedropper;

        public EyedropperPage()
        {
            this.InitializeComponent();

            this.StrawButton.Click += async (s, e) =>
            {
                this.StartingEyedropper = this.StrawButton.TransformToVisual(this.GetEyedropperTarget()).TransformPoint(new Point(20, 20));
                this.ColorFlyout.Hide();

                bool result = await this.ClickEyedropper.RenderAsync(this.GetEyedropperTarget());
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingEyedropper);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;

                this.ColorPicker.Color = await this.ClickEyedropper.OpenAsync();

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };


            this.ColorButton.ManipulationStarted += async (s, e) =>
            {
                if (true)
                {
                    this.StartingEyedropper = this.ColorButton.TransformToVisual(this.GetEyedropperTarget()).TransformPoint(new Point(30, 25));
                }
                else
                {
                    this.StartingEyedropper.X = base.ActualWidth - 35;
                    this.StartingEyedropper.Y = 25;
                }

                bool result = await this.Eyedropper.RenderAsync(this.GetEyedropperTarget());
                if (result is false) return;

                Window.Current.CoreWindow.PointerCursor = null;
                this.Eyedropper.Visibility = Visibility.Visible;
            };
            this.ColorButton.ManipulationDelta += (s, e) =>
            {
                this.StartingEyedropper.X += e.Delta.Translation.X;
                this.StartingEyedropper.Y += e.Delta.Translation.Y;
                this.Eyedropper.Move(this.StartingEyedropper);
            };
            this.ColorButton.ManipulationCompleted += (s, e) =>
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.Eyedropper.Visibility = Visibility.Collapsed;

                this.ColorPicker.Color = this.Eyedropper.Color;
            };
        }
        
        private UIElement GetEyedropperTarget()
        {
            return this;

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