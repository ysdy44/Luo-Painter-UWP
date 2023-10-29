﻿using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {
        public void ConstructTexture()
        {
            this.ImportShapeButton.Click += async (s, e) =>
            {
                StorageFile file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                this.InkPresenter.ClearShape();
                StorageFile copyFile = await FileUtil.CopySingleImageFileAsync(file);

                // Select Texture
                this.ShapeImage.UriSource = new System.Uri(copyFile.Path);
                this.InkPresenter.ConstructShape(copyFile.Name, await CanvasBitmap.LoadAsync(this.CanvasDevice, copyFile.Path));
                this.InkType = this.InkPresenter.GetType();
                this.TryInk();
            };
            this.TextureShapeButton.Click += async (s, e) =>
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
                        this.ShapeImage.UriSource = new BrushTextureUri(path);
                        this.InkPresenter.ConstructShape(path, await CanvasBitmap.LoadAsync(this.CanvasDevice, path.GetTextureSource()));
                        this.InkType = this.InkPresenter.GetType();
                        this.TryInk();
                        break;
                    default:
                        break;
                }
            };

            this.RecolorShapeButton.Toggled += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;
                bool recolor = this.RecolorShapeButton.IsOn is true;

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
                if (this.IsInkEnabled is false) return;

                bool rotate = this.RotateButton.IsOn;
                this.InkPresenter.Rotate = rotate;
                this.TryInk();
            };


            this.ImportGrainButton.Click += async (s, e) =>
            {
                StorageFile file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                this.InkPresenter.ClearGrain();
                StorageFile copyFile = await FileUtil.CopySingleImageFileAsync(file);

                // Select Texture
                this.GrainImage.UriSource = new System.Uri(copyFile.Path);
                this.InkPresenter.ConstructGrain(copyFile.Name, await CanvasBitmap.LoadAsync(this.CanvasDevice, copyFile.Path));
                this.InkType = this.InkPresenter.GetType();
                this.TryInk();
            };
            this.TextureGrainButton.Click += async (s, e) =>
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
                        this.GrainImage.UriSource = new BrushTextureUri(path);
                        this.InkPresenter.ConstructGrain(path, await CanvasBitmap.LoadAsync(this.CanvasDevice, path.GetTextureSource()));
                        this.InkType = this.InkPresenter.GetType();
                        this.TryInk();
                        break;
                    default:
                        break;
                }
            };

            this.RecolorGrainButton.Toggled += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;
                bool recolor = this.RecolorGrainButton.IsOn is true;

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

            this.GrainScaleSlider.ValueChanged += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;
                this.InkPresenter.GrainScale = (float)e.NewValue / 100f;
            };

            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;
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