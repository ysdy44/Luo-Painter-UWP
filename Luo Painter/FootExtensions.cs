using Luo_Painter.Edits;
using Luo_Painter.Options;
using Luo_Painter.Tools;

namespace Luo_Painter
{
    public static class FootExtensions
    {

        public static FootType GetType(EditType ediType, OptionType optionType, ToolType toolType)
        {
            switch (ediType)
            {
                case EditType.Feather: return FootType.Feather;
                case EditType.Transform: return FootType.MarqueeTransform;
                case EditType.Grow: return FootType.Grow;
                case EditType.Shrink: return FootType.Shrink;
                default:
                    switch (optionType)
                    {
                        case OptionType.Transform: return FootType.Transform;
                        case OptionType.GradientMapping: return FootType.GradientMapping;
                        case OptionType.RippleEffect: return FootType.RippleEffect;

                        case OptionType.Exposure: return FootType.Exposure;
                        case OptionType.Brightness: return FootType.Brightness;
                        case OptionType.Saturation: return FootType.Saturation;
                        case OptionType.HueRotation: return FootType.HueRotation;
                        case OptionType.Contrast: return FootType.Contrast;
                        case OptionType.Temperature: return FootType.Temperature;
                        case OptionType.HighlightsAndShadows: return FootType.HighlightsAndShadows;

                        case OptionType.LuminanceToAlpha: return FootType.LuminanceToAlpha;

                        default:
                            switch (toolType)
                            {
                                case ToolType.MarqueeRectangular:
                                case ToolType.MarqueeElliptical:
                                case ToolType.MarqueePolygon:
                                case ToolType.MarqueeFreeHand:
                                    return FootType.Marquee;

                                case ToolType.SelectionFlood:
                                case ToolType.SelectionBrush:
                                    return FootType.Selection;

                                case ToolType.PaintBrush:
                                case ToolType.PaintWatercolorPen:
                                case ToolType.PaintPencil:
                                case ToolType.PaintEraseBrush:
                                case ToolType.PaintLiquefaction:
                                    return FootType.Paint;

                                default:
                                    return FootType.None;
                            }
                    }
            }
        }

        public static bool HasDifference(this FootType type)
        {
            switch (type)
            {
                case FootType.MarqueeTransform:
                    return true;

                case FootType.Transform:
                case FootType.RippleEffect:
                    return true;

                default:
                    return false;
            }
        }

        public static bool HasHead(this FootType type)
        {
            switch (type)
            {
                case FootType.Feather:
                case FootType.MarqueeTransform:
                case FootType.Grow:
                case FootType.Shrink:
                    return true;

                case FootType.Exposure:
                case FootType.Brightness:
                case FootType.Saturation:
                case FootType.HueRotation:
                case FootType.Contrast:
                case FootType.Temperature:
                case FootType.HighlightsAndShadows:
                case FootType.LuminanceToAlpha:
                case FootType.Transform:
                case FootType.GradientMapping:
                case FootType.RippleEffect:
                    return true;

                default:
                    return false;
            }
        }

    }
}