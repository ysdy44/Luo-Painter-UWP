using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class CirclePicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        
        Point Ring;
        double Slider;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="CirclePicker"/>. </summary>
        private RingTemplateSettings RingSize
        {
            get => (RingTemplateSettings)base.GetValue(RingSizeProperty);
            set => base.SetValue(RingSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "CirclePicker.RingSize" /> dependency property. </summary>
        private static readonly DependencyProperty RingSizeProperty = DependencyProperty.Register(nameof(RingSize), typeof(RingTemplateSettings), typeof(CirclePicker), new PropertyMetadata(new RingTemplateSettings(new BoxTemplateSettings(320)), (sender, e) =>
        {
            CirclePicker control = (CirclePicker)sender;

            if (e.NewValue is RingTemplateSettings value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public CirclePicker()
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
                this.Ring = this.RingSize.CircleSize.Offset(e.Position);
                this.Move();
            };
            this.RingEllipse.ManipulationDelta += (_, e) =>
            {
                this.Ring = this.RingSize.CircleSize.Offset(e.Position);

                this.Ring.X += e.Delta.Translation.X;
                this.Ring.Y += e.Delta.Translation.Y;
                this.Move();
            };
            this.RingEllipse.ManipulationCompleted += (_, e) =>
            {
                Color color = this.HSV.ToColor();
                this.Color(color);
            };

            this.SliderRectangle.ManipulationMode = ManipulationModes.TranslateX;
            this.SliderRectangle.ManipulationStarted += (_, e) =>
            {
                this.Slider = e.Position.X;
                this.Zoom();
            };
            this.SliderRectangle.ManipulationDelta += (_, e) =>
            {
                this.Slider += e.Delta.Translation.X;
                this.Zoom();
            };
            this.SliderRectangle.ManipulationCompleted += (_, e) =>
            {
                this.Color(this.HSV.ToColor());
            };
        }
    }

    public sealed partial class CirclePicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Slider = this.HSV.Y * this.RingSize.BoxSize.Width;

            this.Line(this.Slider);
            this.Ellipse(this.HSV.Z * Math.PI / 180d, this.HSV.X);
        }
        private void Reset(RingTemplateSettings b)
        {
            this.Slider = this.HSV.Y * b.BoxSize.Width;

            this.Line(this.Slider);
            this.Ellipse(this.HSV.Z * Math.PI / 180d, this.HSV.X);
        }


        private void Move()
        {
            double h = WheelTemplateSettings.Atan2(this.Ring);
            this.HSV.Z = (float)((h * 180d / Math.PI + 360d) % 360d);

            double s = Math.Clamp(Ring.ToVector2().Length() / this.RingSize.CircleSize.Radius, 0d, 1d);
            this.HSV.X = (float)s;

            this.Ellipse(h, s);

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            this.HSV.Y = (float)Math.Clamp(this.Slider / this.RingSize.BoxSize.Width, 0d, 1d);
            this.Line(this.HSV.Y * this.RingSize.BoxSize.Width);

            Color color = this.HSV.ToColor();
            this.Color(color);
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

    public sealed partial class CirclePicker
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
            var xy = this.RingSize.CircleSize.XY(h, s);
            xy.X += this.RingSize.Left;
            xy.Y += this.RingSize.Top;

            Canvas.SetLeft(this.BlackEllipse, xy.X - 14);
            Canvas.SetTop(this.BlackEllipse, xy.Y - 14);
            Canvas.SetLeft(this.WhiteEllipse, xy.X - 13);
            Canvas.SetTop(this.WhiteEllipse, xy.Y - 13);
        }
    }
}