using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class CanvasTransformer3Page : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        //@Key
        private bool IsCenter => false;
        private bool IsRatio => false;

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


        CanvasBitmap Bitmap;

        Vector2 StartingPosition;
        Vector2 Position;

        Vector2 StartingPoint;
        Vector2 Point;

        Transform Transform;

        public CanvasTransformer3Page()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                args.TrackAsyncAction(this.CreateResourcesAsync(sender).AsAsyncAction());
                this.Transformer.Fit();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawCard(new ColorSourceEffect
                {
                    Color = Colors.White
                }, this.Transformer, Colors.Black);

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.Bitmap,
                    TransformMatrix = this.Transform.Matrix * this.Transformer.GetMatrix()
                });

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());

                args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);
            };
        }

        private async Task CreateResourcesAsync(ICanvasResourceCreator sender)
        {
            this.Bitmap = await CanvasBitmap.LoadAsync(sender, "Assets\\Square150x150Logo.scale-200.png");
            this.Transform.Transformer = new Transformer(this.Bitmap.SizeInPixels.Width, this.Bitmap.SizeInPixels.Height, Vector2.Zero);
            this.Transform.Border = new TransformerBorder(this.Bitmap.SizeInPixels.Width, this.Bitmap.SizeInPixels.Height);
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;


                this.Transform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.Transform.Transformer, this.CanvasControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));

                this.Transform.IsMove = this.Transform.Mode is TransformerMode.None && this.Transform.Transformer.FillContainsPoint(this.StartingPosition);
                this.Transform.StartingTransformer = this.Transform.Transformer;

                this.CanvasControl.Invalidate(); // Invalidate


                this.CursorMode = this.GetCursorMode();
                switch (this.CursorMode)
                {
                    case CursorMode.OneWay:
                    case CursorMode.OneTime:
                        this.SetCursorType();
                        break;
                }
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;


                if (this.Transform.IsMove)
                {
                    this.Transform.Transformer = this.Transform.StartingTransformer + (this.Position - this.StartingPosition);
                    this.Transform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.Transform.Border, this.Transform.Transformer);

                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else if (this.Transform.Mode != default)
                {
                    this.Transform.Transformer = FanKit.Transformers.Transformer.Controller(this.Transform.Mode, this.StartingPosition, this.Position, this.Transform.StartingTransformer, this.IsRatio, this.IsCenter);
                    this.Transform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.Transform.Border, this.Transform.Transformer);

                    this.CanvasControl.Invalidate(); // Invalidate
                }


                switch (this.CursorMode)
                {
                    case CursorMode.OneWay:
                        this.SetCursorType();
                        break;
                }
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate


                switch (this.CursorMode)
                {
                    case CursorMode.OneWay:
                    case CursorMode.OneTime:
                        this.CursorType = default;
                        break;
                }
            };


            // Right
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private CursorMode GetCursorMode()
        {
            if (this.Transform.IsMove)
                return CursorMode.OneTime;
            else
                return this.GetTransformCursorMode(this.Transform.Mode);
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
            if (this.Transform.IsMove)
                this.CursorType = CoreCursorType.SizeAll;
            else
                this.SetTransformCursorType(this.Transform.Mode, this.Transform.Transformer);
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