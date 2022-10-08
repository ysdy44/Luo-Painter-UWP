namespace Luo_Painter.Brushes
{
    public partial struct StrokeSegment
    {
        public readonly float Spacing;

        public readonly float Size;
        public readonly float StartingSize;

        public readonly float Distance;
        public readonly float StartingDistance;

        public bool InRadius() => this.Distance <= this.StartingDistance;

        public StrokeSegment(Stroke stroke, float size = 22f, float spacing = 0.25f)
        {
            this.Spacing = spacing;
            
            this.Size = size;
            this.StartingSize = System.Math.Max(1, this.Size * stroke.StartingPressure);

            this.Distance = stroke.Distance();
            this.StartingDistance = this.Spacing * this.StartingSize;
        }
    }
}