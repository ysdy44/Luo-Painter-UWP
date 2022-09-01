using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal enum BSplineType : byte
    {
        None,
        BeginFigure,
        Node,
        EndFigure,
        Close,
    }

    internal sealed class BSplines : List<Vector2>
    {

        public readonly bool IsClosed = false;
        public readonly int Steps = 16;

        public readonly float Tension = 0;
        public readonly float Continuity = 0;
        public readonly float Bias = 0;

        Vector2 SplineA;
        Vector2 SplineB;

        Vector2 Spline1;
        Vector2 Spline2;
        Vector2 Spline3;
        Vector2 Spline4;

        private BSplineType GetType(int i)
        {
            if (i is 0) return BSplineType.BeginFigure;
            if (i < base.Count - 2) return BSplineType.Node;
            if (i < base.Count - 1) return BSplineType.EndFigure;
            return this.IsClosed ? BSplineType.Close : BSplineType.None;
        }

        public void Draw(CanvasDrawingSession ds)
        {
            switch (base.Count)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    ds.DrawLine(base[0], base[1], Colors.DodgerBlue);
                    break;
                default:
                    this.SplineA = default;

                    for (int i = 0; i < base.Count; i++)
                    {
                        switch (this.GetType(i))
                        {
                            case BSplineType.BeginFigure:
                                this.Spline1 = base[this.IsClosed ? base.Count - 1 : i];
                                this.Spline2 = base[i];
                                this.Spline3 = base[i + 1];
                                this.Spline4 = base[i + 2];
                                break;
                            case BSplineType.Node:
                                this.Spline1 = base[i - 1];
                                this.Spline2 = base[i];
                                this.Spline3 = base[i + 1];
                                this.Spline4 = base[i + 2];
                                break;
                            case BSplineType.EndFigure:
                                this.Spline1 = base[i - 1];
                                this.Spline2 = base[i];
                                this.Spline3 = base[i + 1];
                                this.Spline4 = base[this.IsClosed ? 0 : i + 1];
                                break;
                            case BSplineType.Close:
                                this.Spline1 = base[i - 1];
                                this.Spline2 = base[i];
                                this.Spline3 = base[0];
                                this.Spline4 = base[1];
                                break;
                            default:
                                continue;
                        }


                        for (int j = 0; j < this.Steps; j++)
                        {
                            float scale = (float)j / this.Steps;
                            float pow2 = (float)Math.Pow(scale, 2);
                            float pow3 = (float)Math.Pow(scale, 3);

                            float h1 = 2 * pow3 - 3 * pow2 + 1;
                            float h2 = -2 * pow3 + 3 * pow2;
                            float h3 = pow3 - 2 * pow2 + scale;
                            float h4 = pow3 - pow2;


                            float t = 1 - this.Tension;
                            float c1 = 1 + this.Continuity;
                            float c2 = 1 - this.Continuity;
                            float b1 = 1 + this.Bias;
                            float b2 = 1 - this.Bias;

                            float tc1b1 = t * c1 * b1 / 2;
                            float tc2b2 = t * c2 * b2 / 2;
                            float tc2b1 = t * c2 * b1 / 2;
                            float tc1b2 = t * c1 * b2 / 2;


                            float dix = tc1b1 * (this.Spline2.X - this.Spline1.X) + tc2b2 * (this.Spline3.X - this.Spline2.X);
                            float diy = tc1b1 * (this.Spline2.Y - this.Spline1.Y) + tc2b2 * (this.Spline3.Y - this.Spline2.Y);

                            float six = tc2b1 * (this.Spline3.X - this.Spline2.X) + tc1b2 * (this.Spline4.X - this.Spline3.X);
                            float siy = tc2b1 * (this.Spline3.Y - this.Spline2.Y) + tc1b2 * (this.Spline4.Y - this.Spline3.Y);

                            this.SplineB.X = h1 * this.Spline2.X + h2 * this.Spline3.X + h3 * dix + h4 * six;
                            this.SplineB.Y = h1 * this.Spline2.Y + h2 * this.Spline3.Y + h3 * diy + h4 * siy;


                            if (this.SplineA != default) ds.DrawLine(this.SplineA, this.SplineB, Colors.DodgerBlue);
                            this.SplineA = this.SplineB;
                        }
                    }
                    break;
            }
        }

    }

    public sealed partial class PenBSplinesPage : Page
    {

        CanvasBitmap CanvasBitmap;
        readonly BSplines BSplines = new BSplines();

        public PenBSplinesPage()
        {
            this.InitializeComponent();
            this.ConstructPenBSplines();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenBSplines()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BSplines.Clear();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(this.CanvasBitmap);

                this.BSplines.Draw(args.DrawingSession);

                foreach (Vector2 item in this.BSplines)
                {
                    args.DrawingSession.FillCircle(item, 4, Colors.White);
                    args.DrawingSession.FillCircle(item, 3, Colors.DodgerBlue);
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                if (this.BSplines.Count is 0)
                {
                    this.BSplines.Add(point);
                    this.BSplines.Add(point);
                }
                else
                {
                    this.BSplines.Add(point);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.BSplines.Count < 2) return;

                this.BSplines[this.BSplines.Count - 1] = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.BSplines.Count < 2) return;

                this.BSplines[this.BSplines.Count - 1] = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                if (this.BSplines.Count < 2) return;

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                switch (pp.PointerDevice.PointerDeviceType)
                {
                    case PointerDeviceType.Touch:
                        return;
                }

                Vector2 point = pp.Position.ToVector2();
                this.BSplines[this.BSplines.Count - 1] = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        public async Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".bmp"
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.CanvasBitmap = bitmap;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}