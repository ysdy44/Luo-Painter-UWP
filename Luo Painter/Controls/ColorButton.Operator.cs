using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
    {

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                switch (this.Operator.Device)
                {
                    case InkInputDevice.Pen:
                        this.StartingPressure = this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.StartingPressure = this.Pressure = 1;
                        break;
                }

                this.Paint_Start();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                switch (this.Operator.Device)
                {
                    case InkInputDevice.Pen:
                        this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.Pressure = 1;
                        break;
                }

                this.Paint_Delta();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Paint_Complete();
            };
        }

    }
}