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
    public sealed partial class BrushShaderPage : Page
    {

        private bool IsPointerPressed;

        public byte[] generalCodeBytes { get; private set; }
        public byte[] sparyGunCodeBytes { get; private set; }
        CanvasRenderTarget brushRender;
        //绘制位置
        Vector2 drawPos;

        float brushSize = 100;
        public BrushShaderPage()
        {
            this.InitializeComponent();

            ConstructOperator();
        }

        private async Task CreateResourcesAsync()
        {
            this.generalCodeBytes = await ShaderType.GeneralBrush.LoadAsync();
            this.sparyGunCodeBytes = await ShaderType.SprayGun.LoadAsync();
            brushRender = new CanvasRenderTarget(canvasControl, new Size(1000, 1000));
        }

        void DrawBrush()
        {
            if (IsPointerPressed)
            {
                var color = ColorPicker.Color;
                var col = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
                if (general.IsChecked == true)
                {
                    using (var ds = brushRender.CreateDrawingSession())
                    {
                        var effect = new PixelShaderEffect(generalCodeBytes)
                        {
                            Properties = { ["size"] = brushSize, ["color"] = col }
                        };
                        var render = new CanvasRenderTarget(canvasControl, new Size(brushSize, brushSize));
                        using (var draw = render.CreateDrawingSession())
                        {
                            draw.Clear(Colors.Transparent);

                            draw.DrawImage(effect);
                        }
                        ds.DrawImage(render, drawPos);
                    }
                }
                else if (sprayGun.IsChecked == true)
                {

                    using (var ds = brushRender.CreateDrawingSession())
                    {
                        var effect = new PixelShaderEffect(sparyGunCodeBytes)
                        {
                            Properties = { ["size"] = brushSize, ["color"] = col }
                        };
                        ds.DrawImage(effect, drawPos);
                    }
                }


                canvasControl.Invalidate();
            }
        }

        float Saturate(float x)
        {
            if (x < 0)
                return x;
            if (x > 1)
                return 1;
            return x;
        }

        float Smoothstep(float a, float b, float t)
        {
            var v = Saturate((t - a) / (b - a));
            return v * v * (3.0f - (2.0f * v));
        }

        void ConstructOperator()
        {



            //创建资源
            canvasControl.CreateResources += (s, e) =>
            {
                e.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };

            //指针按下
            canvasControl.PointerPressed += (s, e) =>
            {
                IsPointerPressed = !IsPointerPressed;
                DrawBrush();
            };

            //指针释放
            canvasControl.PointerReleased += (s, e) =>
            {
                IsPointerPressed = !IsPointerPressed;

            };

            //指针移动
            canvasControl.PointerMoved += (s, e) =>
            {
                drawPos = e.GetCurrentPoint(canvasControl).Position.ToVector2() - new Vector2(brushSize / 2);
                brush.Translation = new Vector3(drawPos, 0);
                DrawBrush();
            };

            canvasControl.PointerEntered += (s, e) =>
            {
                brush.Visibility = Visibility.Visible;
            };
            canvasControl.PointerExited += (s, e) =>
            {
                brush.Visibility = Visibility.Collapsed;
            };

            //绘制
            canvasControl.Draw += (s, e) =>
            {
                e.DrawingSession.DrawImage(brushRender);
            };
        }
    }
}
