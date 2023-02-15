using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class NightSkyTimeLapsePage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        readonly Random Random = new Random(2134566);
        BitmapLayer BitmapLayer;
        BitmapLayer BitmapLayer1;
        BitmapLayer BitmapLayer2;
        BitmapLayer BitmapLayer3;
        BitmapLayer BitmapLayer4;

        Vector2 Position = new Vector2(512, 512);
        bool IsMoving;

        public NightSkyTimeLapsePage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 2048, 2048);
                this.Render(this.BitmapLayer, false);
                this.BitmapLayer1 = new BitmapLayer(this.Device, 2048, 2048);
                this.Render(this.BitmapLayer1, false);
                this.BitmapLayer2 = new BitmapLayer(this.Device, 2048, 2048);
                this.Render(this.BitmapLayer2, false);
                this.BitmapLayer3 = new BitmapLayer(this.Device, 2048, 2048);
                this.Render(this.BitmapLayer3, false);
                this.BitmapLayer4 = new BitmapLayer(this.Device, 2048, 2048);
                this.Render(this.BitmapLayer4, false);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                if (this.IsMoving)
                {
                    args.DrawingSession.FillCircle(this.Position, 12, Colors.White);
                }
                else
                {
                    args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
                    args.DrawingSession.DrawImage(new OpacityEffect
                    {
                        Opacity = 0.6f,
                        Source = this.BitmapLayer1[BitmapType.Source]
                    });
                    args.DrawingSession.DrawImage(new OpacityEffect
                    {
                        Opacity = 0.55f,
                        Source = this.BitmapLayer2[BitmapType.Source]
                    });
                    args.DrawingSession.DrawImage(new OpacityEffect
                    {
                        Opacity = 0.5f,
                        Source = this.BitmapLayer3[BitmapType.Source]
                    });
                    args.DrawingSession.DrawImage(new OpacityEffect
                    {
                        Opacity = 0.45f,
                        Source = this.BitmapLayer4[BitmapType.Source]
                    });
                }

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Temp]);
                args.DrawingSession.DrawImage(new OpacityEffect
                {
                    Opacity = 0.6f,
                    Source = this.BitmapLayer1[BitmapType.Temp]
                });
                args.DrawingSession.DrawImage(new OpacityEffect
                {
                    Opacity = 0.55f,
                    Source = this.BitmapLayer2[BitmapType.Temp]
                });
                args.DrawingSession.DrawImage(new OpacityEffect
                {
                    Opacity = 0.5f,
                    Source = this.BitmapLayer1[BitmapType.Temp]
                });
                args.DrawingSession.DrawImage(new OpacityEffect
                {
                    Opacity = 0.45f,
                    Source = this.BitmapLayer2[BitmapType.Temp]
                });
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.Black);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Origin],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 2048)
                });
            };

            this.SourceCanvasControl.UseSharedDevice = true;
            this.SourceCanvasControl.CustomDevice = this.Device;
            this.SourceCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.Black);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Source],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 2048)
                });
            };

            this.TempCanvasControl.UseSharedDevice = true;
            this.TempCanvasControl.CustomDevice = this.Device;
            this.TempCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.Black);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Temp],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 2048)
                });
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.IsMoving = true;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Render(this.BitmapLayer, true);
                this.Render(this.BitmapLayer1, true);
                this.Render(this.BitmapLayer2, true);
                this.Render(this.BitmapLayer3, true);
                this.Render(this.BitmapLayer4, true);
                this.IsMoving = false;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void Render(BitmapLayer bitmapLayer, bool isClear, int stars = 22, int pow = 16, float sweepAngle = 90)
        {
            if (isClear)
            {
                bitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                bitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                bitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
            }

            using (CanvasDrawingSession ds1 = bitmapLayer.CreateDrawingSession(BitmapType.Origin))
            using (CanvasDrawingSession ds2 = bitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                //@DPI 
                ds1.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds1.Blend = CanvasBlend.Copy;

                //@DPI 
                ds2.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds2.Blend = CanvasBlend.Copy;

                for (int i = 0; i < stars; i++)
                {
                    Color color = Colors.White;
                    switch (this.Random.Next(0, 12) % 3)
                    {
                        case 0:
                            color.B = (byte)(255 - this.Random.Next(0, 16));
                            break;
                        case 1:
                            color.G = (byte)(255 - this.Random.Next(0, 16));
                            break;
                        default:
                            color.R = (byte)(255 - this.Random.Next(0, 16));
                            break;
                    }

                    int x = this.Random.Next(0, this.BitmapLayer.Width);
                    int y = this.Random.Next(0, this.BitmapLayer.Height);

                    ds1.FillCircle(x, y, 1, color);
                    ds2.FillCircle(x, y, 1, color);
                }
            }

            float angle = (float)(360 / System.Math.Pow(2, pow));

            do
            {
                using (CanvasDrawingSession ds = bitmapLayer.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    float radians = FanKit.Math.Pi * angle / 180;
                    ds.DrawImage(new Transform2DEffect
                    {
                        TransformMatrix = Matrix3x2.CreateRotation(radians, this.Position),
                        Source = bitmapLayer[BitmapType.Origin]
                    });
                }
                bitmapLayer.Flush();

                angle *= 2;
            } while (angle < sweepAngle);
        }

    }
}