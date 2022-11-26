using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Controls
{
    public interface INumberSlider
    {
        /// <summary> <see cref="RangeBase.Value"/> </summary>
        int Number { get; }
        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        int NumberMinimum { get; }
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        int NumberMaximum { get; }

        string Unit { get; }

        /// <summary> <see cref="FlyoutBase.Target"/> </summary>
        FrameworkElement PlacementTarget { get; }
    }

    //@Delegate
    public delegate void NumberChangedHandler(object sender, int number);

    public sealed partial class NumberPicker : UserControl
    {
        //@Delegate
        public event NumberChangedHandler NumberChanging = null;
        public event NumberChangedHandler PrimaryButtonClick = null;
        public event NumberChangedHandler SecondaryButtonClick = null;

        //@Content
        public bool IsNegative { get; private set; }
        public int Absnumber { get; private set; }

        /// <summary> <see cref="RangeBase.Minimum"/> </summary>
        public int Minimum { get; private set; }
        /// <summary> <see cref="RangeBase.Maximum"/> </summary>
        public int Maximum { get; private set; } = 100;

        public string Unit { get; private set; }

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
                    this.Absnumber = 0;
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    if (this.CanInput() is false) return;
                    this.Absnumber *= 10;

                    this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };

            this.ClearButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Absnumber = 0;
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.IsNegative = false;
                    this.Absnumber = 0;

                    this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };
            this.BackButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Absnumber = 0;
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.Absnumber /= 10;

                    this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };
            this.NegativeButton.Click += (s, e) =>
            {
                if (this.Timers is 0)
                {
                    this.Invalidate(); // Invalidate
                }
                else
                {
                    if (this.IsZero()) return;
                    this.IsNegative = !this.IsNegative;

                    this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
                    this.Invalidate(); // Invalidate
                }
            };

            this.OKButton.Click += (s, e) =>
            {
                this.PrimaryButtonClick?.Invoke(this, this.ToValue()); // Delegate
            };
            this.CancelButton.Click += (s, e) =>
            {
                this.SecondaryButtonClick?.Invoke(this, this.ToValue()); // Delegate
            };

            this.PasteButton.Click += async (s, e) =>
            {
                DataPackageView view = Clipboard.GetContent();
                if (view.Contains(StandardDataFormats.Text))
                {
                    string text = await view.GetTextAsync();
                    if (string.IsNullOrEmpty(text)) return;

                    text = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]+", "");
                    if (string.IsNullOrEmpty(text)) return;

                    if (int.TryParse(text, out int value))
                    {
                        this.IsNegative = value < 0;
                        this.Absnumber = System.Math.Abs(value);

                        this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
                        this.Invalidate(); // Invalidate
                    }
                    else if (double.TryParse(text, out double value2))
                    {
                        int value3 = (int)value2;
                        this.IsNegative = value3 < 0;
                        this.Absnumber = System.Math.Abs(value3);

                        this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
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
        }
        public void Open()
        {
            // Cursor
            this.Tick = true;
            this.CursorBrush.Opacity = 1;

            this.Timer.Start();
        }

        public void Construct(INumberSlider placementTarget)
        {
            this.IsNegative = placementTarget.Number < 0;
            this.Absnumber = System.Math.Abs(placementTarget.Number);

            this.Minimum = placementTarget.NumberMinimum;
            this.Maximum = placementTarget.NumberMaximum;

            this.Unit = placementTarget.Unit;

            // Cursor
            this.Tick = true;
            this.CursorBrush.Opacity = 1;

            // UI
            this.Timers = 0;
            VisualStateManager.GoToState(this, this.InRange() ? nameof(Normal) : nameof(Disabled), true);
            this.TitleRun.Text = this.ToString();
            this.TitleTextBlock.SelectAll();
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
            this.TitleRun.Text = this.ToString();
        }
        private void Input(int value)
        {
            if (this.Timers is 0)
            {
                this.Absnumber = value;
            }
            else
            {
                if (this.CanInput() is false) return;
                this.Absnumber = this.Absnumber * 10 + value;
            }

            this.NumberChanging?.Invoke(this, this.ToValue()); // Delegate
            this.Invalidate(); // Invalidate
        }


        //@Override   
        /// <summary> <see cref="Int32.MaxValue"/> / 100 </summary>
        public bool CanInput() => this.Absnumber < 21474836;
        public bool IsZero() => this.Absnumber is 0;
        public bool InRange()
        {
            int value = this.IsNegative ? -this.Absnumber : this.Absnumber;
            if (value < this.Minimum) return false;
            if (value > this.Maximum) return false;
            return true;
        }
        public int ToValue()
        {
            int value = this.IsNegative ? -this.Absnumber : this.Absnumber;
            if (value <= this.Minimum) return this.Minimum;
            if (value >= this.Maximum) return this.Maximum;
            return value;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Unit))
            {
                if (this.IsNegative)
                    return $"- {this.Absnumber}";
                else
                    return $"{this.Absnumber}";
            }
            else
            {
                if (this.IsNegative)
                    return $"- {this.Absnumber} {this.Unit}";
                else
                    return $"{this.Absnumber} {this.Unit}";
            }
        }

    }
}