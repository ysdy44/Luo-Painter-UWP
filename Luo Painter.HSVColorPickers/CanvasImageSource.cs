using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
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

    public abstract class GrayAndWhiteImageSource : CanvasImageSourceBase
    {
        protected readonly int Count;
        protected readonly int Length;
        protected readonly int Square;

        internal GrayAndWhiteImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square * 2, dpi)
        {
            this.Count = length / square;
            this.Length = length;
            this.Square = square;
            this.Draw();
        }
    }

    public sealed class OpacityImageSource : GrayAndWhiteImageSource
    {
        public OpacityImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square, dpi) { }
        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.White))
            {
                for (int i = 0; i < base.Count; i++)
                {
                    ds.FillRectangle(i * this.Square, i % 2 * this.Square, this.Square, this.Square, Colors.LightGray);
                }
            }
        }
    }

    public sealed class AlphaImageSource : GrayAndWhiteImageSource
    {
        Color Color0 = Colors.DarkGray;
        Color Color1 = Colors.DimGray;
        public AlphaImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square, dpi) { }
        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.DodgerBlue))
            {
                for (int i = 0; i < base.Count; i++)
                {
                    this.Color0.A = this.Color1.A = (byte)(255f - 255f * i / base.Count);

                    if (i % 2 is 0)
                    {
                        ds.FillRectangle(i * this.Square, 0, this.Square, this.Square, this.Color0);
                        ds.FillRectangle(i * this.Square, this.Square, this.Square, this.Square, this.Color1);
                    }
                    else
                    {
                        ds.FillRectangle(i * this.Square, 0, this.Square, this.Square, this.Color1);
                        ds.FillRectangle(i * this.Square, this.Square, this.Square, this.Square, this.Color0);
                    }
                }
            }
        }
    }

    public sealed class WheelImageSource : CanvasImageSourceBase
    {
        readonly CircleTemplateSettingsF CircleSize;
        readonly float StrokeWidth;

        public WheelImageSource(CanvasDevice canvasDevice, CircleTemplateSettingsF size, float dpi) : base(canvasDevice, size.Diameter, size.Diameter, dpi)
        {
            this.CircleSize = size;
            this.StrokeWidth = size.Diameter / 100;
            this.Draw();
        }

        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.White))
            {
                for (int i = 0; i < 360; i++)
                {
                    ds.DrawLine(this.CircleSize.XY(MathF.PI * i / 180), this.CircleSize.Center, HSVExtensions.ToColor(i), this.StrokeWidth);
                }

                ds.FillCircle(this.CircleSize.Center, this.CircleSize.Radius, new CanvasRadialGradientBrush(base.CanvasDevice, Colors.White, Colors.Transparent)
                {
                    Center = this.CircleSize.Center,
                    RadiusX = this.CircleSize.Radius,
                    RadiusY = this.CircleSize.Radius,
                });
            }
        }
    }
}