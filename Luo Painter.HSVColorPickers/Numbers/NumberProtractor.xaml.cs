using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class NumberProtractor : UserControl, INumberBase
    {
        //@Delegate
        public event RoutedEventHandler Click { add => this.Button.Click += value; remove => this.Button.Click -= value; }
        public event NumberChangedHandler NumberChanged = null;

        //@Content
        public FrameworkElement PlacementTarget => this.Button;

        /// <summary> <see cref="RangeBase.Value"/> </summary>
        public float Angle { get; private set; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public float Minimum => 0;
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public float Maximum => System.MathF.PI * 2;

        /// <summary> <see cref="RangeBase.Value"/> </summary>
        public int Number { get; private set; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public int NumberMinimum => 0;
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public int NumberMaximum => 360;

        public string Unit => "º";

        Point Wheel;

        public NumberProtractor()
        {
            this.InitializeComponent();

            this.Line.X1 = 80 + 74;
            this.Line.Y1 = 80;
            this.Line.X2 = 80 + 70;
            this.Line.Y2 = 80;
            this.Button.Content = 0;

            this.Ellipse.ManipulationMode = ManipulationModes.All;
            this.Ellipse.ManipulationStarted += (_, e) =>
            {
                this.Wheel.X = e.Position.X - 80;
                this.Wheel.Y = 80 - e.Position.Y;

                this.SetAngle((float)WheelTemplateSettings.Atan2(this.Wheel));
                this.NumberChanged?.Invoke(this, this.Number); // Delegate
            };
            this.Ellipse.ManipulationDelta += (_, e) =>
            {
                this.Wheel.X += e.Delta.Translation.X;
                this.Wheel.Y -= e.Delta.Translation.Y;

                this.SetAngle((float)WheelTemplateSettings.Atan2(this.Wheel));
                this.NumberChanged?.Invoke(this, this.Number); // Delegate
            };
            this.Ellipse.ManipulationCompleted += (_, e) =>
            {
                this.SetAngle((float)WheelTemplateSettings.Atan2(this.Wheel));
                this.NumberChanged?.Invoke(this, this.Number); // Delegate
            };
        }

        public void SetAngle(float angle)
        {
            this.Angle = angle;
            this.Zoom(this.Angle);

            this.Number = (int)(angle * 180 / System.Math.PI);
            this.Button.Content = $"{this.Number}{this.Unit}";
        }
        public void SetNumber(int number)
        {
            this.Number = number;
            this.Button.Content = $"{this.Number}{this.Unit}";

            this.Angle = number * System.MathF.PI / 180;
            this.Zoom(this.Angle);
        }
        private void Zoom(double h)
        {
            double cos = System.Math.Cos(h);
            double sin = System.Math.Sin(h);

            this.Line.X1 = 80 + 74 * cos;
            this.Line.Y1 = 80 - 74 * sin;
            this.Line.X2 = 80 + 70 * cos;
            this.Line.Y2 = 80 - 70 * sin;
        }
    }
}