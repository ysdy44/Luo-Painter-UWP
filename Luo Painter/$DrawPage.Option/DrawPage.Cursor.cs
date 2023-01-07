using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using System.Numerics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        //@Cursor
        private readonly CoreCursorDictionary Cursors = new CoreCursorDictionary();
        private CursorMode CursorMode;
        private CoreCursorType cursorType;
        public CoreCursorType CursorType
        {
            get => this.cursorType;
            set
            {
                if (this.cursorType == value) return;
                this.cursorType = value;

                Window.Current.CoreWindow.PointerCursor = this.Cursors[value];
            }
        }

        private void Single_Start()
        {
            this.Tool_Start();

            this.CursorMode = this.GetCursorMode();
            switch (this.CursorMode)
            {
                case CursorMode.OneWay:
                case CursorMode.OneTime:
                    this.SetCursorType();
                    break;
            }
        }
        private void Single_Delta()
        {
            this.Tool_Delta();

            switch (this.CursorMode)
            {
                case CursorMode.OneWay:
                    this.SetCursorType();
                    break;
            }
        }
        private void Single_Complete()
        {
            this.Tool_Complete();

            switch (this.CursorMode)
            {
                case CursorMode.OneWay:
                case CursorMode.OneTime:
                    this.CursorType = default;
                    break;
            }
        }

        private CursorMode GetCursorMode()
        {
            switch (this.OptionType)
            {
                case OptionType.MarqueeTransform:
                    if (this.Transform.IsMove)
                        return CursorMode.OneTime;
                    else
                        return this.GetTransformCursorMode(this.Transform.Mode);

                case OptionType.CropCanvas:
                    if (this.CropTransform.IsMove)
                        return CursorMode.OneTime;
                    else
                        return this.GetTransformCursorMode(this.CropTransform.Mode);

                case OptionType.Move:
                    return CursorMode.OneTime;
                case OptionType.Transform:
                    if (this.Transform.IsMove)
                        return CursorMode.OneTime;
                    else
                        return this.GetTransformCursorMode(this.Transform.Mode);
                case OptionType.FreeTransform:
                    switch (this.Transform.Mode)
                    {
                        case TransformerMode.ScaleLeftTop:
                        case TransformerMode.ScaleRightTop:
                        case TransformerMode.ScaleRightBottom:
                        case TransformerMode.ScaleLeftBottom:
                            return CursorMode.OneTime;
                        default:
                            return default;
                    }

                case OptionType.DisplacementLiquefaction:
                    return CursorMode.OneTime;
                case OptionType.RippleEffect:
                    return CursorMode.OneTime;

                case OptionType.Border:
                    if (this.BorderTransform.IsMove)
                        return CursorMode.OneTime;
                    else
                        return this.GetTransformCursorMode(this.BorderTransform.Mode);

                case OptionType.Lighting:
                    return CursorMode.OneTime;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    return CursorMode.OneTime;

                case OptionType.SelectionBrush:
                    return CursorMode.OneTime;

                case OptionType.PaintBrush:
                case OptionType.PaintLine:
                case OptionType.PaintBrushForce:
                case OptionType.PaintBrushMulti:
                    return CursorMode.OneTime;

                //case OptionType.Cursor:
                //    break;
                case OptionType.View:
                    return CursorMode.OneTime;
                case OptionType.Straw:
                    return CursorMode.OneTime;
                case OptionType.Brush:
                    return CursorMode.OneTime;
                case OptionType.Transparency:
                    return CursorMode.OneTime;

                case OptionType.Pen:
                    return CursorMode.OneTime;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        return CursorMode.OneTime;
                    }
                    return default;
            }
        }
        private CursorMode GetTransformCursorMode(TransformerMode mode)
        {
            switch (mode)
            {
                case TransformerMode.Rotation:
                    return CursorMode.OneWay;

                case TransformerMode.SkewLeft:
                case TransformerMode.SkewTop:
                case TransformerMode.SkewRight:
                case TransformerMode.SkewBottom:
                    return CursorMode.OneTime;

                case TransformerMode.ScaleLeft:
                case TransformerMode.ScaleTop:
                case TransformerMode.ScaleRight:
                case TransformerMode.ScaleBottom:
                    return CursorMode.OneTime;

                case TransformerMode.ScaleLeftTop:
                case TransformerMode.ScaleRightTop:
                case TransformerMode.ScaleRightBottom:
                case TransformerMode.ScaleLeftBottom:
                    return CursorMode.OneWay;

                default:
                    return default;
            }
        }

        private void SetCursorType()
        {
            switch (this.OptionType)
            {
                case OptionType.MarqueeTransform:
                    if (this.Transform.IsMove)
                        this.CursorType = CoreCursorType.SizeAll;
                    else
                        this.SetTransformCursorType(this.Transform.Mode, this.Transform.Transformer);
                    break;

                case OptionType.CropCanvas:
                    if (this.CropTransform.IsMove)
                        this.CursorType = CoreCursorType.SizeAll;
                    else
                        this.SetTransformCursorType(this.CropTransform.Mode, this.CropTransform.Transformer);
                    break;

                case OptionType.Move:
                    this.CursorType = CoreCursorType.SizeAll;
                    break;
                case OptionType.Transform:
                    if (this.Transform.IsMove)
                        this.CursorType = CoreCursorType.SizeAll;
                    else
                        this.SetTransformCursorType(this.Transform.Mode, this.Transform.Transformer);
                    break;
                case OptionType.FreeTransform:
                    switch (this.Transform.Mode)
                    {
                        case TransformerMode.ScaleLeftTop:
                        case TransformerMode.ScaleRightTop:
                        case TransformerMode.ScaleRightBottom:
                        case TransformerMode.ScaleLeftBottom:
                            this.CursorType = CoreCursorType.Cross;
                            break;
                        default:
                            break;
                    }
                    break;

                case OptionType.DisplacementLiquefaction:
                    this.CursorType = CoreCursorType.Custom;
                    break;
                case OptionType.RippleEffect:
                    if (this.IsRipplerPoint)
                        this.CursorType = CoreCursorType.Cross;
                    else
                        this.CursorType = CoreCursorType.SizeAll;
                    break;

                case OptionType.Border:
                    if (this.BorderTransform.IsMove)
                        this.CursorType = CoreCursorType.SizeAll;
                    else
                        this.SetTransformCursorType(this.BorderTransform.Mode, this.BorderTransform.Transformer);
                    break;

                case OptionType.Lighting:
                    if (this.IsLightPoint)
                        this.CursorType = CoreCursorType.Cross;
                    else
                        this.CursorType = CoreCursorType.SizeAll;
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.CursorType = CoreCursorType.Cross;
                    break;

                case OptionType.SelectionBrush:
                    this.CursorType = CoreCursorType.Custom;
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintLine:
                case OptionType.PaintBrushForce:
                case OptionType.PaintBrushMulti:
                    this.CursorType = CoreCursorType.Custom;
                    break;

                //case OptionType.Cursor:
                //    break;
                case OptionType.View:
                    this.CursorType = CoreCursorType.Hand;
                    break;
                case OptionType.Straw:
                    this.CursorType = CoreCursorType.Cross;
                    break;
                case OptionType.Brush:
                    this.CursorType = CoreCursorType.Cross;
                    break;
                case OptionType.Transparency:
                    this.CursorType = CoreCursorType.Cross;
                    break;

                case OptionType.Pen:
                    this.CursorType = CoreCursorType.Cross;
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.CursorType = CoreCursorType.Cross;
                    }
                    break;
            }
        }
        private void SetTransformCursorType(TransformerMode mode, Transformer transformer)
        {
            switch (mode)
            {
                case TransformerMode.Rotation:
                    Vector2 center = this.ToPoint(transformer.Center);
                    this.CursorType = (this.Point - center).ToCursorType(Orientation.Horizontal);
                    break;

                case TransformerMode.SkewLeft:
                case TransformerMode.SkewRight:
                    this.CursorType = this.Transform.Transformer.Horizontal.ToCursorType(Orientation.Horizontal);
                    break;
                case TransformerMode.SkewTop:
                case TransformerMode.SkewBottom:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(Orientation.Horizontal);
                    break;

                case TransformerMode.ScaleLeft:
                case TransformerMode.ScaleRight:
                    this.CursorType = this.Transform.Transformer.Horizontal.ToCursorType(Orientation.Vertical);
                    break;
                case TransformerMode.ScaleTop:
                case TransformerMode.ScaleBottom:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(Orientation.Vertical);
                    break;

                case TransformerMode.ScaleLeftTop:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(6);
                    break;
                case TransformerMode.ScaleRightTop:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(4);
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(2);
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.CursorType = this.Transform.Transformer.Vertical.ToCursorType(0);
                    break;

                default:
                    break;
            }
        }

    }
}