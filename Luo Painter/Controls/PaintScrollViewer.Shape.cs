using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        public void ConstructShape()
        {
            this.HardnessListView.ItemClick += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (e.ClickedItem is BrushEdgeHardness item)
                {
                    this.InkPresenter.Hardness = item;
                    this.TryInk();
                }
            };


            this.TipListBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                switch (this.TipListBox.SelectedIndex)
                {
                    case 0:
                        this.InkPresenter.Tip = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = false;
                        this.TryInk();
                        break;
                    case 1:
                        this.InkPresenter.Tip = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = true;
                        this.TryInk();
                        break;
                    case 2:
                        this.InkPresenter.Tip = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = false;
                        this.TryInk();
                        break;
                    case 3:
                        this.InkPresenter.Tip = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = true;
                        this.TryInk();
                        break;
                    default:
                        break;
                }
            };


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
        }

    }
}