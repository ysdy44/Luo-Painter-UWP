using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.Brushes
{
    partial class InkPresenter
    {

        public void Preview(CanvasDrawingSession ds, InkType type, ICanvasImage image, ICanvasImage wet)
        {
            ds.Blend = CanvasBlend.Copy;
            switch (type)
            {
                case InkType.General:
                case InkType.General_Mix:
                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Mix:
                case InkType.Tip:
                case InkType.Line:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    ds.DrawImage(wet);
                    break;

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Line_Grain:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    {
                        ds.DrawImage(grain);
                    }
                    break;

                case InkType.General_Opacity:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Mix:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    {
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.General_Opacity_Grain:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.Tip_Opacity_Grain:
                case InkType.Line_Opacity_Grain:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    using (OpacityEffect opacity = this.GetOpacity(grain))
                    {
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.General_Opacity_Grain_Blend:
                case InkType.General_Opacity_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                case InkType.Tip_Opacity_Grain_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    using (OpacityEffect opacity = this.GetOpacity(grain))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Blend:
                case InkType.General_Blend_Mix:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Grain_Blend:
                case InkType.General_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
                case InkType.Tip_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Blur:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect blur = this.GetBlur(image, wet))
                    {
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Mosaic:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (ScaleEffect mosaic = this.GetMosaic(image, wet))
                    {
                        ds.DrawImage(mosaic);
                    }
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    using (ArithmeticCompositeEffect erase = this.GetErase(image, wet))
                    {
                        ds.DrawImage(erase);
                    }
                    break;

                case InkType.Liquefy:
                    ds.DrawImage(image);
                    break;

                default:
                    break;
            }
        }

        public ICanvasImage GetPreview(InkType type, ICanvasImage image, ICanvasImage wet)
        {
            switch (type)
            {
                case InkType.General:
                case InkType.General_Mix:
                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Mix:
                case InkType.Tip:
                case InkType.Line:
                    return InkPresenter.GetComposite(image, wet);

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Line_Grain:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        return InkPresenter.GetComposite(image, grain);
                    }

                case InkType.General_Opacity:
                case InkType.ShapeGeneral_Opacity:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.General_Opacity_Grain:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.Tip_Opacity_Grain:
                case InkType.Line_Opacity_Grain:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        OpacityEffect opacity = this.GetOpacity(grain);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.General_Opacity_Grain_Blend:
                case InkType.General_Opacity_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                case InkType.Tip_Opacity_Grain_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        OpacityEffect opacity = this.GetOpacity(grain);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.General_Blend:
                case InkType.General_Blend_Mix:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Grain_Blend:
                case InkType.General_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
                case InkType.Tip_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Blur:
                    {
                        AlphaMaskEffect blur = this.GetBlur(image, wet);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Mosaic:
                    {
                        ScaleEffect mosaic = this.GetMosaic(image, wet);
                        return InkPresenter.GetComposite(image, mosaic);
                    }

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    {
                        ArithmeticCompositeEffect erase = this.GetErase(image, wet);
                        return erase;
                    }

                case InkType.Liquefy:
                    return image;

                default:
                    return image;
            }
        }

    }
}