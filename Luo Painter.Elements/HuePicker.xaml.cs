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

        Point Box;
        double Slider;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="HuePicker"/>. </summary>
        private BoxSize B
        {
            get => (BoxSize)base.GetValue(BProperty);
            set => base.SetValue(BProperty, value);
        }
        /// <summary> Identifies the <see cref = "HuePicker.B" /> dependency property. </summary>
        private static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(BoxSize), typeof(HuePicker), new PropertyMetadata(new BoxSize(320), (sender, e) =>
        {
            HuePicker control = (HuePicker)sender;

            if (e.NewValue is BoxSize value)
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

                this.B = new BoxSize(e.NewSize.Width, e.NewSize.Height);
            };

            this.BoxRectangle.ManipulationMode = ManipulationModes.All;
            this.BoxRectangle.ManipulationStarted += (_, e) =>
            {
                this.Box = e.Position;
                this.Move();
            };
            this.BoxRectangle.ManipulationDelta += (_, e) =>
            {
                this.Box.X += e.Delta.Translation.X;
                this.Box.Y += e.Delta.Translation.Y;
                this.Move();
            };
            this.BoxRectangle.ManipulationCompleted += (_, e) =>
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
                Color color = HSVExtensions.ToColor(this.HSV.Z);
                this.Stop(color);
                this.Color(this.HSV.ToColor());
            };
        }
    }

    public sealed partial class HuePicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Slider = this.HSV.Z * this.B.W / 360d;
            this.Box.X = this.HSV.X * this.B.W;
            this.Box.Y = this.B.H - this.HSV.Y * this.B.H;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }
        private void Reset(BoxSize b)
        {
            this.Slider = this.HSV.Z * b.W / 360d;
            this.Box.X = this.HSV.X * b.W;
            this.Box.Y = b.H - this.HSV.Y * b.H;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }


        private void Move()
        {
            this.HSV.X = (float)Math.Clamp(this.Box.X / this.B.W, 0d, 1d);
            this.HSV.Y = (float)Math.Clamp((this.B.H - this.Box.Y) / this.B.H, 0d, 1d);
            this.Ellipse(Math.Clamp(this.Box.X, 0d, this.B.W), Math.Clamp(this.Box.Y, 0d, this.B.H));

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            this.HSV.Z = (float)Math.Clamp(this.Slider * 360d / this.B.W, 0d, 360d);
            this.Line(this.HSV.Z * this.B.W / 360d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(color);
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

            this.Line(this.HSV.Z * this.B.W / 360d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * this.B.W / 360d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
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