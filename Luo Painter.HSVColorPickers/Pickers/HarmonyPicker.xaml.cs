using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class HarmonyPicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event EventHandler<Color> ColorChangedCompleted;

        public event EventHandler<Color> Color1Changed;
        public event EventHandler<Color> Color2Changed;
        public event EventHandler<Color> Color3Changed;

        public ColorType Type => ColorType.Harmony;
        public HarmonyMode Mode { get; private set; }

        Point Ring;
        double Slider;
        Vector4 HSV = Vector4.UnitW;
        Vector4 HSV1 = Vector4.UnitW;
        Vector4 HSV2 = Vector4.UnitW;
        Vector4 HSV3 = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="HarmonyPicker"/>. </summary>
        private RingTemplateSettings RingSize
        {
            get => (RingTemplateSettings)base.GetValue(RingSizeProperty);
            set => base.SetValue(RingSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "HarmonyPicker.RingSize" /> dependency property. </summary>
        private static readonly DependencyProperty RingSizeProperty = DependencyProperty.Register(nameof(RingSize), typeof(RingTemplateSettings), typeof(HarmonyPicker), new PropertyMetadata(new RingTemplateSettings(new BoxTemplateSettings(320)), (sender, e) =>
        {
            HarmonyPicker control = (HarmonyPicker)sender;

            if (e.NewValue is RingTemplateSettings value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public HarmonyPicker()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.RingSize = new RingTemplateSettings(new BoxTemplateSettings(e.NewSize.Width, e.NewSize.Height));
            };

            this.RingEllipse.ManipulationMode = ManipulationModes.All;
            this.RingEllipse.ManipulationStarted += (_, e) =>
            {
                if (e.PointerDeviceType == default)
                {
                    this.Transform.ScaleX = 1.8;
                    this.Transform.ScaleY = 1.8;
                }

                this.Ring = this.RingSize.CircleSize.Offset(e.Position);
                this.Move();
            };
            this.RingEllipse.ManipulationDelta += (_, e) =>
            {
                this.Ring = this.RingSize.CircleSize.Offset(e.Position);

                switch (base.FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        this.Ring.X += e.Delta.Translation.X;
                        break;
                    case FlowDirection.RightToLeft:
                        this.Ring.X -= e.Delta.Translation.X;
                        break;
                    default:
                        break;
                }
                this.Ring.Y += e.Delta.Translation.Y;
                this.Move();
            };
            this.RingEllipse.ManipulationCompleted += (_, e) =>
            {
                Color color = this.HSV.ToColor();
                this.ColorChangedCompleted?.Invoke(this, color); // Delegate

                if (e.PointerDeviceType == default)
                {
                    this.Transform.ScaleX = 1;
                    this.Transform.ScaleY = 1;
                }
            };

            this.SliderRectangle.ManipulationMode = ManipulationModes.TranslateX;
            this.SliderRectangle.ManipulationStarted += (_, e) =>
            {
                this.Slider = e.Position.X;
                this.Zoom();
            };
            this.SliderRectangle.ManipulationDelta += (_, e) =>
            {
                switch (base.FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        this.Slider += e.Delta.Translation.X;
                        break;
                    case FlowDirection.RightToLeft:
                        this.Slider -= e.Delta.Translation.X;
                        break;
                    default:
                        break;
                }
                this.Zoom();
            };
            this.SliderRectangle.ManipulationCompleted += (_, e) =>
            {              
                Color color = this.HSV.ToColor();
                this.ColorChangedCompleted?.Invoke(this, color); // Delegate
            };
        }
    }

    public sealed partial class HarmonyPicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Slider = this.HSV.Y * this.RingSize.BoxSize.Width;

            this.Line(this.Slider);
            this.Ellipse(this.HSV.Z * Math.PI / 180d, this.HSV.X);

            this.EllipseSolidColorBrush.Color = color;

            this.Point();
            this.PointToPoint();
        }
        private void Reset(RingTemplateSettings b)
        {
            this.Slider = this.HSV.Y * b.BoxSize.Width;

            this.Line(this.Slider);
            this.Ellipse(this.HSV.Z * Math.PI / 180d, this.HSV.X);

            this.PointToPoint();
        }


        private void Move()
        {
            double h = WheelTemplateSettings.Atan2(this.Ring);
            this.HSV.Z = (float)((h * 180d / Math.PI + 360d) % 360d);

            double s = Math.Clamp(Ring.ToVector2().Length() / this.RingSize.CircleSize.Radius, 0d, 1d);
            this.HSV.X = (float)s;

            this.Ellipse(h, s);

            this.Color(this.HSV.ToColor());

            this.Point();
            this.PointToPoint();
        }

        private void Zoom()
        {
            this.HSV.Y = (float)Math.Clamp(this.Slider / this.RingSize.BoxSize.Width, 0d, 1d);
            this.Line(this.HSV.Y * this.RingSize.BoxSize.Width);

            this.Color(this.HSV.ToColor());

            this.Point();
            this.PointToPoint();
        }


        public void Left()
        {
            this.Ring.X -= 1;
            this.Move();
        }
        public void Right()
        {
            this.Ring.X += 1;
            this.Move();
        }

        public void Down()
        {
            this.Ring.Y += 1;
            this.Move();
        }
        public void Up()
        {
            this.Ring.Y -= 1;
            this.Move();
        }

        public void ZoomOut()
        {
            this.HSV.Y = (float)Math.Clamp(this.HSV.Y - 0.01d, 0d, 1d);
            this.Line(this.HSV.Y * this.RingSize.BoxSize.Width);

            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Y = (float)Math.Clamp(this.HSV.Y + 0.01d, 0d, 1d);
            this.Line(this.HSV.Y * this.RingSize.BoxSize.Width);

            this.Color(this.HSV.ToColor());
        }
    }

    public sealed partial class HarmonyPicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate

            this.EllipseSolidColorBrush.Color = color;
        }

        private void Line(double z)
        {
            this.BlackLine.X1 = this.WhiteLine.X1 = z;
            this.BlackLine.X2 = this.WhiteLine.X2 = z;
        }

        private void Ellipse(double h, double s)
        {
            Point xy = this.RingSize.XY(h, s);
            Canvas.SetLeft(this.BlackEllipse, xy.X - 14);
            Canvas.SetTop(this.BlackEllipse, xy.Y - 14);
            Canvas.SetLeft(this.WhiteEllipse, xy.X - 13);
            Canvas.SetTop(this.WhiteEllipse, xy.Y - 13);
        }
    }

    public sealed partial class HarmonyPicker
    {
        public void Remode(HarmonyMode mode)
        {
            if (this.Mode == mode) return;
            this.Mode = mode;

            this.BlackEllipse1.Visibility = mode.HasFlag(HarmonyMode.HasPoint1) ? Visibility.Visible : Visibility.Collapsed;
            this.BlackEllipse2.Visibility = mode.HasFlag(HarmonyMode.HasPoint2) ? Visibility.Visible : Visibility.Collapsed;
            this.BlackEllipse3.Visibility = mode.HasFlag(HarmonyMode.HasPoint3) ? Visibility.Visible : Visibility.Collapsed;

            this.BlackLine1.Visibility = mode.HasFlag(HarmonyMode.HasPointToPoint1) ? Visibility.Visible : Visibility.Collapsed;
            this.BlackLine2.Visibility = mode.HasFlag(HarmonyMode.HasPointToPoint2) ? Visibility.Visible : Visibility.Collapsed;
            this.BlackLine3.Visibility = mode.HasFlag(HarmonyMode.HasPointToPoint3) ? Visibility.Visible : Visibility.Collapsed;
            this.BlackLine4.Visibility = mode.HasFlag(HarmonyMode.HasPointToPoint4) ? Visibility.Visible : Visibility.Collapsed;

            this.Point();
            this.PointToPoint();
        }


        private void Point()
        {
            if (this.Mode == default) return;
            if (this.Mode.HasFlag(HarmonyMode.HasPoint1)) this.HSV1 = this.HSV;
            if (this.Mode.HasFlag(HarmonyMode.HasPoint2)) this.HSV2 = this.HSV;
            if (this.Mode.HasFlag(HarmonyMode.HasPoint3)) this.HSV3 = this.HSV;

            switch (this.Mode)
            {
                case HarmonyMode.None:
                    break;
                case HarmonyMode.Complementary:
                    this.HSV1.Z += 180;
                    break;
                case HarmonyMode.SplitComplementary:
                    this.HSV1.Z += 165;
                    this.HSV2.Z -= 165;
                    break;
                case HarmonyMode.Analogous:
                    this.HSV1.Z -= 15;
                    this.HSV2.Z += 15;
                    break;
                case HarmonyMode.Triadic:
                    this.HSV1.Z += 120;
                    this.HSV2.Z -= 120;
                    break;
                case HarmonyMode.Tetradic:
                    this.HSV1.Z += 90;
                    this.HSV2.Z += 180;
                    this.HSV3.Z -= 90;
                    break;
                default:
                    break;
            }

            if (this.Mode.HasFlag(HarmonyMode.HasPoint1))
            {
                this.HSV1.Z = (this.HSV1.Z + 360f) % 360f;

                Color color = this.HSV1.ToColor();
                this.Color1Changed?.Invoke(this, color); // Delegate

                this.Ellipse1SolidColorBrush.Color = color;
            }
            if (this.Mode.HasFlag(HarmonyMode.HasPoint2))
            {
                this.HSV2.Z = (this.HSV2.Z + 360f) % 360f;

                Color color = this.HSV2.ToColor();
                this.Color2Changed?.Invoke(this, color); // Delegate

                this.Ellipse2SolidColorBrush.Color = color;
            }
            if (this.Mode.HasFlag(HarmonyMode.HasPoint3))
            {
                this.HSV3.Z = (this.HSV3.Z + 360f) % 360f;

                Color color = this.HSV3.ToColor();
                this.Color3Changed?.Invoke(this, color); // Delegate

                this.Ellipse3SolidColorBrush.Color = color;
            }
        }


        private void PointToPoint()
        {
            if (this.Mode == default) return;
            Point xy = this.RingSize.XY(this.HSV.Z * Math.PI / 180d, this.HSV1.X);

            if (this.Mode.HasFlag(HarmonyMode.HasPoint1))
            {
                Point xy1 = this.RingSize.XY(this.HSV1.Z * Math.PI / 180d, this.HSV1.X);
                Canvas.SetLeft(this.BlackEllipse1, xy1.X - 9);
                Canvas.SetTop(this.BlackEllipse1, xy1.Y - 9);

                switch (this.Mode)
                {
                    case HarmonyMode.Complementary:
                        this.BlackLine1.X1 = xy.X;
                        this.BlackLine1.Y1 = xy.Y;

                        this.BlackLine1.X2 = xy1.X;
                        this.BlackLine1.Y2 = xy1.Y;
                        break;
                    default:
                        break;
                }

                if (this.Mode.HasFlag(HarmonyMode.HasPoint2))
                {
                    Point xy2 = this.RingSize.XY(this.HSV2.Z * Math.PI / 180d, this.HSV2.X);
                    Canvas.SetLeft(this.BlackEllipse2, xy2.X - 9);
                    Canvas.SetTop(this.BlackEllipse2, xy2.Y - 9);

                    switch (this.Mode)
                    {
                        case HarmonyMode.SplitComplementary:
                        case HarmonyMode.Analogous:
                            this.BlackLine1.X1 = xy.X;
                            this.BlackLine1.Y1 = xy.Y;

                            this.BlackLine1.X2 = this.RingSize.Center.X;
                            this.BlackLine1.Y2 = this.RingSize.Center.Y;


                            this.BlackLine2.X1 = xy1.X;
                            this.BlackLine2.Y1 = xy1.Y;

                            this.BlackLine2.X2 = this.RingSize.Center.X;
                            this.BlackLine2.Y2 = this.RingSize.Center.Y;


                            this.BlackLine3.X1 = xy2.X;
                            this.BlackLine3.Y1 = xy2.Y;

                            this.BlackLine3.X2 = this.RingSize.Center.X;
                            this.BlackLine3.Y2 = this.RingSize.Center.Y;
                            break;
                        case HarmonyMode.Triadic:
                            this.BlackLine1.X1 = xy.X;
                            this.BlackLine1.Y1 = xy.Y;

                            this.BlackLine1.X2 = xy1.X;
                            this.BlackLine1.Y2 = xy1.Y;


                            this.BlackLine2.X1 = xy1.X;
                            this.BlackLine2.Y1 = xy1.Y;

                            this.BlackLine2.X2 = xy2.X;
                            this.BlackLine2.Y2 = xy2.Y;


                            this.BlackLine3.X1 = xy.X;
                            this.BlackLine3.Y1 = xy.Y;

                            this.BlackLine3.X2 = xy2.X;
                            this.BlackLine3.Y2 = xy2.Y;
                            break;
                        default:
                            break;
                    }

                    if (this.Mode.HasFlag(HarmonyMode.HasPoint3))
                    {
                        Point xy3 = this.RingSize.XY(this.HSV3.Z * Math.PI / 180d, this.HSV3.X);
                        Canvas.SetLeft(this.BlackEllipse3, xy3.X - 9);
                        Canvas.SetTop(this.BlackEllipse3, xy3.Y - 9);

                        switch (this.Mode)
                        {
                            case HarmonyMode.Tetradic:
                                this.BlackLine1.X1 = xy.X;
                                this.BlackLine1.Y1 = xy.Y;

                                this.BlackLine1.X2 = xy1.X;
                                this.BlackLine1.Y2 = xy1.Y;


                                this.BlackLine2.X1 = xy2.X;
                                this.BlackLine2.Y1 = xy2.Y;

                                this.BlackLine2.X2 = xy1.X;
                                this.BlackLine2.Y2 = xy1.Y;


                                this.BlackLine3.X1 = xy2.X;
                                this.BlackLine3.Y1 = xy2.Y;

                                this.BlackLine3.X2 = xy3.X;
                                this.BlackLine3.Y2 = xy3.Y;


                                this.BlackLine4.X1 = xy.X;
                                this.BlackLine4.Y1 = xy.Y;

                                this.BlackLine4.X2 = xy3.X;
                                this.BlackLine4.Y2 = xy3.Y;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}