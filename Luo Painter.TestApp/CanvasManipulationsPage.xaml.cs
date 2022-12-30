using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.TestApp
{
    public sealed partial class CanvasManipulationsPage : Page
    {

        public float Width2;
        public float Height2;
        public Vector2 Center2;

        public float Scale2 = 1;
        public float Rotation2;
        public Vector2 Translation2;

        Matrix3x2 Matrix = Matrix3x2.Identity;
        CanvasBitmap CanvasBitmap;

        public CanvasManipulationsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructCanvasManipulations();
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CanvasBitmap,
                    TransformMatrix = this.Matrix
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.CanvasBitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, "Assets\\Square150x150Logo.scale-200.png");
            this.Width2 = (float)this.CanvasBitmap.Size.Width;
            this.Height2 = (float)this.CanvasBitmap.Size.Height;
            this.Center2.X = this.Width2 / 2;
            this.Center2.Y = this.Height2 / 2;
        }

        private void ConstructCanvasManipulations()
        {
            this.CanvasControl.ManipulationMode = ManipulationModes.All;
            this.CanvasControl.ManipulationStarted += (s, e) => { };
            this.CanvasControl.ManipulationDelta += (s, e) =>
            {
                this.Scale2 *= e.Delta.Scale;
                this.Rotation2 += e.Delta.Rotation * (float)Math.PI / 180;
                this.Translation2 += e.Delta.Translation.ToVector2();

                this.Matrix =
                   Matrix3x2.CreateTranslation(-this.Center2) *
                    Matrix3x2.CreateScale(this.Scale2) *
                    Matrix3x2.CreateRotation(this.Rotation2) *
                    Matrix3x2.CreateTranslation(this.Translation2) *
                    Matrix3x2.CreateTranslation(this.Center2);

                this.CanvasControl.Invalidate();
            };
            this.CanvasControl.ManipulationCompleted += (s, e) => { };

            this.ClearButton.Click += (s, e) =>
            {
                this.Width2 = default;
                this.Height2 = default;
                this.Center2 = default;

                this.Scale2 = 1;
                this.Rotation2 = default;
                this.Translation2 = default;

                this.Matrix = Matrix3x2.Identity;

                this.CanvasControl.Invalidate();
            };
        }

    }
}