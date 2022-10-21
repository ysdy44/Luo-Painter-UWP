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
            this.ShapeButton.Toggled += async (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                bool isOn = this.ShapeButton.IsOn;

                // 1. Turn Off
                if (isOn is false)
                {
                    this.InkPresenter.TurnOffShape();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 2. Turn On
                if (this.InkPresenter.ShapeSource is null is false)
                {
                    this.InkPresenter.TryTurnOnShape();
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                    return;
                }

                // 3. Show Dialog
                this.TextureDialog.Construct(this.ShapeImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.ShapeImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructShape(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
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
                if (this.ShapeButton.IsOn is false) return;
                else
                {
                    this.InkIsEnabled = false;
                    this.ShapeButton.IsOn = false;
                    this.InkIsEnabled = true;
                    return;
                }
            };

            this.SelectShapeButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.TextureDialog.Construct(this.ShapeImage.UriSource?.ToString());

                this.ScrollViewer.IsHitTestVisible = false;
                ContentDialogResult result = await this.TextureDialog.ShowInstance();
                this.ScrollViewer.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        if (this.TextureDialog.SelectedItem is PaintTexture item)
                        {
                            // Select Texture
                            this.ShapeImage.UriSource = new System.Uri(item.Texture);
                            this.InkPresenter.ConstructShape(item.Texture, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
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