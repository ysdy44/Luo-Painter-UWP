using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Tools
{
    internal sealed class ElementIcon : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Content = value;
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class ElementItem : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

    internal sealed class ScaleRange
    {
        public readonly Range XRange;
        public readonly Range YRange = new Range
        {
            Default = 1,
            IP = new InverseProportion
            {
                Minimum = 0.1,
                Maximum = 10,
            }
        };

        public readonly InverseProportion XIP;
        public readonly InverseProportion YIP = new InverseProportion
        {
            Minimum = 0.3333333333333333333333333333333333333333333333333333333333333,
            Maximum = 1,
        };

        public ScaleRange()
        {
            this.XIP = this.YIP.Convert();
            this.XRange = this.YRange.Convert(this.YIP, this.YRange.IP, 1000);
        }

        public double ConvertXToY(double x) => InverseProportion.Convert(x, this.XIP, this.XRange.IP, this.YIP, this.YRange.IP, RangeRounding.Maximum, RangeRounding.Minimum);
        public double ConvertYToX(double y) => InverseProportion.Convert(y, this.YIP, this.YRange.IP, this.XIP, this.XRange.IP, RangeRounding.Minimum, RangeRounding.Maximum);
    }

    public sealed partial class ViewTool : StackPanel, ICase< OptionType>
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