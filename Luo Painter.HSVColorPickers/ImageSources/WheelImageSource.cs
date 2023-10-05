using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{
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