using System;
using System.Numerics;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    public abstract class EyedropperButton : Button, ICommand
    {
        //@Content
        public Eyedropper Eyedropper { get; set; }
        public ClickEyedropper ClickEyedropper { get; set; }

        //@Abstract
        public abstract void OnColorChanged(Color color);

        Point StartingStraw;

        public EyedropperButton()
        {
            base.ManipulationStarted += async (s, e) =>
            {
                if (this.Eyedropper is null) return;

                Point position = base.TransformToVisual(Window.Current.Content).TransformPoint(default);
                this.StartingStraw.X = position.X + base.ActualWidth / 2;
                this.StartingStraw.Y = position.Y + base.ActualHeight / 2;

                bool result = await this.Eyedropper.RenderAsync();
            };
            base.ManipulationDelta += (s, e) =>
            {
                if (this.Eyedropper is null) return;

                this.StartingStraw.X += e.Delta.Translation.X;
                this.StartingStraw.Y += e.Delta.Translation.Y;

                switch (this.Eyedropper.Visibility)
                {
                    case Visibility.Collapsed:
                        if (e.Cumulative.Translation.ToVector2().LengthSquared() > 625)
                        {
                            Window.Current.CoreWindow.PointerCursor = null;
                            this.Eyedropper.Move(this.StartingStraw);
                            this.Eyedropper.Visibility = Visibility.Visible;
                        }
                        break;
                    case Visibility.Visible:
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
                        this.OnColorChanged(color);
                        break;
                    default:
                        break;
                }
            };
        }

        //@Command
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public async void Execute(object parameter)
        {
            if (this.ClickEyedropper is null) return;

            this.StartingStraw.X = -200;
            this.StartingStraw.Y = -200;

            base.Flyout?.Hide();
            {
                bool result = await this.ClickEyedropper.RenderAsync();
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingStraw);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;
                {
                    Color color = await this.ClickEyedropper.OpenAsync();
                    this.OnColorChanged(color);
                }
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            }
            base.Flyout?.ShowAt(this);
        }
    }
}