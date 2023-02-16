using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal class FileInkCanvas : InkCanvas
    {
        readonly CanvasDevice CanvasDevice = new CanvasDevice();
        readonly float DPI;

        public FileInkCanvas()
        {
            DisplayInformation display = DisplayInformation.GetForCurrentView();
            this.DPI = 96f * (float)display.RawPixelsPerViewPixel;

            base.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Touch |
                CoreInputDeviceTypes.Mouse |
                CoreInputDeviceTypes.Pen;

            base.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
            {
                Color = Colors.DodgerBlue
            });
        }

        public async Task SaveAsync()
        {
            IReadOnlyList<InkStroke> inks = base.InkPresenter.StrokeContainer.GetStrokes();

            CanvasCommandList image = new CanvasCommandList(this.CanvasDevice);
            using (CanvasDrawingSession ds = image.CreateDrawingSession())
            {
                ds.DrawInk(inks);
            }

            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Thumbnail.png"))
            {
                await CanvasImage.SaveAsync(image, new Rect
                {
                    Width = base.ActualWidth,
                    Height = base.ActualHeight,
                }, this.DPI, this.CanvasDevice, stream, CanvasBitmapFileFormat.Png);
            }
        }
    }

    public sealed partial class FileImageSourcePage : Page
    {
        static string Path = System.IO.Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "Thumbnail.png");
        readonly FileImageSource FileImageSource = new FileImageSource(FileImageSourcePage.Path);

        public FileImageSourcePage()
        {
            this.InitializeComponent();
            this.LocalButton.Click += async (s, e) => await Launcher.LaunchFolderAsync(ApplicationData.Current.TemporaryFolder);
            this.RefreshButton.Click += async (s, e) =>
            {
                // 1. Save all Ink-Strokes to Image-File.
                await this.InkCanvas.SaveAsync();

                // 2. Refresh the Source of the Image-Control with a Image-File.
                await this.FileImageSource.Refresh(FileImageSourcePage.Path);

                // 3. UI
                this.Image.Source = this.FileImageSource.ImageSource;
            };
        }
    }
}