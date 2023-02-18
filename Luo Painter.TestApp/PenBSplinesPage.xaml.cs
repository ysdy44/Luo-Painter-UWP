using Luo_Painter.Blends;
using Microsoft.Graphics.Canvas;
using System;
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
    public sealed partial class PenBSplinesPage : Page
    {

        CanvasBitmap CanvasBitmap;
        BSpline BSplines = new BSpline();
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
            this.ListView.ItemsSource = this.BSplines;
        }

        private void ConstructParameter()
        {
            this.TensionButton.Click += (s, e) => this.TensionSlider.Value = 0;
            this.TensionSlider.Value = this.Tension * 100;
            this.TensionSlider.ValueChanged += (s, e) =>
            {
                this.Tension = (float)e.NewValue / 100;

                if (this.CheckBox.IsChecked is true)
                {
                    int index = this.ListView.SelectedIndex;
                    if (index >= 0 && index < this.BSplines.Count) this.BSplines[index].Tension = this.Tension;
                }
                else
                {
                    foreach (BSplinePoints item in this.BSplines)
                    {
                        item.Tension = this.Tension;
                    }
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.ContinuityButton.Click += (s, e) => this.ContinuitySlider.Value = 0;
            this.ContinuitySlider.Value = this.Continuity * 100;
            this.ContinuitySlider.ValueChanged += (s, e) =>
            {
                this.Continuity = (float)e.NewValue / 100;

                if (this.CheckBox.IsChecked is true)
                {
                    int index = this.ListView.SelectedIndex;
                    if (index >= 0 && index < this.BSplines.Count) this.BSplines[index].Continuity = this.Continuity;
                }
                else
                {
                    foreach (BSplinePoints item in this.BSplines)
                    {
                        item.Continuity = this.Continuity;
                    }
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.BiasButton.Click += (s, e) => this.BiasSlider.Value = 0;
            this.BiasSlider.Value = this.Bias * 100;
            this.BiasSlider.ValueChanged += (s, e) =>
            {
                this.Bias = (float)e.NewValue / 100;

                if (this.CheckBox.IsChecked is true)
                {
                    int index = this.ListView.SelectedIndex;
                    if (index >= 0 && index < this.BSplines.Count) this.BSplines[index].Bias = this.Bias;
                }
                else
                {
                    foreach (BSplinePoints item in this.BSplines)
                    {
                        item.Bias = this.Bias;
                    }
                }

                this.BSplines.Arrange();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.StepsSlider.ValueChanged += (s, e) =>
            {
                this.Steps = (int)e.NewValue;

                if (this.CheckBox.IsChecked is true)
                {
                    int index = this.ListView.SelectedIndex;
                    if (index >= 0 && index < this.BSplines.Count) this.BSplines[index].Steps = this.Steps;
                }
                else
                {
                    foreach (BSplinePoints item in this.BSplines)
                    {
                        item.Steps = this.Steps;
                    }
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

                this.ListView.ItemsSource = null;
                this.ListView.ItemsSource = this.BSplines;
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

                    this.ListView.ItemsSource = null;
                    this.ListView.ItemsSource = this.BSplines;
                }
                else
                {
                    this.BSplines.Add(new BSplinePoints { Point = point });
                    this.BSplines.Arrange();
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.ListView.ItemsSource = null;
                    this.ListView.ItemsSource = this.BSplines;
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

                this.ListView.ItemsSource = null;
                this.ListView.ItemsSource = this.BSplines;
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