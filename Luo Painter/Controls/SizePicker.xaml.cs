using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    /// <summary>
    /// Represents a control for selecting a size.
    /// </summary>
    public sealed partial class SizePicker : UserControl
    {

        //@Converter
        public  string Round2Converter(double value) => $"{value:0.00}";

        //@Key
        private bool IsRatio => this.ToggleButton.IsChecked is true;

        /// <summary> Minimum. Default 16. </summary>
        public int Minimum { get; set; } = 16;
        /// <summary> Maximum. Default 16384. </summary>
        public int Maximum { get; set; } = 16384;

        /// <summary> Size. </summary>
        public System.Drawing.Size Size => this.SizeCore;
        private System.Drawing.Size SizeCore = new System.Drawing.Size
        {
            Width = 1024,
            Height = 1024
        };

        private double CacheWidth = 1024;
        private double CacheHeight = 1024;

        //@Construct
        /// <summary>
        /// Initializes a SizePicker. 
        /// </summary>
        public SizePicker()
        {
            this.InitializeComponent();

            this.WidthTextBox.Text = 1024.ToString();
            this.WidthTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.HeightTextBox.Focus(FocusState.Programmatic);
                        break;
                    default:
                        break;
                }
            };
            this.WidthTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(this.WidthTextBox.Text))
                {
                    this.WidthTextBox.Text = this.Round2Converter(this.CacheWidth);
                    return;
                }

                bool result = double.TryParse(this.WidthTextBox.Text, out double value);
                if (result is false)
                {
                    this.WidthTextBox.Text = this.Round2Converter(this.CacheWidth);
                    return;
                }

                if (value < this.Minimum)
                {
                    if (this.IsRatio) this.ResezingHeight(this.Minimum / this.CacheWidth * this.CacheHeight);

                    this.SizeCore.Width = this.Minimum;
                    this.CacheWidth = this.Minimum;
                    this.WidthTextBox.Text = this.Minimum.ToString();
                }
                else if (value > this.Maximum)
                {
                    if (this.IsRatio) this.ResezingHeight(this.Maximum / this.CacheWidth * this.CacheHeight);

                    this.SizeCore.Width = this.Maximum;
                    this.CacheWidth = this.Maximum;
                    this.WidthTextBox.Text = this.Maximum.ToString();
                }
                else
                {
                    if (this.IsRatio) this.ResezingHeight(value / this.CacheWidth * this.CacheHeight);

                    this.SizeCore.Width = (int)value;
                    this.CacheWidth = value;
                    this.WidthTextBox.Text = this.Round2Converter(value);
                }
            };

            this.HeightTextBox.Text = 1024.ToString();
            this.HeightTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.WidthTextBox.Focus(FocusState.Programmatic);
                        break;
                    default:
                        break;
                }
            };
            this.HeightTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(this.HeightTextBox.Text))
                {
                    this.HeightTextBox.Text = this.Round2Converter(this.CacheHeight);
                    return;
                }

                bool result = double.TryParse(this.HeightTextBox.Text, out double value);
                if (result is false)
                {
                    this.HeightTextBox.Text = this.Round2Converter(this.CacheHeight);
                    return;
                }

                if (value < this.Minimum)
                {
                    if (this.IsRatio) this.ResezingWidth(this.Minimum / this.CacheHeight * this.CacheWidth);

                    this.SizeCore.Height = this.Minimum;
                    this.CacheHeight = this.Minimum;
                    this.HeightTextBox.Text = this.Minimum.ToString();
                }
                else if (value > this.Maximum)
                {
                    if (this.IsRatio) this.ResezingWidth(this.Maximum / this.CacheHeight * this.CacheWidth);

                    this.SizeCore.Height = this.Maximum;
                    this.CacheHeight = this.Maximum;
                    this.HeightTextBox.Text = this.Maximum.ToString();
                }
                else
                {
                    if (this.IsRatio) this.ResezingWidth(value / this.CacheHeight * this.CacheWidth);

                    this.SizeCore.Height = (int)value;
                    this.CacheHeight = value;
                    this.HeightTextBox.Text = this.Round2Converter(value);
                }
            };
        }

        public void ResezingWidth(double value)
        {
            if (value < this.Minimum)
            {
                this.SizeCore.Width = this.Minimum;
                this.CacheWidth = this.Minimum;
                this.WidthTextBox.Text = this.Minimum.ToString();
            }
            else if (value > this.Maximum)
            {
                this.SizeCore.Width = this.Maximum;
                this.CacheWidth = this.Maximum;
                this.WidthTextBox.Text = this.Maximum.ToString();
            }
            else
            {
                this.SizeCore.Width = (int)value;
                this.CacheWidth = value;
                this.WidthTextBox.Text = this.Round2Converter(value);
            }
        }

        public void ResezingHeight(double value)
        {
            if (value < this.Minimum)
            {
                this.SizeCore.Height = this.Minimum;
                this.CacheHeight = this.Minimum;
                this.HeightTextBox.Text = this.Minimum.ToString();
            }
            else if (value > this.Maximum)
            {
                this.SizeCore.Height = this.Maximum;
                this.CacheHeight = this.Maximum;
                this.HeightTextBox.Text = this.Maximum.ToString();
            }
            else
            {
                this.SizeCore.Height = (int)value;
                this.CacheHeight = value;
                this.HeightTextBox.Text = this.Round2Converter(value);
            }
        }

    }
}