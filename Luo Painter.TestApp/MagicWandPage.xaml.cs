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

namespace Luo_Painter.TestApp
{
    public sealed partial class MagicWandPage : Page
    {
        CanvasBitmap originalImage;
        CanvasBitmap effectImage;
        Color[] originalColors;
        Color[] effectColors;

        int imageWidth;
        int imageHeight;

        byte[] dottedLineCode;
        byte[] colorMatchCode;
        PixelShaderEffect pse;


        CanvasRenderTarget colorMatchTarget;
        CanvasRenderTarget matchTarget;
        CanvasRenderTarget effectTarget;


        /// <summary>
        /// 用于记录已近检索的像素
        /// </summary>
        Dictionary<Pos, int> tempDir = new Dictionary<Pos, int>();

        Vector2 offset = new Vector2();
        float scale = 1f;
        float radin = 0f;
        public MagicWandPage()
        {
            this.InitializeComponent();
            Init();
        }

        async void Init()
        {

            effectCanvas.CreateResources += (s, e) =>
            {
                effectTarget = new CanvasRenderTarget(effectCanvas, new Size(1920, 1080));
            };
            dottedLineCode = await ShaderType.DottedLine.LoadAsync();
            colorMatchCode = await ShaderType.ColorMatch.LoadAsync();

            this.Unloaded += (s, e) =>
            {
                effectCanvas = null;
                originalCanvas = null;
            };
            selectPicture.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file != null)
                {
                    originalImage = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), await file.OpenReadAsync());
                    imageWidth = (int)originalImage.Size.Width;
                    imageHeight = (int)originalImage.Size.Height;
                    originalColors = originalImage.GetPixelColors();
                    effectColors = new Color[originalColors.Length];
                    colorMatchTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), imageWidth, imageHeight, 96f);
                    effectImage = CanvasBitmap.CreateFromColors(effectCanvas, effectColors, (int)originalImage.Size.Width, (int)originalImage.Size.Height, 96f);
                    originalCanvas.Invalidate();
                }
            };
            //原图画布
            originalCanvas.Draw += (s, e) =>
            {
                if (originalImage == null)
                    return;
                var m32 = new Matrix3x2();
                m32 = Matrix3x2.CreateScale(scale) * Matrix3x2.CreateRotation(radin, new Vector2(0.5f, 0.5f)) * Matrix3x2.CreateTranslation(offset);
                var te = new Transform2DEffect()
                {
                    Source = originalImage,
                    TransformMatrix = m32
                };
                e.DrawingSession.DrawImage(te);
            };
            //效果画布绘制
            effectCanvas.Draw += (s, e) =>
                {
                    if (effectImage == null || dottedLineCode == null)
                        return;
                    var m32 = new Matrix3x2();
                    m32 = Matrix3x2.CreateScale(scale) * Matrix3x2.CreateRotation(radin, new Vector2(0.5f, 0.5f)) * Matrix3x2.CreateTranslation(offset);

                    Transform2DEffect te = new Transform2DEffect()
                    {
                        Source = effectImage,
                        TransformMatrix = m32,
                    };
                    using (var ds = effectTarget.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        ds.DrawImage(te);
                    }

                    pse = new PixelShaderEffect(dottedLineCode)
                    {
                        Source1 = effectTarget,
                        Source1Interpolation = CanvasImageInterpolation.NearestNeighbor,
                        Properties = {
                            ["time"] =(float)e.Timing.TotalTime.TotalSeconds,
                            ["lineWidth"]=1f,
                            ["color1"]= new Vector3(0),
                            ["color2"]=new Vector3(1),
                            ["lineGap"]=100f,
                            ["lineSpeed"] =100f,
                            }
                    };
                    e.DrawingSession.DrawImage(pse);
                };

            PointerEventHandler OnCanvasMove = (s, e) =>
             {
                 var can = (FrameworkElement)s;
                 var point = e.GetCurrentPoint(can).Position;
                 Vector3 v3 = new Vector3((float)point.X - 15f, (float)point.Y - 15f, 0);
                 e1.Translation = e2.Translation = v3;
             };
            originalCanvas.PointerMoved += OnCanvasMove;
            effectCanvas.PointerMoved += OnCanvasMove;

            //点击事件
            TappedEventHandler CanvasTapped = (s, e) =>
            {
                if (originalImage == null || colorMatchCode == null)
                    return;
                var can = (FrameworkElement)s;
                var point = e.GetPosition(can);

                var m32 = new Matrix3x2();
                m32 = Matrix3x2.CreateScale(scale) * Matrix3x2.CreateRotation(radin, new Vector2(0.5f, 0.5f)) * Matrix3x2.CreateTranslation(offset);
               point = Vector2.Transform(point.ToVector2(), m32).ToPoint();
                if (point.X < 0 || point.Y < 0 || point.X > imageWidth || point.Y > imageHeight)
                    return;
                MagicWand(point);
                effectImage = CanvasBitmap.CreateFromColors(effectCanvas, effectColors, (int)originalImage.Size.Width, (int)originalImage.Size.Height, 96f);
                effectCanvas.Invalidate();
            };
            originalCanvas.Tapped += CanvasTapped;
            effectCanvas.Tapped += CanvasTapped;

            //滚轮事件
            PointerEventHandler WheelChanged = (s, e) =>
            {
                FrameworkElement element = (FrameworkElement)s;
                var dir = e.GetCurrentPoint(element).Properties.MouseWheelDelta;
                //缩放
                if (e.KeyModifiers == Windows.System.VirtualKeyModifiers.Control)
                {
                    scale += dir < 0 ? 0.1f : -0.1f;
                }
                //旋转
                else
                {
                    var angle = (float)(2 * Math.PI / 360);
                    radin += dir < 0 ? angle : -angle;
                }
                originalCanvas.Invalidate();
            };
            originalCanvas.PointerWheelChanged += WheelChanged;
            effectCanvas.PointerWheelChanged += WheelChanged;
            originalCanvas.ManipulationMode = effectCanvas.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            ManipulationDeltaEventHandler Detal = (s, e) =>
            {
                offset += e.Delta.Translation.ToVector2();
                originalCanvas.Invalidate();
            };

            originalCanvas.ManipulationDelta += Detal;
            effectCanvas.ManipulationDelta += Detal;
        }

        



        void MagicWand(Point point)
        {
            float threshold = (float)thresholdSlider.Value / 100f;
            Pos cp = new Pos((int)Math.Round(point.X), (int)Math.Round(point.Y));
            Color selectColor = originalColors[cp.X + cp.Y * imageWidth];

            using (var ds = colorMatchTarget.CreateDrawingSession())
            {
                var colorMatchEffect = new PixelShaderEffect(colorMatchCode)
                {
                    Source1 = originalImage,
                    Properties = {
                    ["threshold"] = threshold,
                    ["matchColor"] = new Vector4((float)selectColor.R/255,(float)selectColor.G/255,(float)selectColor.B/255,(float)selectColor.A/255)
                }
                };
                ds.Clear(Colors.Transparent);
                ds.DrawImage(colorMatchEffect);
            }
            var tempColor = colorMatchTarget.GetPixelColors();


            tempDir = new Dictionary<Pos, int>();
            tempDir.Add(cp, 1);
            List<Pos> retrieves = new List<Pos>();
            retrieves.Add(cp);
            while (retrieves.Count > 0)
            {
                var p = retrieves[0];
                retrieves.RemoveAt(0);
                var index = p.X + p.Y * imageWidth;
                Color currentColor = tempColor[index];
                if (currentColor.A > 0)
                {
                    effectColors[index] = originalColors[index];
                    AddRetrieves(p, retrieves);
                }
            }
        }

        void AddRetrieves(Pos cp, List<Pos> retrieves)
        {
            Action<int, int> AddRetrieves = (cx, cy) =>
            {
                if (cx < 0 || cy < 0)
                {
                    var i = 2;
                }

                Pos pos = new Pos(cx, cy);
                if (!tempDir.ContainsKey(pos))
                {
                    //添加到以检索队列中
                    tempDir.Add(pos, 1);
                    retrieves.Add(pos);
                }
            };

            var x = cp.X;
            var y = cp.Y;

            //上边
            if (y - 1 >= 0)
                AddRetrieves(x, y - 1);
            //下边
            if (y + 1 < imageHeight)
                AddRetrieves(x, y + 1);
            //左边
            if (x - 1 >= 0)
                AddRetrieves(x - 1, y);
            //左边
            if (x + 1 < imageWidth)
                AddRetrieves(x + 1, y);

            //for (int rx = -1; rx <= 1; rx++)
            //{
            //    for (int ry = -1; ry <= 1; ry++)
            //    {
            //        int x = cp.X + rx;
            //        int y = cp.Y + ry;
            //        if (x >= 0 && y >= 0 && x < imageWidth && y < imageHeight)
            //        {
            //            Pos pos = new Pos(x, y);
            //            if (!tempDir.ContainsKey(pos))
            //            {
            //                //添加到以检索队列中
            //                tempDir.Add(pos, 1);
            //                retrieves.Add(pos);
            //            }
            //        }
            //    }
            //}
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
