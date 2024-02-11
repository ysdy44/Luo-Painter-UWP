using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    partial class InkPresenter
    {

        //@Const
        public const int Width = 240;
        public const int Height = 120;
        public const int HeightHalf = 60;

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        // 0 ~ Π
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Radian(double value) => System.Math.Clamp(value, 0, 1) * FanKit.Math.Pi;
        // Sin 0 ~ Π ︵
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Pressure(double radian) => System.Math.Clamp(System.Math.Sin(radian), 0, 1);
        // Sin 0 ~ 2Π ~
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double OffsetY(double radian) => (float)System.Math.Clamp(System.Math.Sin(radian + radian), -1, 1);


        //@Pressure
        private float GetSizePressed(float x) => this.MinSize + (1 - this.MinSize) * this.GetPressure(this.SizePressure, x);
        private float GetFlowPressed(float x) => this.MinFlow + (1 - this.MinFlow) * this.GetPressure(this.FlowPressure, x);
        private float GetPressure(BrushEasePressure pressure, float x)
        {
            switch (pressure)
            {
                case BrushEasePressure.None: return 1;

                case BrushEasePressure.Linear: return x;

                case BrushEasePressure.Quadratic: return x * x;
                case BrushEasePressure.QuadraticFlipReverse: return 2 * x - x * x;

                case BrushEasePressure.Symmetry:
                    x *= 2;
                    x -= 1;
                    float y = (x > 0) ? (2 * x - x * x) : (2 * x + x * x);
                    y += 1;
                    y /= 2;
                    return y;

                default: return 1;
            }
        }

        public void IsometricTipFillCircle(CanvasDrawingSession ds, Color color)
        {
            float size = this.Size / 24 + 1;
            float spacing = 0.25f;

            float open = 10 + this.GetSizePressed(0.001f) * size;
            float end = InkPresenter.Width - 10 - this.GetSizePressed(0.001f) * size;

            float startingSizePressure = this.GetSizePressed(0.001f) * size + 1;
            float x = open + startingSizePressure * spacing;

            ds.FillCircle(open, InkPresenter.HeightHalf, startingSizePressure, color);
            ds.FillCircle(end, InkPresenter.HeightHalf, startingSizePressure, color);

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);
                float pressure = this.GetSizePressed((float)this.Pressure(radian));

                // 2. Get Position
                Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                // 3. Draw
                float sizePressed = this.GetSizePressed(pressure) * size + 1;
                ds.FillCircle(position, sizePressed, color);

                // 4. Foreach
                x += sizePressed * spacing;
            } while (x < end);
        }


        public void IsometricTip(CanvasDrawingSession ds, Color color)
        {
            float size = this.Size / 24 + 1;
            float spacing = this.Spacing;

            float open = 10 + this.GetSizePressed(0.001f) * size;
            float end = InkPresenter.Width - 10 - this.GetSizePressed(0.001f) * size;

            float startingSizePressure = this.GetSizePressed(0.001f) * size + 1;
            float x = open + startingSizePressure * spacing;

            switch (this.Tip)
            {
                case PenTipShape.Circle:
                    if (this.IsStroke)
                    {
                        ds.DrawCircle(open, InkPresenter.HeightHalf, startingSizePressure, color);
                        ds.DrawCircle(end, InkPresenter.HeightHalf, startingSizePressure, color);
                    }
                    else
                    {
                        ds.FillCircle(open, InkPresenter.HeightHalf, startingSizePressure, color);
                        ds.FillCircle(end, InkPresenter.HeightHalf, startingSizePressure, color);
                    }
                    break;
                case PenTipShape.Rectangle:
                    if (this.IsStroke)
                    {
                        ds.DrawRectangle(open - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                        ds.DrawRectangle(end - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                    }
                    else
                    {
                        ds.FillRectangle(open - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                        ds.FillRectangle(end - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                    }
                    break;
                default:
                    break;
            }

            switch (this.Tip)
            {
                case PenTipShape.Circle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian((x - open) / (end - open));
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = this.GetSizePressed((float)this.Pressure(radian));

                        // 2. Get Position
                        Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressed = this.GetSizePressed(pressure) * size + 1;
                        if (this.IsStroke)
                            ds.DrawCircle(position, sizePressed, color);
                        else
                            ds.FillCircle(position, sizePressed, color);

                        // 4. Foreach
                        x += sizePressed * spacing;
                    } while (x < end);
                    break;
                case PenTipShape.Rectangle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian(x / InkPresenter.Width);
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = this.GetSizePressed((float)this.Pressure(radian));

                        // 2. Get Position
                        Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressed = this.GetSizePressed(pressure) * size + 1;
                        if (this.IsStroke)
                            ds.DrawRectangle(position.X - sizePressed, position.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);
                        else
                            ds.FillRectangle(position.X - sizePressed, position.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);

                        // 4. Foreach
                        x += sizePressed * spacing;
                    } while (x < end);
                    break;
                default:
                    break;
            }
        }

        public void DrawLine(CanvasDrawingSession ds, Color color)
        {
            float strokeWidth = this.Size / 24 + 1;
            float spacing = this.Spacing;

            float open = strokeWidth + 10;
            float end = InkPresenter.Width - strokeWidth - 10;

            Vector2 position = new Vector2(open, InkPresenter.HeightHalf);
            float startingSizePressure = strokeWidth + 1;
            float x = open + startingSizePressure * spacing;

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);

                // 2. Get Position
                Vector2 targetPosition = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                // 3. Draw
                ds.DrawLine(position, targetPosition, color, strokeWidth, this.CanvasStrokeStyle);
                position = targetPosition;

                // 4. Foreach
                x += strokeWidth * spacing * 12;
            } while (x < end);

            ds.DrawLine(position.X, position.Y, end, InkPresenter.HeightHalf, color, strokeWidth, this.CanvasStrokeStyle);
        }

    }
}