using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum NumberPickerMode : int
    {
        None = 0,

        Case0 = 0,
        Case1 = 1,
        Case2 = 2,
        Case3 = 3,

        LayerOpacity = -1,

        ViewRadian = -2,
        ViewScale = -3,
    }

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
            this.LayerListView.OpacitySliderClick += (s, e) =>
            {
                this.NumberPickerMode = NumberPickerMode.LayerOpacity;
                this.NumberFlyout.ShowAt(this.LayerListView.OpacityNumber.PlacementTarget);
                this.NumberPicker.Construct(this.LayerListView.OpacityNumber);
            };

            this.RadianSlider.Click += (s, e) => this.NumberShowAt(this.RadianSlider, NumberPickerMode.ViewRadian);
            this.ScaleSlider.Click += (s, e) => this.NumberShowAt(this.ScaleSlider, NumberPickerMode.ViewScale);

            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();
            this.NumberPicker.ValueChanged += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                NumberPickerMode mode = this.NumberPickerMode;
                switch (mode)
                {
                    case NumberPickerMode.LayerOpacity:
                        this.LayerListView.OpacitySliderValue = e;
                        break;

                    case NumberPickerMode.ViewRadian:
                        this.RadianSlider.Value = e;
                        break;
                    case NumberPickerMode.ViewScale:
                        this.ScaleSlider.Value = e;
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
                                switch (mode)
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
                                    case NumberPickerMode.Case0:
                                        this.DirectionalBlurSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
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
                                    case NumberPickerMode.Case0:
                                        this.ShadowAmountSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.ShadowOpacitySlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
                                        this.ShadowXSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case3:
                                        this.ShadowYSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.EdgeDetection:
                                switch (mode)
                                {
                                    case NumberPickerMode.Case0:
                                        this.EdgeDetectionSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
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
                                    case NumberPickerMode.Case0:
                                        this.EmbossSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
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
                                    case NumberPickerMode.Case0:
                                        this.DisplacementLiquefactionSizeSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.DisplacementLiquefactionPressureSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case OptionType.RippleEffect:
                                switch (mode)
                                {
                                    case NumberPickerMode.Case0:
                                        this.FrequencySlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.PhaseSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
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
                                    case NumberPickerMode.Case0:
                                        this.HSBHueSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.HSBSaturationSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
                                        this.HSBBrightnessSlider.Value = e;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case OptionType.Lighting:
                                switch (mode)
                                {
                                    case NumberPickerMode.Case0:
                                        this.LightDistanceSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.LightAmbientSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
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
                                    case NumberPickerMode.Case0:
                                        this.StarPointsSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
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
                                    case NumberPickerMode.Case0:
                                        this.CogCountSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.CogInnerRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
                                        this.CogToothSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case3:
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
                                    case NumberPickerMode.Case0:
                                        this.CookieInnerRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
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
                                    case NumberPickerMode.Case0:
                                        this.GridStrokeWidthSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.GridColumnSpanSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
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
                                    case NumberPickerMode.Case0:
                                        this.SpottedRadiusSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case1:
                                        this.SpottedSpanSlider.Value = e;
                                        break;
                                    case NumberPickerMode.Case2:
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