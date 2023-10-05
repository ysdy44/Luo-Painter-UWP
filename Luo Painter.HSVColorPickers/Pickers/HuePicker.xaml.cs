using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class HuePicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event EventHandler<Color> ColorChangedCompleted;

        public ColorType Type => ColorType.Hue;

        Point Box;
        double Slider;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="HuePicker"/>. </summary>
        private BoxTemplateSettings BoxSize
        {
            get => (BoxTemplateSettings)base.GetValue(BoxSizeProperty);
            set => base.SetValue(BoxSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "HuePicker.BoxSize" /> dependency property. </summary>
        private static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register(nameof(BoxSize), typeof(BoxTemplateSettings), typeof(HuePicker), new PropertyMetadata(new BoxTemplateSettings(320), (sender, e) =>
        {
            HuePicker control = (HuePicker)sender;

            if (e.NewValue is BoxTemplateSettings value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public HuePicker()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.BoxSize = new BoxTemplateSettings(e.NewSize.Width, e.NewSize.Height);
            };

            this.BoxRectangle.ManipulationMode = ManipulationModes.All;
            this.BoxRectangle.ManipulationStarted += (_, e) =>
            {
                if (e.PointerDeviceType == default)
                {
                    this.Transform.ScaleX = 1.8;
                    this.Transform.ScaleY = 1.8;
                }

                this.Box = e.Position;
                this.Move();
            };
            this.BoxRectangle.ManipulationDelta += (_, e) =>
            {
                switch (base.FlowDirection)
                {
                    case FlowDirection.LeftToRight:
                        this.Box.X += e.Delta.Translation.X;
                        break;
                    case FlowDirection.RightToLeft:
                        this.Box.X -= e.Delta.Translation.X;
                        break;
                    default:
                        break;
                }
                this.Box.Y += e.Delta.Translation.Y;
                this.Move();
            };
            this.BoxRectangle.ManipulationCompleted += (_, e) =>
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
                this.Stop(HSVExtensions.ToColor(this.HSV.Z));

                Color color = this.HSV.ToColor();
                this.ColorChangedCompleted?.Invoke(this, color); // Delegate
            };
        }
    }

    public sealed partial class HuePicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Slider = this.HSV.Z * this.BoxSize.Width / 360d;
            this.Box.X = this.HSV.X * this.BoxSize.Width;
            this.Box.Y = this.BoxSize.Height - this.HSV.Y * this.BoxSize.Height;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.EllipseSolidColorBrush.Color = color;
        }
        private void Reset(BoxTemplateSettings b)
        {
            this.Slider = this.HSV.Z * b.Width / 360d;
            this.Box.X = this.HSV.X * b.Width;
            this.Box.Y = b.Height - this.HSV.Y * b.Height;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }


        private void Move()
        {
            this.HSV.X = (float)Math.Clamp(this.Box.X / this.BoxSize.Width, 0d, 1d);
            this.HSV.Y = (float)Math.Clamp((this.BoxSize.Height - this.Box.Y) / this.BoxSize.Height, 0d, 1d);
            this.Ellipse(Math.Clamp(this.Box.X, 0d, this.BoxSize.Width), Math.Clamp(this.Box.Y, 0d, this.BoxSize.Height));
        
            this.Color(this.HSV.ToColor());
        }

        private void Zoom()
        {
            this.HSV.Z = (float)Math.Clamp(this.Slider * 360d / this.BoxSize.Width, 0d, 360d);
            this.Line(this.HSV.Z * this.BoxSize.Width / 360d);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.Color(this.HSV.ToColor());
        }


        public void Left()
        {
            this.Box.X -= 1;
            this.Move();
        }
        public void Right()
        {
            this.Box.X += 1;
            this.Move();
        }

        public void Down()
        {
            this.Box.Y += 1;
            this.Move();
        }
        public void Up()
        {
            this.Box.Y -= 1;
            this.Move();
        }

        public void ZoomOut()
        {
            this.HSV.Z -= 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * this.BoxSize.Width / 360d);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * this.BoxSize.Width / 360d);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.Color(this.HSV.ToColor());
        }
    }

    public sealed partial class HuePicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate

            this.EllipseSolidColorBrush.Color = color;
        }

        private void Stop(Color color)
        {
            this.LineSolidColorBrush.Color = color;

            this.EndStop.Color = color;
        }

        private void Line(double z)
        {
            this.BlackLine.X1 = this.WhiteLine.X1 = z;
            this.BlackLine.X2 = this.WhiteLine.X2 = z;
        }

        private void Ellipse(double x, double y)
        {
            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}