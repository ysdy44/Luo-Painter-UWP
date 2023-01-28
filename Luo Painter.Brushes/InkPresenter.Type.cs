using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public new InkType GetType()
        {
            switch (base.Type)
            {
                case InkType.General:
                    if (this.AllowShape)
                    {
                        #region ShapeGeneral

                        if (base.Flow == 0f) return InkType.None;
                        else if (base.Opacity == 0f) return InkType.None;
                        else if (base.Opacity == 1f)
                        {
                            if (this.AllowGrain)
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Grain_Blend;
                                    else
                                        return InkType.ShapeGeneral_Grain_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Grain;
                                    else
                                        return InkType.ShapeGeneral_Grain_Mix;
                                }
                            }
                            else
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Blend;
                                    else
                                        return InkType.ShapeGeneral_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral;
                                    else
                                        return InkType.ShapeGeneral_Mix;
                                }
                            }
                        }
                        else
                        {
                            if (this.AllowGrain)
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Opacity_Grain_Blend;
                                    else
                                        return InkType.ShapeGeneral_Opacity_Grain_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Opacity_Grain;
                                    else
                                        return InkType.ShapeGeneral_Opacity_Grain_Mix;
                                }
                            }
                            else
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Opacity_Blend;
                                    else
                                        return InkType.ShapeGeneral_Opacity_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.ShapeGeneral_Opacity;
                                    else
                                        return InkType.ShapeGeneral_Opacity_Mix;
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region General

                        if (base.Flow == 0f) return InkType.None;
                        else if (base.Opacity == 0f) return InkType.None;
                        else if (base.Opacity == 1f)
                        {
                            if (this.AllowGrain)
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Grain_Blend;
                                    else
                                        return InkType.General_Grain_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Grain;
                                    else
                                        return InkType.General_Grain_Mix;
                                }
                            }
                            else
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Blend;
                                    else
                                        return InkType.General_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General;
                                    else
                                        return InkType.General_Mix;
                                }
                            }
                        }
                        else
                        {
                            if (this.AllowGrain)
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Opacity_Grain_Blend;
                                    else
                                        return InkType.General_Opacity_Grain_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Opacity_Grain;
                                    else
                                        return InkType.General_Opacity_Grain_Mix;
                                }
                            }
                            else
                            {
                                if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Opacity_Blend;
                                    else
                                        return InkType.General_Opacity_Blend_Mix;
                                }
                                else
                                {
                                    if (base.Mix == 0f)
                                        return InkType.General_Opacity;
                                    else
                                        return InkType.General_Opacity_Mix;
                                }
                            }
                        }

                        #endregion
                    }
                case InkType.Tip:
                    #region Tip

                    if (base.Flow == 0f) return InkType.None;
                    else if (base.Opacity == 0f) return InkType.None;
                    else if (base.Opacity == 1f)
                    {
                        if (this.AllowGrain)
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Tip_Grain_Blend;
                            else
                                return InkType.Tip_Grain;
                        }
                        else
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Tip_Blend;
                            else
                                return InkType.Tip;
                        }
                    }
                    else
                    {
                        if (this.AllowGrain)
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Tip_Opacity_Grain_Blend;
                            else
                                return InkType.Tip_Opacity_Grain;
                        }
                        else
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Tip_Opacity_Blend;
                            else
                                return InkType.Tip_Opacity;
                        }
                    }

                #endregion
                case InkType.Line:
                    #region Line

                    if (base.Flow == 0f) return InkType.None;
                    else if (base.Opacity == 0f) return InkType.None;
                    else if (base.Opacity == 1f)
                    {
                        if (this.AllowGrain)
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Line_Grain_Blend;
                            else
                                return InkType.Line_Grain;
                        }
                        else
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Line_Blend;
                            else
                                return InkType.Line;
                        }
                    }
                    else
                    {
                        if (this.AllowGrain)
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Line_Opacity_Grain_Blend;
                            else
                                return InkType.Line_Opacity_Grain;
                        }
                        else
                        {
                            if (System.Enum.IsDefined(typeof(BlendEffectMode), base.BlendMode))
                                return InkType.Line_Opacity_Blend;
                            else
                                return InkType.Line_Opacity;
                        }
                    }

                #endregion
                case InkType.Blur:
                    if (base.Flow == 0f) return InkType.None;
                    else if (base.Opacity == 0f) return InkType.None;
                    else return InkType.Blur;
                case InkType.Mosaic:
                    if (base.Opacity == 0f) return InkType.None;
                    else return InkType.Mosaic;
                case InkType.Erase:
                    if (base.Flow == 0f) return InkType.None;
                    else if (base.Opacity == 0f) return InkType.None;
                    else if (base.Opacity == 1f) return InkType.Erase;
                    else return InkType.Erase_Opacity;
                case InkType.Liquefy:
                    return InkType.Liquefy;
                default:
                    return InkType.None;
            }
        }

    }
}