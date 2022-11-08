using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
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