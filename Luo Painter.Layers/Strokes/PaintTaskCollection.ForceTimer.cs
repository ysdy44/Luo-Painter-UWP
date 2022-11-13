using System;
using System.Collections.Generic;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public sealed partial class PaintTaskCollection : List<StrokeSegment>, IDisposable
    {

        //@Const
        const float Gm1 = 100.0f;
        const float ttm = PaintTaskCollection.dt * PaintTaskCollection.dt;
        const float dt = 0.001666f;

        float Acceleration = 0;
        float Mass = 10000;
        Vector2 Velocity;
        public float Speed { get; set; } = 80;

        public Vector2 StartingStabilizer { get; private set; }
        public Vector2 Stabilizer { get; private set; }

        public Vector2 Position { get; set; }
        public float Pressure { get; set; } = 1;

        readonly System.Timers.Timer Timer = new System.Timers.Timer
        {
            Interval = 20
        };

        public PaintTaskCollection()
        {
            this.Timer.Elapsed += (s, e) =>
            {
                float durationTime = 20 / this.Speed;
                int times = 0;

                for (this.Acceleration += durationTime; this.Acceleration >= PaintTaskCollection.dt; this.Acceleration -= PaintTaskCollection.dt, times++)
                {
                    float strokeWidth = 12 * 2 * this.Pressure;
                    if (strokeWidth < 1) return;

                    Vector2 vector = this.Position - this.Stabilizer;
                    float length = vector.Length();
                    if (length < 2) return;

                    float f = PaintTaskCollection.Gm1 * this.Mass / 10;
                    Vector2 fd = f * vector / length;

                    this.Velocity += fd * PaintTaskCollection.ttm;
                    this.Velocity *= 1 - PaintTaskCollection.dt / length * 100;
                    this.Stabilizer += this.Velocity * PaintTaskCollection.dt * 1;

                    StrokeSegment segment = new StrokeSegment(this.StartingStabilizer, this.Stabilizer);
                    if (segment.InRadius) continue;

                    base.Add(segment);

                    this.StartingStabilizer = this.Stabilizer;
                }
            };
        }

        public void StopForce() => this.Timer.Stop();
        public void StartForce(Vector2 position)
        {
            this.Pressure = 1;
            this.Position = position;

            this.StartingStabilizer = position;
            this.Stabilizer = position;

            this.Velocity = Vector2.Zero;

            this.Timer.Start();
        }

        public void Dispose()
        {
            this.Timer.Dispose();
        }
    }
}