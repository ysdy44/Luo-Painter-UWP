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

            double scaleX = point.X / base.ActualWidth; // 0~1
            double scaleY = point.Y / base.ActualHeight; // 0~1

            switch (base.FlowDirection)
            {
                case FlowDirection.LeftToRight:
                    //scaleX = scaleX;
                    break;
                case FlowDirection.RightToLeft:
                    scaleX = 1 - scaleX;
                    break;
                default:
                    break;
            }

            int x = System.Math.Clamp((int)(scaleX * this.RenderTargetBitmap.PixelWidth), 0, this.RenderTargetBitmap.PixelWidth);
            int y = System.Math.Clamp((int)(scaleY * this.RenderTargetBitmap.PixelHeight), 0, this.RenderTargetBitmap.PixelHeight);

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