namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public new InkType GetType()
        {
            switch (this.Type)
            {
                case InkType.Brush:
                    if (this.AllowShape)
                    {
                        switch (this.Mode)
                        {
                            case InkType.Blend:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.ShapeBrush_Grain_Blend;
                                    else return InkType.ShapeBrush_Blend;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.ShapeBrush_Grain_Opacity_Blend;
                                    else return InkType.ShapeBrush_Opacity_Blend;
                                }
                            case InkType.Mix:
                                if (this.AllowGrain) return InkType.ShapeBrush_Grain_Mix;
                                else return InkType.ShapeBrush_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.ShapeBrush_Grain;
                                    else return InkType.ShapeBrush;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.ShapeBrush_Grain_Opacity;
                                    else return InkType.ShapeBrush_Opacity;
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
                                    if (this.AllowGrain) return InkType.Brush_Grain_Blend;
                                    else return InkType.Brush_Blend;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.Brush_Grain_Opacity_Blend;
                                    else return InkType.Brush_Opacity_Blend;
                                }
                            case InkType.Mix:
                                if (this.AllowGrain) return InkType.Brush_Grain_Mix;
                                else return InkType.Brush_Mix;
                            default:
                                if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.Brush_Grain;
                                    else return InkType.Brush;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.Brush_Grain_Opacity;
                                    else return InkType.Brush_Opacity;
                                }
                        }
                    }
                case InkType.Tip:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowGrain) return InkType.Tip_Grain_Blend;
                                else return InkType.Tip_Blend;
                            }
                            else
                            {
                                if (this.AllowGrain) return InkType.Tip_Grain_Opacity_Blend;
                                else return InkType.Tip_Opacity_Blend;
                            }
                        case InkType.Mix:
                            if (this.AllowGrain) return InkType.Tip_Grain_Mix;
                            else return InkType.Tip_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowGrain) return InkType.Tip_Grain;
                                else return InkType.Tip;
                            }
                            else
                            {
                                if (this.AllowGrain) return InkType.Tip_Grain_Opacity;
                                else return InkType.Tip_Opacity;
                            }
                    }
                case InkType.Line:
                    switch (this.Mode)
                    {
                        case InkType.Blend:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowGrain) return InkType.Line_Grain_Blend;
                                else return InkType.Line_Blend;
                            }
                            else
                            {
                                if (this.AllowGrain) return InkType.Line_Grain_Opacity_Blend;
                                else return InkType.Line_Opacity_Blend;
                            }
                        case InkType.Blur:
                        case InkType.Mix:
                            if (this.AllowGrain) return InkType.Line_Grain_Mix;
                            else return InkType.Line_Mix;
                        default:
                            if (this.Opacity == 0f) return InkType.None;
                            else if (this.Opacity == 1f)
                            {
                                if (this.AllowGrain) return InkType.Line_Grain;
                                else return InkType.Line;
                            }
                            else
                            {
                                if (this.AllowGrain) return InkType.Line_Grain_Opacity;
                                else return InkType.Line_Opacity;
                            }
                    }
                case InkType.Blur:
                    return InkType.Blur;
                case InkType.Mosaic:
                    return InkType.Mosaic;
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