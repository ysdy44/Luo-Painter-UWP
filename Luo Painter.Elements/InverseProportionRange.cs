namespace Luo_Painter.Elements
{
    public class InverseProportionRange
    {
        public readonly Range XRange;
        public readonly Range YRange;

        public readonly InverseProportion XIP = new InverseProportion
        {
            Minimum = 1,
            Maximum = 3,
        };
        public readonly InverseProportion YIP;

        public InverseProportionRange(double def, double minimum, double maximum, double scale)
        {
            if (minimum >= maximum)
                throw new System.IndexOutOfRangeException("The minimum must be less than the maximum.");

            this.YIP = this.XIP.Convert();
            this.YRange = new Range
            {
                Default = def,
                IP = new InverseProportion
                {
                    Minimum = minimum,
                    Maximum = maximum,
                }
            };

            // ConvertYToX
            double yOne = this.YRange.IP.ConvertValueToOne(def, RangeRounding.Minimum);
            double yIP = this.YIP.ConvertOneToValue(yOne);

            double xIP = InverseProportion.Convert(yIP);
            double xOne = this.XIP.ConvertValueToOne(xIP, RangeRounding.Maximum);

            this.XRange = new Range
            {
                Default = xOne * scale,
                IP = new InverseProportion
                {
                    Minimum = 0,
                    Maximum = scale
                }
            };
        }

        public double ConvertXToY(double x)
        {
            double xOne = this.XRange.IP.ConvertValueToOne(x, RangeRounding.Maximum);
            double xIP = this.XIP.ConvertOneToValue(xOne);

            double yIP = InverseProportion.Convert(xIP);
            double yOne = this.YIP.ConvertValueToOne(yIP, RangeRounding.Minimum);

            return this.YRange.IP.ConvertOneToValue(yOne);
        }
        public double ConvertYToX(double y)
        {
            double yOne = this.YRange.IP.ConvertValueToOne(y, RangeRounding.Minimum);
            double yIP = this.YIP.ConvertOneToValue(yOne);

            double xIP = InverseProportion.Convert(yIP);
            double xOne = this.XIP.ConvertValueToOne(xIP, RangeRounding.Maximum);

            return this.XRange.IP.ConvertOneToValue(xOne);
        }

    }
}