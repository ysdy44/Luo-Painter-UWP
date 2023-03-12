using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PenPreviewPage : Page
    {

        //@Converter
        private float ToPressure(float value)
        {
            if (value > 0) return 1 + value;
            if (value < 0) return 1 / (1 - value);
            return 1;
        }

        AnchorCollection Anchors;

        public PenPreviewPage()
        {
            this.InitializeComponent();
            this.ConstructPenPreview();
            this.ConstructCanvas();
        }

        private void ValueChanged(int index, float value)
        {
            if (this.Anchors is null) return;

            this.Anchors[index].Pressure = this.ToPressure(value);
            this.Anchors.Segment(this.CanvasControl);
            this.Anchors.Invalidate();
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void ConstructPenPreview()
        {
            this.ResetPressure0Button.Click += (s, e) => this.Pressure0Slider.Value = -4;
            this.Pressure0Slider.ValueChanged += (s, e) => this.ValueChanged(0, (float)e.NewValue);

            this.ResetPressure1Button.Click += (s, e) => this.Pressure1Slider.Value = 0;
            this.Pressure1Slider.ValueChanged += (s, e) => this.ValueChanged(1, (float)e.NewValue);

            this.ResetPressure2Button.Click += (s, e) => this.Pressure2Slider.Value = 0;
            this.Pressure2Slider.ValueChanged += (s, e) => this.ValueChanged(2, (float)e.NewValue);

            this.ResetPressure3Button.Click += (s, e) => this.Pressure3Slider.Value = -4;
            this.Pressure3Slider.ValueChanged += (s, e) => this.ValueChanged(3, (float)e.NewValue);
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Anchors = new AnchorCollection(this.CanvasControl, 650, 200)
                {
                    new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(100, 80), Pressure = 0.2f },
                    new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(250, 120) },
                    new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(400, 120) },
                    new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(550, 80), Pressure = 0.2f },
                };

                this.Anchors.IsClosed = true;
                this.Anchors.Color = Colors.DodgerBlue;
                this.Anchors.StrokeWidth = 22;

                this.Anchors.Segment(sender);
                this.Anchors.Invalidate();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(this.Anchors.Source);
                args.DrawingSession.DrawAnchorCollection(this.Anchors);
            };
        }

    }
}