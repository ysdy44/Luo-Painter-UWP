using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Luo_Painter.Shaders
{
    public static class ShaderFunction
    {
        public static double Saturate(double x)
        {
            if (x < 0)
                return x;
            if (x > 1)
                return 1;
            return x;
        }

        public static double Smoothstep(double a, double b, double t)
        {
            var v = Saturate((t - a) / (b - a));
            return v * v * (3.0f - (2.0f * v));
        }

        #region Lerp
        public static double Lerp(double start, double stop, double amt)
        {
            return amt * (stop - start) + start;
        }

        public static Vector2 Learp(Vector2 start, Vector2 stop, double amt)
        {
            return Vector2.Lerp(start, stop, (float)amt);
        }

        public static Vector3 Learp(Vector3 start, Vector3 stop, double amt)
        {
            return Vector3.Lerp(start, stop, (float)amt);
        }
        public static Vector4 Learp(Vector4 start, Vector4 stop, double amt)
        {
            return Vector4.Lerp(start, stop, (float)amt);
        }

        public static Color Lerp(Color start, Color stop, double amt, bool lerpAlpha = false)
        {
            var r = (byte)Lerp(start.R, stop.R, amt);
            var g = (byte)Lerp(start.G, stop.G, amt);
            var b = (byte)Lerp(start.B, stop.B, amt);
            var a = start.A;
            if (lerpAlpha)
            {
                a = (byte)Lerp(start.A, stop.A, amt);
            }

            return Color.FromArgb(a, r, g, b);
        }

        #endregion


        
    }
}
