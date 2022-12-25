using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class ClickEyedropper : Eyedropper
    {
        //@Task
        private TaskCompletionSource<Color> TaskSource;
        public ClickEyedropper() : base()
        {
            this.InitializeComponent();
            this.Content.PointerMoved += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.Move(e.GetCurrentPoint(this).Position);
            };

            this.Content.Tapped += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.TrySetResult();
                this.Dispose();
            };
            this.Content.PointerReleased += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.TrySetResult();
                this.Dispose();
            };
        }
        public void TrySetResult()
        {
            if (this.TaskSource != null && this.TaskSource.Task.IsCanceled == false)
            {
                this.TaskSource.TrySetResult(this.Color);
            }
        }
        public async Task<Color> OpenAsync()
        {
            this.TaskSource = new TaskCompletionSource<Color>();

            Color resultcolor = await this.TaskSource.Task;
            this.TaskSource = null;
            return resultcolor;
        }
        public void Dispose()
        {
            this.TaskSource?.TrySetCanceled();
            this.TaskSource = null;
        }
    }

    public partial class Eyedropper : UserControl
    {

        //@Converter
        private string ColorToStringConverter(Color color) => HSVExtensions.ToHex(color);

        //@Content
        protected readonly RenderTargetBitmap RenderTargetBitmap = new RenderTargetBitmap();
        protected IBuffer Pixels;

        public Color Color => this.Brush.Color;

        public Eyedropper()
        {
            this.InitializeComponent();
        }

        public async Task<bool> RenderAsync(UIElement element = null)
        {
            if (element is null) element = Eyedropper.GetWindow();

            try
            {
                await this.RenderTargetBitmap.RenderAsync(element);
                this.Pixels = await this.RenderTargetBitmap.GetPixelsAsync();
                return true;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        public void Move(Point point)
        {
            double offsetX = point.X - 50;
            double offsetY = point.Y - 50;
            Canvas.SetLeft(this.Border, offsetX);
            Canvas.SetTop(this.Border, offsetY);
            Canvas.SetLeft(this.Image, -offsetX);
            Canvas.SetTop(this.Image, -offsetY);

            if (this.Pixels == null) return;

            int x = System.Math.Clamp((int)(point.X / base.ActualWidth * this.RenderTargetBitmap.PixelWidth), 0, this.RenderTargetBitmap.PixelWidth);
            int y = System.Math.Clamp((int)(point.Y / base.ActualHeight * this.RenderTargetBitmap.PixelHeight), 0, this.RenderTargetBitmap.PixelHeight);

            int offset = 4 * (x + y * this.RenderTargetBitmap.PixelWidth);
            if (offset + 4 >= this.Pixels.Length) return;

            this.Brush.Color = new Color
            {
                A = this.Pixels.GetByte((uint)(offset + 3)),
                R = this.Pixels.GetByte((uint)(offset + 2)),
                G = this.Pixels.GetByte((uint)(offset + 1)),
                B = this.Pixels.GetByte((uint)offset),
            };
        }

        //@Static
        public static UIElement GetWindow()
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