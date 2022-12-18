using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        public void ConstructInk2()
        {
            this.ImportShapeButton.Click += async (s, e) =>
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

            this.RecolorShapeButton.Click += (s, e) =>
            {
                bool recolor = this.RecolorShapeButton.IsChecked is true;

                // Select Texture
                this.ShapeImage.ShowAsMonochrome = recolor;
                this.InkPresenter.RecolorShape = recolor;
                this.TryInk();
            };

            this.RemoveShapeButton.Click += (s, e) =>
            {
                // Select Texture
                this.ShapeImage.UriSource = null;
                this.InkPresenter.ClearShape();
                this.InkType = this.InkPresenter.GetType();
                this.TryInk();
            };

            this.RotateButton.Toggled += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                bool rotate = this.RotateButton.IsOn;
                this.InkPresenter.Rotate = rotate;
                this.TryInk();
            };
        }

    }
}