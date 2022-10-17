using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class Star
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public readonly float Mass;
        public readonly float Size;
        public readonly Color Color;

        public Star(float mass, Color color)
        {
            this.Mass = mass;
            this.Size = (float)Math.Log(Mass) * 2;
            this.Color = color;
        }

        public void Move(float step)
        {
            this.Position += this.Velocity * step;
        }

        //@Static
        public static IEnumerable<Star> CreateStars(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                float angle = 1.0f * i / count * (float)Math.PI * 2;
                float mass = 10000 * 2 / (count * (float)Math.Sqrt(count) * (float)Math.Log(count));

                yield return new Star(mass, Colors.Gray)
                {
                    Position = 45 * new Vector2((float)Math.Sin(angle), (float)-Math.Cos(angle)),
                    Velocity = 5 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)),
                };
            }
        }
    }

    public sealed partial class SolarSystemPage : Page
    {
        const float StepDurationTime = 0.01666f; // Dt

        float Width2 => 500;
        float Height2 => 500;

        float Scale2 => 5;
        float Acceleration = 0;

        float Speed = 1000;
        readonly Star[] Stars = new Star[]
       {
            // Solar
            new Star(1000, Colors.OrangeRed)
            {
                Velocity = new Vector2(0.6f, 0)
            },
            // Earth
            new Star(100, Colors.DodgerBlue)
            {
                Position = new Vector2(0, -41),
                Velocity = new Vector2(-5, 0)
            },
            // Moon
            new Star(10, Colors.Gray)
            {
                Position = new Vector2(0, -45),
                Velocity = new Vector2(-10, 0)
            }
       };

        public SolarSystemPage()
        {
            this.InitializeComponent();
            this.ConstructMainPage();
            this.ConstructCanvas();
        }

        private void ConstructMainPage()
        {
            this.Slider.ValueChanged += (s, e) => this.Speed = 10 * (float)e.NewValue;
        }

        private void ConstructCanvas()
        {
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                foreach (Star star in this.Stars)
                {
                    Vector2 position = new Vector2(this.Height2 / 2 + (this.Width2 - this.Height2) / 2, this.Height2 / 2);
                    Vector2 positionScaled = star.Position * Scale2 + position;
                    args.DrawingSession.FillCircle(positionScaled, star.Size, star.Color);
                }
            };
            this.CanvasAnimatedControl.Update += (sender, args) =>
            {
                float durationTime = (float)args.Timing.ElapsedTime.TotalMilliseconds / this.Speed;
                int times = 0;
                for (this.Acceleration += durationTime; this.Acceleration >= SolarSystemPage.StepDurationTime; this.Acceleration -= SolarSystemPage.StepDurationTime, times++)
                {
                    this.Step();
                }
            };
        }

        private void Step()
        {
            foreach (Star star in this.Stars)
            {
                // Star Velocity
                // F = G * m1 * m2 / r^2
                // Force has a Direction: 
                Vector2 fd = Vector2.Zero;

                // G*s1.m
                const float Gm1 = 100.0f;
                // t*t/s1.m
                float ttm = SolarSystemPage.StepDurationTime * SolarSystemPage.StepDurationTime;

                foreach (Star star2 in this.Stars)
                {
                    if (star == star2) continue;

                    // Direction
                    Vector2 d = star2.Position - star.Position;
                    float f = Gm1 * star2.Mass / d.LengthSquared();
                    fd += f * d / d.Length();
                }

                // Ft = ma -> a = Ft/m
                // v  = at -> v = Ftt/m
                star.Velocity += fd * ttm;
            }

            foreach (Star star in this.Stars)
            {
                star.Move(SolarSystemPage.StepDurationTime);
            }
        }
    }
}