using System;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Elements;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructDialog()
        {
            this.SettingButton.Click += async (s, e) =>
            {
                float radian = this.Transformer.Radian;
                this.RadianSlider.Value = radian * 180 / Math.PI;

                float scale = this.Transformer.Scale;
                this.ScaleSlider.Value = this.ScaleRange.InverseProportion.ConvertYToX(scale);

                await this.SettingDislog.ShowInstance();
            };

            this.ExportButton.Click += async (s, e) =>
            {
                this.Tip("Saving...", this.ApplicationView.Title); // Tip

                bool? result = await this.Export();
                if (result == null) return;

                if (result.Value)
                    this.Tip("Saved successfully", this.ApplicationView.Title); // Tip
                else
                    this.Tip("Failed to Save", "Try again?"); // Tip
            };
        }

        private void ConstructRadian()
        {
            this.RadianStoryboard.Completed += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.RadianClearButton.Tapped += (s, e) => this.RadianStoryboard.Begin(); // Storyboard
            this.RadianSlider.ValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * Math.PI;
                this.Transformer.Radian = (float)radian;
                this.Transformer.ReloadMatrix();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructScale()
        {
            this.ScaleStoryboard.Completed += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ScaleClearButton.Tapped += (s, e) => this.ScaleStoryboard.Begin(); // Storyboard
            this.ScaleSlider.ValueChanged += (s, e) =>
            {
                double scale = this.ScaleRange.InverseProportion.ConvertXToY(e.NewValue);
                this.Transformer.Scale = (float)scale;
                this.Transformer.ReloadMatrix();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}