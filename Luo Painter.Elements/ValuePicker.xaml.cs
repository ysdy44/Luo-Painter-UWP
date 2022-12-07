using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public sealed partial class ValuePicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        Point Box;
        double Slider;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="ValuePicker"/>. </summary>
        private BoxSize B
        {
            get => (BoxSize)base.GetValue(BProperty);
            set => base.SetValue(BProperty, value);
        }
        /// <summary> Identifies the <see cref = "ValuePicker.B" /> dependency property. </summary>
        private static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(BoxSize), typeof(ValuePicker), new PropertyMetadata(new BoxSize(320), (sender, e) =>
        {
            ValuePicker control = (ValuePicker)sender;

            if (e.NewValue is BoxSize value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public ValuePicker()
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
                this.Color(this.HSV.ToColor());
            };
        }
    }

    public sealed partial class ValuePicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Slider = this.HSV.Y * this.B.W;
            this.Box.X = this.HSV.Z * this.B.W / 360d;
            this.Box.Y = this.B.H - this.HSV.X * this.B.H;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }
        private void Reset(BoxSize b)
        {
            this.Slider = this.HSV.Y * b.W;
            this.Box.X = this.HSV.Z * b.W / 360d;
            this.Box.Y = b.H - this.HSV.X * b.H;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }


        private void Move()
        {
            this.HSV.Z = (float)Math.Clamp(this.Box.X * 360d / this.B.W, 0d, 360d);
            this.HSV.X = (float)Math.Clamp((this.B.H - this.Box.Y) / this.B.H, 0d, 1d);
            this.Ellipse(Math.Clamp(this.Box.X, 0d, this.B.W), Math.Clamp(this.Box.Y, 0d, this.B.H));

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            this.HSV.Y = (float)Math.Clamp(this.Slider / this.B.W, 0d, 1d);
            this.Line(this.HSV.Y * this.B.W);

            Color color = this.HSV.ToColor();
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
            this.HSV.Y = (float)Math.Clamp(this.HSV.Y - 0.01d, 0d, 1d);
            this.Line(this.HSV.Y * this.B.W);

            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Y = (float)Math.Clamp(this.HSV.Y + 0.01d, 0d, 1d);
            this.Line(this.HSV.Y * this.B.W);

            this.Color(this.HSV.ToColor());
        }
    }

    public sealed partial class ValuePicker
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

        private void Ellipse(double x, double y)
        {
            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}