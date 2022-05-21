using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal enum DisplacementMapMode
    {
        Glassy,
        Clockwise,
        ZoomOut,
        ZoomIn,
    }

    internal static class DisplacementExtension
    {
        public static readonly Color ClearColor = Color.FromArgb(255, 127, 127, 255);
        public static readonly Color Left = Color.FromArgb(255, 255, 127, 255);
        public static readonly Color Right = Color.FromArgb(255, 0, 127, 255);
        public static readonly Color Up = Color.FromArgb(255, 127, 255, 255);
        public static readonly Color Down = Color.FromArgb(255, 127, 0, 255);
        public static Color ToColor(this Vector2 vectorUnit) // (-1, -1) ~ (1, 1)
        {
            return new Color
            {
                A = 255,
                R = (byte)(127 - 127 * vectorUnit.X),
                G = (byte)(127 - 127 * vectorUnit.Y),
                B = 255,
            };
        }

        public static CanvasRenderTarget GetZoomInBrush(this ICanvasResourceCreator sender)
        {
            CanvasRenderTarget brush = new CanvasRenderTarget(sender, 256, 256, 96);

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    Vector2 vector = new Vector2(x - 127, y - 127);
                    float length = vector.Length();
                    if (length < 127)
                    {
                        float opacity = 1 - length / 127;
                        Vector2 unit = opacity * vector / 127;
                        Color color = unit.ToColor();

                        brush.SetPixelColors(new Color[]
                        {
                            color
                        }, x, y, 1, 1);
                    }
                }
            }

            return brush;
        }

        public static CanvasRenderTarget GetZoomOutBrush(this ICanvasResourceCreator sender)
        {
            CanvasRenderTarget brush = new CanvasRenderTarget(sender, 256, 256, 96);

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    Vector2 vector = new Vector2(127 - x, 127 - y);
                    float length = vector.Length();
                    if (length < 127)
                    {
                        float opacity = 1 - length / 127;
                        Vector2 unit = opacity * vector / 127;
                        Color color = unit.ToColor();

                        brush.SetPixelColors(new Color[]
                        {
                            color
                        }, x, y, 1, 1);
                    }
                }
            }

            return brush;
        }

        public static CanvasRenderTarget GetClockwiseBrush(this ICanvasResourceCreator sender)
        {
            CanvasRenderTarget brush = new CanvasRenderTarget(sender, 256, 256, 96);

            // Clockwise 
            float radianOffset = (float)(Math.PI / 4);

            // CounterClockwise
            float radianOffset2 = -(float)(Math.PI / 4);

            float amount = 100; // 100 is DisplacementMapEffect's Amount

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    Vector2 vector = new Vector2(x - 127, y - 127);
                    float length = vector.Length();
                    if (length < 127)
                    {
                        float opacity = 1 - length / 127;
                        float radian = GetRadian(vector);
                        Vector2 vector2 = length * GetVector(radian + radianOffset * opacity);

                        Vector2 unit = (vector2 - vector) / amount;
                        Color color = unit.ToColor();

                        brush.SetPixelColors(new Color[]
                        {
                            color
                        }, x, y, 1, 1);
                    }
                }
            }

            return brush;
        }

        public static CanvasRenderTarget GetGlassyBrush(this ICanvasResourceCreator sender)
        {
            CanvasRenderTarget brush = new CanvasRenderTarget(sender, 256, 256, 96);

            float radianOffset = (float)(10f / 180f * Math.PI);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    Vector2 vector = new Vector2(x - 127, y - 127);
                    float length = vector.Length();
                    if (length < 127)
                    {
                        float opacity = 1 - length / 127;
                        float radian = GetRadian(vector);
                        Vector2 vector2 = GetVector(radian + radianOffset);

                        Vector2 unit = (vector2 - vector) * 100;
                        Color color = unit.ToColor();

                        brush.SetPixelColors(new Color[]
                        {
                            color
                        }, x, y, 1, 1);
                    }
                }
            }

            return brush;
        }


        public static float GetRadian(Vector2 vector) // 0 ~ 360
        {
            float tan = (float)Math.Atan(Math.Abs(vector.Y / vector.X));

            if (vector.X > 0 && vector.Y > 0) // 1
                return tan;
            else if (vector.X > 0 && vector.Y < 0) // 2
                return -tan;
            else if (vector.X < 0 && vector.Y > 0) // 3
                return (float)Math.PI - tan;
            else
                return tan - (float)Math.PI;
        }
        public static Vector2 GetVector(float radian) // (-1, -1) ~ (1, 1)
        {
            return new Vector2
            {
                X = (float)Math.Cos(radian),
                Y = (float)Math.Sin(radian),
            };
        }
    }

    public sealed partial class DisplacementMapPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;
        BitmapLayer BitmapLayer;

        IDictionary<DisplacementMapMode, CanvasRenderTarget> Brushs;

        Vector2 Position;

        float InkOpacity = 0.4f;

        public DisplacementMapPage()
        {
            this.InitializeComponent();
            this.ConstructDisplacementMap();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructDisplacementMap()
        {
            this.ModeListView.ItemsSource = System.Enum.GetValues(typeof(DisplacementMapMode));
            this.ModeListView.SelectedIndex = 0;

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                this.BitmapLayer.DrawCopy(this.CanvasBitmap);
                this.BitmapLayer.Flush();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)e.NewValue;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                this.BitmapLayer.DrawCopy(this.CanvasBitmap);
                this.BitmapLayer.Flush();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Brushs = new Dictionary<DisplacementMapMode, CanvasRenderTarget>
                {
                    [DisplacementMapMode.Glassy] = DisplacementExtension.GetGlassyBrush(sender),
                    [DisplacementMapMode.Clockwise] = DisplacementExtension.GetClockwiseBrush(sender),
                    [DisplacementMapMode.ZoomOut] = DisplacementExtension.GetZoomOutBrush(sender),
                    [DisplacementMapMode.ZoomIn] = DisplacementExtension.GetZoomInBrush(sender),
                };
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.White);

                args.DrawingSession.DrawImage(new CropEffect
                {
                    SourceRectangle = this.BitmapLayer.Bounds.ToRect(),
                    Source = new DisplacementMapEffect
                    {
                        Source = this.BitmapLayer.Source,
                        Displacement = this.BitmapLayer.Temp,
                        XChannelSelect = EffectChannelSelect.Red,
                        YChannelSelect = EffectChannelSelect.Green,
                        Amount = 100,
                    }
                });
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Origin,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.SourceCanvasControl.UseSharedDevice = true;
            this.SourceCanvasControl.CustomDevice = this.Device;
            this.SourceCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Source,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.TempCanvasControl.UseSharedDevice = true;
            this.TempCanvasControl.CustomDevice = this.Device;
            this.TempCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Temp,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                float pressure = properties.Pressure;

                Vector2 vector = position - this.Position;
                float length = vector.Length();
                if (length < 2) return;

                Vector2 unit = vector / length;
                Color color = unit.ToColor();

                CanvasRenderTarget brush = this.Brushs[(DisplacementMapMode)this.ModeListView.SelectedIndex];
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                {
                    ds.DrawImage(brush, position.X - 127, position.Y - 127);
                }

                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.BitmapLayer.DrawCopy(new DisplacementMapEffect
                {
                    Source = this.BitmapLayer.Origin,
                    Displacement = this.BitmapLayer.Temp,
                    XChannelSelect = EffectChannelSelect.Red,
                    YChannelSelect = EffectChannelSelect.Green,
                    Amount = 100,
                });
                this.BitmapLayer.Flush();
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);

                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
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
}