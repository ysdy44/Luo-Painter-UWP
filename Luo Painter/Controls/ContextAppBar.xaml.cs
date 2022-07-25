using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Controls
{
    internal enum ContextAppBarState
    {
        None,
        Main,
        Preview,
        DragPreview,
    }

    public sealed partial class ContextAppBar : UserControl
    {
        //@Delegate 
        public event System.Action CanvasControlInvalidate;
        public event System.Action CanvasVirtualControlInvalidate;
        public event RoutedEventHandler PrimaryButtonClick;
        public event RoutedEventHandler SecondaryButtonClick;
        public event RangeBaseValueChangedEventHandler CropCanvasValueChanged { add => this.CropCanvasSlider.ValueChanged += value; remove => this.CropCanvasSlider.ValueChanged -= value; }

        //@Converter
        private Symbol FlowDirectionToSymbolConverter(FlowDirection value) => value is FlowDirection.LeftToRight ? Symbol.Back : Symbol.Forward;

        //@Content
        public Rippler Rippler = Rippler.Zero;
        public GradientStopSelectorWithUI Selector => this.GradientMappingSelector;
        public int TransformMode => this.TransformComboBox.SelectedIndex;
        public int DisplacementLiquefactionMode => this.DisplacementLiquefactionModeComboBox.SelectedIndex;
        public int BrushMode => this.BrushComboBox.SelectedIndex;
        public int TransparencyMode => this.TransparencyComboBox.SelectedIndex;
        public bool SelectionIsSubtract => this.SelectionComboBox.SelectedIndex is 0 is false;
        public double DisplacementLiquefactionSize => (float)this.DisplacementLiquefactionSizeSlider.Value;
        public double DisplacementLiquefactionPressure => (float)this.DisplacementLiquefactionPressureSlider.Value;
        public double CropCanvasValue
        {
            get => this.CropCanvasSlider.Value;
            set => this.CropCanvasSlider.Value = value;
        }
        public MarqueeCompositeMode MarqueeCompositeMode
        {
            get
            {
                switch (this.MarqueeComboBox.SelectedIndex)
                {
                    case 0: return MarqueeCompositeMode.New;
                    case 1: return MarqueeCompositeMode.Add;
                    case 2: return MarqueeCompositeMode.Subtract;
                    case 3: return MarqueeCompositeMode.Intersect;
                    default: return MarqueeCompositeMode.New;
                }
            }
        }

        Point StartingPosition;

        public ContextAppBar()
        {
            this.InitializeComponent();

            this.PrimaryButton.Click += (s, e) => this.PrimaryButtonClick?.Invoke(s, e);
            this.SecondaryButton.Click += (s, e) => this.SecondaryButtonClick?.Invoke(s, e);

            this.DragPrimaryButton.Click += (s, e) => this.PrimaryButtonClick?.Invoke(s, e);
            this.DragSecondaryButton.Click += (s, e) => this.SecondaryButtonClick?.Invoke(s, e);


            this.Thumb.DragStarted += (s, e) => this.StartingPosition = new Point(this.Transform.X, this.Transform.Y);
            this.Thumb.DragDelta += (s, e) =>
            {
                if (this.RootGrid.Margin.Bottom is 0)
                {
                    this.StartingPosition.Y += e.VerticalChange;

                    this.Transform.Y = System.Math.Clamp(this.StartingPosition.Y, 0, this.SwitchPresenter4.ActualHeight);
                }
                else
                {
                    this.StartingPosition.X += e.HorizontalChange;
                    this.StartingPosition.Y += e.VerticalChange;

                    this.Transform.X = this.StartingPosition.X;
                    this.Transform.Y = this.StartingPosition.Y;
                }
            };
            this.Thumb.DragCompleted += (s, e) => { };


            this.FrequencySlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Frequency = (float)(e.NewValue);
                this.CanvasVirtualControlInvalidate?.Invoke();
            };
            this.PhaseSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Phase = (float)(e.NewValue);
                this.CanvasVirtualControlInvalidate?.Invoke();
            };
            this.AmplitudeSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Amplitude = (float)(e.NewValue);
                this.CanvasVirtualControlInvalidate?.Invoke();
            };


            this.FeatherSlider.ValueChanged += (s, e) => this.CanvasControlInvalidate?.Invoke();
            this.GrowSlider.ValueChanged += (s, e) => this.CanvasControlInvalidate?.Invoke();
            this.ShrinkSlider.ValueChanged += (s, e) => this.CanvasControlInvalidate?.Invoke();

            this.ExposureSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.BrightnessSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.SaturationSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.HueRotationSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.ContrastSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.TemperatureSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.TintSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.ShadowsSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.HighlightsSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.ClaritySlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();
            this.BlurSlider.ValueChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();

            this.LuminanceToAlphaComboBox.SelectionChanged += (s, e) => this.CanvasVirtualControlInvalidate?.Invoke();

            this.TransformComboBox.SelectionChanged += (s, e) =>
            {
                this.CanvasVirtualControlInvalidate?.Invoke();
                this.CanvasControlInvalidate?.Invoke();
            };
        }

        public void Construct(OptionType type)
        {
            switch (this.GetState(type))
            {
                case ContextAppBarState.None:
                    this.SwitchPresenter1.Value = this.SwitchPresenter2.Value = this.SwitchPresenter3.Value = this.SwitchPresenter4.Value = default;
                    VisualStateManager.GoToState(this, "Normal", false);
                    break;
                case ContextAppBarState.Main:
                    this.SwitchPresenter1.Value = this.GetType(type);
                    this.SwitchPresenter2.Value = this.SwitchPresenter3.Value = this.SwitchPresenter4.Value = default;
                    VisualStateManager.GoToState(this, "Main", false);
                    break;
                case ContextAppBarState.Preview:
                    this.SwitchPresenter1.Value = this.SwitchPresenter3.Value = default;
                    this.SwitchPresenter2.Value = this.SwitchPresenter4.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, "Preview", false);
                    break;
                case ContextAppBarState.DragPreview:
                    this.SwitchPresenter1.Value = this.SwitchPresenter2.Value = default;
                    this.SwitchPresenter3.Value = this.SwitchPresenter4.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, "DragPreview", false);
                    break;
                default:
                    break;
            }
        }

        private OptionType GetType(OptionType type)
        {
            if (type is OptionType.MarqueeTransform) return OptionType.Transform;
            else if (type.HasFlag(OptionType.Marquee)) return OptionType.Marquee;
            else if (type.HasFlag(OptionType.Selection)) return OptionType.Selection;
            else return type;
        }
        private ContextAppBarState GetState(OptionType type)
        {
            if (type == default) return default;

            if (type.HasPreview() is false)
                return ContextAppBarState.Main;

            return type.AllowDrag() ? ContextAppBarState.DragPreview : ContextAppBarState.Preview;
        }

        public ICanvasImage GetPreview(OptionType type, ICanvasImage image)
        {
            switch (type)
            {
                case OptionType.Feather:
                    return new GaussianBlurEffect
                    {
                        BlurAmount = (float)this.FeatherSlider.Value,
                        Source = image
                    };
                case OptionType.Grow:
                    int grow = (int)this.GrowSlider.Value;
                    return new MorphologyEffect
                    {
                        Mode = MorphologyEffectMode.Dilate,
                        Height = grow,
                        Width = grow,
                        Source = image
                    };
                case OptionType.Shrink:
                    int shrink = (int)this.ShrinkSlider.Value;
                    return new MorphologyEffect
                    {
                        Mode = MorphologyEffectMode.Erode,
                        Height = shrink,
                        Width = shrink,
                        Source = image
                    };


                case OptionType.Gray:
                    return new GrayscaleEffect
                    {
                        Source = image
                    };
                case OptionType.Invert:
                    return new InvertEffect
                    {
                        Source = image
                    };
                case OptionType.Exposure:
                    return new ExposureEffect
                    {
                        Exposure = (float)this.ExposureSlider.Value / 100,
                        Source = image
                    };
                case OptionType.Brightness:
                    float brightness = (float)this.BrightnessSlider.Value / 100;
                    return new BrightnessEffect
                    {
                        WhitePoint = new Vector2(System.Math.Clamp(2 - brightness, 0, 1), 1),
                        BlackPoint = new Vector2(System.Math.Clamp(1 - brightness, 0, 1), 0),
                        Source = image
                    };
                case OptionType.Saturation:
                    return new SaturationEffect
                    {
                        Saturation = (float)this.SaturationSlider.Value / 100,
                        Source = image
                    };
                case OptionType.HueRotation:
                    return new HueRotationEffect
                    {
                        Angle = (float)this.HueRotationSlider.Value / 180 * FanKit.Math.Pi,
                        Source = image
                    };
                case OptionType.Contrast:
                    return new ContrastEffect
                    {
                        Contrast = (float)this.ContrastSlider.Value / 100,
                        Source = image
                    };
                case OptionType.Temperature:
                    return new TemperatureAndTintEffect
                    {
                        Temperature = (float)this.TemperatureSlider.Value / 100,
                        Tint = (float)this.TintSlider.Value / 100,
                        Source = image
                    };
                case OptionType.HighlightsAndShadows:
                    return new HighlightsAndShadowsEffect
                    {
                        Shadows = (float)this.ShadowsSlider.Value / 100,
                        Highlights = (float)this.HighlightsSlider.Value / 100,
                        Clarity = (float)this.ClaritySlider.Value / 100,
                        MaskBlurAmount = (float)this.BlurSlider.Value / 100,
                        Source = image
                    };

                case OptionType.LuminanceToAlpha:
                    switch (this.LuminanceToAlphaComboBox.SelectedIndex)
                    {
                        case 0:
                            return new LuminanceToAlphaEffect
                            {
                                Source = image
                            };
                        case 1:
                            return new LuminanceToAlphaEffect
                            {
                                Source = new InvertEffect
                                {
                                    Source = image
                                }
                            };
                        case 2:
                            return new InvertEffect
                            {
                                Source = new LuminanceToAlphaEffect
                                {
                                    Source = new InvertEffect
                                    {
                                        Source = image
                                    }
                                }
                            };
                        default: return image;
                    }
                //case OptionType.Fog:
                case OptionType.Sepia:
                    return new SepiaEffect
                    {
                        Source = image
                    };
                case OptionType.Posterize:
                    return new PosterizeEffect
                    {
                        Source = image
                    };

                default:
                    return image;
            }
        }

    }
}