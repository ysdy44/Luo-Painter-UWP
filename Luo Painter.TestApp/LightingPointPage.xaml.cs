using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class LightingPointPage : Page
    {

        CanvasBitmap BitmapTiger;
        Vector2 Size2;

        LuminanceToAlphaEffect HeightMap;
        PointDiffuseEffect Diffuse;
        PointSpecularEffect Specular;

        Vector2 Position2 = new Vector2(1, 0) * 100;
        Vector3 Position = new Vector3(1, 0, 1) * 100;

        Vector2 Target2 = new Vector2(100, 100);
        Vector3 Target = new Vector3(100, 100, 0);

        float Ambient = 0;

        public LightingPointPage()
        {
            this.InitializeComponent();
            this.ConstructLighting();
            this.ConstructCanvas();
        }

        public void ConstructLighting()
        {
            this.AmbientSlider.ValueChanged += (s, e) =>
            {
                this.Ambient = (float)e.NewValue / 100;
            };
        }

        public void ConstructCanvas()
        {
            //Canvas
            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                args.TrackAsyncAction(this.CreateResourcesAsync(sender).AsAsyncAction());
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(this.Specular, 0, this.Size2.Y);
                args.DrawingSession.DrawImage(this.Diffuse, this.Size2.X, 0);

                ICanvasImage source1 = new ArithmeticCompositeEffect
                {
                    Source1 = this.BitmapTiger,
                    Source2 = this.Diffuse,
                    Source1Amount = this.Ambient,
                    Source2Amount = 0,
                    MultiplyAmount = 1 - this.Ambient,
                };
                ICanvasImage effect = new ArithmeticCompositeEffect
                {
                    Source1 = source1,
                    Source2 = this.Specular,
                    Source1Amount = 1,
                    Source2Amount = 1,
                    MultiplyAmount = 0,
                };

                args.DrawingSession.DrawImage(effect, this.Size2);

                args.DrawingSession.DrawRectangle(0, 0, this.Size2.X, this.Size2.Y, Colors.Red);

                args.DrawingSession.DrawLine(this.Target2, this.Position2 + this.Target2, Colors.Red);
                args.DrawingSession.FillCircle(this.Position2 + this.Target2, 4, Colors.Red);
                args.DrawingSession.FillCircle(this.Target2, 4, Colors.Red);
            };
            this.CanvasAnimatedControl.Update += (sender, args) =>
            {
                float elapsedTime = (float)args.Timing.TotalTime.TotalSeconds;

                this.Position2 = new Vector2((float)Math.Cos(elapsedTime), (float)Math.Sin(elapsedTime)) * 100;
                this.Position = new Vector3(this.Position2, 100);
                this.Diffuse.LightPosition = this.Position + this.Target;
                this.Specular.LightPosition = this.Position + this.Target;
            };
        }

        private async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            this.BitmapTiger = await CanvasBitmap.LoadAsync(sender, "Assets\\Square150x150Logo.scale-200.png");
            this.Size2 = this.BitmapTiger.Size.ToVector2();

            this.Target2 = this.Size2 / 2;
            this.Target = new Vector3(this.Target2, 0);

            this.HeightMap = new LuminanceToAlphaEffect
            {
                Source = this.BitmapTiger
            };

            this.Diffuse = new PointDiffuseEffect
            {
                Source = this.HeightMap,

                HeightMapScale = 2,

                LightPosition = this.Position + this.Target,
            };

            this.Specular = new PointSpecularEffect
            {
                Source = this.HeightMap,

                SpecularExponent = 16,

                LightPosition = this.Position + this.Target,
            };
        }

    }
}