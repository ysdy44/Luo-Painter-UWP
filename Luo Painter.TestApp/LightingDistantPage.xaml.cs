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
    public sealed partial class LightingDistantPage : Page
    {

        CanvasBitmap BitmapTiger;
        Vector2 Size2;

        LuminanceToAlphaEffect HeightMap;
        DistantDiffuseEffect Diffuse;
        DistantSpecularEffect Specular;

        float Azimuth;
        float Elevation;

        float Ambient = 0;

        public LightingDistantPage()
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

                args.DrawingSession.DrawText($"Azimuth: {(int)(this.Azimuth * 180 / MathF.PI)}", 0, 0, Colors.Red);
                args.DrawingSession.DrawText($"Elevation: {(int)(this.Elevation * 180 / MathF.PI)}", 0, this.Size2.Y / 2, Colors.Red);
            };
            this.CanvasAnimatedControl.Update += (sender, args) =>
            {
                var elapsedTime = (float)args.Timing.TotalTime.TotalSeconds;

                this.Azimuth = elapsedTime % (MathF.PI * 2);
                this.Elevation = MathF.Sin(elapsedTime / 2) * MathF.PI / 8 + MathF.PI / 4;

                this.Diffuse.Azimuth = this.Azimuth;
                this.Specular.Elevation = this.Elevation;
            };
        }

        private async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            this.BitmapTiger = await CanvasBitmap.LoadAsync(sender, "Assets\\Square150x150Logo.scale-200.png");
            this.Size2 = this.BitmapTiger.Size.ToVector2();

            this.HeightMap = new LuminanceToAlphaEffect
            {
                Source = this.BitmapTiger
            };

            this.Diffuse = new DistantDiffuseEffect
            {
                Source = this.HeightMap,

                HeightMapScale = 2
            };

            this.Specular = new DistantSpecularEffect
            {
                Source = this.HeightMap,

                SpecularExponent = 16,
            };
        }

    }
}