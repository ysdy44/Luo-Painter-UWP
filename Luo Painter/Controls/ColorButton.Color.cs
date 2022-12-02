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
    public sealed partial class ColorButton : Button, IInkParameter, IColorBase, IColorHdrBase
    {

        private void ConstructColor()
        {
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += (s, e) => this.SetColor(e.NewColor);
            this.ColorPicker.ColorChanged += (s, e) => this.SetColorHdr(e.NewColor);
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;

            this.Timer.Tick += (s, e) =>
            {
                this.Timer.Stop();

                this.SolidColorBrush.Color = this.ColorPicker.Color;

                foreach (Color item in this.ObservableCollection)
                {
                    if (item == this.Color) return;
                }

                while (this.ObservableCollection.Count > 10)
                {
                    this.ObservableCollection.RemoveAt(0);
                }
                this.ObservableCollection.Add(this.Color);
            };

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Color item)
                {
                    this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
                    {
                        this.ColorPicker.Color = item;
                    }
                    this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
                }
            };
        }

        private void ConstructStraw()
        {
            base.ManipulationStarted += async (s, e) =>
            {
                if (this.Eyedropper is null) return;

                this.StartingStraw.X = Window.Current.Bounds.Width - 35;
                this.StartingStraw.Y = 25;

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

                        this.ColorPicker.Color = this.Eyedropper.Color;
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

                this.ColorPicker.Color = await this.ClickEyedropper.OpenAsync();

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };
        }

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            this.Timer.Stop();
            this.Timer.Start();
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