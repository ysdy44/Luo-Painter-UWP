﻿using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
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
using Windows.Storage.Streams;
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
        CanvasRenderTarget originalTarget;
        byte[] blackOrWhiteCode;
        double distance = 0.2;
        float thresholse = 0;
        double colorThresholdse = 128;
        public DirectionDistancePage()
        {
            this.InitializeComponent();
            InitCanvas();
            InitButton();
        }

        void InitButton()
        {
            selectPicture.Click += (s, e) =>
            {
                SingleImage();
            };

            action.Click += (s, e) =>
            {
                if (originalTarget is null)
                    return;
                GenerateSDF();
                //GenarateDirectionDistanceEffect();
            };

            export.Click += async (s, e) =>
            {
                if (effectImage is null)
                    return;
                FileSavePicker savePicker = new FileSavePicker
                {
                    DefaultFileExtension = ".png",
                    SuggestedFileName = "有向距离场",
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    FileTypeChoices =
                {
                    ["png"] = new List<string>
                    {
                        ".png"
                    },
                    ["jpg"]=new List<string>
                    {
                        ".jpg"
                    }
                }
                };
                StorageFile saveFile = await savePicker.PickSaveFileAsync();
                if (saveFile != null)
                {
                    var type = saveFile.FileType == ".png" ? CanvasBitmapFileFormat.Png : CanvasBitmapFileFormat.Jpeg;
                    using (IRandomAccessStream stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await effectImage.SaveAsync(stream, type);
                        await stream.FlushAsync();
                    }
                }
            };

            thresholseSlide.ValueChanged += (s, e) =>
            {
                GetThresholdse();
                DrawBlackOrWhiteTarget();
            };

            colorSlide.ValueChanged += (s, e) =>
            {
                GetColorThresholdse();
            };
        }

        void GetThresholdse()
        {
            thresholse = (float)thresholseSlide.Value;
        }

        void GetColorThresholdse()
        {
            colorThresholdse = colorSlide.Value;
        }
        private async void InitCanvas()
        {
            blackOrWhiteCode = await new ShaderUri(ShaderType.BlackOrWhite).LoadAsync();
            GetThresholdse();
            originalCanvas.Draw += (s, e) =>
            {
                if (originalTarget is null)
                    return;
                var te = EvalImageCentered(originalCanvas.ActualWidth, originalCanvas.ActualHeight, imageWidth, imageHeight);
                te.Source = originalTarget;
                e.DrawingSession.DrawImage(te);
            };
            bool pressed = false;
            Point pos = new Point();
            float scale = 1f;
            effectCanvas.Draw += (s, e) =>
            {
                if (effectImage is null)
                    return;

                var mat= EvalCenterdTransform(originalCanvas.ActualWidth, originalCanvas.ActualHeight, imageWidth, imageHeight);
                var te = new Transform2DEffect()
                {
                    Source = effectImage,
                    TransformMatrix = mat,
                };
                e.DrawingSession.DrawImage(te);
                //if (pressed)
                //{
                   


                //    var p = Vector2.Transform(pos.ToVector2(), mat);
                    
                //    int x = (int)Math.Round(p.X);
                //    int y = (int)Math.Round(p.Y);

                    

                //    if (x > imageWidth - 1 || y > imageHeight - 1)
                //        return;
                //    var data = distanceDatas[x, y];
                //    if (data.RecentItem is null)
                //        return;
                  
                //    var startPosition = new Vector2(x, y);
                //    var endPosition = new Vector2(data.Recent.X * imageWidth, data.Recent.Y * imageHeight);
                //    e.DrawingSession.DrawLine(startPosition, endPosition, Colors.Blue, 2f);
                //}
            };

            //弃用
            effectCanvas.PointerWheelChanged += (s, e) =>
            {
                //FrameworkElement element = (FrameworkElement)s;
                //var dir = e.GetCurrentPoint(element).Properties.MouseWheelDelta;
                ////缩放
                //if (e.KeyModifiers == Windows.System.VirtualKeyModifiers.Control)
                //{
                //    scale += dir > 0 ? 0.1f : -0.1f;
                //}
                //effectCanvas.Invalidate();
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



        }

        void GenarateDirectionDistanceEffect()
        {
            var sourceColor = originalTarget.GetPixelColors();
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
            for (int x = 0; x < imageWidth; x++)
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
            for (int x = imageWidth - 1; x > 0; x--)
            {
                for (int y = 0; y < imageHeight; y++)
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
                        targetColor[index] = Colors.Black;
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
                        Color color = ShaderFunction.Lerp(Colors.Gray, Colors.White, ShaderFunction.Smoothstep(0, distance, item.Distance));

                        targetColor[index] = color;
                    }
                }
            }
            effectImage = CanvasBitmap.CreateFromColors(effectCanvas, targetColor, imageWidth, imageHeight);
            effectCanvas.Invalidate();
        }

        void GenerateSDF()
        {
            var sourceColor = originalTarget.GetPixelColors();
            var targetColor = new Color[sourceColor.Length];
            distance = disSlider.Value / 100d;

            SDFData[,] grid1 = new SDFData[imageWidth, imageHeight], grid2 = new SDFData[imageWidth, imageHeight];

            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    var index = x+imageWidth*y;
                    var col = sourceColor[index];
                    if (col.G < 128)
                    {
                        grid1[x, y] = new SDFData(0, 0);
                        grid2[x, y] = new SDFData(9999, 9999);
                    }
                    else
                    {
                        grid1[x, y] = new SDFData(9999, 9999);
                        grid2[x, y] = new SDFData(0, 0);
                    }
                }
            }

            GenerateSDF(ref grid1);
            GenerateSDF(ref grid2);
            int d =(int) disSlider.Value ;

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    int dist1 = (int)(Math.Sqrt((double)grid1[x, y].DistSq()));
                    int dist2 = (int)(Math.Sqrt((double)grid2[x, y].DistSq()));
                    int dist = dist1 - dist2;
                    int c = dist*d + (int)colorThresholdse;
                    if (c < 0) c = 0;
                    if(c > 255) c = 255;
                    targetColor[x + y * imageWidth] = Color.FromArgb(255, (byte)c, (byte)c, (byte)c);
                }
            }

            effectImage = CanvasBitmap.CreateFromColors(effectCanvas, targetColor, imageWidth, imageHeight);
            effectCanvas.Invalidate();
        }

        void GenerateSDF(ref SDFData[,] datas)
        {
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    Compare(ref datas, ref datas[x, y], x, y, -1, 0);
                    Compare(ref datas, ref datas[x, y], x, y, 0, -1);
                    Compare(ref datas, ref datas[x, y], x, y, -1, -1);
                    Compare(ref datas, ref datas[x, y], x, y, 1, 1);
                }

                for (int x= imageWidth - 1; x >= 0; x--)
                {
                    Compare(ref datas, ref datas[x, y], x, y, 1, 0);

                }
            }

            for (int y = imageHeight-1;y>= 0; y--)
            {
                for (int x = imageWidth-1;x>=0; x--)
                {
          
                    Compare(ref datas, ref datas[x, y], x, y, 1, 0);
                    Compare(ref datas, ref datas[x, y], x, y, 0, 1);
                    Compare(ref datas, ref datas[x, y], x, y, -1, 1);
                    Compare(ref datas,ref datas[x, y], x, y, 1, 1);
 
                }
                for (int x = imageWidth - 1; x >= 0; x--)
                {
                    Compare(ref datas, ref datas[x, y], x, y, -1, 0);
                }
            }
        }

        void Compare(ref SDFData[,] datas,ref SDFData data,int x,int y,int ox,int oy)
        {

            var other = Get(ref datas, x+ox, y+oy);
            other.Dx += ox;
            other.Dy += oy;
            if (other.DistSq() < data.DistSq())
                datas[x, y] = other;
        }

        SDFData Get(ref SDFData[,] datas,int x,int y)
        {
            if (x >= 0 && y >= 0 && x < imageWidth && y < imageHeight)
                return datas[x, y];
            return new SDFData(9999, 9999);
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
            var file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
            if (file != null)
                originalImage = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), await file.OpenReadAsync());
            originalTarget = new CanvasRenderTarget(originalCanvas, originalImage.Size);
            DrawBlackOrWhiteTarget();
            imageWidth = (int)originalImage.SizeInPixels.Width;
            imageHeight = (int)originalImage.SizeInPixels.Height;
            distanceDatas = new DistanceData[imageWidth, imageHeight];

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

        async void DrawBlackOrWhiteTarget()
        {
            if (originalTarget is null)
                return;
            if (blackOrWhiteCode is null)
            {
                blackOrWhiteCode = await new ShaderUri(ShaderType.BlackOrWhite).LoadAsync();

            }
            PixelShaderEffect pse = new PixelShaderEffect(blackOrWhiteCode)
            {
                Source1 = originalImage,
                Properties = { ["threshold"] = thresholse }
            };
            using (var ds = originalTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(pse);
            }
            originalCanvas.Invalidate();
        }


        public static Transform2DEffect EvalImageCentered(double canvasWidth, double canvasHeight, double imageWidth, double imageHeight)
        {
            var matrix3X2 = EvalCenterdTransform(canvasWidth, canvasHeight, imageWidth, imageHeight);
            Transform2DEffect te = new Transform2DEffect()
            {
                TransformMatrix = matrix3X2
            };
            return te;
        }

        public static Matrix3x2 EvalCenterdTransform(double canvasWidth, double canvasHeight, double imageWidth, double imageHeight)
        {
            float f = (float)Math.Min(canvasWidth / imageWidth, canvasHeight / imageHeight);
            float ox = (float)(canvasWidth - imageWidth * f) / 2;
            float oy = (float)(canvasHeight - imageHeight * f) / 2;
            Matrix3x2 matrix3X2 = Matrix3x2.CreateScale(f) * Matrix3x2.CreateTranslation(ox, oy);
            return matrix3X2;
        }
    }

    public struct SDFData
    {
        public int Dx;
        public int Dy;

        public SDFData(int x,int y)
        {
            Dx = x;
            Dy = y;
        }

        public int DistSq()
        {
            return Dx * Dx + Dy * Dy;
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
