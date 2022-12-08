using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    internal sealed class WheelImageSource : CanvasImageSourceExtensions
    {
        readonly WheelSizeF W;
        readonly float StrokeWidth;
        public WheelImageSource(WheelSizeF size) : base(size.D, size.D)
        {
            this.W = size;
            this.StrokeWidth = size.D / 100;
            this.Draw();
        }
        public void SurfaceContentsLost(object sender, object e) => base.Redraw();
        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.White))
            {
                for (int i = 0; i < 360; i++)
                {
                    double radians = Math.PI * i / 180;

                    float cos = (float)Math.Cos(radians);
                    float sin = (float)Math.Sin(radians);

                    ds.DrawLine(this.W.XY1(cos), this.W.XY1(sin), this.W.XY2(cos), this.W.XY2(sin), HSVExtensions.ToColor(i), this.StrokeWidth);
                }
            }
        }
    }

    public sealed partial class CanvasImageSourcePage : Page
    {
        readonly WheelImageSource WheelImageSource = new WheelImageSource(new WheelSizeF(320, 0.87f));
    
        public CanvasImageSourcePage()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.WheelImageSource.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.WheelImageSource.SurfaceContentsLost;

            this.AddButton.Click += (s, e) => this.TricolorPicker.ZoomOut();
            this.RemoveButton.Click += (s, e) => this.TricolorPicker.ZoomIn();

            this.LeftButton.Click += (s, e) => this.TricolorPicker.Left();
            this.RightButton.Click += (s, e) => this.TricolorPicker.Right();

            this.DownButton.Click += (s, e) => this.TricolorPicker.Down();
            this.UpButton.Click += (s, e) => this.TricolorPicker.Up();

            this.TricolorPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color = e;
            this.TricolorPicker.PointerWheelChanged += (s, e) =>
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this.TricolorPicker);

                float space = pointerPoint.Properties.MouseWheelDelta;

                if (space > 0)
                {
                    this.TricolorPicker.ZoomOut();
                }
                else if (space < 0)
                {
                    this.TricolorPicker.ZoomIn();
                }
            };
        }
    }
}