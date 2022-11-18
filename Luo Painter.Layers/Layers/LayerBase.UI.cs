using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase : IDisposable
    {

        public string UIBlendMode
        {
            get
            {
                if (this.BlendMode.HasValue is false) return "N";

                switch (this.BlendMode.Value)
                {
                    // None
                    case BlendEffectMode.Dissolve: return "D";

                    // Darken
                    case BlendEffectMode.Darken: return "D";
                    case BlendEffectMode.Multiply: return "M";
                    case BlendEffectMode.ColorBurn: return "C";
                    case BlendEffectMode.LinearBurn: return "L";
                    case BlendEffectMode.DarkerColor: return "D";

                    // Lighten
                    case BlendEffectMode.Lighten: return "L";
                    case BlendEffectMode.Screen: return "S";
                    case BlendEffectMode.ColorDodge: return "C";
                    case BlendEffectMode.LinearDodge: return "L";
                    case BlendEffectMode.LighterColor: return "L";

                    // Contrast
                    case BlendEffectMode.Overlay: return "O";
                    case BlendEffectMode.SoftLight: return "S";
                    case BlendEffectMode.HardLight: return "H";
                    case BlendEffectMode.VividLight: return "V";
                    case BlendEffectMode.LinearLight: return "L";
                    case BlendEffectMode.PinLight: return "P";
                    case BlendEffectMode.HardMix: return "H";

                    // Difference
                    case BlendEffectMode.Difference: return "D";
                    case BlendEffectMode.Exclusion: return "E";
                    case BlendEffectMode.Subtract: return "S";
                    case BlendEffectMode.Division: return "D";

                    // Color
                    case BlendEffectMode.Hue: return "H";
                    case BlendEffectMode.Saturation: return "S";
                    case BlendEffectMode.Color: return "C";
                    case BlendEffectMode.Luminosity: return "L";

                    default: return "N";
                }
            }
        }

        public double UIDepth => this.Depth * 20;

        public double UIVisibility => this.Visibility is Visibility.Visible ? 1d : 0.5d;

        public double UIIsExpand => this.IsExpand ? 90 : 0;

    }
}