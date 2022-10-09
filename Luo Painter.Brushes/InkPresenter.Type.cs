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
        /// <see cref=" InkType.Mosaic"/>
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
                                    if (this.AllowPattern) return InkType.MaskBrush_WetComposite_Pattern_Blend;
                                    else return InkType.MaskBrush_WetComposite_Blend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend;
                                    else return InkType.MaskBrush_WetComposite_Opacity_Blend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.MaskBrush_WetBlur_Pattern_Blur;
                                else return InkType.MaskBrush_WetBlur_Blur;
                            case InkType.Mosaic:
                                if (this.AllowPattern) return InkType.MaskBrush_WetMosaic_Pattern_Mosaic;
                                else return InkType.MaskBrush_WetMosaic_Mosaic;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.MaskBrush_Wet_Pattern_Mix;
                                else return InkType.MaskBrush_Dry_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_Wet_Pattern;
                                    else return InkType.MaskBrush_Dry;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_Wet_Pattern_Opacity;
                                    else return InkType.MaskBrush_Wet_Opacity;
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
                                    if (this.AllowPattern) return InkType.Brush_WetComposite_Pattern_Blend;
                                    else return InkType.Brush_WetComposite_Blend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.Brush_WetComposite_Pattern_Opacity_Blend;
                                    else return InkType.Brush_WetComposite_Opacity_Blend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.Brush_WetBlur_Pattern_Blur;
                                else return InkType.Brush_WetBlur_Blur;
                            case InkType.Mosaic:
                                if (this.AllowPattern) return InkType.Brush_WetMosaic_Pattern_Mosaic;
                                else return InkType.Brush_WetMosaic_Mosaic;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.Brush_Wet_Pattern_Mix;
                                else return InkType.Brush_Dry_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.Brush_Wet_Pattern;
                                    else return InkType.Brush_Dry;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.Brush_Wet_Pattern_Opacity;
                                    else return InkType.Brush_Wet_Opacity;
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
                                if (this.AllowPattern) return InkType.Circle_WetComposite_Pattern_Blend;
                                else return InkType.Circle_WetComposite_Blend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Circle_WetComposite_Pattern_Opacity_Blend;
                                else return InkType.Circle_WetComposite_Opacity_Blend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.Circle_WetBlur_Pattern_Blur;
                            else return InkType.Circle_WetBlur_Blur;
                        case InkType.Mosaic:
                            if (this.AllowPattern) return InkType.Circle_WetMosaic_Pattern_Mosaic;
                            else return InkType.Circle_WetMosaic_Mosaic;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.Circle_Wet_Pattern_Mix;
                            else return InkType.Circle_Dry_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Circle_Wet_Pattern;
                                else return InkType.Circle_Dry;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Circle_Wet_Pattern_Opacity;
                                else return InkType.Circle_Wet_Opacity;
                            }
                    }
                case InkType.Line:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Line_WetComposite_Pattern_Blend;
                                else return InkType.Line_WetComposite_Blend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Line_WetComposite_Pattern_Opacity_Blend;
                                else return InkType.Line_WetComposite_Opacity_Blend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.Line_WetBlur_Pattern_Blur;
                            else return InkType.Line_WetBlur_Blur;
                        case InkType.Mosaic:
                            if (this.AllowPattern) return InkType.Line_WetMosaic_Pattern_Mosaic;
                            else return InkType.Line_WetMosaic_Mosaic;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.Line_Wet_Pattern_Mix;
                            else return InkType.Line_Dry_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Line_Wet_Pattern;
                                else return InkType.Line_Dry;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Line_Wet_Pattern_Opacity;
                                else return InkType.Line_Wet_Opacity;
                            }
                    }
                case InkType.Erase:
                    if (this.Opacity == 0f) return InkType.None;
                    else if (this.Opacity == 1f) return InkType.Erase_Dry;
                    else return InkType.Erase_WetComposite_Opacity;
                case InkType.Liquefy:
                    return InkType.Liquefy;
                default:
                    return InkType.None;
            }
        }

    }
}