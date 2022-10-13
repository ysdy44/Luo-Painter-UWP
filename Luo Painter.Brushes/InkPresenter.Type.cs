namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

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
                                    if (this.AllowPattern) return InkType.MaskBrush_Pattern_Blend;
                                    else return InkType.MaskBrush_Blend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_Pattern_Opacity_Blend;
                                    else return InkType.MaskBrush_Opacity_Blend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.MaskBrush_Pattern_Blur;
                                else return InkType.MaskBrush_Blur;
                            case InkType.Mosaic:
                                if (this.AllowPattern) return InkType.MaskBrush_Pattern_Mosaic;
                                else return InkType.MaskBrush_Mosaic;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.MaskBrush_Pattern_Mix;
                                else return InkType.MaskBrush_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_Pattern;
                                    else return InkType.MaskBrush;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.MaskBrush_Pattern_Opacity;
                                    else return InkType.MaskBrush_Opacity;
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
                                    if (this.AllowPattern) return InkType.Brush_Pattern_Blend;
                                    else return InkType.Brush_Blend;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.Brush_Pattern_Opacity_Blend;
                                    else return InkType.Brush_Opacity_Blend;
                                }
                            case InkType.Blur:
                                if (this.AllowPattern) return InkType.Brush_Pattern_Blur;
                                else return InkType.Brush_Blur;
                            case InkType.Mosaic:
                                if (this.AllowPattern) return InkType.Brush_Pattern_Mosaic;
                                else return InkType.Brush_Mosaic;
                            case InkType.Mix:
                                if (this.AllowPattern) return InkType.Brush_Pattern_Mix;
                                else return InkType.Brush_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowPattern) return InkType.Brush_Pattern;
                                    else return InkType.Brush;
                                }
                                else
                                {
                                    if (this.AllowPattern) return InkType.Brush_Pattern_Opacity;
                                    else return InkType.Brush_Opacity;
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
                                if (this.AllowPattern) return InkType.Circle_Pattern_Blend;
                                else return InkType.Circle_Blend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Circle_Pattern_Opacity_Blend;
                                else return InkType.Circle_Opacity_Blend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.Circle_Pattern_Blur;
                            else return InkType.Circle_Blur;
                        case InkType.Mosaic:
                            if (this.AllowPattern) return InkType.Circle_Pattern_Mosaic;
                            else return InkType.Circle_Mosaic;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.Circle_Pattern_Mix;
                            else return InkType.Circle_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Circle_Pattern;
                                else return InkType.Circle;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Circle_Pattern_Opacity;
                                else return InkType.Circle_Opacity;
                            }
                    }
                case InkType.Line:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Line_Pattern_Blend;
                                else return InkType.Line_Blend;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Line_Pattern_Opacity_Blend;
                                else return InkType.Line_Opacity_Blend;
                            }
                        case InkType.Blur:
                            if (this.AllowPattern) return InkType.Line_Pattern_Blur;
                            else return InkType.Line_Blur;
                        case InkType.Mosaic:
                            if (this.AllowPattern) return InkType.Line_Pattern_Mosaic;
                            else return InkType.Line_Mosaic;
                        case InkType.Mix:
                            if (this.AllowPattern) return InkType.Line_Pattern_Mix;
                            else return InkType.Line_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowPattern) return InkType.Line_Pattern;
                                else return InkType.Line;
                            }
                            else
                            {
                                if (this.AllowPattern) return InkType.Line_Pattern_Opacity;
                                else return InkType.Line_Opacity;
                            }
                    }
                case InkType.Erase:
                    if (this.Opacity == 0f) return InkType.None;
                    else if (this.Opacity == 1f) return InkType.Erase;
                    else return InkType.Erase_Opacity;
                case InkType.Liquefy:
                    return InkType.Liquefy;
                default:
                    return InkType.None;
            }
        }

    }
}