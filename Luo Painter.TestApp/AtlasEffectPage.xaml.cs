using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using FanKit.Transformers;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/direct2d/atlas
    /// https://docs.microsoft.com/zh-cn/windows/win32/direct2d/atlas
    /// </summary>
    public sealed partial class AtlasEffectPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;

        Rect Region;
        Rect Rect;
        Vector2 A;
        Vector2 B;
        bool IsA;
        bool IsB;

        public AtlasEffectPage()
        {
            this.InitializeComponent();
            this.ConstructAtlasEffect();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructAtlasEffect()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                Rect rect = this.CanvasBitmap.Bounds;
                this.Region = rect;
                this.Rect = rect;
                this.A = Vector2.Zero;
                this.B = new Vector2((float)rect.Width, (float)rect.Height);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.SortButton.Click += (s, e) =>
            {
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap == null) return;

                args.DrawingSession.DrawImage(this.CanvasBitmap);

                args.DrawingSession.DrawRectangle(this.Rect, Colors.White, 3);
                args.DrawingSession.DrawRectangle(this.Rect, Colors.DodgerBlue);

                args.DrawingSession.DrawNode(this.A);
                args.DrawingSession.DrawNode(this.B);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap == null) return;

                if (this.SortButton.IsChecked == true)
                {
                    args.DrawingSession.DrawImage(new CropEffect
                    {
                        SourceRectangle = this.Rect,
                        Source = this.CanvasBitmap,
                    });
                }
                else
                {
                    args.DrawingSession.DrawImage(new AtlasEffect
                    {
                        SourceRectangle = this.Rect,
                        PaddingRectangle = this.Rect,
                        Source = this.CanvasBitmap,
                    });
                }
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
                this.Rect = new Rect(this.A.ToPoint(), this.B.ToPoint());
                this.Rect.Intersect(this.Region);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
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

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.CanvasBitmap = bitmap;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}