using Windows.Foundation;
using Windows.System;
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
        public double Angle { get; private set; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public double Minimum => 0;
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public double Maximum => System.MathF.PI * 2;

        /// <summary> <see cref="RangeBase.Value"/> </summary>
        public double Number { get; private set; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public double NumberMinimum => 0;
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public double NumberMaximum => 360;

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

                base.Focus(FocusState.Programmatic);
            };

            base.PointerWheelChanged += (s, e) =>
            {
                switch (base.FocusState)
                {
                    case FocusState.Unfocused:
                        break;
                    default:
                        if (e.GetCurrentPoint(this).Properties.MouseWheelDelta < 0)
                        {
                            switch (e.KeyModifiers)
                            {
                                case VirtualKeyModifiers.Menu:
                                case VirtualKeyModifiers.Shift:
                                    if (this.SetNumber((this.Number - 45) / 45 * 45 % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                                case VirtualKeyModifiers.Control:
                                case VirtualKeyModifiers.Windows:
                                    if (this.SetNumber((this.Number - 1) % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                                default:
                                    if (this.SetNumber((this.Number - 5) % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                            }
                        }
                        else
                        {
                            switch (e.KeyModifiers)
                            {
                                case VirtualKeyModifiers.Menu:
                                case VirtualKeyModifiers.Shift:
                                    if (this.SetNumber((this.Number + 45) / 45 * 45 % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                                case VirtualKeyModifiers.Control:
                                case VirtualKeyModifiers.Windows:
                                    if (this.SetNumber((this.Number + 1) % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                                default:
                                    if (this.SetNumber((this.Number + 5) % 360))
                                        this.NumberChanged?.Invoke(this, this.Number); // Delegate
                                    break;
                            }
                        }
                        break;
                }
            };
         
            base.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Down:
                    case VirtualKey.Left:
                        if (this.SetNumber((this.Number - 1) % 360))
                            this.NumberChanged?.Invoke(this, this.Number); // Delegate
                        break;
                    case VirtualKey.Up:
                    case VirtualKey.Right:
                        if (this.SetNumber((this.Number + 1) % 360))
                            this.NumberChanged?.Invoke(this, this.Number); // Delegate
                        break;
                    default:
                        break;
                }
            };
        }

        public bool SetAngle(float angle)
        {
            if (this.Angle == angle) return false;
            this.Angle = angle;
            this.Zoom(this.Angle);

            this.Number = (int)(angle * 180 / System.Math.PI);
            this.Button.Content = $"{this.Number}{this.Unit}";
            return true;
        }
        public bool SetNumber(double number)
        {
            if (this.Number == number) return false;
            this.Number = number;
            this.Button.Content = $"{this.Number}{this.Unit}";

            this.Angle = number * System.MathF.PI / 180;
            this.Zoom(this.Angle);
            return true;
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