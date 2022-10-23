using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
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
                    this.TryInk();
                    return;
                }

                // 2. Turn On
                if (this.InkPresenter.ShapeSource is null is false)
                {
                    this.InkPresenter.TryTurnOnShape();
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInk();
                    return;
                }

                // 3. Show Dialog
                this.ConstructTexture(this.InkPresenter.Shape);

                base.IsHitTestVisible = false;
                ContentDialogResult result = await this.ShowTextureAsync();
                base.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        string path = this.TextureSelectedItem;
                        if (string.IsNullOrEmpty(path)) break;

                        // Select Texture
                        this.ShapeImage.UriSource = new System.Uri(path.GetTexture());
                        this.InkPresenter.ConstructShape(path, await CanvasBitmap.LoadAsync(this.CanvasDevice, path.GetTextureSource()));
                        this.InkType = this.InkPresenter.GetType();
                        this.TryInk();
                        return;
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
                this.ConstructTexture(this.InkPresenter.Shape);

                base.IsHitTestVisible = false;
                ContentDialogResult result = await this.ShowTextureAsync();
                base.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        string path = this.TextureSelectedItem;
                        if (string.IsNullOrEmpty(path)) break;

                        // Select Texture
                        this.ShapeImage.UriSource = new System.Uri(path.GetTexture());
                        this.InkPresenter.ConstructShape(path, await CanvasBitmap.LoadAsync(this.CanvasDevice, path.GetTextureSource()));
                        this.InkType = this.InkPresenter.GetType();
                        this.TryInk();
                        return;
                    default:
                        break;
                }
            };

            this.RotateButton.Unchecked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Rotate = false;
                this.TryInk();
            };
            this.RotateButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Rotate = true;
                this.TryInk();
            };
        }

    }
}