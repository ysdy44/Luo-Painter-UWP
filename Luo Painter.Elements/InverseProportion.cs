using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Rounding of <see cref="Range"/>.
    /// </summary>
    public enum RangeRounding
    {
        /// <summary>
        /// Minimum of <see cref="Range"/>.
        /// </summary>
        Minimum,
        /// <summary>
        /// Maximum of <see cref="Range"/>.
        /// </summary>
        Maximum,
    }

    /// <summary>
    /// Range of <see cref="InverseProportion"/>.
    /// </summary>
    public struct Range
    {
        /// <summary> The default of <see cref="Slider.Value"/> </summary>
        public double Default;
        /// <summary> <see cref="Slider.Minimum"/> </summary>
        public double Minimum => this.IP.Minimum;
        /// <summary> <see cref="Slider.Maximum"/> </summary>
        public double Maximum => this.IP.Maximum;

        /// <summary> Inverse Proportion </summary>
        public InverseProportion IP;

        /// <summary>
        /// Converts x to y or y to x.
        /// </summary>
        /// <param name="sourceInner"> The inner source. </param>
        /// <param name="sourceOuter"> The outer source. </param>
        /// <param name="destinationScale"> The scale of destination. </param>
        /// <returns> The product value. </returns>
        public Range Convert(InverseProportion sourceInner, InverseProportion sourceOuter, double destinationScale) => new Range
        {
            Default = InverseProportion.Convert(this.Default, sourceInner, sourceOuter, destinationScale),
            IP = new InverseProportion
            {
                Minimum = InverseProportion.Convert(this.Maximum, sourceInner, sourceOuter, destinationScale),
                Maximum = InverseProportion.Convert(this.Minimum, sourceInner, sourceOuter, destinationScale)
            }
        };

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString() => $"{this.Minimum}<{this.Default}<{this.Maximum}";
    }

    /// <summary>
    /// A curves for Inverse Proportion.
    /// </summary>
    public struct InverseProportion
    {
        /// <summary> <see cref="Slider.Minimum"/> </summary>
        public double Minimum;
        /// <summary> <see cref="Slider.Maximum"/> </summary>
        public double Maximum;

        /// <summary>
        /// Converts value to one.
        /// </summary>
        /// <param name="value"> The value. Range <see cref="InverseProportion.Minimum"/> <see cref="InverseProportion.Maximum"/>. </param>
        /// <returns> The product one. Range 0 1. </returns>
        public double ConvertValueToOne(double value, RangeRounding rounding = RangeRounding.Minimum)
        {
            switch (rounding)
            {
                case RangeRounding.Minimum: return (value - this.Minimum) / (this.Maximum - this.Minimum);
                case RangeRounding.Maximum: return (this.Maximum - value) / (this.Maximum - this.Minimum);
                default: return 0;
            }
        }
        /// <summary>
        /// Converts one to value.
        /// </summary>
        /// <param name="one"> The one. Range 0 1. </param>
        /// <returns> The product value. Range <see cref="InverseProportion.Minimum"/> <see cref="InverseProportion.Maximum"/>. </returns>
        public double ConvertOneToValue(double one, RangeRounding rounding = RangeRounding.Minimum)
        {
            switch (rounding)
            {
                case RangeRounding.Minimum: return this.Minimum + one * (this.Maximum - this.Minimum);
                case RangeRounding.Maximum: return this.Maximum - one * (this.Maximum - this.Minimum);
                default: return 0;
            }
        }

        /// <summary>
        /// Converts x to y or y to x.
        /// </summary>
        /// <returns> The product value. </returns>
        public InverseProportion Convert() => new InverseProportion
        {
            Minimum = InverseProportion.Convert(this.Maximum),
            Maximum = InverseProportion.Convert(this.Minimum),
        };


        //@Static
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Convert(double value) => 1.0 / value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Convert(double value, InverseProportion sourceInner, InverseProportion sourceOuter, double destinationScale)
        {
            double oneInner = sourceOuter.ConvertValueToOne(value);
            double ipInner = sourceInner.ConvertOneToValue(oneInner);

            double ipOuter = InverseProportion.Convert(ipInner);
            double oneOuter = sourceInner.ConvertValueToOne(ipOuter);

            return destinationScale * oneOuter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Convert(double value, InverseProportion sourceInner, InverseProportion sourceOuter, InverseProportion destinationInner, InverseProportion destinationOuter, RangeRounding sourceRounding = RangeRounding.Minimum, RangeRounding destinationRounding = RangeRounding.Minimum)
        {
            double oneInner = sourceOuter.ConvertValueToOne(value, sourceRounding);
            double ipInner = sourceInner.ConvertOneToValue(oneInner);

            double ipOuter = InverseProportion.Convert(ipInner);
            double oneOuter = destinationInner.ConvertValueToOne(ipOuter, destinationRounding);

            return destinationOuter.ConvertOneToValue(oneOuter);
        }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString() => $"{this.Minimum}<{this.Maximum}";
    }
}