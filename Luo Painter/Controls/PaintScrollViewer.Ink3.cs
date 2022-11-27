using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        public void ConstructInk3()
        {
            this.ImportGrainButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.ConstructTexture(this.InkPresenter.Grain);

                base.IsHitTestVisible = false;
                ContentDialogResult result = await this.ShowTextureAsync();
                base.IsHitTestVisible = true;

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        string path = this.TextureSelectedItem;
                        if (string.IsNullOrEmpty(path)) break;

                        // Select Texture
                        this.GrainImage.UriSource = new System.Uri(path.GetTexture());
                        this.InkPresenter.ConstructGrain(path, await CanvasBitmap.LoadAsync(this.CanvasDevice, path.GetTextureSource()));
                        this.InkType = this.InkPresenter.GetType();
                        this.TryInk();
                        return;
                    default:
                        break;
                }
            };

            this.RecolorGrainButton.Click += (s, e) =>
            {
                bool recolor = this.RecolorGrainButton.IsChecked is true;

                // Select Texture
                this.GrainImage.ShowAsMonochrome = recolor;
                this.InkPresenter.RecolorGrain = recolor;
                this.TryInk();
            };

            this.RemoveGrainButton.Click += (s, e) =>
            {
                // Select Texture
                this.GrainImage.UriSource = null;
                this.InkPresenter.ClearGrain();
                this.InkType = this.InkPresenter.GetType();
                this.TryInk();
            };
        }

    }
}