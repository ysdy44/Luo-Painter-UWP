namespace Luo_Painter.Blends
{
    internal readonly struct BSplineParameter
    {
        public readonly float tc1b1;
        public readonly float tc2b2;
        public readonly float tc2b1;
        public readonly float tc1b2;
        public BSplineParameter(float tension, float continuity, float bias)
        {
            float t = 1 - tension;

            float c1 = 1 + continuity;
            float c2 = 1 - continuity;
            float b1 = 1 + bias;
            float b2 = 1 - bias;

            this.tc1b1 = t * c1 * b1 / 2;
            this.tc2b2 = t * c2 * b2 / 2;
            this.tc2b1 = t * c2 * b1 / 2;
            this.tc1b2 = t * c1 * b2 / 2;
        }
    }
}