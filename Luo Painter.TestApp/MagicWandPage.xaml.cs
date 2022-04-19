using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MagicWandPage : Page
    {
        CanvasBitmap originalImage;
        CanvasBitmap effectImage;
        Color[] originalColors;
        Color[] effectColors;

        int imageWidth;
        int imageHeight;


        /// <summary>
        /// 用于记录已近检索的像素
        /// </summary>
        Dictionary<Pos, int> tempDir = new Dictionary<Pos, int>();

        public MagicWandPage()
        {
            this.InitializeComponent();
            Init();
        }

        void Init()
        {
            selectPicture.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file != null)
                {
                    originalImage = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), await file.OpenReadAsync());
                    originalColors = originalImage.GetPixelColors();
                    effectColors = new Color[originalColors.Length];
                    effectCanvas.Width = originalCanvas.Width = originalImage.Size.Width;
                    effectCanvas.Height = originalCanvas.Height = originalImage.Size.Height;
                    effectImage = CanvasBitmap.CreateFromColors(effectCanvas, effectColors, (int)originalImage.Size.Width, (int)originalImage.Size.Height, 96f);

                    imageWidth = (int)originalImage.Size.Width;
                    imageHeight = (int)originalImage.Size.Height;
                    originalCanvas.Invalidate();
                    effectCanvas.Invalidate();
                }
            };

            originalCanvas.Draw += (s, e) =>
            {
                if (originalImage == null)
                    return;
                e.DrawingSession.DrawImage(originalImage);
            };

            effectCanvas.Draw += (s, e) =>
            {
                if (effectImage == null)
                    return;
                e.DrawingSession.DrawImage(effectImage);
            };

            PointerEventHandler func = (s, e) =>
             {
                 var can = (FrameworkElement)s;

                 var point = e.GetCurrentPoint(can).Position;
                 Vector3 v3 = new Vector3((float)point.X - 15f + can.ActualOffset.X, (float)point.Y - 15f + can.ActualOffset.Y, 0);
                 e1.Translation = e2.Translation = v3;
             };

            originalCanvas.PointerMoved += func;
            effectCanvas.PointerMoved += func;

            TappedEventHandler teh = (s, e) =>
            {
                if (originalImage == null)
                    return;

                var point = e.GetPosition(originalCanvas);

                float threshold = (float)thresholdSlider.Value / 100f;
                var tr = threshold * 255;
                var tg = threshold * 255;
                var tb = threshold * 255;
                var ta = threshold * 255;

                Pos cp = new Pos((int)Math.Round(point.X), (int)Math.Round(point.Y));
                Color selectColor = originalColors[cp.X + cp.Y * imageWidth];

                int count = 0;

                tempDir = new Dictionary<Pos, int>();
                tempDir.Add(cp, 1);
                List<Pos> retrieves = new List<Pos>();
                retrieves.Add(cp);
                while (retrieves.Count > 0)
                {
                    var p = retrieves[0];
                    retrieves.RemoveAt(0);
                    var index = p.X + p.Y * imageWidth;
                    Color currentColor = originalColors[index];
                    var r = Math.Abs(currentColor.R - selectColor.R);
                    var g = Math.Abs(currentColor.G - selectColor.G);
                    var b = Math.Abs(currentColor.B - selectColor.B);
                    var a = Math.Abs(currentColor.A - selectColor.A);
                    if (tr >= r && tg >= g && tb >= b && ta >= a)
                    {

                        effectColors[index] = originalColors[index];
                        AddRetrieves(p, retrieves);
                        count++;
                    }

                }
                Console.WriteLine(count);
                effectImage = CanvasBitmap.CreateFromColors(effectCanvas, effectColors, (int)originalImage.Size.Width, (int)originalImage.Size.Height, 96f);
                effectCanvas.Invalidate();
            };

            originalCanvas.Tapped += teh;
            effectCanvas.Tapped += teh;

        }

        void AddRetrieves(Pos cp, List<Pos> retrieves)
        {
            for (int rx = -1; rx <= 1; rx++)
            {
                for (int ry = -1; ry <= 1; ry++)
                {
                    int x = cp.X + rx;
                    int y = cp.Y + ry;
                    if (x >= 0 && y >= 0 && x < imageWidth && y < imageHeight)
                    {
                        Pos pos = new Pos(x, y);
                        if (!tempDir.ContainsKey(pos))
                        {
                            //添加到以检索队列中
                            tempDir.Add(pos, 1);
                            retrieves.Add(pos);
                        }
                        else
                        {
                            var a = "a";
                        }

                    }
                }
            }
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

    }

    struct Pos
    {
        public int X;
        public int Y;
        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
