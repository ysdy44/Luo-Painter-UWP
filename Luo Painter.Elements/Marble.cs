using Windows.Foundation;

namespace Luo_Painter.Elements
{
    public enum MarblePlacement
    {
        Default,
        Minimum,
        Maximum,
    }

    public sealed class Marble
    {

        MarblePlacement XPlacement;
        MarblePlacement YPlacement;

        MarblePlacement StartingXPlacement;
        MarblePlacement StartingYPlacement;

        public Point Move(double x, double y, Point delta, double width, double height, bool inertiaStarting)
        {
            if (inertiaStarting == false)
                return new Point
                {
                    X = System.Math.Max(0, System.Math.Min(width, x + delta.X)),
                    Y = System.Math.Max(0, System.Math.Min(height, y + delta.Y))
                };

            x += this.GetTranslation(delta.X, this.StartingXPlacement, this.XPlacement);
            y += this.GetTranslation(delta.Y, this.StartingYPlacement, this.YPlacement);

            MarblePlacement xp = this.GetPlacement(x, width);
            if (this.XPlacement != xp)
            {
                this.StartingXPlacement = this.XPlacement;
                this.XPlacement = xp;
            }

            MarblePlacement yp = this.GetPlacement(y, height);
            if (this.YPlacement != yp)
            {
                this.StartingYPlacement = this.YPlacement;
                this.YPlacement = yp;
            }

            return new Point(x, y);
        }

        private MarblePlacement GetPlacement(double x, double width)
        {
            if (x < 0) return MarblePlacement.Minimum;
            else if (x > width) return MarblePlacement.Maximum;
            else return MarblePlacement.Default;
        }

        private double GetTranslation(double x, MarblePlacement ox, MarblePlacement xx)
        {
            switch (ox)
            {
                case MarblePlacement.Minimum:
                    switch (xx)
                    {
                        case MarblePlacement.Minimum: return System.Math.Abs(x);
                        case MarblePlacement.Default: return System.Math.Abs(x);
                        case MarblePlacement.Maximum: return -System.Math.Abs(x);
                        default: return x;
                    }
                case MarblePlacement.Default:
                    switch (xx)
                    {
                        case MarblePlacement.Minimum: return System.Math.Abs(x);
                        case MarblePlacement.Default: return x;
                        case MarblePlacement.Maximum: return -System.Math.Abs(x);
                        default: return x;
                    }
                case MarblePlacement.Maximum:
                    switch (xx)
                    {
                        case MarblePlacement.Minimum: return System.Math.Abs(x);
                        case MarblePlacement.Default: return -System.Math.Abs(x);
                        case MarblePlacement.Maximum: return -System.Math.Abs(x);
                        default: return x;
                    }
                default: return x;
            }
        }
    }
}