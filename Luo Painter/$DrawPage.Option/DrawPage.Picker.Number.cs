using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        NumberPickerMode NumberPickerMode;

        private void NumberShowAt(INumberBase number, NumberPickerMode mode = default)
        {
            this.NumberPickerMode = mode;
            this.NumberFlyout.ShowAt(number.PlacementTarget);
            this.NumberPicker.Construct(number);
        }

        public void ConstructNumberPicker()
        {
            this.OpacitySlider.Click += (s, e) =>
            {
                this.NumberPickerMode = NumberPickerMode.OpacitySlider;
                this.NumberFlyout.ShowAt(this.OpacitySlider.PlacementTarget);
                this.NumberPicker.Construct(this.OpacitySlider);
            };

            this.RadianSlider.Click += (s, e) => this.NumberShowAt(this.RadianSlider, NumberPickerMode.RadianSlider);
            this.ScaleSlider.Click += (s, e) => this.NumberShowAt(this.ScaleSlider, NumberPickerMode.ScaleSlider);
         
            this.SelectionFloodToleranceSlider.Click += (s, e) => this.NumberShowAt(this.SelectionFloodToleranceSlider, NumberPickerMode.SelectionFloodToleranceSlider);
            
            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();
            this.NumberPicker.ValueChanged += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                NumberPickerMode mode = this.NumberPickerMode;
                switch (mode)
                {
                    case NumberPickerMode.OpacitySlider:
                        this.OpacitySlider.Value = e;
                        break;

                    case NumberPickerMode.RadianSlider:
                        this.RadianSlider.Value = e;
                        break;
                    case NumberPickerMode.ScaleSlider:
                        this.ScaleSlider.Value = e;
                        break;

                    case NumberPickerMode.SelectionFloodToleranceSlider:
                        this.SelectionFloodToleranceSlider.Value = e;
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
                                switch (mode)
                                {
                                    case NumberPickerMode.TemperatureSlider:
                                        this.TemperatureSlider.Value = e;
                                        break;
                                    case NumberPickerMode.TintSlider:
                                        this.TintSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.HighlightsAndShadows:
                                switch (mode)
                                {
                                    case NumberPickerMode.ShadowsSlider:
                                        this.ShadowsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.HighlightsSlider:
                                        this.HighlightsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.ClaritySlider:
                                        this.ClaritySlider.Value = e;
                                        break;
                                    case NumberPickerMode.BlurSlider:
                                        this.BlurSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case OptionType.GammaTransfer:
                                this.SetGammaTransfer(mode, (float)e);
                                break;
                            case OptionType.Vignette:
                                this.VignetteSlider.Value = e;
                                break;

                            case OptionType.GaussianBlur:
                                this.GaussianBlurSlider.Value = e;
                                break;
                            case OptionType.DirectionalBlur:
                                switch (mode)
                                {
                                    case NumberPickerMode.DirectionalBlurSlider:
                                        this.DirectionalBlurSlider.Value = e;
                                        break;
                                    case NumberPickerMode.DirectionalBlurAngleSlider:
                                        this.DirectionalBlurAngleSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Sharpen:
                                this.SharpenSlider.Value = e;
                                break;
                            case OptionType.Shadow:
                                switch (mode)
                                {
                                    case NumberPickerMode.ShadowAmountSlider:
                                        this.ShadowAmountSlider.Value = e;
                                        break;
                                    case NumberPickerMode.ShadowOpacitySlider:
                                        this.ShadowOpacitySlider.Value = e;
                                        break;
                                    case NumberPickerMode.ShadowXSlider:
                                        this.ShadowXSlider.Value = e;
                                        break;
                                    case NumberPickerMode.ShadowYSlider:
                                        this.ShadowYSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.EdgeDetection:
                                switch (mode)
                                {
                                    case NumberPickerMode.EdgeDetectionSlider:
                                        this.EdgeDetectionSlider.Value = e;
                                        break;
                                    case NumberPickerMode.EdgeDetectionBlurAmountSlider:
                                        this.EdgeDetectionBlurAmountSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Morphology:
                                this.MorphologySlider.Value = e;
                                break;
                            case OptionType.Emboss:
                                switch (mode)
                                {
                                    case NumberPickerMode.EmbossSlider:
                                        this.EmbossSlider.Value = e;
                                        break;
                                    case NumberPickerMode.EmbossAngleSlider:
                                        this.EmbossAngleSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Straighten:
                                this.StraightenSlider.Value = e;
                                break;

                            case OptionType.ChromaKey:
                                this.ChromaKeySlider.Value = e;
                                break;
                            case OptionType.DiscreteTransfer:
                                this.SetDiscreteTransfer(mode, (float)e);
                                break;

                            case OptionType.Move:
                                this.SetMove(mode, (float)e);
                                break;
                            case OptionType.Transform:
                                this.SetTransform(mode, (float)e);
                                break;
                            case OptionType.FreeTransform:
                                this.SetFreeTransform(mode, (float)e);
                                break;

                            case OptionType.DisplacementLiquefaction:
                                switch (mode)
                                {
                                    case NumberPickerMode.DisplacementLiquefactionSizeSlider:
                                        this.DisplacementLiquefactionSizeSlider.Value = e;
                                        break;
                                    case NumberPickerMode.DisplacementLiquefactionPressureSlider:
                                        this.DisplacementLiquefactionPressureSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.RippleEffect:
                                switch (mode)
                                {
                                    case NumberPickerMode.FrequencySlider:
                                        this.FrequencySlider.Value = e;
                                        break;
                                    case NumberPickerMode.PhaseSlider:
                                        this.PhaseSlider.Value = e;
                                        break;
                                    case NumberPickerMode.AmplitudeSlider:
                                        this.AmplitudeSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Threshold:
                                this.ThresholdSlider.Value = e;
                                break;
                            case OptionType.HSB:
                                switch (mode)
                                {
                                    case NumberPickerMode.HSBHueSlider:
                                        this.HSBHueSlider.Value = e;
                                        break;
                                    case NumberPickerMode.HSBSaturationSlider:
                                        this.HSBSaturationSlider.Value = e;
                                        break;
                                    case NumberPickerMode.HSBBrightnessSlider:
                                        this.HSBBrightnessSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case OptionType.Lighting:
                                switch (mode)
                                {
                                    case NumberPickerMode.LightDistanceSlider:
                                        this.LightDistanceSlider.Value = e;
                                        break;
                                    case NumberPickerMode.LightAmbientSlider:
                                        this.LightAmbientSlider.Value = e;
                                        break;
                                    case NumberPickerMode.LightAngleSlider:
                                        this.LightAngleSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.Fog:
                                this.FogSlider.Value = e;
                                break;
                            case OptionType.Glass:
                                this.GlassSlider.Value = e;
                                break;

                            case OptionType.GeometryRoundRect:
                            case OptionType.GeometryRoundRectTransform:
                                this.RoundRectCornerSlider.Value = e;
                                break;
                            case OptionType.GeometryTriangle:
                            case OptionType.GeometryTriangleTransform:
                                this.TriangleCenterSlider.Value = e;
                                break;
                            case OptionType.GeometryDiamond:
                            case OptionType.GeometryDiamondTransform:
                                this.DiamondMidSlider.Value = e;
                                break;
                            case OptionType.GeometryPentagon:
                            case OptionType.GeometryPentagonTransform:
                                this.PentagonPointsSlider.Value = e;
                                break;
                            case OptionType.GeometryStar:
                            case OptionType.GeometryStarTransform:
                                switch (mode)
                                {
                                    case NumberPickerMode.StarPointsSlider:
                                        this.StarPointsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.StarInnerRadiusSlider:
                                        this.StarInnerRadiusSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.GeometryCog:
                            case OptionType.GeometryCogTransform:
                                switch (mode)
                                {
                                    case NumberPickerMode.CogCountSlider:
                                        this.CogCountSlider.Value = e;
                                        break;
                                    case NumberPickerMode.CogInnerRadiusSlider:
                                        this.CogInnerRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.CogToothSlider:
                                        this.CogToothSlider.Value = e;
                                        break;
                                    case NumberPickerMode.CogNotchSlider:
                                        this.CogNotchSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.GeometryDonut:
                            case OptionType.GeometryDonutTransform:
                                this.DonutHoleRadiusSlider.Value = e;
                                break;
                            case OptionType.GeometryPie:
                            case OptionType.GeometryPieTransform:
                                this.PieSweepAngleSlider.Value = e;
                                break;
                            case OptionType.GeometryCookie:
                            case OptionType.GeometryCookieTransform:
                                switch (mode)
                                {
                                    case NumberPickerMode.CookieInnerRadiusSlider:
                                        this.CookieInnerRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.CookieSweepAngleSlider:
                                        this.CookieSweepAngleSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.GeometryArrow:
                            case OptionType.GeometryArrowTransform:
                                this.ArrowWidthSlider.Value = e;
                                break;
                            case OptionType.GeometryHeart:
                            case OptionType.GeometryHeartTransform:
                                this.HeartSpreadSlider.Value = e;
                                break;

                            case OptionType.PatternGrid:
                            case OptionType.PatternGridTransform:
                                switch (mode)
                                {
                                    case NumberPickerMode.GridStrokeWidthSlider:
                                        this.GridStrokeWidthSlider.Value = e;
                                        break;
                                    case NumberPickerMode.GridColumnSpanSlider:
                                        this.GridColumnSpanSlider.Value = e;
                                        break;
                                    case NumberPickerMode.GridRowSpanSlider:
                                        this.GridRowSpanSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.PatternSpotted:
                            case OptionType.PatternSpottedTransform:
                                switch (mode)
                                {
                                    case NumberPickerMode.SpottedRadiusSlider:
                                        this.SpottedRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.SpottedSpanSlider:
                                        this.SpottedSpanSlider.Value = e;
                                        break;
                                    case NumberPickerMode.SpottedFadeSlider:
                                        this.SpottedFadeSlider.Value = e;
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