using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.HSVColorPickers
{
    public abstract class CanvasImageSourceBase
    {
        //@Content
        public ImageSource ImageSource => this.CanvasImageSource;

        protected readonly CanvasDevice CanvasDevice;
        protected readonly CanvasImageSource CanvasImageSource;

        //@Construct
        protected CanvasImageSourceBase(CanvasDevice canvasDevice, float width, float height, float dpi)
        {
            this.CanvasDevice = canvasDevice;
            this.CanvasImageSource = new CanvasImageSource(this.CanvasDevice, width, height, dpi);
        }

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