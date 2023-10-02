using FanKit.Transformers;
using Luo_Painter.Layers;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Strings;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;

                switch (device)
                {
                    case InputDevice.Pen:
                        this.StartingPressure = this.Pressure = properties.Pressure;
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

                this.Single_Start();
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;

                switch (device)
                {
                    case InputDevice.Pen:
                        this.Pressure = properties.Pressure;
                        break;
                    default:
                        this.Pressure = 1;
                        break;
                }

                if (this.ReferenceImage is null)
                {
                    this.Single_Delta();
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
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                if (this.ReferenceImage is null)
                {
                    this.Single_Complete();
                }
                else
                {
                    this.ReferenceImage = null;
                    this.CanvasControl.Invalidate();
                }
            };


            // Holding
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.StartingPoint = this.Point = point;
                if (isHolding)
                    this.Straw_Start();
                else
                    this.View_Start();
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Point = point;
                if (isHolding)
                    this.Straw_Delta();
                else
                    this.View_Delta();
            };
            this.Operator.Right_Complete += (point, isHolding) =>
            {
                if (isHolding)
                    this.Straw_Complete();
                else
                    this.View_Complete();
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasAnimatedControl.Invalidate(true); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasAnimatedControl.Invalidate(this.OptionType.HasPreview()); // Invalidate

                this.ConstructView(this.Transformer);
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.Tip(TipType.Zoom, $"{this.Transformer.Scale * 100:0.00}%");

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.ConstructView(this.Transformer);
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