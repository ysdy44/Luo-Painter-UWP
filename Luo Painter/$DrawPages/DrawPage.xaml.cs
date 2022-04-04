using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal sealed class ToolIcon : ContentPresenter
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="ToolIcon"/>. </summary>
        public ToolType Type
        {
            get => (ToolType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(ToolType), typeof(ToolIcon), new PropertyMetadata(ToolType.None, (sender, e) =>
        {
            ToolIcon control = (ToolIcon)sender;

            if (e.NewValue is ToolType value)
            {
                control.Content = new ContentControl
                {
                    Content = value,
                    Template = value.GetTemplate(out ResourceDictionary resource),
                    Resources = resource,
                };
                ToolTipService.SetToolTip(control, new ToolTip
                {
                    Content = value,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            }
        }));


        #endregion
    }

    internal sealed class BlendIcon : ContentPresenter
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="ToolIcon"/>. </summary>
        public BlendEffectMode Type
        {
            get => (BlendEffectMode)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "BlendIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(BlendEffectMode), typeof(BlendIcon), new PropertyMetadata(BlendEffectMode.Multiply, (sender, e) =>
        {
            BlendIcon control = (BlendIcon)sender;

            if (e.NewValue is BlendEffectMode value)
            {
                control.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new ContentControl
                        {
                            Content = value,
                            Template = value.GetTemplate(out ResourceDictionary resource, out string title),
                            Resources = resource,
                        },
                        new ContentControl
                        {
                            Width = 12
                        },
                        new TextBlock
                        {
                            Text = title
                        }
                    }
                };
            }
        }));


        #endregion
    }


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


    public sealed partial class DrawPage : Page
    {

        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private string RoundConverter(double value) => $"{(value):0}";
        private string Round2Converter(double value) => $"{(value):0.00}";

        //@Converter
        private string ScaleXToYConverter(double value) => this.Round2Converter(this.ScaleRange.InverseProportion.ConvertXToY(value));


        Historian<IHistory> History { get; } = new Historian<IHistory>();
        ObservableCollection<ILayer> ObservableCollection { get; } = new ObservableCollection<ILayer>();


        public DrawPage()
        {
            this.InitializeComponent();

            this.ConstructTip();
            this.ConstructColor();
            this.ConstructColorShape();
        }
    }
}