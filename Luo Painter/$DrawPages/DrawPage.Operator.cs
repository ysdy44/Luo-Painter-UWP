using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.SetCanvasState(true);

                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;

                switch (this.Operator.Device)
                {
                    case InkInputDevice.Pen:
                        this.StartingPressure = this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.StartingPressure = this.Pressure = 1;
                        break;
                }

                for (int i = 0; i < this.ReferenceImages.Count; i++)
                {
                    ReferenceImage item = this.ReferenceImages[i];
                    if (FanKit.Math.InNodeRadius(this.ToPoint(item.Size + item.Position), point))
                    {
                        this.ReferenceImage = item;
                        this.IsReferenceImageResizing = true;
                        this.CanvasControl.Invalidate();
                        return;
                    }
                    if (item.Contains(this.Position))
                    {
                        item.Cache();
                        this.ReferenceImage = item;
                        this.IsReferenceImageResizing = false;
                        this.CanvasControl.Invalidate();
                        return;
                    }
                }

                this.Tool_Start();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;

                switch (this.Operator.Device)
                {
                    case InkInputDevice.Pen:
                        this.Pressure = properties.Pressure * properties.Pressure;
                        break;
                    default:
                        this.Pressure = 1;
                        break;
                }

                if (this.ReferenceImage is null)
                {
                    this.Tool_Delta();
                }
                else
                {
                    if (this.IsReferenceImageResizing)
                        this.ReferenceImage.Resizing(this.Position);
                    else
                        this.ReferenceImage.Add(this.Position - this.StartingPosition);

                    this.CanvasControl.Invalidate();
                }
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.SetCanvasState(this.OptionType.IsMarquees() || this.OptionType.IsEffect());

                if (this.ReferenceImage is null)
                {
                    this.Tool_Complete();
                }
                else
                {
                    this.ReferenceImage = null;
                    this.CanvasControl.Invalidate();
                }
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.StartingPoint = this.Point = point;
                this.View_Start();
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Point = point;
                this.View_Delta();
            };
            this.Operator.Right_Complete += (point) =>
            {
                this.View_Complete();
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.SetCanvasState(true);
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.SetCanvasState(this.OptionType.IsMarquees() || this.OptionType.IsEffect());

                this.AppBar.ConstructView(this.Transformer);
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.Tip(TipType.Zoom);

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.AppBar.ConstructView(this.Transformer);
            };
        }

        private void ConstructSimulate()
        {
            this.SimulateCanvas.Start += (point) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;
                this.StartingPressure = this.Pressure = 1f;
                this.Tool_Start();
            };
            this.SimulateCanvas.Delta += (point) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;
                //this.Pressure = 1f;
                this.Tool_Delta();
            };
            this.SimulateCanvas.Complete += (point) =>
            {
                this.Tool_Complete();
            };
        }

    }
}