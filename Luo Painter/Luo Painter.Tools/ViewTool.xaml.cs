using FanKit.Transformers;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Tools
{
    internal sealed class RadianRange
    {
        public Range Range { get; } = new Range
        {
            Default = 0,
            Minimum = -180,
            Maximum = 180,
        };
    }

    internal sealed class ScaleRange
    {
        public Range XRange { get; private set; }
        public Range YRange { get; } = new Range
        {
            Default = 1,
            Minimum = 0.1,
            Maximum = 10,
        };
        public InverseProportion InverseProportion { get; } = new InverseProportion
        {
            A = -1,
            B = 10,
            C = 1,
        };
        public ScaleRange() => this.XRange = this.InverseProportion.ConvertYToX(this.YRange);
    }

    public sealed partial class ViewTool : UserControl
    {
        //@Delegate
        public event EventHandler<float> RadianValueChanged;
        public event EventHandler<float> ScaleValueChanged;

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";
        private string PercentageConverter(double value) => $"{value * 100:0}";
        private string ScaleXToYConverter(double value) => this.PercentageConverter(this.ScaleRange.InverseProportion.ConvertXToY(value));

        //@Content
        public RemoteControl RemoteControl => this.RemoteControlCore;

        //@Construct
        public ViewTool()
        {
            this.InitializeComponent();

            this.RadianClearButton.Tapped += (s, e) => this.RadianStoryboard.Begin(); // Storyboard
            this.RadianSlider.ValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * System.Math.PI;
                this.RadianValueChanged?.Invoke(this, (float)radian); // Delegate
            };

            this.ScaleClearButton.Tapped += (s, e) => this.ScaleStoryboard.Begin(); // Storyboard
            this.ScaleSlider.ValueChanged += (s, e) =>
            {
                double scale = this.ScaleRange.InverseProportion.ConvertXToY(e.NewValue);
                this.ScaleValueChanged?.Invoke(this, (float)scale); // Delegate
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Construct(CanvasTransformer transformer)
        {
            this.RadianSlider.Value = transformer.Radian * 180 / System.Math.PI;
            this.ScaleSlider.Value = this.ScaleRange.InverseProportion.ConvertYToX(transformer.Scale);
        }

    }
}