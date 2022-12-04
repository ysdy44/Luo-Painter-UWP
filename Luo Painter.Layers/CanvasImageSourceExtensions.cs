using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public abstract class CanvasImageSourceExtensions
    {
        //@Content
        public ImageSource ImageSource => this.CanvasImageSource;
        protected float Dpi => DisplayInformation.GetForCurrentView().LogicalDpi;

        protected readonly CanvasDevice CanvasDevice = new CanvasDevice();
        protected readonly CanvasImageSource CanvasImageSource;

        //@Construct
        protected CanvasImageSourceExtensions(float width, float height) => this.CanvasImageSource = new CanvasImageSource(this.CanvasDevice, width, height, this.Dpi);

        //@Abstract
        protected abstract void Draw();

        protected CanvasDrawingSession CreateDrawingSession(Color clearColor) => this.CanvasImageSource.CreateDrawingSession(clearColor);
        protected CanvasDrawingSession CreateDrawingSession(Color clearColor, Rect updateRectangle) => this.CanvasImageSource.CreateDrawingSession(clearColor, updateRectangle);

        public void Redraw() => this.Redraw(true);
        private void Redraw(bool surfaceContentsLost)
        {
            // If the window isn't visible then we cannot update the image source
            //if (Window.Current.Visible is false) return;

            if (surfaceContentsLost || this.CanvasDevice != this.CanvasImageSource.Device)
            {
                this.CanvasImageSource.Recreate(this.CanvasDevice);
            }

            try
            {
                this.Draw();
            }
            catch (Exception e)
            {
                // XAML will also raise a SurfaceContentsLost event, and we use this to trigger redrawing
                // the surface.
                if (this.CanvasImageSource.Device.IsDeviceLost(e.HResult))
                    this.CanvasImageSource.Device.RaiseDeviceLost();
            }
        }
    }
}