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
    public sealed partial class DirectionDistancePage : Page
    {
        CanvasBitmap originalImage;
        CanvasBitmap effectImage;
        int imageWidth;
        int imageHeight;
        DistanceData[,] distanceDatas;
        double distance = 0.2;
        public DirectionDistancePage()
        {
            this.InitializeComponent();
            Init();
        }

        private void Init()
        {
            selectPicture.Click += (s, e) =>
            {
                SingleImage();
            };

            originalCanvas.Draw += (s, e) =>
            {
                if (originalImage == null)
                    return;
                e.DrawingSession.DrawImage(originalImage);
            };

            bool pressed = false;
            Point pos = new Point();
            float scale = 1f;
            effectCanvas.Draw += (s, e) =>
            {
                if (effectImage == null)
                    return;

                var te = new Transform2DEffect()
                {
                    Source = effectImage,
                    TransformMatrix = Matrix3x2.CreateScale(scale),
                };

                e.DrawingSession.DrawImage(te);
                if (pressed)
                {
                    int x = (int)Math.Round(pos.X);
                    int y = (int)Math.Round(pos.Y);
                    if (x > imageWidth - 1 || y > imageHeight - 1)
                        return;
                    var data = distanceDatas[x, y];
                    if (data.RecentItem is null)
                        return;
                    var p = data.RecentItem.Position;
                    var startPosition = new Vector2(x, y);
                    var endPosition = new Vector2(data.Recent.X * imageWidth, data.Recent.Y * imageHeight);

                    e.DrawingSession.DrawLine(startPosition, endPosition, Colors.Blue, 2f);

                }
            };

            effectCanvas.PointerWheelChanged += (s, e) =>
            {

                FrameworkElement element = (FrameworkElement)s;
                var dir = e.GetCurrentPoint(element).Properties.MouseWheelDelta;
                //缩放
                if (e.KeyModifiers == Windows.System.VirtualKeyModifiers.Control)
                {
                    scale += dir > 0 ? 0.1f : -0.1f;
                }
                effectCanvas.Invalidate();
            };

            effectCanvas.PointerPressed += (s, e) =>
            {
                pressed = true;
                pos = e.GetCurrentPoint((FrameworkElement)s).Position;
                effectCanvas.Invalidate();
            };
            effectCanvas.PointerReleased += (s, e) =>
            {
                pressed = false;
                effectCanvas.Invalidate();
            };

            effectCanvas.PointerMoved += (s, e) =>
            {
                if (pressed)
                {
                    pos = e.GetCurrentPoint((FrameworkElement)s).Position;
                    effectCanvas.Invalidate();
                }
            };


            action.Click += (s, e) =>
            {
                if (originalImage == null)
                    return;
                GenarateDirectionDistanceEffect();
            };
        }

        void GenarateDirectionDistanceEffect()
        {
            var sourceColor = originalImage.GetPixelColors();
            var targetColor = new Color[sourceColor.Length];
            distance = disSlider.Value / 100d;
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    var index = x + imageWidth * y;
                    var pos = FixUV(x, y);
                    distanceDatas[x, y] = new DistanceData(pos, sourceColor[index]);
                }
            }
            //由左，从上至下寻找
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    var item = distanceDatas[x, y];
                    int index = x + imageWidth * y;
                    if (item.Color.R < 255)
                    {
                        targetColor[index] = Colors.Black;
                        continue;
                    }
                    //左边
                    if (x - 1 >= 0)
                        FindTheNearest(x, y, x - 1, y, distanceDatas);
                    if (y - 1 >= 0)
                        FindTheNearest(x, y, x, y - 1, distanceDatas);
                    if (x + 1 < imageWidth)
                        FindTheNearest(x, y, x + 1, y, distanceDatas);
                    if (y + 1 < imageHeight)
                        FindTheNearest(x, y, x, y + 1, distanceDatas);
                }
            }

            //由左，从下至上寻找 这是有问题后多加的循环
            for (int x = 0;x<imageWidth;x++)
            {
                for (int y = imageHeight - 1; y >= 0; y--)
                {
                    var item = distanceDatas[x, y];
                    int index = x + imageWidth * y;
                    if (item.Color.R < 255)
                    {
                        targetColor[index] = Colors.Red;
                        continue;
                    }
                    //左边
                    if (x - 1 >= 0)
                        FindTheNearest(x, y, x - 1, y, distanceDatas);
                    if (x + 1 < imageWidth)
                        FindTheNearest(x, y, x + 1, y, distanceDatas);
                    if (y - 1 >= 0)
                        FindTheNearest(x, y, x, y - 1, distanceDatas);
                    if (y + 1 < imageHeight)
                        FindTheNearest(x, y, x, y + 1, distanceDatas);

                    if (item.Distance < double.PositiveInfinity)
                    {
                        Color color = ShaderFunction.Lerp(Colors.Black, Colors.White, ShaderFunction.Smoothstep(0, 0.3, item.Distance));
                        targetColor[index] = Colors.Red;
                        targetColor[index] = color;
                    }
                }
            }
            //由右，从下至上寻找 这是有问题后多加的循环
            for (int x = imageWidth-1; x >0; x--)
            {
                for (int y = 0;y<imageHeight;y++)
                {
                    var item = distanceDatas[x, y];
                    int index = x + imageWidth * y;
                    if (item.Color.R < 255)
                    {
                        targetColor[index] = Colors.Red;
                        continue;
                    }
                    //左边
                    if (x - 1 >= 0)
                        FindTheNearest(x, y, x - 1, y, distanceDatas);
                    if (x + 1 < imageWidth)
                        FindTheNearest(x, y, x + 1, y, distanceDatas);
                    if (y - 1 >= 0)
                        FindTheNearest(x, y, x, y - 1, distanceDatas);
                    if (y + 1 < imageHeight)
                        FindTheNearest(x, y, x, y + 1, distanceDatas);

                    if (item.Distance < double.PositiveInfinity)
                    {
                        Color color = ShaderFunction.Lerp(Colors.Black, Colors.White, ShaderFunction.Smoothstep(0, 0.3, item.Distance));
                        targetColor[index] = Colors.Red;
                        targetColor[index] = color;
                    }
                }
            }
            //由右，从下至下寻找
            for (int x = imageWidth - 1; x >= 0; x--)
            {
                for (int y = imageHeight - 1; y >= 0; y--)
                {
                    var item = distanceDatas[x, y];
                    int index = x + imageWidth * y;
                    if (item.Color.R < 255)
                    {
                        targetColor[index] = Colors.Red;
                        continue;
                    }
                    //左边
                    if (x - 1 >= 0)
                        FindTheNearest(x, y, x - 1, y, distanceDatas);
                    if (x + 1 < imageWidth)
                        FindTheNearest(x, y, x + 1, y, distanceDatas);
                    if (y - 1 >= 0)
                        FindTheNearest(x, y, x, y - 1, distanceDatas);
                    if (y + 1 < imageHeight)
                        FindTheNearest(x, y, x, y + 1, distanceDatas);

                    if (item.Distance < double.PositiveInfinity)
                    {
                        Color color = ShaderFunction.Lerp(Colors.Black, Colors.White, ShaderFunction.Smoothstep(0, distance, item.Distance));
       
                        targetColor[index] = color;
                    }
                }
            }
            effectImage = CanvasBitmap.CreateFromColors(effectCanvas, targetColor, imageWidth, imageHeight);
            effectCanvas.Invalidate();
        }



        /// <summary>
        /// 查找最近
        /// </summary>
        void FindTheNearest(int x, int y, int tx, int ty, DistanceData[,] distanceDatas)
        {
            var self = distanceDatas[x, y];

            var neighbor = distanceDatas[tx, ty];
            //有颜色即是黑色
            if (neighbor.Color.R < 255)
            {
                var dis = Vector2.Distance(self.Position, neighbor.Position);
                if (dis < self.Distance)
                {
                    self.Distance = dis;
                    self.Recent = neighbor.Position;
                    self.RecentItem = neighbor;
                }
            }
            else if (neighbor.Recent.X >= 0 && neighbor.Recent.Y >= 0)
            {
                var dis = Vector2.Distance(self.Position, neighbor.Recent);
                if (dis < self.Distance)
                {
                    self.Distance = dis;
                    self.Recent = neighbor.Recent;
                    self.RecentItem = neighbor.RecentItem;
                }

            }
        }

        Vector2 FixUV(int x, int y)
        {
            Vector2 pos = new Vector2((float)x / (float)(imageWidth - 1), (float)y / (float)(imageHeight - 1));
            return pos;
        }

        async void SingleImage()
        {
            var file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
            if (file != null)
                originalImage = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), await file.OpenReadAsync());

            imageWidth = (int)originalImage.SizeInPixels.Width;
            imageHeight = (int)originalImage.SizeInPixels.Height;
            distanceDatas = new DistanceData[imageWidth, imageHeight];
            effectCanvas.Invalidate();
            originalCanvas.Invalidate();
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

    public class DistanceData
    {

        public double Distance;
        public Vector2 Recent;
        public Vector2 Position;
        public Color Color;
        public DistanceData RecentItem;
        public DistanceData(Vector2 position, Color color)
        {
            Recent = new Vector2(-1, -1);
            Position = position;
            this.Color = color;
            this.Distance = double.PositiveInfinity;
        }
    }


}
