using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {
        public void ConstructTexture()
        {
            this.ImportShapeButton.Click += async (s, e) =>
            {
                // Show Dialog
                this.ConstructTexture(this.InkPresenter.Shape);

                ContentDialogResult result = await this.ShowTextureAsync();
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
                        break;
                    default:
                        break;
                }
            };

            this.RecolorShapeButton.Click += (s, e) =>
            {
                bool recolor = this.RecolorShapeButton.IsChecked is true;

                // Select Texture
                Uri source = this.ShapeImage.UriSource;
                this.ShapeImage.UriSource = null;
                this.ShapeImage.ShowAsMonochrome = recolor;
                this.ShapeImage.UriSource = source;
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
                Uri source = this.GrainImage.UriSource;
                this.GrainImage.UriSource = null;
                this.GrainImage.ShowAsMonochrome = recolor;
                this.GrainImage.UriSource = source;
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
        }
    }
}