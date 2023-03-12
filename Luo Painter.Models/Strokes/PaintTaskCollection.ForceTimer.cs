using System.Numerics;

namespace Luo_Painter.Models
{
    public sealed partial class TaskCollection
    {

        //@Const
        const float Gm1 = 100.0f;
        const float ttm = TaskCollection.dt * TaskCollection.dt;
        const float dt = 0.001666f;

        // Force
        float Acceleration = 0;
        float Mass = 10000;
        Vector2 Velocity;
        public float Speed { get; set; } = 80;

        // Stabilizer
        public Vector2 StartingPositionStabilizer { get; private set; }
        public Vector2 PositionStabilizer { get; private set; }

        public float PressureStabilizer { get; private set; } = 1;
        public float StartingPressureStabilizer { get; private set; } = 1;

        // Paint 
        public Vector2 Position { get; set; }
        public float Pressure { get; set; } = 1;
        // Ink 
        float Size = 12;
        float Spacing = 0.25f;

        readonly System.Timers.Timer Timer = new System.Timers.Timer
        {
            Interval = 20
        };

        public TaskCollection()
        {
            this.Timer.Elapsed += (s, e) =>
            {
                float durationTime = 20 / this.Speed;
                int times = 0;

                for (this.Acceleration += durationTime; this.Acceleration >= TaskCollection.dt; this.Acceleration -= TaskCollection.dt, times++)
                {
                    Vector2 vector = this.Position - this.PositionStabilizer;
                    float length = vector.Length();
                    if (length < 2) return;

                    float f = TaskCollection.Gm1 * this.Mass / 10;
                    Vector2 fd = f * vector / length;

                    this.Velocity += fd * TaskCollection.ttm;
                    this.Velocity *= 1 - TaskCollection.dt / length * 100;
                    this.PositionStabilizer += this.Velocity * TaskCollection.dt * 1;

                    StrokeSegment segment = new StrokeSegment(this.StartingPositionStabilizer, this.PositionStabilizer, this.StartingPressureStabilizer, this.Pressure, this.Size, this.Spacing);
                    if (segment.InRadius) continue;

                    base.Add(segment);

                    this.StartingPositionStabilizer = this.PositionStabilizer;
                    this.StartingPressureStabilizer = this.PressureStabilizer;
                    this.PressureStabilizer = this.Pressure;
                }
            };
        }

        public void StopForce() => this.Timer.Stop();
        public void StartForce(Vector2 position, float pressure, float size, float spacing)
        {
            // Force
            this.Velocity = Vector2.Zero;

            // Stabilizer
            this.StartingPositionStabilizer = position;
            this.PositionStabilizer = position;

            this.StartingPressureStabilizer = pressure;
            this.PressureStabilizer = pressure;

            // Paint 
            this.Position = position;
            this.Pressure = pressure;
            // Ink 
            this.Size = size;
            this.Spacing = spacing;

            this.Timer.Start();
        }

        public void Dispose()
        {
            this.Timer.Dispose();
        }
    }
}