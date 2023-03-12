using System;
using System.Collections.Generic;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public sealed class BSplinePoints : List<Vector2>
    {
        public Vector2 Point { get; set; }

        public float Tension { get; set; }
        public float Continuity { get; set; }
        public float Bias { get; set; }
        public int Steps { get; set; } = 16;

        public void Spline(Vector2 p1, Vector2 p3, Vector2 p4)
        {
            Vector2 p2 = this.Point;
            {
                const float y = 1f / 3f;
                float distance = Vector2.DistanceSquared(p3, p2); // 0~512*512
                float pow = (float)MathF.Pow(distance, y); // 0~64
                float scale = pow / 64; // 0~1
                this.Tension = 1 - scale * 2; // 1~-1
            }
            BSplineParameter p = new BSplineParameter(this.Tension, this.Continuity, this.Bias);

            base.Clear();
            for (int j = 0; j < this.Steps; j++)
            {
                float scale = (float)j / (float)this.Steps;
                float pow2 = (float)Math.Pow(scale, 2);
                float pow3 = (float)Math.Pow(scale, 3);

                float h1 = 2 * pow3 - 3 * pow2 + 1;
                float h2 = -2 * pow3 + 3 * pow2;
                float h3 = pow3 - 2 * pow2 + scale;
                float h4 = pow3 - pow2;

                float dix = p.tc1b1 * (p2.X - p1.X) + p.tc2b2 * (p3.X - p2.X);
                float diy = p.tc1b1 * (p2.Y - p1.Y) + p.tc2b2 * (p3.Y - p2.Y);

                float six = p.tc2b1 * (p3.X - p2.X) + p.tc1b2 * (p4.X - p3.X);
                float siy = p.tc2b1 * (p3.Y - p2.Y) + p.tc1b2 * (p4.Y - p3.Y);

                base.Add(new Vector2
                {
                    X = h1 * p2.X + h2 * p3.X + h3 * dix + h4 * six,
                    Y = h1 * p2.Y + h2 * p3.Y + h3 * diy + h4 * siy
                });
            }
        }

        public override string ToString() => Point.ToString();
    }
}