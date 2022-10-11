using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        public void ConstructInk2()
        {
            this.MaskButton.Toggled += async (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                bool isOn = this.MaskButton.IsOn;

                // 1. Turn Off
                if (isOn is false)
                {
                    this.InkPresenter.TurnOffMask();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 2. Turn On
                if (this.InkPresenter.Mask is null is false)
                {
                    this.InkPresenter.TryTurnOnMask();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 3. Show Dialog
                this.TextureDialog.Construct(this.MaskImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.MaskImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructMask(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
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
                if (this.MaskButton.IsOn is false) return;
                else
                {
                    this.InkIsEnabled = false;
                    this.MaskButton.IsOn = false;
                    this.InkIsEnabled = true;
                    return;
                }
            };

            this.SelectMaskButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.TextureDialog.Construct(this.MaskImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.MaskImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructMask(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
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

            this.RotateButton.Unchecked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Rotate = false;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.RotateButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Rotate = true;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
        }

    }
}