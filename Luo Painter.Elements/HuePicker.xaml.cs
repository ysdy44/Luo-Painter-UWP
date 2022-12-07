using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public sealed partial class HuePicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        double L => 20;
        double W => 320d;
        double H => 358d;
        double D => this.H - this.W;
        double Y1 => (this.H + this.W + this.L) / 2;
        double Y2 => (this.H + this.W - this.L) / 2;

        Point Box;
        double Slider;
        Vector4 HSV;

        //@Construct
        public HuePicker()
        {
            this.InitializeComponent();
            this.Recolor(Colors.Black);

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

            this.Slider = this.HSV.Z * this.W / 360d;
            this.Box.X = this.HSV.X * this.W;
            this.Box.Y = this.W - this.HSV.Y * this.W;

            this.Line(this.Slider);
            this.Ellipse(this.Box.X, this.Box.Y);
        }


        private void Move()
        {
            this.HSV.X = (float)Math.Clamp(this.Box.X / this.W, 0d, 1d);
            this.HSV.Y = (float)Math.Clamp((this.W - this.Box.Y) / this.W, 0d, 1d);
            this.Ellipse(this.HSV.X * this.W, this.W - this.HSV.Y * this.W);

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            this.HSV.Z = (float)Math.Clamp(this.Slider * 360d / this.W, 0d, 360d);
            this.Line(this.HSV.Z * this.W / 360d);

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

            this.Line(this.HSV.Z * this.W / 360d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * this.W / 360d);

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
            color.A = 0;
            this.StartStop.Color = color;
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