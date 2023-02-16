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

    internal readonly struct BSplineParameter
    {
        public readonly float tc1b1;
        public readonly float tc2b2;
        public readonly float tc2b1;
        public readonly float tc1b2;
        public BSplineParameter(float tension, float continuity, float bias)
        {
            float t = 1 - tension;

            float c1 = 1 + continuity;
            float c2 = 1 - continuity;
            float b1 = 1 + bias;
            float b2 = 1 - bias;

            this.tc1b1 = t * c1 * b1 / 2;
            this.tc2b2 = t * c2 * b2 / 2;
            this.tc2b1 = t * c2 * b1 / 2;
            this.tc1b2 = t * c1 * b2 / 2;
        }
    }

    internal sealed class BSplinePoints : List<Vector2>
    {
        public override string ToString() => Point.ToString();

        public Vector2 Point { get; set; }

        public float Tension { get; set; }
        public float Continuity { get; set; }
        public float Bias { get; set; }
        public int Steps { get; set; } = 16;

        public void Spline(Vector2 p1, Vector2 p3, Vector2 p4)
        {
            Vector2 p2 = this.Point;

            BSplineParameter p = new BSplineParameter(this.Tension, this.Continuity, this.Bias);

            base.Clear();
            for (int j = 0; j < this.Steps; j++)
            {
                float scale = (float)j / (float)this.Steps;
                float pow2 = (float)Math.Pow(scale, 2);
                float pow3 = (float)Math.Pow(scale, 3);

                float h1 = 2 * pow3 - 3 * pow2 + 1;
                float h2 = -2 * pow3 + 3 * pow2;
                float h3 = pow3 - 2 * pow2 + scale;
                float h4 = pow3 - pow2;

                float dix = p.tc1b1 * (p2.X - p1.X) + p.tc2b2 * (p3.X - p2.X);
                float diy = p.tc1b1 * (p2.Y - p1.Y) + p.tc2b2 * (p3.Y - p2.Y);

                float six = p.tc2b1 * (p3.X - p2.X) + p.tc1b2 * (p4.X - p3.X);
                float siy = p.tc2b1 * (p3.Y - p2.Y) + p.tc1b2 * (p4.Y - p3.Y);

                base.Add(new Vector2
                {
                    X = h1 * p2.X + h2 * p3.X + h3 * dix + h4 * six,
                    Y = h1 * p2.Y + h2 * p3.Y + h3 * diy + h4 * siy
                });
            }
        }
    }

    internal sealed class BSplines2 : List<BSplinePoints>
    {

        public bool IsClosed { get; set; } = false;

        Vector2 Point;

        private BSplineType GetType(int i)
        {
            if (i is 0) return BSplineType.BeginFigure;
            if (i < base.Count - 2) return BSplineType.Node;
            if (i < base.Count - 1) return BSplineType.EndFigure;
            return this.IsClosed ? BSplineType.Close : BSplineType.None;
        }

        public void Arrange()
        {
            switch (base.Count)
            {
                case 0:
                case 1:
                case 2:
                    foreach (BSplinePoints item in this)
                    {
                        item.Clear();
                    }
                    break;
                default:
                    this.Point = default;

                    for (int i = 0; i < base.Count; i++)
                    {
                        switch (this.GetType(i))
                        {
                            case BSplineType.BeginFigure: base[i].Spline(base[this.IsClosed ? base.Count - 1 : i].Point, base[i + 1].Point, base[i + 2].Point); break;
                            case BSplineType.Node: base[i].Spline(base[i - 1].Point, base[i + 1].Point, base[i + 2].Point); break;
                            case BSplineType.EndFigure: base[i].Spline(base[i - 1].Point, base[i + 1].Point, base[this.IsClosed ? 0 : i + 1].Point); break;
                            case BSplineType.Close: base[i].Spline(base[i - 1].Point, base[0].Point, base[1].Point); break;
                            default: continue;
                        }
                    }
                    break;
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    ds.DrawLine(base[0].Point, base[1].Point, Colors.DodgerBlue);
                    break;
                default:
                    foreach (BSplinePoints item in this)
                    {
                        foreach (Vector2 p in item)
                        {
                            if (this.Point != default) ds.DrawLine(this.Point, p, Colors.DodgerBlue);
                            this.Point = p;
                        }
                    }
                    break;
            }
        }

    }

    public sealed partial class PenBSplinesPage : Page
    {

        CanvasBitmap CanvasBitmap;
        BSplines2 BSplines = new BSplines2();
        float Tension;
        float Continuity;
        float Bias;
        int Steps = 16;

        public PenBSplinesPage()
        {
            this.InitializeComponent();
            this.ConstructParameter();
            this.ConstructPenBSplines();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.BSplines.Arrange();
        }

        private void ConstructParameter()
        {
            this.TensionButton.Click += (s, e) => this.TensionSlider.Value = 0;
            this.TensionSlider.Value = this.Tension * 100;
            this.TensionSlider.ValueChanged += (s, e) =>
            {
                this.Tension = (float)e.NewValue / 100;

                foreach (BSplinePoints item in this.BSplines)
                {
                    item.Tension = this.Tension;
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.ContinuityButton.Click += (s, e) => this.ContinuitySlider.Value = 0;
            this.ContinuitySlider.Value = this.Continuity * 100;
            this.ContinuitySlider.ValueChanged += (s, e) =>
            {
                this.Continuity = (float)e.NewValue / 100;

                foreach (BSplinePoints item in this.BSplines)
                {
                    item.Continuity = this.Continuity;
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.BiasButton.Click += (s, e) => this.BiasSlider.Value = 0;
            this.BiasSlider.Value = this.Bias * 100;
            this.BiasSlider.ValueChanged += (s, e) =>
            {
                this.Bias = (float)e.NewValue / 100;

                foreach (BSplinePoints item in this.BSplines)
                {
                    item.Bias = this.Bias;
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.StepsSlider.ValueChanged += (s, e) =>
            {
                this.Steps = (int)e.NewValue;

                foreach (BSplinePoints item in this.BSplines)
                {
                    item.Steps = this.Steps;
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };
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
                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(this.CanvasBitmap);

                this.BSplines.Draw(args.DrawingSession);

                foreach (BSplinePoints item in this.BSplines)
                {
                    args.DrawingSession.FillCircle(item.Point, 4, Colors.White);
                    args.DrawingSession.FillCircle(item.Point, 3, Colors.DodgerBlue);
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.CanvasControl.PointerPressed += (s, e) =>
            {
                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                Vector2 point = pp.Position.ToVector2();

                if (this.BSplines.Count is 0)
                {
                    this.BSplines.Add(new BSplinePoints { Point = point });
                    this.BSplines.Add(new BSplinePoints { Point = point });
                    this.BSplines.Arrange();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else
                {
                    this.BSplines.Add(new BSplinePoints { Point = point });
                    this.BSplines.Arrange();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.CanvasControl.PointerMoved += (s, e) =>
            {
                if (this.BSplines.Count < 2) return;

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                Vector2 point = pp.Position.ToVector2();

                this.BSplines[this.BSplines.Count - 1].Point = point;
                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.CanvasControl.PointerReleased += (s, e) =>
            {
                if (this.BSplines.Count < 2) return;

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                Vector2 point = pp.Position.ToVector2();

                this.BSplines[this.BSplines.Count - 1].Point = point;
                this.BSplines.Arrange();
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
                this.BSplines[this.BSplines.Count - 1].Point = point;
                this.BSplines.Arrange();
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