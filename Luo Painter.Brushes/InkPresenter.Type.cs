namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        /// <summary>
        /// <see cref=" InkType.None"/>
        /// <see cref=" InkType.Brush"/>
        /// <see cref=" InkType.Circle"/>
        /// <see cref=" InkType.Line"/>
        /// <see cref=" InkType.Erase"/>
        /// <see cref=" InkType.Liquefy"/>
        /// </summary>
        public InkType ToolType { get; set; }

        /// <summary>
        /// <see cref=" InkType.None"/>
        /// <see cref=" InkType.Blend"/>
        /// <see cref=" InkType.Mix"/>
        /// <see cref=" InkType.Blur"/>
        /// </summary>
        public InkType Mode { get; set; }

        public new InkType GetType()
        {
            switch (this.ToolType)
            {
                case InkType.Brush:
                    if (this.AllowMask)
                    {
                        switch (this.Mode)
                        {
                            case InkType.Blend:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.MaskBrushWetPatternBlend;
                                    else return InkType.MaskBrushWetBlend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrushWetPatternOpacityBlend;
                                    else return InkType.MaskBrushWetOpacityBlend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.MaskBrushWetPatternBlur;
                                else return InkType.MaskBrushWetBlur;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.MaskBrushWetPatternMix;
                                else return InkType.MaskBrushDryMix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.MaskBrushWetPattern;
                                    else return InkType.MaskBrushDry;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrushWetPatternOpacity;
                                    else return InkType.MaskBrushWetOpacity;
                                }
                        }
                    }
                    else
                    {
                        switch (this.Mode)
                        {
                            case InkType.Blend:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.BrushWetPatternBlend;
                                    else return InkType.BrushWetBlend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.BrushWetPatternOpacityBlend;
                                    else return InkType.BrushWetOpacityBlend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.BrushWetPatternBlur;
                                else return InkType.BrushWetBlur;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.BrushWetPatternMix;
                                else return InkType.BrushDryMix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.BrushWetPattern;
                                    else return InkType.BrushDry;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.BrushWetPatternOpacity;
                                    else return InkType.BrushWetOpacity;
                                }
                        }
                    }
                case InkType.Circle:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.CircleWetPatternBlend;
                                else return InkType.CircleWetBlend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.CircleWetPatternOpacityBlend;
                                else return InkType.CircleWetOpacityBlend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.CircleWetPatternBlur;
                            else return InkType.CircleWetBlur;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.CircleWetPatternMix;
                            else return InkType.CircleDryMix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.CircleWetPattern;
                                else return InkType.CircleDry;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.CircleWetPatternOpacity;
                                else return InkType.CircleWetOpacity;
                            }
                    }
                case InkType.Line:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.LineWetPatternBlend;
                                else return InkType.LineWetBlend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.LineWetPatternOpacityBlend;
                                else return InkType.LineWetOpacityBlend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.LineWetPatternBlur;
                            else return InkType.LineWetBlur;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.LineWetPatternMix;
                            else return InkType.LineDryMix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.LineWetPattern;
                                else return InkType.LineDry;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.LineWetPatternOpacity;
                                else return InkType.LineWetOpacity;
                            }
                    }
                case InkType.Erase:
                    if (this.Opacity == 0f) return InkType.None;
                    if (this.AllowPattern) return InkType.EraseWetPatternOpacity;
                    if (this.Opacity == 1f) return InkType.EraseDry;
                    return InkType.EraseWetOpacity;
                case InkType.Liquefy:
                    return InkType.Liquefy;
                default:
                    return InkType.None;
            }
        }

    }
}