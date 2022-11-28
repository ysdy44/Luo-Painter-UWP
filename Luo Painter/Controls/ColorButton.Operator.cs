using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System.IO;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
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
                        this.StartingPressure = this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.StartingPressure = this.Pressure = 1;
                        break;
                }

                switch (this.ToolComboBox.SelectedIndex)
                {
                    case 0:
                        this.PaintBrush_Start();
                        break;
                    case 1:
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
                        this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.Pressure = 1;
                        break;
                }

                switch (this.ToolComboBox.SelectedIndex)
                {
                    case 0:
                        this.PaintBrush_Delta();
                        break;
                    case 1:
                        this.Straw_Delta();
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                switch (this.ToolComboBox.SelectedIndex)
                {
                    case 0:
                        this.PaintBrush_Complete();
                        break;
                    case 1:
                        this.Straw_Complete();
                        break;
                    default:
                        break;
                }
            };
        }

    }
}