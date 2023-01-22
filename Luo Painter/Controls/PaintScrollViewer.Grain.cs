using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        public void ConstructGrain()
        {
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (e.ClickedItem is BlendEffectMode item)
                {
                    this.InkPresenter.BlendMode = item;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInk();
                }
            };
  

            this.ImportGrainButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.ConstructTexture(this.InkPresenter.Grain);

                ContentDialogResult result = await this.ShowTextureAsync();
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
                        break;
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