﻿using Luo_Painter.Options;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum ColorPickerMode : byte
    {
        None,

        Case0,
        Case1,
        Case2,
        Case3,
        Case4,
        Case5,
        Case6,
        Case7,
    }

    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        ColorPickerMode ColorPickerMode;

        private void ColorShowAt(IColorBase color, ColorPickerMode mode = default)
        {
            this.ColorPickerMode = mode;
            this.ColorPicker.Color = color.Color;
            this.ColorFlyout.ShowAt(color.PlacementTarget);
        }

        public void ConstructColorPicker()
        {
            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (this.ColorFlyout.IsOpen is false) return;

                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingSelector.SetColor(e.NewColor);

                        this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        break;
                    case OptionType.Threshold:
                        switch (this.ColorPickerMode)
                        {
                            case ColorPickerMode.Case0:
                                this.ThresholdColor0Button.SetColor(e.NewColor);
                                this.ThresholdColor0Button.SetColorHdr(e.NewColor);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            case ColorPickerMode.Case1:
                                this.ThresholdColor1Button.SetColor(e.NewColor);
                                this.ThresholdColor1Button.SetColorHdr(e.NewColor);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            };
        }

    }
}