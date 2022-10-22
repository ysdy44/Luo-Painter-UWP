﻿using Luo_Painter.Brushes;
using Luo_Painter.Elements;

namespace Luo_Painter.Controls
{
    public sealed partial class PaletteMenu : Expander, IInkParameter
    {

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (properties.IsEraser || properties.IsBarrelButtonPressed)
                    this.StartingPressure = this.Pressure = 1;
                else
                    this.StartingPressure = this.Pressure = properties.Pressure * properties.Pressure;

                this.Paint_Start();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (properties.IsEraser || properties.IsBarrelButtonPressed)
                    this.Pressure = 1;
                else
                    this.Pressure = properties.Pressure * properties.Pressure;

                this.Paint_Delta();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Paint_Complete();
            };
        }

    }
}