using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        int SymmetryCount => (int)this.SymmetrySlider.Value;
        SymmetryMode SymmetryMode
        {
            get
            {
                switch (this.SymmetryComboBox.SelectedIndex)
                {
                    case 0: return SymmetryMode.Horizontal;
                    case 1: return SymmetryMode.Vertical;
                    case 2: return SymmetryMode.Symmetry;
                    case 3: return SymmetryMode.Mirror;
                    default: return SymmetryMode.None;
                }
            }
        }

        public void ConstructSymmetry()
        {
            this.SymmetrySlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.SymmetryComboBox.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintBrushMulti_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            //@Paint
            int count = this.SymmetryCount;
            this.SymmetryType = this.Symmetryer.GetType(this.SymmetryMode, count);
            if (this.SymmetryMode == default) return;

            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            //@Paint
            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.Symmetryer.Construct(count, this.BitmapLayer.Center);
            foreach (StrokeCap cap in this.Symmetryer.GetCaps(this.SymmetryType, new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size, this.InkPresenter.IgnoreSizePressure), this.BitmapLayer.Center))
            {
                this.PaintStarted(cap);
            }

            //@Paint
            this.TasksStart();
        }

        private void PaintBrushMulti_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.SymmetryMode == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            foreach (StrokeSegment segment in this.Symmetryer.GetSegments(this.SymmetryType, new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing, this.InkPresenter.IgnoreSizePressure), this.BitmapLayer.Center))
            {
                this.Tasks.Add(segment);
            }

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;
        }

        private void PaintBrushMulti_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.SymmetryMode == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate

            //@Paint
            this.TasksStop();
        }

        private void DrawPaintBrushMulti(CanvasControl sender, CanvasDrawingSession ds, Vector2 center)
        {
            switch (this.SymmetryMode)
            {
                case SymmetryMode.None:
                    break;
                case SymmetryMode.Horizontal:
                    if (this.Transformer.Radian == default)
                        ds.DrawLine(center.X, 0, center.X, (float)sender.Size.Height, Colors.Gray);
                    else
                        ds.DrawLine(
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, FanKit.Math.PiOver2 - this.Transformer.Radian, false),
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, FanKit.Math.PiOver2 - this.Transformer.Radian, true),
                            Colors.Gray);
                    break;
                case SymmetryMode.Vertical:
                    if (this.Transformer.Radian == default)
                        ds.DrawLine(0, center.Y, (float)sender.Size.Width, center.Y, Colors.Gray);
                    else
                        ds.DrawLine(
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, -this.Transformer.Radian, false),
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, -this.Transformer.Radian, true),
                            Colors.Gray);
                    break;
                case SymmetryMode.Symmetry:
                case SymmetryMode.Mirror:
                    ds.FillCircle(center, 13, Colors.Gray);
                    int count = this.SymmetryCount;
                    if (count > 2)
                        for (int i = 0; i < count; i++)
                        {
                            ds.DrawLine(
                                this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, FanKit.Math.PiTwice * i / count - this.Transformer.Radian, false),
                                center,
                                Colors.Gray);
                        }
                    // Vertical
                    else if (this.Transformer.Radian == default)
                        ds.DrawLine(0, center.Y, (float)sender.Size.Width, center.Y, Colors.Gray);
                    else
                        ds.DrawLine(
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, this.Transformer.Radian, false),
                            this.GetSymmetryerBorder(center, (float)sender.Size.Width, (float)sender.Size.Height, this.Transformer.Radian, true),
                            Colors.Gray);
                    ds.FillCircle(center, 12, Colors.White);
                    break;
                default:
                    break;
            }
        }

        private Vector2 GetSymmetryerBorder(Vector2 center, float width, float height, float radian, bool reverse)
        {
            Vector2 r = new Vector2(System.MathF.Cos(radian), -System.MathF.Sin(radian));
            bool x = reverse ? r.X < 0 : r.X > 0;
            bool y = reverse ? r.Y < 0 : r.Y > 0;

            if (x && y)
            {
                float right = center.Y + (width - center.X) / r.X * r.Y;
                if (right >= 0 && right <= height) return new Vector2(width, right);

                float bottom = center.X + (height - center.Y) / r.Y * r.X;
                if (bottom >= 0 && bottom <= width) return new Vector2(bottom, height);

                return center;
            }
            else if (x is false && y)
            {
                float left = center.Y - center.X / r.X * r.Y;
                if (left >= 0 && left <= height) return new Vector2(0, left);

                float bottom = center.X + (height - center.Y) / r.Y * r.X;
                if (bottom >= 0 && bottom <= width) return new Vector2(bottom, height);

                return center;
            }
            else if (x && y is false)
            {
                float right = center.Y + (width - center.X) / r.X * r.Y;
                if (right >= 0 && right <= height) return new Vector2(width, right);

                float top = center.X - center.Y / r.Y * r.X;
                if (top >= 0 && top <= width) return new Vector2(top, 0);

                return center;
            }
            else
            {
                float left = center.Y - center.X / r.X * r.Y;
                if (left >= 0 && left <= height) return new Vector2(0, left);

                float top = center.X - center.Y / r.Y * r.X;
                if (top >= 0 && top <= width) return new Vector2(top, 0);

                return center;
            }
        }

    }
}