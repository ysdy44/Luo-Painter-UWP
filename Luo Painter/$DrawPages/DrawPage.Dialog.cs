using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        public int DPI
        {
            get
            {
                switch (this.DPIComboBox.SelectedIndex)
                {
                    case 0: return 72;
                    case 1: return 96;
                    case 2: return 144;
                    case 3: return 192;
                    case 4: return 300;
                    case 5: return 400;
                    default: return 96;
                }
            }
        }

        public CanvasBitmapFileFormat FileFormat
        {
            get
            {
                switch (this.FormatComboBox.SelectedIndex)
                {
                    case 0: return CanvasBitmapFileFormat.Jpeg;
                    case 1: return CanvasBitmapFileFormat.Png;
                    case 2: return CanvasBitmapFileFormat.Bmp;
                    case 3: return CanvasBitmapFileFormat.Gif;
                    case 4: return CanvasBitmapFileFormat.Tiff;
                    default: return CanvasBitmapFileFormat.Jpeg;
                }
            }
        }

        public string FileChoices
        {
            get
            {
                switch (this.FormatComboBox.SelectedIndex)
                {
                    case 0: return ".jpeg";
                    case 1: return ".png";
                    case 2: return ".bmp";
                    case 3: return ".gif";
                    case 4: return ".tiff";
                    default: return ".jpeg";
                }
            }
        }

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
                this.ExportFlyout.Hide();

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