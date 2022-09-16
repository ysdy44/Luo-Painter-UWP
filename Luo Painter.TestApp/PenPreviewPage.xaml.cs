using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
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

        readonly AnchorCollection Anchors = new AnchorCollection
        {
            new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(100, 80), Pressure = 0.2f },
            new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(250, 120) },
            new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(400, 120) },
            new Anchor{ IsSmooth = true, IsChecked = true, Point = new Vector2(550, 80), Pressure = 0.2f },
        };

        public PenPreviewPage()
        {
            this.InitializeComponent();
            this.ConstructPenPreview();
            this.ConstructCanvas();
        }

        private void ConstructPenPreview()
        {
            void valueChanged(int index, float value)
            {
                this.Anchors[index].Pressure = this.ToPressure(value);
                this.CanvasControl.Invalidate(); // Invalidate
            }

            this.ResetPressure0Button.Click += (s, e) => this.Pressure0Slider.Value = -4;
            this.Pressure0Slider.ValueChanged += (s, e) => valueChanged(0, (float)e.NewValue);

            this.ResetPressure1Button.Click += (s, e) => this.Pressure1Slider.Value = 0;
            this.Pressure1Slider.ValueChanged += (s, e) => valueChanged(1, (float)e.NewValue);

            this.ResetPressure2Button.Click += (s, e) => this.Pressure2Slider.Value = 0;
            this.Pressure2Slider.ValueChanged += (s, e) => valueChanged(2, (float)e.NewValue);

            this.ResetPressure3Button.Click += (s, e) => this.Pressure3Slider.Value = -4;
            this.Pressure3Slider.ValueChanged += (s, e) => valueChanged(3, (float)e.NewValue);
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Anchors.BuildGeometry(sender);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.FillAnchorCollection(this.Anchors, Colors.DodgerBlue, 5);
                args.DrawingSession.DrawAnchorCollection(this.Anchors);
            };
        }

    }
}