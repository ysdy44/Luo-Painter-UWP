using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Tools
{
    internal sealed class ScaleRange : InverseProportionRange
    {
        public ScaleRange() : base(1, 0.1, 10, 100) { }
    }

    public sealed partial class ViewTool : StackPanel, ICase<OptionType>
    {
        //@Delegate
        public event EventHandler<float> RadianValueChanged;
        public event EventHandler<float> ScaleValueChanged;

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";
        private string PercentageConverter(double value) => $"{value * 100:0}";
        private string ScaleXToYConverter(double value) => this.PercentageConverter(this.ScaleRange.ConvertXToY(value));

        //@Content
        public RemoteControl RemoteControl => this.RemoteControlCore;

        //@Interface
        public object Content => this;
        public OptionType Value => OptionType.View;

        bool IsEnable;

        //@Construct
        public ViewTool()
        {
            this.InitializeComponent();

            this.RadianClearButton.Tapped += (s, e) => this.RadianStoryboard.Begin(); // Storyboard
            this.RadianSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double radian = e.NewValue / 180 * System.Math.PI;
                this.RadianValueChanged?.Invoke(this, (float)radian); // Delegate
            };

            this.ScaleClearButton.Tapped += (s, e) => this.ScaleStoryboard.Begin(); // Storyboard
            this.ScaleSlider.Minimum = this.ScaleRange.XRange.Minimum;
            this.ScaleSlider.Maximum = this.ScaleRange.XRange.Maximum;
            this.ScaleSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double scale = this.ScaleRange.ConvertXToY(e.NewValue);
                this.ScaleValueChanged?.Invoke(this, (float)scale); // Delegate
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Construct(CanvasTransformer transformer)
        {
            this.IsEnable = false;
            this.RadianSlider.Value = transformer.Radian * 180 / System.Math.PI;
            this.ScaleSlider.Value = this.ScaleRange.ConvertYToX(transformer.Scale);
            this.IsEnable = true;
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

    }
}