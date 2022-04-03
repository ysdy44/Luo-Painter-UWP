using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class CanvasImageSourcePage : Page
    {
        public ImageSource Thumbnail => this.ThumbnailImageSource;
        CanvasDevice CanvasControl => CanvasDevice.GetSharedDevice();

        readonly CanvasImageSource ThumbnailImageSource = null;

        CanvasBitmap RenderTarget;

        public CanvasImageSourcePage()
        {
            this.InitializeComponent();

            this.ThumbnailImageSource = new CanvasImageSource(this.CanvasControl, 50, 50, 96);
            this.RenderThumbnail();

            base.Unloaded += (s, e) =>
            {
                CompositionTarget.SurfaceContentsLost -= this.CompositionTarget_SurfaceContentsLost;
            };
            base.Loaded += (s, e) =>
            {
                CompositionTarget.SurfaceContentsLost += this.CompositionTarget_SurfaceContentsLost;
            };
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.RenderThumbnail();
            };
        }

        private void CompositionTarget_SurfaceContentsLost(object sender, object e)
        {
            this.RenderThumbnail(true);
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
            if (reference == null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.RenderTarget = bitmap;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RenderThumbnail(bool surfaceContentsLost = false)
        {
            if (this.RenderTarget == null) return;

            // If the window isn't visible then we cannot update the image source
            if (Window.Current.Visible == false) return;

            if (this.CanvasControl != this.ThumbnailImageSource.Device || surfaceContentsLost)
            {
                this.ThumbnailImageSource.Recreate(this.CanvasControl);
            }

            try
            {
                using (CanvasDrawingSession ds = this.ThumbnailImageSource.CreateDrawingSession(Colors.White))
                {
                    ds.DrawImage(new ScaleEffect
                    {
                        Scale = new Vector2(50f / Math.Max(this.RenderTarget.SizeInPixels.Width, this.RenderTarget.SizeInPixels.Height)),
                        BorderMode = EffectBorderMode.Hard,
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                        Source = this.RenderTarget,
                    });
                }
            }
            catch (Exception e)
            {
                // XAML will also raise a SurfaceContentsLost event, and we use this to trigger redrawing
                // the surface.
                if (this.ThumbnailImageSource.Device.IsDeviceLost(e.HResult))
                    this.ThumbnailImageSource.Device.RaiseDeviceLost();
            }
        }

    }
}