using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Range of <see cref="InverseProportion"/>.
    /// </summary>
    public struct Range
    {
        /// <summary> The default of <see cref="Slider.Value"/> </summary>
        public double Default;
        /// <summary> <see cref="Slider.Minimum"/> </summary>
        public double Minimum;
        /// <summary> <see cref="Slider.Maximum"/> </summary>
        public double Maximum;

        /// <summary>
        /// Converts value to one.
        /// </summary>
        /// <param name="value"> The value. Range <see cref="Range.Minimum"/> <see cref="Range.Maximum"/>. </param>
        /// <returns> The product one. Range 0 1. </returns>
        public double ConvertValueToOne(double value) => (value - this.Minimum) / (this.Maximum - this.Minimum);
        /// <summary>
        /// Converts one to value.
        /// </summary>
        /// <param name="one"> The one. Range 0 1. </param>
        /// <returns> The product value. Range <see cref="Range.Minimum"/> <see cref="Range.Maximum"/>. </returns>
        public double ConvertOneToValue(double one) => this.Minimum + one * (this.Maximum - this.Minimum);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString() => $"{this.Minimum}<{this.Default}<{this.Maximum}";
    }

    /// <summary>
    /// A curves for Y = B / (A*X+C).
    /// </summary>
    public struct InverseProportion
    {
        /// <summary> The A. E.g: -1. </summary>
        public double A;
        /// <summary> The B. E.g: 10. </summary>
        public double B;
        /// <summary> The C. E.g: 1. </summary>
        public double C;

        /// <summary>
        /// Converts X to Y.
        /// </summary>
        /// <param name="x"> The X. </param>
        /// <returns> The Y. </returns>
        public double ConvertXToY(double x) => this.B / (this.A * x + this.C);
        /// <summary>
        /// Converts Y to X.
        /// </summary>
        /// <param name="x"> The Y. </param>
        /// <returns> The X. </returns>
        public double ConvertYToX(double y) => (this.B / y - this.C) / this.A;

        /// <summary>
        /// Converts X to Y.
        /// </summary>
        /// <param name="x"> The X. </param>
        /// <returns> The Y. </returns>
        public Range ConvertXToY(Range x) => new Range
        {
            Default = this.ConvertXToY(x.Default),
            Minimum = this.ConvertXToY(x.Minimum),
            Maximum = this.ConvertXToY(x.Maximum),
        };
        /// <summary>
        /// Converts Y to X.
        /// </summary>
        /// <param name="x"> The Y. </param>
        /// <returns> The X. </returns>
        public Range ConvertYToX(Range y) => new Range
        {
            Default = this.ConvertYToX(y.Default),
            Minimum = this.ConvertYToX(y.Minimum),
            Maximum = this.ConvertYToX(y.Maximum),
        };

    }
}