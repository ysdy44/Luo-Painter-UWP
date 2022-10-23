namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes
    {

        public new InkType GetType()
        {
            switch (base.Type)
            {
                case InkType.General:
                    if (this.AllowShape)
                    {
                        switch (base.Mode)
                        {
                            case InkType.Blend:
                                if (this.Flow == 0f) return InkType.None;
                                else if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.ShapeGeneral_Grain_Blend;
                                    else return InkType.ShapeGeneral_Blend;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.ShapeGeneral_Grain_Opacity_Blend;
                                    else return InkType.ShapeGeneral_Opacity_Blend;
                                }
                            case InkType.Mix:
                                if (this.AllowGrain) return InkType.ShapeGeneral_Grain_Mix;
                                else return InkType.ShapeGeneral_Mix;
                            default:
                                if (this.Flow == 0f) return InkType.None;
                                else if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.ShapeGeneral_Grain;
                                    else return InkType.ShapeGeneral;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.ShapeGeneral_Grain_Opacity;
                                    else return InkType.ShapeGeneral_Opacity;
                                }
                        }
                    }
                    else
                    {
                        switch (base.Mode)
                        {
                            case InkType.Blend:
                                if (this.Flow == 0f) return InkType.None;
                                else if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.General_Grain_Blend;
                                    else return InkType.General_Blend;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.General_Grain_Opacity_Blend;
                                    else return InkType.General_Opacity_Blend;
                                }
                            case InkType.Mix:
                                if (this.AllowGrain) return InkType.General_Grain_Mix;
                                else return InkType.General_Mix;
                            default:
                                if (this.Flow == 0f) return InkType.None;
                                else if (this.Opacity == 0f) return InkType.None;
                                else if (this.Opacity == 1f)
                                {
                                    if (this.AllowGrain) return InkType.General_Grain;
                                    else return InkType.General;
                                }
                                else
                                {
                                    if (this.AllowGrain) return InkType.General_Grain_Opacity;
                                    else return InkType.General_Opacity;
                                }
                        }
                    }
                case InkType.Tip:
                    switch (base.Mode)
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
                    switch (base.Mode)
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