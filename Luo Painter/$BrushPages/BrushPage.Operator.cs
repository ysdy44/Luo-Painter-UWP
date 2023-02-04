using Luo_Painter.Brushes;
using Luo_Painter.Elements;

namespace Luo_Painter
{
    public sealed partial class BrushPage
    {

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                switch (device)
                {
                    case InkInputDevice.Pen:
                        this.StartingPressure = this.Pressure = properties.Pressure;
                        break;
                    default:
                        this.StartingPressure = this.Pressure = 1;
                        break;
                }

                switch (this.ToggleButton.IsChecked)
                {
                    case true:
                        this.PaintBrush_Start();
                        break;
                    case false:
                        this.Straw_Start();
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                switch (device)
                {
                    case InkInputDevice.Pen:
                        this.Pressure = properties.Pressure;
                        break;
                    default:
                        this.Pressure = 1;
                        break;
                }

                switch (this.ToggleButton.IsChecked)
                {
                    case true:
                        this.PaintBrush_Delta();
                        break;
                    case false:
                        this.Straw_Delta();
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                switch (this.ToggleButton.IsChecked)
                {
                    case true:
                        this.PaintBrush_Complete();
                        break;
                    case false:
                        this.Straw_Complete();
                        break;
                    default:
                        break;
                }
            };
        }

    }
}