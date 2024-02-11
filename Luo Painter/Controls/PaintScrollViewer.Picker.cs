﻿using Luo_Painter.HSVColorPickers;

namespace Luo_Painter.Controls
{
    internal enum PaintNumberPickerMode
    {
        None,

        Size,
        Opacity,
        Spacing,
        Flow,

        MinSize,
        MinFlow,

        Mix,
        Wet,
        Persistence,

        GrainScale,
    }

    partial class PaintScrollViewer
    {

        PaintNumberPickerMode NumberPickerMode;

        private void NumberShowAt(INumberBase number, PaintNumberPickerMode mode = default)
        {
            this.NumberPickerMode = mode;
            this.NumberFlyout.ShowAt(number.PlacementTarget);
            this.NumberPicker.Construct(number);
        }

        public void ConstructPicker()
        {
            this.SizeSlider.Click += (s, e) => this.NumberShowAt(this.SizeSlider, PaintNumberPickerMode.Size);
            this.OpacitySlider.Click += (s, e) => this.NumberShowAt(this.OpacitySlider, PaintNumberPickerMode.Opacity);
            this.SpacingSlider.Click += (s, e) => this.NumberShowAt(this.SpacingSlider, PaintNumberPickerMode.Spacing);
            this.FlowSlider.Click += (s, e) => this.NumberShowAt(this.FlowSlider, PaintNumberPickerMode.Flow);

            this.MinSizeSlider.Click += (s, e) => this.NumberShowAt(this.MinSizeSlider, PaintNumberPickerMode.MinSize);
            this.MinFlowSlider.Click += (s, e) => this.NumberShowAt(this.MinFlowSlider, PaintNumberPickerMode.MinFlow);

            this.MixSlider.Click += (s, e) => this.NumberShowAt(this.MixSlider, PaintNumberPickerMode.Mix);
            this.WetSlider.Click += (s, e) => this.NumberShowAt(this.WetSlider, PaintNumberPickerMode.Wet);
            this.PersistenceSlider.Click += (s, e) => this.NumberShowAt(this.PersistenceSlider, PaintNumberPickerMode.Persistence);

            this.GrainScaleSlider.Click += (s, e) => this.NumberShowAt(this.GrainScaleSlider, PaintNumberPickerMode.GrainScale);

            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();
            this.NumberPicker.ValueChanged += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                switch (this.NumberPickerMode)
                {
                    case PaintNumberPickerMode.None:
                        break;

                    case PaintNumberPickerMode.Size:
                        this.SizeSlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Opacity:
                        this.OpacitySlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Spacing:
                        this.SpacingSlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Flow:
                        this.FlowSlider.Value = e;
                        break;

                    case PaintNumberPickerMode.MinSize:
                        this.MinSizeSlider.Value = e;
                        break;
                    case PaintNumberPickerMode.MinFlow:
                        this.MinFlowSlider.Value = e;
                        break;

                    case PaintNumberPickerMode.Mix:
                        this.MixSlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Wet:
                        this.WetSlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Persistence:
                        this.PersistenceSlider.Value = e;
                        break;

                    case PaintNumberPickerMode.GrainScale:
                        this.InkPresenter.GrainScale = (float)e / 100;
                        this.TryInk();

                        this.GrainScaleSlider.Value = e;
                        break;

                    default:
                        break;
                }
            };
        }

    }
}