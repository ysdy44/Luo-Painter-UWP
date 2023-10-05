using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.HSVColorPickers
{
    public interface INumberPickerBase
    {
        //@Delegate
        event NumberChangedHandler ValueChanging;
        event NumberChangedHandler ValueChanged;

        bool IsNegative { get; }
        string Absnumber { get; }

        double Minimum { get; }
        double Maximum { get; }

        string Unit { get; }

        // UI
        void Close();
        void Open();

        void Construct(INumberBase number);

        bool IsZero();
        bool InRange();
        double ToValue();
        string ToString();
    }

    public interface INumberBase
    {
        /// <summary> <see cref="RangeBase.Value"/> </summary>
        double Value { get; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        double Minimum { get; }
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        double Maximum { get; }

        string Unit { get; }

        // UI
        FlowDirection FlowDirection { get; }

        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }
    }

    //@Delegate
    public delegate void NumberChangedHandler(object sender, double number);

    public sealed partial class NumberPicker : UserControl, INumberPickerBase
    {
        //@Delegate
        public event NumberChangedHandler ValueChanging = null;
        public event NumberChangedHandler ValueChanged = null;

        //@Content
        public bool IsNegative { get; private set; }
        public string Absnumber { get; private set; }

        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public double Minimum { get; private set; }
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public double Maximum { get; private set; } = 100;

        public string Unit { get; private set; }

        // Popup
        readonly Popup Popup = new Popup();
        readonly Border Border = new Border
        {
            CornerRadius = new CornerRadius(2),
            Padding = new Thickness(2, 0, 2, 0),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
            Background = new SolidColorBrush(Windows.UI.Colors.White),
        };
        readonly TextBlock TextBlock = new TextBlock
        {
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
        };

        // Cursor
        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(600)
        };

        bool Tick;
        int Timers;

        //@Construct
        public NumberPicker()
        {
            this.InitializeComponent();
            this.Border.Child = this.TextBlock;
            this.Popup.Child = this.Border;

            this.Timer.Tick += (s, e) =>
            {
                this.Tick = !this.Tick;
                this.CursorBrush.Opacity = this.Tick ? 1 : 0;
            };

            this.OneButton.Click += (s, e) => this.Input(1);
            this.TwoButton.Click += (s, e) => this.Input(2);
            this.ThreeButton.Click += (s, e) => this.Input(3);
            this.FourButton.Click += (s, e) => this.Input(4);
            this.FiveButton.Click += (s, e) => this.Input(5);
            this.SixButton.Click += (s, e) => this.Input(6);
            this.SevenButton.Click += (s, e) => this.Input(7);
            this.EightButton.Click += (s, e) => this.Input(8);
            this.NineButton.Click += (s, e) => this.Input(9);
            this.ZeroButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Absnumber = "0";
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.Absnumber += "0";

                    this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };
            this.DecimalButton.Click += (s, e) =>
            {
                if (this.Absnumber.Contains(".")) return;
                this.Absnumber += ".";

                this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                this.Invalidate(); // Invalidate
            };

            this.ClearButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Absnumber = "0";
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.IsNegative = false;
                    this.Absnumber = "0";

                    this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Absnumber = "0";
                    this.Invalidate(); // Invalidate
                }
                else if (this.Absnumber.Length < 2)
                {
                    this.Absnumber = "0";
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.Absnumber = this.Absnumber.Remove(this.Absnumber.Length - 2, 1);

                    this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };
            this.NegativeButton.Click += (s, e) =>
            {
                this.IsNegative = !this.IsNegative;

                this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                this.Invalidate(); // Invalidate
            };

            this.OKButton.Click += (s, e) =>
            {
                this.ValueChanged?.Invoke(this, this.ToValue()); // Delegate
            };

            this.PasteButton.Click += async (s, e) =>
            {
                DataPackageView view = Clipboard.GetContent();
                if (view.Contains(StandardDataFormats.Text))
                {
                    string text = await view.GetTextAsync();
                    if (string.IsNullOrEmpty(text)) return;

                    text = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]+[.]", "");
                    if (string.IsNullOrEmpty(text)) return;

                    if (double.TryParse(text, out double value))
                    {
                        this.IsNegative = value < 0;
                        this.Absnumber = System.Math.Abs(System.Math.Round(value, 2, MidpointRounding.ToEven)).ToString();

                        this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
                        this.Invalidate(); // Invalidate
                    }
                }
            };
        }

        public void Close()
        {
            // Cursor
            this.Tick = false;
            this.CursorBrush.Opacity = 0;

            this.Timer.Stop();

            // Popup
            this.Popup.IsOpen = false;
        }
        public void Open()
        {
            // Cursor
            this.Tick = true;
            this.CursorBrush.Opacity = 1;

            this.Timer.Start();

            // Popup
            this.Popup.IsOpen = true;
        }

        public void Construct(INumberBase number)
        {
            this.IsNegative = number.Value < 0;
            this.Absnumber = System.Math.Abs(System.Math.Round(number.Value, 2, MidpointRounding.ToEven)).ToString();

            this.Minimum = number.Minimum;
            this.Maximum = number.Maximum;

            this.Unit = number.Unit;

            // Cursor
            this.Tick = true;
            this.CursorBrush.Opacity = 1;

            // UI
            this.Timers = 0;
            VisualStateManager.GoToState(this, this.InRange() ? nameof(Normal) : nameof(Disabled), true);
            this.Run1.Text = this.IsNegative ? "-" : "";
            this.Run2.Text = this.Absnumber;
            this.TextBlock.Text = this.ToString();
            this.TitleTextBlock.SelectAll();

            // Popup
            Point transform = number.PlacementTarget.TransformToVisual(Window.Current.Content).TransformPoint(default);
            this.Popup.FlowDirection = number.FlowDirection;
            this.Popup.HorizontalOffset = transform.X;
            this.Popup.VerticalOffset = transform.Y;
            this.Popup.Width = this.Border.Width = number.PlacementTarget.ActualWidth;
            this.Popup.Height = this.Border.Height = number.PlacementTarget.ActualHeight;
        }
        private void Invalidate()
        {
            // Cursor
            this.Tick = true;
            this.CursorBrush.Opacity = 1;

            this.Timer.Stop();
            this.Timer.Start();

            // UI
            this.Timers++;
            VisualStateManager.GoToState(this, this.InRange() ? nameof(Normal) : nameof(Disabled), true);
            this.Run1.Text = this.IsNegative ? "-" : "";
            this.Run2.Text = this.Absnumber;
            this.TextBlock.Text = this.ToString();
        }
        private void Input(int value)
        {
            if (this.Timers is 0)
            {
                this.Absnumber = value.ToString();
            }
            else if (this.Absnumber is "0")
            {
                this.Absnumber = value.ToString();
            }
            else
            {
                this.Absnumber += value.ToString();
            }

            this.ValueChanging?.Invoke(this, this.ToValue()); // Delegate
            this.Invalidate(); // Invalidate
        }


        //@Override   
        public bool IsZero() => this.Absnumber is "0";
        public bool InRange()
        {
            if (double.TryParse(this.Absnumber, out double value))
            {
                value = this.IsNegative ? -value : value;
                if (value < this.Minimum) return false;
                if (value > this.Maximum) return false;
                return true;
            }
            else return false;
        }
        public double ToValue()
        {
            if (double.TryParse(this.Absnumber, out double value))
            {
                if (value <= this.Minimum) return this.Minimum;
                if (value >= this.Maximum) return this.Maximum;
                return value;
            }
            else return 0;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Unit))
            {
                if (this.IsNegative)
                    return $"-{this.Absnumber}";
                else
                    return $"{this.Absnumber}";
            }
            else
            {
                if (this.IsNegative)
                    return $"-{this.Absnumber}{this.Unit}";
                else
                    return $"{this.Absnumber}{this.Unit}";
            }
        }

    }
}