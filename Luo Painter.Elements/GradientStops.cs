using Windows.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Windows.UI;

namespace Luo_Painter.Elements
{
    public sealed class GradientStops : Dictionary<double, Color>
    {

        readonly Random Ran = new Random();
        readonly Timer Timer = new Timer
        {
            AutoReset = false,
            Interval = 100
        };
        public bool Enabled => this.Timer.Enabled;
        public void Start() => this.Timer.Start();

        public bool Space(IEnumerable<GradientStop> source)
        {
            int count = source.Count();
            switch (count)
            {
                case 0:
                    return false;
                case 1:
                    return false;
                case 2:
                    return false;
                default:
                    int index = 0;
                    int length = count - 1;
                    double space = 1f / length;

                    base.Clear();
                    foreach (GradientStop item in source.OrderBy(c => c.Offset))
                    {
                        if (index == 0) base.Add(0, item.Color);
                        else if (index == length) base.Add(1, item.Color);
                        else base.Add(space * index, item.Color);
                        index++;
                    }
                    return true;
            }
        }

        public bool Reverse(IEnumerable<GradientStop> source)
        {
            int count = source.Count();
            switch (count)
            {
                case 0:
                    return false;
                case 1:
                    return false;
                default:
                    base.Clear();
                    foreach (GradientStop item in source)
                    {
                        base.Add(1 - item.Offset, item.Color);
                    }
                    return true;
            }
        }

        public void Random()
        {
            int count = this.Ran.Next(3, 10);
            float space = 1f / (count - 1);

            base.Clear();
            for (int i = 0; i < count; i++)
            {
                base.Add(i * space, new Color
                {
                    A = 255,
                    R = (byte)this.Ran.Next(0, 256),
                    G = (byte)this.Ran.Next(0, 256),
                    B = (byte)this.Ran.Next(0, 256),
                });
            }
        }

    }
}