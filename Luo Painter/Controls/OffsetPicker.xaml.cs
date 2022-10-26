using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    /// <summary>
    /// Represents a control for selecting a offset.
    /// </summary>
    public sealed partial class OffsetPicker : UserControl
    {

        //@Converter
        public  string Round2Converter(double value) => $"{value:0.00}";

        //@Content
        /// <summary> XTextBlock's Text. </summary>
        public string XText { get => this.XTextBlock.Text; set => this.XTextBlock.Text = value; }
        /// <summary> YTextBlock's Text. </summary>
        public string YText { get => this.YTextBlock.Text; set => this.YTextBlock.Text = value; }

        /// <summary> Minimum. Default -16384. </summary>
        public int Minimum { get; set; } = -16384;
        /// <summary> Maximum. Default 16384. </summary>
        public int Maximum { get; set; } = 16384;

        /// <summary> Offset. </summary>
        public System.Drawing.Point Offset => this.OffsetCore;
        private System.Drawing.Point OffsetCore = new System.Drawing.Point
        {
            X = 0,
            Y = 0
        };

        private double CacheX = 0;
        private double CacheY = 0;

        //@Construct
        /// <summary>
        /// Initializes a OffsetPicker. 
        /// </summary>
        public OffsetPicker()
        {
            this.InitializeComponent();

            this.XTextBox.Text = 0.ToString();
            this.XTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.YTextBox.Focus(FocusState.Programmatic);
                        break;
                    default:
                        break;
                }
            };
            this.XTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(this.XTextBox.Text))
                {
                    this.XTextBox.Text = this.Round2Converter(this.CacheX);
                    return;
                }

                bool result = double.TryParse(this.XTextBox.Text, out double value);
                if (result is false)
                {
                    this.XTextBox.Text = this.Round2Converter(this.CacheX);
                    return;
                }

                if (value < this.Minimum)
                {
                    this.OffsetCore.X = this.Minimum;
                    this.CacheX = this.Minimum;
                    this.XTextBox.Text = this.Minimum.ToString();
                }
                else if (value > this.Maximum)
                {
                    this.OffsetCore.X = this.Maximum;
                    this.CacheX = this.Maximum;
                    this.XTextBox.Text = this.Maximum.ToString();
                }
                else
                {
                    this.OffsetCore.X = (int)value;
                    this.CacheX = value;
                    this.XTextBox.Text = this.Round2Converter(value);
                }
            };

            this.YTextBox.Text = 0.ToString();
            this.YTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.XTextBox.Focus(FocusState.Programmatic);
                        break;
                    default:
                        break;
                }
            };
            this.YTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(this.YTextBox.Text))
                {
                    this.YTextBox.Text = this.Round2Converter(this.CacheY);
                    return;
                }

                bool result = double.TryParse(this.YTextBox.Text, out double value);
                if (result is false)
                {
                    this.YTextBox.Text = this.Round2Converter(this.CacheY);
                    return;
                }

                if (value < this.Minimum)
                {
                    this.OffsetCore.Y = this.Minimum;
                    this.CacheY = this.Minimum;
                    this.YTextBox.Text = this.Minimum.ToString();
                }
                else if (value > this.Maximum)
                {
                    this.OffsetCore.Y = this.Maximum;
                    this.CacheY = this.Maximum;
                    this.YTextBox.Text = this.Maximum.ToString();
                }
                else
                {
                    this.OffsetCore.Y = (int)value;
                    this.CacheY = value;
                    this.YTextBox.Text = this.Round2Converter(value);
                }
            };
        }

        public void ResezingX(double value)
        {
            if (value < this.Minimum)
            {
                this.OffsetCore.X = this.Minimum;
                this.CacheX = this.Minimum;
                this.XTextBox.Text = this.Minimum.ToString();
            }
            else if (value > this.Maximum)
            {
                this.OffsetCore.X = this.Maximum;
                this.CacheX = this.Maximum;
                this.XTextBox.Text = this.Maximum.ToString();
            }
            else
            {
                this.OffsetCore.X = (int)value;
                this.CacheX = value;
                this.XTextBox.Text = this.Round2Converter(value);
            }
        }

        public void ResezingY(double value)
        {
            if (value < this.Minimum)
            {
                this.OffsetCore.Y = this.Minimum;
                this.CacheY = this.Minimum;
                this.YTextBox.Text = this.Minimum.ToString();
            }
            else if (value > this.Maximum)
            {
                this.OffsetCore.Y = this.Maximum;
                this.CacheY = this.Maximum;
                this.YTextBox.Text = this.Maximum.ToString();
            }
            else
            {
                this.OffsetCore.Y = (int)value;
                this.CacheY = value;
                this.YTextBox.Text = this.Round2Converter(value);
            }
        }

    }
}