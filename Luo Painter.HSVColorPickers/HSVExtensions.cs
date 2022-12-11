using System;
using System.Globalization;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    /// <summary>
    /// Provides extended methods for color to convert HSV to color.
    /// <para/>
    /// <see cref="Vector4.W"/> = <see cref="ColorPickerHsvChannel.Alpha"/>. Default value 0, range 0 to 1. <para/>
    /// <see cref="Vector4.Z"/> = <see cref="ColorPickerHsvChannel.Hue"/>. Default value 0, range 0 to 360. <para/>
    /// <see cref="Vector4.X"/> = <see cref="ColorPickerHsvChannel.Saturation"/>. Default value 0, range 0 to 1. <para/>
    /// <see cref="Vector4.Y"/> = <see cref="ColorPickerHsvChannel.Value"/>. Default value 0, range 0 to 1. <para/>
    /// </summary>
    public static class HSVExtensions
    {
        /// <summary>
        /// Color to Hex.
        /// </summary>
        /// <param name="color"> Color. </param>
        /// <returns> Hex. </returns>
        public static string ToHex(this Color color)
        {
            string r = color.R.ToString("x2");
            string g = color.G.ToString("x2");
            string b = color.B.ToString("x2");

            return $"{r}{g}{b}".ToUpper();
        }
        /// <summary>
        /// Hex to Color.
        /// </summary>
        /// <param name="hex"> Hex. </param>
        /// <returns> Color. </returns>
        public static Color? ToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return null;

            int length = hex.Length;
            if (length < 6) return null;
            else if (length > 6) hex = hex.Substring(length - 6, 6);

            int hexNumber;
            try
            {
                hexNumber = int.Parse(hex, NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return null;
            }

            int r = (hexNumber >> 16) & 255;
            int g = (hexNumber >> 8) & 255;
            int b = (hexNumber >> 0) & 255;
            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// HSV to Color.
        /// </summary>
        /// <param name="h"> Hue. </param>
        /// <returns> Color. </returns>
        public static Color ToColor(float h)
        {
            float hh = h / 60f;
            byte xhh = (byte)((1 - MathF.Abs(hh % 2 - 1)) * 255);

            if (hh < 1f) return Color.FromArgb(255, 255, xhh, 0);
            else if (hh < 2f) return Color.FromArgb(255, xhh, 255, 0);
            else if (hh < 3f) return Color.FromArgb(255, 0, 255, xhh);
            else if (hh < 4f) return Color.FromArgb(255, 0, xhh, 255);
            else if (hh < 5f) return Color.FromArgb(255, xhh, 0, 255);
            else return Color.FromArgb(255, 255, 0, xhh);
        }

        /// <summary>
        /// HSV to Color.
        /// </summary>
        /// <param name="hsv"> HSV </param>
        /// <returns> Color. </returns>
        public static Color ToColor(this Vector4 hsv)
        {
            if (hsv.X == 0f)
            {
                byte ll = (byte)(hsv.Y * 255f);
                return Color.FromArgb((byte)(hsv.W * 255), ll, ll, ll);
            }

            float h = hsv.Z == 360f ? 0f : hsv.Z;
            float s = hsv.X;
            float v = hsv.Y;

            int h1 = (int)(h / 60f);
            float F = h / 60 - h1;

            float P = v * (1f - s);
            float Q = v * (1f - F * s);
            float T = v * (1f - (1f - F) * s);

            float r = 0;
            float g = 0;
            float b = 0;

            switch (h1)
            {
                case 0: r = v; g = T; b = P; break;
                case 1: r = Q; g = v; b = P; break;
                case 2: r = P; g = v; b = T; break;
                case 3: r = P; g = Q; b = v; break;
                case 4: r = T; g = P; b = v; break;
                case 5: r = v; g = P; b = Q; break;
            }

            r *= 255;
            while (r > 255) r -= 255;
            while (r < 0) r += 255;

            g *= 255;
            while (g > 255) g -= 255;
            while (g < 0) g += 255;

            b *= 255;
            while (b > 255) b -= 255;
            while (b < 0) b += 255;

            return Color.FromArgb((byte)(hsv.W * 255), (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Color to HSV.
        /// </summary>
        /// <param name="color"> Color. </param>
        /// <returns> HSV. </returns>
        public static Vector4 ToHSV(this Color color)
        {
            float r = color.R * 1f / 255;
            float g = color.G * 1f / 255;
            float b = color.B * 1f / 255;

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);

            float h = 0;
            if (max == min) { h = 0; }
            else if (max == r && g > b) h = 60 * (g - b) * 1f / (max - min) + 0;
            else if (max == r && g < b) h = 60 * (g - b) * 1f / (max - min) + 360;
            else if (max == g) h = 60 * (b - r) * 1f / (max - min) + 120;
            else if (max == b) h = 60 * (r - g) * 1f / (max - min) + 240;

            float s = max == 0f ? 0f : (max - min) * 1f / max;

            float v = max;

            return new Vector4
            {
                W = color.A / 255f,
                Z = h,
                X = s,
                Y = v
            };
        }
    }
}