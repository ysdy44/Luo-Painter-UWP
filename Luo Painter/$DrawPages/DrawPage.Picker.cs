using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum NumberPickerMode : byte
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

        LayerOpacity,
    }

    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        NumberPickerMode NumberPickerMode;
    
        private void NumberShowAt(INumberBase number, NumberPickerMode mode = default)
        {
            this.NumberPickerMode = mode;
            this.NumberFlyout.ShowAt(number.PlacementTarget);
            this.NumberPicker.Construct(number);
        }

        public void ConstructPicker()
        {
            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (this.ColorFlyout.IsOpen is false) return;

                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingColorChanged(e.NewColor);
                        break;
                    case OptionType.Threshold:
                        this.ThresholdColorChanged(e.NewColor);
                        break;
                    default:
                        break;
                }
            };

            this.LayerListView.OpacitySliderClick += (s, e) =>
            {
                this.NumberPickerMode = NumberPickerMode.LayerOpacity;
                this.NumberFlyout.ShowAt(this.LayerListView.OpacitySliderPlacementTarget);
                this.NumberPicker.Construct(this.LayerListView.IOpacitySlider);
            };

            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();

            this.NumberPicker.SecondaryButtonClick += (s, e) => this.NumberFlyout.Hide();
            this.NumberPicker.PrimaryButtonClick += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                switch (this.NumberPickerMode)
                {
                    case NumberPickerMode.LayerOpacity:
                        this.LayerListView.OpacitySliderValue = e;
                        break;

                    default:
                        switch (this.OptionType)
                        {
                            case OptionType.Feather:
                                this.FeatherSlider.Value = e;
                                break;
                            case OptionType.Grow:
                                this.GrowSlider.Value = e;
                                break;
                            case OptionType.Shrink:
                                this.ShrinkSlider.Value = e;
                                break;

                            case OptionType.Exposure:
                                this.ExposureSlider.Value = e;
                                break;
                            case OptionType.Saturation:
                                this.SaturationSlider.Value = e;
                                break;
                            case OptionType.HueRotation:
                                this.HueRotationSlider.Value = e;
                                break;
                            case OptionType.Contrast:
                                this.ContrastSlider.Value = e;
                                break;
                            case OptionType.Temperature:
                                switch (this.NumberPickerMode)
                                {
                                    case NumberPickerMode.Case0:
                                        this.TemperatureSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.TintSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.HighlightsAndShadows:
                                switch (this.NumberPickerMode)
                                {
                                    case NumberPickerMode.Case0:
                                        this.ShadowsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.HighlightsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
                                        this.ClaritySlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case3:
                                        this.BlurSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            default:
                                break;
                        }
                        break;
                }
            };
        }

    }
}