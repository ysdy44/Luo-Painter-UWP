using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShaderTestPage : Page
    {
        CanvasBitmap originalBitmap1;
        CanvasBitmap originalBitmap2;
        CanvasBitmap sdfBitmap1;
        CanvasBitmap sdfBitmap2;

        byte[] sdfMorphCode;
        byte[] filmicCode;
        byte[] luminanceHeatmapCode;
        byte[] reinhardCode;
        byte[] sdrOverlayCode;
        Vector2 canvasSize;
        float fade;
        public ShaderTestPage()
        {
            this.InitializeComponent();
            InitImageOperate();
            InitCanvasOprate();
        }

        async Task LoadShaderCode()
        {
            sdfMorphCode = await ShaderType.SDFMorph.LoadAsync();
            filmicCode = await ShaderType.FilmicEffect.LoadAsync();
            luminanceHeatmapCode = await ShaderType.LuminanceHeatmapEffect.LoadAsync();
            reinhardCode = await ShaderType.ReinhardEffect.LoadAsync();
            sdrOverlayCode = await ShaderType.SdrOverlayEffect.LoadAsync();
        }

        public void InitCanvasOprate()
        {
            effectCanvas.CreateResources += (s, e) =>
            {
                e.TrackAsyncAction(LoadShaderCode().AsAsyncAction());
                canvasSize = effectCanvas.ActualSize;
            };

            effectCanvas.SizeChanged += (s, e) =>
            {
                canvasSize = effectCanvas.ActualSize;
            };

            effectCanvas.Draw += (s, e) =>
            {
                if (originalBitmap1 is null)
                    return;

                PixelShaderEffect pse = new PixelShaderEffect(filmicCode)
                {
                    Source1 = originalBitmap1,
                };
                var te = DirectionDistancePage.EvalImageCentered(s.Size.Width, s.Size.Height, originalBitmap1.Size.Width, originalBitmap1.Size.Height);
                te.Source = pse;

                e.DrawingSession.DrawImage(te);
            };

            effectCanvas1.Draw += (s, e) =>
            {

                if (originalBitmap2 is null)
                    return;



                PixelShaderEffect pse = new PixelShaderEffect(luminanceHeatmapCode)
                {
                    Source1 = originalBitmap2,
                };
                var te = DirectionDistancePage.EvalImageCentered(s.Size.Width, s.Size.Height, originalBitmap2.Size.Width, originalBitmap2.Size.Height);
                te.Source = pse;

                e.DrawingSession.DrawImage(te);
            };

            effectCanvas2.Draw += (s, e) =>
            {
                if (sdfBitmap1 is null)
                    return;

                PixelShaderEffect pse = new PixelShaderEffect(reinhardCode)
                {
                    Source1 = sdfBitmap1,
                };
                var te = DirectionDistancePage.EvalImageCentered(s.Size.Width, s.Size.Height, sdfBitmap1.Size.Width, sdfBitmap1.Size.Height);
                te.Source = pse;

                e.DrawingSession.DrawImage(te);
            };

            effectCanvas3.Draw += (s, e) =>
            {
                if (sdfBitmap2 is null)
                    return;

                PixelShaderEffect pse = new PixelShaderEffect(sdrOverlayCode)
                {
                    Source1 = sdfBitmap2,
                };

                

                var te = DirectionDistancePage.EvalImageCentered(s.Size.Width, s.Size.Height, sdfBitmap2.Size.Width, sdfBitmap2.Size.Height);
                te.Source = pse;

                e.DrawingSession.DrawImage(te);
            };


        }

        private void InitImageOperate()
        {
            sdfPanel2.AllowDrop = sdfPanel1.AllowDrop = originalPanel2.AllowDrop = originalPanel1.AllowDrop = true;


            originalPanel1.Drop += async (s, e) =>
            {
                originalBitmap1 = await ReadImageResoureAsync(e, originalImage1,effectCanvas.Device);
            };
            originalPanel2.Drop += async (s, e) =>
            {
                originalBitmap2 = await ReadImageResoureAsync(e, originalImage2, effectCanvas1.Device);
            };
            sdfPanel1.Drop += async (s, e) =>
            {
                sdfBitmap1 = await ReadImageResoureAsync(e, sdfImage1, effectCanvas2.Device);
            };
            sdfPanel2.Drop += async (s, e) =>
            {
                sdfBitmap2 = await ReadImageResoureAsync(e, sdfImage2, effectCanvas3.Device);
            };

            DragEventHandler OnOver = (s, e) => e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            originalPanel1.DragOver += OnOver;
            originalPanel2.DragOver += OnOver;
            sdfPanel1.DragOver += OnOver;
            sdfPanel2.DragOver += OnOver;
        }


        async Task<CanvasBitmap> ReadImageResoureAsync(DragEventArgs e, Image image,CanvasDevice device)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var files = await e.DataView.GetStorageItemsAsync();
                StorageFile file = files.OfType<StorageFile>().First(p => { return p.FileType == ".png" || p.FileType == ".jpg"; });
                if (file == null)
                    return null;
                var bitmap = await CanvasBitmap.LoadAsync(device, await file.OpenReadAsync());
                BitmapImage bi = new BitmapImage();
                await bi.SetSourceAsync(await file.OpenReadAsync());
                image.Source = bi;
                return bitmap;
            }
            return null;
        }
    }
}
