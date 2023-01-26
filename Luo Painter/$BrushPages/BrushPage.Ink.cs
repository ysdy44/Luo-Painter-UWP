using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class BrushPage
    {

        private void ConstructInk()
        {
            this.ClearButton.Click += (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker))
                {
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);

                    this.CanvasControl.Invalidate();

                    System.Threading.Monitor.Exit(this.Locker);
                }
            };
            this.ImageButton.Click += async (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker)) System.Threading.Monitor.Exit(this.Locker);
                else return;

                IRandomAccessStreamReference file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                try
                {
                    using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                    {
                        CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Draw(bitmap);

                            // History
                            this.BitmapLayer.Flush();

                            this.CanvasControl.Invalidate(); // Invalidate
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            };
        }

    }
}