using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
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

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));


        Historian<IHistory> History { get; } = new Historian<IHistory>();
        ObservableCollection<ILayer> ObservableCollection { get; } = new ObservableCollection<ILayer>();


        public DrawPage()
        {
            this.InitializeComponent();

            this.ConstructTools();
            this.ConstructBlends();
            this.ConstructPaint();

            this.ConstructHistory();

            this.ConstructDialog();
            this.ConstructRadian();
            this.ConstructScale();

            this.ConstructStoryboard();
            this.ConstructSplitStoryboard();

            this.ConstructTip();
            this.ConstructColor();
            this.ConstructColorShape();


            this.ApplicationView.Title = "*Untitled";


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                int count = 0;

                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    foreach (IStorageItem item in items)
                    {
                        if (item is IStorageFile file)
                        {
                            bool? result = await this.AddAsync(file);
                            if (result == true) count++;
                        }
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }

                if (count > 1) this.Tip("Add Images", $"{count}"); // Tip
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };
        }
    }
}