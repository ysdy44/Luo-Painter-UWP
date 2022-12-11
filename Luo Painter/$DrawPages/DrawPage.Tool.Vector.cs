using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        int StrawMode => this.StrawComboBox.SelectedIndex;

        private void ConstructVector()
        {
        }

        private void View_Start()
        {
            this.Transformer.CacheMove(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(this.StartingPoint));

            this.CanvasAnimatedControl.Invalidate(true); // Invalidate
        }
        private void View_Delta()
        {
            this.Transformer.Move(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(this.Point));

            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void View_Complete()
        {
            this.Transformer.Move(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(this.Point));

            this.CanvasAnimatedControl.Invalidate(this.OptionType.HasPreview()); // Invalidate

            this.ConstructView(this.Transformer);
        }


        private void Straw_Start()
        {
            this.Straw();
            this.StrawCanvasControl.Invalidate();

            this.StrawViewer.Move(this.Point.X, this.Point.Y);
            this.StrawViewer.Show();
        }
        private void Straw_Delta()
        {
            this.StrawViewer.Move(this.Point.X, this.Point.Y);

            this.Straw();
            this.StrawCanvasControl.Invalidate();

            switch (this.StrawMode)
            {
                case 0:
                    Color color1 = this.StrawViewer.GetStraw();
                    this.ColorButton.OnColorChanged(color1, ColorChangedMode.WithPrimaryBrush);
                    break;
            }
        }
        private void Straw_Complete()
        {
            this.StrawViewer.Hide();
            switch (this.StrawMode)
            {
                case 0:
                    this.Straw();

                    Color color1 = this.StrawViewer.GetStraw();
                    this.ColorButton.Recolor(color1);
                    this.ColorButton.OnColorChanged(color1);
                    break;
                default:
                    if (this.LayerSelectedItem is null)
                    {
                        this.Tip(TipType.NoLayer);
                        break;
                    }

                    if (this.LayerSelectedItem is BitmapLayer bitmapLayer is false)
                    {
                        this.Tip(TipType.NotBitmapLayer);
                        break;
                    }

                    int x = (int)this.Position.X;
                    int y = (int)this.Position.Y;

                    if (bitmapLayer.Contains(x, y) is false) break;

                    Color color2 = bitmapLayer.GetPixelColor(x, y, BitmapType.Source);
                    if (color2 == default) break;

                    this.ColorButton.Recolor(color2);
                    this.ColorButton.OnColorChanged(color2);
                    break;
            }
        }

    }
}