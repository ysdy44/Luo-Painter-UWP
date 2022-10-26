using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Extension for <see cref="TextBox"/>.
    /// </summary>
    public sealed class TextBoxExtension
    {
        //@Converter
        private string Round2Converter(double value) => $"{value:0.00}";

        /// <summary> Minimum. </summary>
        public int Minimum { get; set; }

        /// <summary> Maximum. </summary>
        public int Maximum { get; set; }

        /// <summary> Value. </summary>
        public int Value { get; set; } = 1024;

        private double Cache = 1024;

        /// <summary>
        ///  Indicates whether the text box finds a number in the input string.
        /// </summary>
        /// <param name="textBox"> The text box. </param>
        /// <returns> **True** if the text box finds a number; otherwise, **False**. </returns>
        public bool IsMatch(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = this.Round2Converter(this.Cache);
                return false;
            }

            bool result = double.TryParse(textBox.Text, out double value);
            if (result is false)
            {
                textBox.Text = this.Round2Converter(this.Cache);
                return false;
            }

            if (value < this.Minimum)
            {
                this.Value = this.Minimum;
                this.Cache = this.Minimum;
                textBox.Text = this.Minimum.ToString();
                return true;
            }
            else if (value > this.Maximum)
            {
                this.Value = this.Maximum;
                this.Cache = this.Maximum;
                return true;
            }
            else
            {
                this.Value = (int)value;
                this.Cache = value;
                textBox.Text = this.Round2Converter(value);
                return true;
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}