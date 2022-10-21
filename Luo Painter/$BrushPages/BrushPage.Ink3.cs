using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        public void ConstructInk3()
        {
            this.GrainButton.Toggled += async (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                bool isOn = this.GrainButton.IsOn;

                // 1. Turn Off
                if (isOn is false)
                {
                    this.InkPresenter.TurnOffGrain();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 2. Turn On
                if (this.InkPresenter.GrainSource is null is false)
                {
                    this.InkPresenter.TurnOffGrain();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 3. Show Dialog
                this.TextureDialog.Construct(this.GrainImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.GrainImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructGrain(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            lock (this.InkLocker) this.Ink();
                            return;
                        }
                        break;
                    default:
                        break;
                }

                // 4. Hides Dialog
                if (this.GrainButton.IsOn is false) return;
                else
                {
                    this.InkIsEnabled = false;
                    this.GrainButton.IsOn = false;
                    this.InkIsEnabled = true;
                    return;
                }
            };

            this.SelectGrainButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.TextureDialog.Construct(this.GrainImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.GrainImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructGrain(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) break;
                            lock (this.InkLocker) this.Ink();
                            return;
                        }
                        break;
                    default:
                        break;
                }
            };

            this.StepTextBox.Text = this.Step.ToString();
            this.StepTextBox.KeyDown += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (SizePickerExtension.IsEnter(e.Key)) this.GrainButton.Focus(FocusState.Programmatic);
            };
            this.StepTextBox.LostFocus += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (this.Step.IsMatch(this.StepTextBox) is false) return;
                this.InkPresenter.Step = this.Step.Size;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
        }

    }
}