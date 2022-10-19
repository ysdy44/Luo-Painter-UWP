﻿using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using System.ServiceModel.Channels;
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
        public event System.Action GeometryInvalidate;
        public event System.Action CanvasControlInvalidate;
        public event System.Action CanvasVirtualControlInvalidate;
        public event RoutedEventHandler PrimaryButtonClick { add => this.DragPrimaryButton.Click += value; remove => this.DragPrimaryButton.Click -= value; }
        public event RoutedEventHandler SecondaryButtonClick { add => this.DragSecondaryButton.Click += value; remove => this.DragSecondaryButton.Click -= value; }
        public event EventHandler<float> RadianValueChanged;
        public event EventHandler<float> ScaleValueChanged;
        public event RangeBaseValueChangedEventHandler SizeValueChanged { add => this.SizeSlider.ValueChanged += value; remove => this.SizeSlider.ValueChanged -= value; }
        public event RangeBaseValueChangedEventHandler OpacityValueChanged { add => this.OpacitySlider.ValueChanged += value; remove => this.OpacitySlider.ValueChanged -= value; }
        public event RangeBaseValueChangedEventHandler CropCanvasValueChanged { add => this.CropCanvasSlider.ValueChanged += value; remove => this.CropCanvasSlider.ValueChanged -= value; }
        public event RoutedEventHandler PenCloseButtonClick { add => this.PenCloseButton.Click += value; remove => this.PenCloseButton.Click -= value; }


        //@Converter
        public double SizeConverter(double value) => this.SizeRange.ConvertXToY(value);
        private double FontSizeConverter(double value) => this.SizeConverter(value) / 4 + 1;
        private string SizeToStringConverter(double value) => System.String.Format("{0:F}", this.SizeConverter(value));
        private double OpacityConverter(double value) => value / 100;
        private string OpacityToStringConverter(double value) => $"{(int)value} %";
        private Symbol FlowDirectionToSymbolConverter(FlowDirection value) => value is FlowDirection.LeftToRight ? Symbol.Back : Symbol.Forward;


        //@Content
        public Rippler Rippler = Rippler.Zero;
        public RemoteControl RemoteControl => this.RemoteControlCore;
        public GradientStopSelectorWithUI Selector => this.GradientMappingSelector;
        public int TransformMode => this.TransformComboBox.SelectedIndex;
        public int DisplacementLiquefactionMode => this.DisplacementLiquefactionModeComboBox.SelectedIndex;
        public int BrushMode => this.BrushComboBox.SelectedIndex;
        public int TransparencyMode => this.TransparencyComboBox.SelectedIndex;
        public bool SelectionIsSubtract => this.SelectionComboBox.SelectedIndex is 0 is false;
        public double DisplacementLiquefactionSize => (float)this.DisplacementLiquefactionSizeSlider.Value;
        public double DisplacementLiquefactionPressure => (float)this.DisplacementLiquefactionPressureSlider.Value;
        public double SizeValue
        {
            get => this.SizeSlider.Value;
            set => this.SizeSlider.Value = value;
        }
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
        public bool PenIsSmooth => this.PenComboBox.SelectedIndex is 0;
        public bool IsBrushOpaque => this.BrushOpaqueButton.IsChecked is true;
        public bool IsBrushReverse => this.BrushReverseButton.IsChecked is true;
        public bool IsTransparencyReverse => this.TransparencyReverseButton.IsChecked is true;


        Point StartingPosition;
        bool ViewIsEnabled;


        public ContextAppBar()
        {
            this.InitializeComponent();

            this.Thumb.DragStarted += (s, e) => this.StartingPosition = new Point(this.Transform.X, this.Transform.Y);
            this.Thumb.DragDelta += (s, e) =>
            {
                if (this.RootGrid.Margin.Bottom is 0)
                {
                    this.StartingPosition.Y += e.VerticalChange;

                    this.Transform.Y = System.Math.Clamp(this.StartingPosition.Y, 50, this.SwitchPresenter.ActualHeight);
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


            this.RadianClearButton.Tapped += (s, e) => this.RadianStoryboard.Begin(); // Storyboard
            this.RadianSlider.ValueChanged += (s, e) =>
            {
                if (this.ViewIsEnabled is false) return;
                double radian = e.NewValue / 180 * System.Math.PI;
                this.RadianValueChanged?.Invoke(this, (float)radian); // Delegate
            };

            this.ScaleClearButton.Tapped += (s, e) => this.ScaleStoryboard.Begin(); // Storyboard
            this.ScaleSlider.Minimum = this.ScaleRange.XRange.Minimum;
            this.ScaleSlider.Maximum = this.ScaleRange.XRange.Maximum;
            this.ScaleSlider.ValueChanged += (s, e) =>
            {
                if (this.ViewIsEnabled is false) return;
                double scale = this.ScaleRange.ConvertXToY(e.NewValue);
                this.ScaleValueChanged?.Invoke(this, (float)scale); // Delegate
            };
            this.TransformComboBox.SelectionChanged += (s, e) =>
            {
                this.CanvasVirtualControlInvalidate?.Invoke();
                this.CanvasControlInvalidate?.Invoke();
            };

            this.RoundRectCornerSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.TriangleCenterSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.DiamondMidSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.PentagonPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.StarPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.StarInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CogCountSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CogInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CogToothSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CogNotchSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.DountHoleRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.PieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CookieInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.CookieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.ArrowWidthSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.ArrowLeftTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.ArrowRightTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate?.Invoke();
            this.HeartSpreadSlider.ValueChanged += (s, e) => this.GeometryInvalidate?.Invoke();
        }


        public void ConstructInk(InkPresenter presenter, bool onlyValue)
        {
            // 1.Minimum
            if (onlyValue is false)
            {
                this.SizeSlider.Minimum = this.SizeRange.XRange.Minimum;
                this.OpacitySlider.Minimum = 0d;
            }

            // 2.Value
            this.SizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
            this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);

            // 3.Maximum
            if (onlyValue is false)
            {
                this.SizeSlider.Maximum = this.SizeRange.XRange.Maximum;
                this.OpacitySlider.Maximum = 100d;
            }
        }

        public void ConstructView(CanvasTransformer transformer)
        {
            this.ViewIsEnabled = false;
            this.RadianSlider.Value = transformer.Radian * 180 / System.Math.PI;
            this.ScaleSlider.Value = this.ScaleRange.ConvertYToX(transformer.Scale);
            this.ViewIsEnabled = true;
        }

        public void Construct(OptionType type)
        {
            switch (this.GetState(type))
            {
                case ContextAppBarState.None:
                    this.SwitchPresenter.Value = default;
                    VisualStateManager.GoToState(this, "Normal", false);
                    break;
                case ContextAppBarState.Main:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, "Main", false);
                    break;
                case ContextAppBarState.Preview:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, "Preview", false);
                    break;
                case ContextAppBarState.DragPreview:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, "DragPreview", false);
                    break;
                default:
                    break;
            }
        }


        private OptionType GetType(OptionType type)
        {
            if (type is OptionType.MarqueeTransform) return OptionType.Transform;
            else if (type.IsMarquee()) return OptionType.Marquee;
            else if (type.IsSelection()) return OptionType.Selection;
            else if (type.IsPaint()) return OptionType.Paint;
            else return type;
        }

        private ContextAppBarState GetState(OptionType type)
        {
            if (type == default) return default;

            if (type.HasPreview() is false)
                return ContextAppBarState.Main;

            return type.AllowDrag() ? ContextAppBarState.DragPreview : ContextAppBarState.Preview;
        }


        public CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, OptionType type, ITransformerLTRB transformerLTRB)
        {
            switch (type)
            {
                case OptionType.GeometryRectangle:
                case OptionType.GeometryRectangleTransform:
                    return TransformerGeometry.CreateRectangle(resourceCreator, transformerLTRB);
                case OptionType.GeometryEllipse:
                case OptionType.GeometryEllipseTransform:
                    return TransformerGeometry.CreateEllipse(resourceCreator, transformerLTRB);
                case OptionType.GeometryRoundRect:
                case OptionType.GeometryRoundRectTransform:
                    return TransformerGeometry.CreateRoundRect(resourceCreator, transformerLTRB,
                        (float)(this.RoundRectCornerSlider.Value / 100));
                case OptionType.GeometryTriangle:
                case OptionType.GeometryTriangleTransform:
                    return TransformerGeometry.CreateTriangle(resourceCreator, transformerLTRB,
                        (float)(this.TriangleCenterSlider.Value / 100));
                case OptionType.GeometryDiamond:
                case OptionType.GeometryDiamondTransform:
                    return TransformerGeometry.CreateDiamond(resourceCreator, transformerLTRB,
                        (float)(this.DiamondMidSlider.Value / 100));
                case OptionType.GeometryPentagon:
                case OptionType.GeometryPentagonTransform:
                    return TransformerGeometry.CreatePentagon(resourceCreator, transformerLTRB,
                        (int)this.PentagonPointsSlider.Value);
                case OptionType.GeometryStar:
                case OptionType.GeometryStarTransform:
                    return TransformerGeometry.CreateStar(resourceCreator, transformerLTRB,
                        (int)this.StarPointsSlider.Value,
                        (float)(this.StarInnerRadiusSlider.Value / 100));
                case OptionType.GeometryCog:
                case OptionType.GeometryCogTransform:
                    return TransformerGeometry.CreateCog(resourceCreator, transformerLTRB,
                        (int)this.CogCountSlider.Value,
                        (float)(this.CogInnerRadiusSlider.Value / 100),
                        (float)(this.CogToothSlider.Value / 100),
                        (float)(this.CogNotchSlider.Value / 100));
                case OptionType.GeometryDount:
                case OptionType.GeometryDountTransform:
                    return TransformerGeometry.CreateDount(resourceCreator, transformerLTRB,
                        (float)(this.DountHoleRadiusSlider.Value / 100));
                case OptionType.GeometryPie:
                case OptionType.GeometryPieTransform:
                    return TransformerGeometry.CreatePie(resourceCreator, transformerLTRB,
                        (float)this.PieSweepAngleSlider.Value * FanKit.Math.Pi / 180);
                case OptionType.GeometryCookie:
                case OptionType.GeometryCookieTransform:
                    return TransformerGeometry.CreateCookie(resourceCreator, transformerLTRB,
                        (float)this.CookieInnerRadiusSlider.Value / 100,
                        (float)this.CookieSweepAngleSlider.Value * FanKit.Math.Pi / 180);
                case OptionType.GeometryArrow:
                case OptionType.GeometryArrowTransform:
                    return TransformerGeometry.CreateArrow(resourceCreator, transformerLTRB,
                        false, 10,
                        (float)(this.ArrowWidthSlider.Value / 100),
                        this.ArrowLeftTailComboBox.SelectedIndex is 0 ?
                            GeometryArrowTailType.None :
                            GeometryArrowTailType.Arrow,
                        this.ArrowRightTailComboBox.SelectedIndex is 0 ?
                            GeometryArrowTailType.None :
                            GeometryArrowTailType.Arrow);
                case OptionType.GeometryCapsule:
                case OptionType.GeometryCapsuleTransform:
                    return TransformerGeometry.CreateCapsule(resourceCreator, transformerLTRB);
                case OptionType.GeometryHeart:
                case OptionType.GeometryHeartTransform:
                    return TransformerGeometry.CreateHeart(resourceCreator, transformerLTRB,
                        (float)(this.HeartSpreadSlider.Value / 100));
                default:
                    return TransformerGeometry.CreateRectangle(resourceCreator, transformerLTRB);
            }
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