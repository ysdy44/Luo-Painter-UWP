using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/direct2d/arithmetic-composite
    /// https://docs.microsoft.com/zh-cn/windows/win32/direct2d/arithmetic-composite
    /// </summary>
    public sealed partial class ArithmeticCompositeEffectPage : Page
    {

        CanvasBitmap ACanvasBitmap;
        CanvasBitmap BCanvasBitmap;
        Vector2 ASize;
        Vector2 BSize;
        Vector2 A;
        Vector2 B;
        bool IsA;
        bool IsB;

        float MultiplyAmount = 1;
        float Source1Amount;
        float Source2Amount;
        float Offset;

        public ArithmeticCompositeEffectPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructArithmeticCompositeEffect();
        }

        private void ConstructArithmeticCompositeEffect()
        {
            this.ClearButton.Click += (s, e) =>
            {
                this.C1Slider.Value = 100;
                this.C2Slider.Value = 0;
                this.C3Slider.Value = 0;
                this.C4Slider.Value = 0;
            };
            this.XorButton.Click += (s, e) =>
            {
                this.C1Slider.Value = 0;
                this.C2Slider.Value = 100;
                this.C3Slider.Value = -100;
                this.C4Slider.Value = 0;
            };
            this.AddButton.Click += (s, e) =>
            {
                this.C1Slider.Value = 0;
                this.C2Slider.Value = -100;
                this.C3Slider.Value = 100;
                this.C4Slider.Value = 0;
            };

            this.C1Slider.ValueChanged += (s, e) =>
            {
                this.MultiplyAmount = (float)(e.NewValue / 100);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.C2Slider.ValueChanged += (s, e) =>
            {
                this.Source1Amount = (float)(e.NewValue / 100);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.C3Slider.ValueChanged += (s, e) =>
            {
                this.Source2Amount = (float)(e.NewValue / 100);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.C4Slider.ValueChanged += (s, e) =>
            {
                this.Offset = (float)(e.NewValue / 100);
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.AddAButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file, true);
                if (result != true) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.AddBButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file, false);
                if (result != true) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.Draw += (sender, args) =>
            {
                bool noA = this.ACanvasBitmap == null;
                bool noB = this.BCanvasBitmap == null;
                if (noA && noB) return;

                if (noA == false && noB)
                {
                    args.DrawingSession.DrawImage(this.ACanvasBitmap, this.A);
                    args.DrawingSession.DrawRectangle(this.A.X, this.A.Y, this.ASize.X, this.ASize.Y, Colors.White);
                    args.DrawingSession.DrawNode(this.A);
                    return;
                }

                if (noA && noB == false)
                {
                    args.DrawingSession.DrawImage(this.BCanvasBitmap, this.B);
                    args.DrawingSession.DrawRectangle(this.B.X, this.B.Y, this.BSize.X, this.BSize.Y, Colors.White);
                    args.DrawingSession.DrawNode(this.B);
                    return;
                }

                args.DrawingSession.DrawImage(new ArithmeticCompositeEffect
                {
                    MultiplyAmount = this.MultiplyAmount,
                    Source1Amount = this.Source1Amount,
                    Source2Amount = this.Source2Amount,
                    Offset = this.Offset,
                    Source1 = new Transform2DEffect
                    {
                        TransformMatrix = Matrix3x2.CreateTranslation(this.A),
                        Source = this.ACanvasBitmap,
                    },
                    Source2 = new Transform2DEffect
                    {
                        TransformMatrix = Matrix3x2.CreateTranslation(this.B),
                        Source = this.BCanvasBitmap,
                    },
                });
                args.DrawingSession.DrawRectangle(this.A.X, this.A.Y, this.ASize.X, this.ASize.Y, Colors.White);
                args.DrawingSession.DrawRectangle(this.B.X, this.B.Y, this.BSize.X, this.BSize.Y, Colors.White);
                args.DrawingSession.DrawNode(this.A);
                args.DrawingSession.DrawNode(this.B);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.IsA = FanKit.Math.InNodeRadius(this.A, point);
                this.IsB = FanKit.Math.InNodeRadius(this.B, point);
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.IsA) this.A = point;
                else if (this.IsB) this.B = point;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
            };
        }

        public async static Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
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

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference, bool isA)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream))
                {
                    if (isA)
                    {
                        this.ACanvasBitmap = bitmap;
                        this.ASize = bitmap.Size.ToVector2();
                    }
                    else
                    {
                        this.BCanvasBitmap = bitmap;
                        this.BSize = bitmap.Size.ToVector2();
                    }
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